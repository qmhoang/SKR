using System;
using System.Diagnostics.Contracts;
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
	public class MeleeAttackAction : LoggedAction {
		private Entity defender;
		private Entity weapon;
		private DefendComponent.AttackablePart bodyPartTargetted;

		private Entity attacker {
			get { return Entity; }
		}

		protected bool TargettingPenalty { get; private set; }

		public MeleeAttackAction(Entity attacker, Entity defender, Entity weapon, DefendComponent.AttackablePart bodyPartTargetted, bool targettingPenalty = false)
				: base(attacker) {
			Contract.Requires<ArgumentNullException>(attacker != null, "attacker");
			Contract.Requires<ArgumentNullException>(weapon != null, "weapon");
			Contract.Requires<ArgumentNullException>(defender != null, "defender");
			Contract.Requires<ArgumentNullException>(bodyPartTargetted != null, "bodyPartTargetted");

			Contract.Requires<ArgumentException>(weapon.Has<MeleeComponent>(), "weapon cannot melee attack");
			Contract.Requires<ArgumentException>(attacker.Has<ActorComponent>(), "attacker doesn't have a location");
			Contract.Requires<ArgumentException>(attacker.Has<Location>(), "attacker doesn't have a location");
			Contract.Requires<ArgumentException>(defender.Has<DefendComponent>(), "defender cannot be attacked");
			Contract.Requires<ArgumentException>(defender.Has<Location>(), "defender doesn't have a location");
			Contract.Requires<ArgumentException>(defender.Get<DefendComponent>() == bodyPartTargetted.Owner, "defender does not contain supplied body part");
			Contract.Requires<ArgumentException>(defender.Get<Location>().Level == attacker.Get<Location>().Level, "attacker is not on the same level as defender");
			Contract.Requires<ArgumentException>(defender.Id == bodyPartTargetted.Owner.OwnerUId);

			this.defender = defender;
			this.weapon = weapon;
			this.bodyPartTargetted = bodyPartTargetted;
			this.TargettingPenalty = targettingPenalty;
		}

		public override int APCost {
			get { return weapon.Get<MeleeComponent>().APToAttack; }
		}

		public override ActionResult OnProcess() {
			int hitBonus = 0;
			var melee = weapon.Get<MeleeComponent>();

			//apply skill
			if (attacker.Has<ActorComponent>()) {
				hitBonus += attacker.Get<Person>().GetSkill(melee.Skill);
			} else {
				hitBonus += World.MEAN;
			}

			var defenderName = Identifier.GetNameOrId(defender);
			var attackerName = Identifier.GetNameOrId(attacker);

			var result = Combat.Attack(attackerName, defenderName, hitBonus + melee.HitBonus - World.MEAN - (TargettingPenalty ? bodyPartTargetted.TargettingPenalty : 0));

			if (result == CombatEventResult.Hit) {
				const int TEMP_STR_BONUS = World.MEAN;
				var damage = Math.Max(melee.Damage.Roll() + Combat.GetStrengthDamage(TEMP_STR_BONUS).Roll(), 1);
				int damageResistance, realDamage;

				Combat.Damage(damage, melee.Penetration, melee.DamageType, defender, bodyPartTargetted, out damageResistance, out realDamage);

				if (World.Player == attacker) {
					World.Log.Good(String.Format("{0} {1} {2}'s {3}.... and inflict {4} wounds.",
					                             attackerName, melee.ActionDescriptionPlural, defenderName, bodyPartTargetted.Name, "todo-description"));
				} else {
					World.Log.Bad(String.Format("{0} {1} {2}'s {3}.... and inflict {4} wounds.",
					                            attackerName, melee.ActionDescriptionPlural, defenderName, bodyPartTargetted.Name, "todo-description"));
				}



				Combat.ProcessCombat(new CombatEventArgs(attacker, defender, weapon, bodyPartTargetted, CombatEventResult.Hit, damage,
				                                         damageResistance, realDamage));
			} else if (result == CombatEventResult.Miss) {
				if (World.Player == attacker) {
					World.Log.Bad(String.Format("{0} {1} {2}'s {3}.... and misses.",
					                            attackerName, melee.ActionDescriptionPlural, defenderName, bodyPartTargetted.Name));
				} else {
					World.Log.Good(String.Format("{0} {1} {2}'s {3}.... and misses.",
					                             attackerName, melee.ActionDescriptionPlural, defenderName, bodyPartTargetted.Name));
				}


				Combat.ProcessCombat(new CombatEventArgs(attacker, defender, weapon, bodyPartTargetted));
			} else if (result == CombatEventResult.Dodge) {
				if (World.Player == attacker) {
					World.Log.Bad(String.Format("{0} {1} {2}'s {3}.... and {2} dodges.",
					                            attackerName, melee.ActionDescriptionPlural, defenderName, bodyPartTargetted.Name));
				} else {
					World.Log.Good(String.Format("{0} {1} {2}'s {3}.... and {2} dodges.",
					                             attackerName, melee.ActionDescriptionPlural, defenderName, bodyPartTargetted.Name));
				}


				Combat.ProcessCombat(new CombatEventArgs(attacker, defender, weapon, bodyPartTargetted, CombatEventResult.Dodge));
			}
			return ActionResult.Success;
		}
	}
}