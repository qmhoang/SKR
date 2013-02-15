using System;
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

namespace SkrGame.Actions {
	public class RangeAttackAction : LoggedAction {
		private Entity defender;
		private Entity rangeWeapon;
		private DefendComponent.AttackablePart bodyPartTargetted;

		private Entity attacker {
			get { return Entity; }
		}

		protected bool TargettingPenalty { get; private set; }

		public RangeAttackAction(Entity attacker, Entity defender, Entity rangeWeapon, DefendComponent.AttackablePart bodyPartTargetted, bool targettingPenalty = false)
				: base(attacker) {
			Contract.Requires<ArgumentNullException>(attacker != null, "attacker");
			Contract.Requires<ArgumentNullException>(rangeWeapon != null, "weapon");
			Contract.Requires<ArgumentNullException>(defender != null, "defender");
			Contract.Requires<ArgumentNullException>(bodyPartTargetted != null, "bodyPartTargetted");

			Contract.Requires<ArgumentException>(rangeWeapon.Has<RangeComponent>(), "weapon cannot range attack");
			Contract.Requires<ArgumentException>(attacker.Has<ActorComponent>(), "attacker doesn't have a location");
			Contract.Requires<ArgumentException>(attacker.Has<Location>(), "attacker doesn't have a location");
			Contract.Requires<ArgumentException>(defender.Has<DefendComponent>(), "defender cannot be attacked");
			Contract.Requires<ArgumentException>(defender.Has<Location>(), "defender doesn't have a location");
			Contract.Requires<ArgumentException>(defender.Get<DefendComponent>() == bodyPartTargetted.Owner, "defender does not contain supplied body part");
			Contract.Requires<ArgumentException>(defender.Get<Location>().Level == attacker.Get<Location>().Level, "attacker is not on the same level as defender");

			this.defender = defender;
			this.rangeWeapon = rangeWeapon;
			this.bodyPartTargetted = bodyPartTargetted;
			this.TargettingPenalty = targettingPenalty;
		}

		public override int APCost {
			get { return rangeWeapon.Get<RangeComponent>().APToAttack; }
		}

		public override ActionResult OnProcess() {
			var hitBonus = 0;
			var weapon = rangeWeapon.Get<RangeComponent>();
			var attackerName = Identifier.GetNameOrId(attacker);
			var defenderName = Identifier.GetNameOrId(defender);
			var attackerLocation = attacker.Get<Location>();
			var targetLocation = defender.Get<Location>();
			

			//apply skill
			if (attacker.Has<ActorComponent>()) {
				hitBonus += attacker.Get<Person>().GetSkill(weapon.Skill);
			} else {
				hitBonus += World.MEAN;
			}

			if (weapon.ShotsRemaining <= 0) {
				World.Log.Normal(String.Format("{0} attempts to use the only to realize the weapon is not loaded",
				                               attackerName));
				attacker.Get<ActorComponent>().AP.ActionPoints -= weapon.APToAttack;
				return ActionResult.Failed;
			}

			weapon.ShotsRemaining--;
			
			var entitiesOnPath = Combat.GetTargetsOnPath(attacker.Get<Location>().Level, attackerLocation.Point, targetLocation.Point).ToList();

			int targetsInTheWay = 0;
			foreach (var currentEntity in entitiesOnPath) {
				var defenderLocation = currentEntity.Get<Location>();

				double range = defenderLocation.DistanceTo(attackerLocation) * World.TILE_LENGTH_IN_METER;
				double rangePenalty = Math.Min(0, -World.STANDARD_DEVIATION * Combat.RANGE_PENALTY_STD_DEV_MULT * Math.Log(range) + World.STANDARD_DEVIATION * 2 / 3);
				Logger.InfoFormat("Target: {2}, range to target: {0}, penalty: {1}", range, rangePenalty, defenderName);

				// not being targetted gives a sigma (std dev) penalty
				rangePenalty -= defender.Id == currentEntity.Id ? 0 : World.STANDARD_DEVIATION;

				double difficultyOfShot = hitBonus + rangePenalty + (targetsInTheWay * Combat.RANGE_PENALTY_TILE_OCCUPIED) - World.MEAN - (TargettingPenalty ? bodyPartTargetted.TargettingPenalty : 0);
				Logger.InfoFormat("Shot difficulty: {0}, targetting penalty: {1}, weapon bonus: {2}, is target: {3}",
				                  difficultyOfShot, bodyPartTargetted.TargettingPenalty, hitBonus,
				                  defender.Id == currentEntity.Id);

				var result = Combat.Attack(attackerName, defenderName, difficultyOfShot);

				if (result == CombatEventResult.Hit) {
					var damage = Math.Max(weapon.Damage.Roll(), 1);
					int damageResistance, realDamage;

					Combat.Damage(weapon.Damage.Roll(), weapon.Penetration, weapon.DamageType, defender, bodyPartTargetted, out damageResistance, out realDamage);

					World.Log.Normal(String.Format("{0} {1} {2}'s {3}.... and inflict {4} wounds.",
					                               attackerName, weapon.ActionDescriptionPlural, defenderName, bodyPartTargetted.Name, "todo-description"));

					Combat.ProcessCombat(new CombatEventArgs(attacker, defender, rangeWeapon, bodyPartTargetted, CombatEventResult.Hit, damage,
					                                         damageResistance, realDamage));
					return ActionResult.Success;
				} else if (result == CombatEventResult.Miss) {
					if (defender.Id == currentEntity.Id) // if this is where the actor targetted
						World.Log.Normal(String.Format("{0} {1} {2}'s {3}.... and misses.",
						                               attackerName, weapon.ActionDescriptionPlural, defenderName, bodyPartTargetted.Name));

					Combat.ProcessCombat(new CombatEventArgs(attacker, defender, rangeWeapon, bodyPartTargetted));
				} else if (result == CombatEventResult.Dodge) {
					if (defender.Id == currentEntity.Id) // if this is where the actor targetted
						World.Log.Normal(String.Format("{0} {1} {2}'s {3}.... and {2} dodges.",
						                               attackerName, weapon.ActionDescriptionPlural, defenderName, bodyPartTargetted.Name));

					Combat.ProcessCombat(new CombatEventArgs(attacker, defender, rangeWeapon, bodyPartTargetted, CombatEventResult.Dodge));
				}
				targetsInTheWay++;
			}

			// todo drop ammo casing

			World.Log.Normal(String.Format("{0} {1} and hits nothing", attackerName, weapon.ActionDescriptionPlural));
			return ActionResult.Failed;
		}
	}
}