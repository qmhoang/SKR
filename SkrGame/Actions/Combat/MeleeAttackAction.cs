using System;
using System.Diagnostics.Contracts;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using SkrGame.Gameplay.Combat;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;

namespace SkrGame.Actions.Combat {
	public class MeleeAttackAction : AttackAction {
		public MeleeAttackAction(Entity attacker, Entity defender, Entity weapon, DefendComponent.Appendage bodyPartTargetted, bool targettingPenalty = false)
				: base(attacker, defender, weapon, bodyPartTargetted, targettingPenalty) {
			Contract.Requires<ArgumentException>(weapon.Has<MeleeComponent>(), "weapon cannot melee attack");
		}

		public override int APCost {
			get { return Weapon.Get<MeleeComponent>().APToAttack; }
		}

		public Rand GetStrengthDamage(int strength) {
			return Rand.Gaussian(strength / 3, strength / 3 - 1, Math.Min(World.STANDARD_DEVIATION / 3, strength / 3));
		}

		public override ActionResult OnProcess() {
			int hitBonus = 0;
			var melee = Weapon.Get<MeleeComponent>();

			//apply skill
			if (Attacker.Has<ActorComponent>()) {
				hitBonus += Attacker.Get<Person>().Skills[melee.Skill];
			} else {
				hitBonus += World.MEAN;
			}

			var defenderName = Identifier.GetNameOrId(Defender);
			var attackerName = Identifier.GetNameOrId(Attacker);

			var result = Attack(attackerName, defenderName, hitBonus + melee.HitBonus - (TargettingPenalty ? BodyPartTargetted.TargettingPenalty : 0));

			if (result == CombatEventResult.Hit) {
				var damage = Math.Max(melee.Damage.Roll() + GetStrengthDamage(Attacker.Get<Person>().Attributes["attribute_strength"]).Roll(), 1);
				int damageResistance, realDamage;

				Damage(damage, melee.Penetration, melee.DamageType, out damageResistance, out realDamage);

				if (World.Player == Attacker) {
					World.Log.Good(String.Format("{0} {1} {2}'s {3}.... and inflict {4} wounds.",
					                             attackerName, melee.ActionDescriptionPlural, defenderName, BodyPartTargetted.Name, "todo-description"));
				} else {
					World.Log.Bad(String.Format("{0} {1} {2}'s {3}.... and inflict {4} wounds.",
					                            attackerName, melee.ActionDescriptionPlural, defenderName, BodyPartTargetted.Name, "todo-description"));
				}
				
				Logger.Info(new CombatEventArgs(Attacker, Defender, Weapon, BodyPartTargetted, CombatEventResult.Hit, damage,
				                                         damageResistance, realDamage));
				return ActionResult.Success;

			} else if (result == CombatEventResult.Miss) {
				if (World.Player == Attacker) {
					World.Log.Bad(String.Format("{0} {1} {2}'s {3}.... and misses.",
					                            attackerName, melee.ActionDescriptionPlural, defenderName, BodyPartTargetted.Name));
				} else {
					World.Log.Good(String.Format("{0} {1} {2}'s {3}.... and misses.",
					                             attackerName, melee.ActionDescriptionPlural, defenderName, BodyPartTargetted.Name));
				}


				Logger.Info(new CombatEventArgs(Attacker, Defender, Weapon, BodyPartTargetted));
			} else if (result == CombatEventResult.Dodge) {
				if (World.Player == Attacker) {
					World.Log.Bad(String.Format("{0} {1} {2}'s {3}.... and {2} dodges.",
					                            attackerName, melee.ActionDescriptionPlural, defenderName, BodyPartTargetted.Name));
				} else {
					World.Log.Good(String.Format("{0} {1} {2}'s {3}.... and {2} dodges.",
					                             attackerName, melee.ActionDescriptionPlural, defenderName, BodyPartTargetted.Name));
				}


				Logger.Info(new CombatEventArgs(Attacker, Defender, Weapon, BodyPartTargetted, CombatEventResult.Dodge));
			}
			return ActionResult.Failed;
		}
	}
}