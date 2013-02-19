using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using DEngine.Actions;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using SkrGame.Gameplay.Combat;
using SkrGame.Universe;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Locations;

namespace SkrGame.Actions {
	public class RangeAttackAction : AttackAction {
		
		public RangeAttackAction(Entity attacker, Entity defender, Entity weapon, DefendComponent.AttackablePart bodyPartTargetted, bool targettingPenalty = false)
				: base(attacker, defender, weapon, bodyPartTargetted, targettingPenalty) {
			Contract.Requires<ArgumentException>(weapon.Has<RangeComponent>(), "weapon cannot range attack");
		}

		public override int APCost {
			get { return Weapon.Get<RangeComponent>().APToAttack; }
		}

		public const double RANGE_PENALTY_STD_DEV_MULT = 0.87;
		public const double RANGE_PENALTY_TILE_OCCUPIED = -World.MEAN * 4 / 3;

		private IEnumerable<Entity> GetTargetsOnPath(Level currentLevel, Point start, Point end) {

			if (!currentLevel.IsWalkable(start))
				throw new ArgumentException("starting point has to be walkable", "start");

			var pointsOnPath = Bresenham.GeneratePointsFromLine(start, end);

			foreach (var location in pointsOnPath) {
				if (!currentLevel.IsWalkable(location)) {
					Logger.InfoFormat("We hit a location:({0}) where it is not walkable.", location);
					yield break;
				}

				var entitiesAt = currentLevel.GetEntitiesAt(location).Where(e => e.Has<DefendComponent>()).ToList();
				if (entitiesAt.Count() > 0) {
					foreach (var entity in entitiesAt) {
						yield return entity;
					}
				}
			}
		}

		public override ActionResult OnProcess() {
			var hitBonus = 0;
			var weapon = Weapon.Get<RangeComponent>();
			var attackerName = Identifier.GetNameOrId(Attacker);
			var defenderName = Identifier.GetNameOrId(Defender);
			var attackerLocation = Attacker.Get<Location>();
			var defenderLocation = Defender.Get<Location>();
			

			//apply skill
			if (Attacker.Has<ActorComponent>()) {
				hitBonus += Attacker.Get<Person>().GetSkill(weapon.Skill).Rank;
			} else {
				hitBonus += World.MEAN;
			}

			if (weapon.ShotsRemaining <= 0) {
				World.Log.Normal(String.Format("{0} attempts to use the only to realize the weapon is not loaded",
				                               attackerName));
				Attacker.Get<ActorComponent>().AP.ActionPoints -= weapon.APToAttack;
				return ActionResult.Failed;
			}

			weapon.ShotsRemaining--;
			
			var entitiesOnPath = GetTargetsOnPath(Attacker.Get<Location>().Level, attackerLocation.Point, defenderLocation.Point).ToList();

			int targetsInTheWay = 0;
			foreach (var currentEntity in entitiesOnPath) {
				var targetLocation = currentEntity.Get<Location>();

				double range = targetLocation.DistanceTo(attackerLocation) * World.TILE_LENGTH_IN_METER;
				double rangePenalty = Math.Min(0, -World.STANDARD_DEVIATION * RANGE_PENALTY_STD_DEV_MULT * Math.Log(range) + World.STANDARD_DEVIATION * 2 / 3);
				Logger.InfoFormat("Target: {2}, range to target: {0}, penalty: {1}", range, rangePenalty, defenderName);

				// not being targetted gives a sigma (std dev) penalty
				rangePenalty -= Defender.Id == currentEntity.Id ? 0 : World.STANDARD_DEVIATION;

				double difficultyOfShot = hitBonus + rangePenalty + (targetsInTheWay * RANGE_PENALTY_TILE_OCCUPIED) - World.MEAN - (TargettingPenalty ? BodyPartTargetted.TargettingPenalty : 0);
				Logger.InfoFormat("Shot difficulty: {0}, targetting penalty: {1}, weapon bonus: {2}, is target: {3}",
				                  difficultyOfShot, BodyPartTargetted.TargettingPenalty, hitBonus,
				                  Defender.Id == currentEntity.Id);

				var result = Attack(attackerName, defenderName, difficultyOfShot);

				if (result == CombatEventResult.Hit) {
					var damage = Math.Max(weapon.Damage.Roll(), 1);
					int damageResistance, realDamage;

					Damage(weapon.Damage.Roll(), weapon.Penetration, weapon.DamageType, out damageResistance, out realDamage);

					World.Log.Normal(String.Format("{0} {1} {2}'s {3}.... and inflict {4} wounds.",
					                               attackerName, weapon.ActionDescriptionPlural, defenderName, BodyPartTargetted.Name, "todo-description"));

					Logger.Info(new CombatEventArgs(Attacker, Defender, Weapon, BodyPartTargetted, CombatEventResult.Hit, damage,
					                                         damageResistance, realDamage));
					return ActionResult.Success;
				} else if (result == CombatEventResult.Miss) {
					if (Defender.Id == currentEntity.Id) // if this is where the actor targetted
						World.Log.Normal(String.Format("{0} {1} {2}'s {3}.... and misses.",
						                               attackerName, weapon.ActionDescriptionPlural, defenderName, BodyPartTargetted.Name));

					Logger.Info(new CombatEventArgs(Attacker, Defender, Weapon, BodyPartTargetted));
				} else if (result == CombatEventResult.Dodge) {
					if (Defender.Id == currentEntity.Id) // if this is where the actor targetted
						World.Log.Normal(String.Format("{0} {1} {2}'s {3}.... and {2} dodges.",
						                               attackerName, weapon.ActionDescriptionPlural, defenderName, BodyPartTargetted.Name));

					Logger.Info(new CombatEventArgs(Attacker, Defender, Weapon, BodyPartTargetted, CombatEventResult.Dodge));
				}
				targetsInTheWay++;
			}

			// todo drop ammo casing

			World.Log.Normal(String.Format("{0} {1} and hits nothing", attackerName, weapon.ActionDescriptionPlural));
			return ActionResult.Failed;
		}
	}
}