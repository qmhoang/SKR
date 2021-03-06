using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using DEngine.Extensions;
using DEngine.Level;
using SkrGame.Gameplay.Combat;
using SkrGame.Universe;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Entities.Items.Equipables;
using SkrGame.Universe.Locations;

namespace SkrGame.Actions.Combat {
	public class RangeAttackAction : AttackAction {
		
		public RangeAttackAction(Entity attacker, Entity defender, Entity weapon, BodyComponent.Appendage bodyPartTargetted, bool targettingPenalty = false)
				: base(attacker, defender, weapon, bodyPartTargetted, targettingPenalty) {
			Contract.Requires<ArgumentException>(weapon.Has<RangeComponent>(), "weapon cannot range attack");
		}

		public override int APCost {
			get { return Weapon.Get<RangeComponent>().APToAttack; }
		}

		private const double RangePenaltyStdDevMultiplier = 0.87;
		private const double RangePenaltyTileOccupied = -World.Mean * 4 / 3;

		private IEnumerable<Entity> GetTargetsOnPath(Level currentLevel, Point start, Point end) {
			if (!currentLevel.IsWalkable(start))
				throw new ArgumentException("starting point has to be walkable", "start");

			var pointsOnPath = Bresenham.GeneratePointsFromLine(start, end, false);

			foreach (var location in pointsOnPath) {
				if (!currentLevel.IsWalkable(location)) {
					Logger.InfoFormat("We hit a location:({0}) where it is not walkable.", location);
					yield break;
				}

				var entitiesAt = currentLevel.GetEntitiesAt<BodyComponent>(location).ToList();
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
			var attackerLocation = Attacker.Get<GameObject>();
			var defenderLocation = Defender.Get<GameObject>();
			
			//apply skill
			if (Attacker.Has<ControllerComponent>()) {
				hitBonus += Attacker.Get<Creature>().Skills[weapon.Skill];
			} else {
				hitBonus += World.Mean;
			}

			if (weapon.ShotsRemaining <= 0) {
				Log.NormalFormat("{0} attempts to use the only to realize the weapon is not loaded",
				                 attackerName);
				return ActionResult.Failed;
			}

			weapon.ShotsRemaining--;

			IEnumerable<Entity> entitiesOnPath;
			if (attackerLocation.Location == defenderLocation.Location) {
				// suicide?
				entitiesOnPath = attackerLocation.Level.GetEntitiesAt<BodyComponent>(defenderLocation.Location).ToList();

			} else
				entitiesOnPath = GetTargetsOnPath(attackerLocation.Level, attackerLocation.Location, defenderLocation.Location).ToList();

			int targetsInTheWay = 0;
			foreach (var currentEntity in entitiesOnPath) {
				var targetLocation = currentEntity.Get<GameObject>();

				double range = targetLocation.DistanceTo(attackerLocation) * World.TileLengthInMeter;
				double rangePenalty = Math.Min(0, -World.StandardDeviation * RangePenaltyStdDevMultiplier * Math.Log(range) + World.StandardDeviation * 2 / 3);
				Logger.InfoFormat("Target: {2}, range to target: {0}, penalty: {1}", range, rangePenalty, defenderName);

				// not being targetted gives a sigma (std dev) penalty
				rangePenalty -= Defender.Id == currentEntity.Id ? 0 : World.StandardDeviation;

				int easeOfShot = (int) Math.Round(hitBonus + rangePenalty + (targetsInTheWay * RangePenaltyTileOccupied) + (TargettingPenalty ? BodyPartTargetted.TargettingPenalty : 0));
				Logger.InfoFormat("Ease of Shot: {0}, targetting penalty: {1}, weapon bonus: {2}, is original target: {3}",
				                  easeOfShot, BodyPartTargetted.TargettingPenalty, hitBonus,
				                  Defender.Id == currentEntity.Id);

				var result = Attack(attackerName, defenderName, easeOfShot);

				if (result == CombatEventResult.Hit) {
					var damage = Math.Max(weapon.Damage.Roll(), 1);
					int damageResistance, realDamage;

					Damage(weapon.Damage.Roll(), weapon.Penetration, weapon.DamageType, out damageResistance, out realDamage);

					Log.NormalFormat("{0} {1} {2}'s {3}.... and inflict {4} wounds.",
					                 attackerName, weapon.ActionDescriptionPlural, defenderName, BodyPartTargetted.Name, "todo-description");

					Logger.Info(new CombatEventArgs(Attacker, Defender, Weapon, BodyPartTargetted, CombatEventResult.Hit, damage,
					                                         damageResistance, realDamage));
					return ActionResult.Success;
				} else if (result == CombatEventResult.Miss) {
					if (Defender.Id == currentEntity.Id) // if this is where the actor targetted
						Log.NormalFormat("{0} {1} {2}'s {3}.... and misses.",
						                 attackerName, weapon.ActionDescriptionPlural, defenderName, BodyPartTargetted.Name);

					Logger.Info(new CombatEventArgs(Attacker, Defender, Weapon, BodyPartTargetted));
					return ActionResult.Failed;

				} else if (result == CombatEventResult.Dodge) {
					if (Defender.Id == currentEntity.Id) // if this is where the actor targetted
						Log.NormalFormat("{0} {1} {2}'s {3}.... and {2} dodges.",
						                 attackerName, weapon.ActionDescriptionPlural, defenderName, BodyPartTargetted.Name);

					Logger.Info(new CombatEventArgs(Attacker, Defender, Weapon, BodyPartTargetted, CombatEventResult.Dodge));
					return ActionResult.Failed;
				}
				targetsInTheWay++;
			}

			// todo drop ammo casing

			Log.NormalFormat("{0} {1} and hits nothing", attackerName, weapon.ActionDescriptionPlural);
			return ActionResult.Failed;
		}
	}
}