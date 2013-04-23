using System;
using System.Diagnostics.Contracts;
using System.Linq;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using DEngine.Extensions;
using DEngine.Random;
using SkrGame.Gameplay.Combat;
using SkrGame.Universe;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Entities.Items.Equipables;

namespace SkrGame.Actions.Combat {
	public abstract class AttackAction : LoggedAction {
		protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public Entity Defender { get; private set; }
		public Entity Weapon { get; private set; }
		public DefendComponent.Appendage BodyPartTargetted { get; private set; }

		protected AttackAction(Entity attacker, Entity defender, Entity weapon, DefendComponent.Appendage bodyPartTargetted, bool targettingPenalty) : base(attacker) {
			Contract.Requires<ArgumentNullException>(attacker != null, "attacker");
			Contract.Requires<ArgumentNullException>(weapon != null, "weapon");
			Contract.Requires<ArgumentNullException>(defender != null, "defender");
			Contract.Requires<ArgumentNullException>(bodyPartTargetted != null, "bodyPartTargetted");

			Contract.Requires<ArgumentException>(attacker.Has<ActorComponent>(), "attacker doesn't have a location");
			Contract.Requires<ArgumentException>(attacker.Has<GameObject>(), "attacker doesn't have a location");
			Contract.Requires<ArgumentException>(defender.Has<DefendComponent>(), "defender cannot be attacked");
			Contract.Requires<ArgumentException>(defender.Has<GameObject>(), "defender doesn't have a location");
			Contract.Requires<ArgumentException>(defender.Get<DefendComponent>() == bodyPartTargetted.Owner, "defender does not contain supplied body part");
			Contract.Requires<ArgumentException>(defender.Get<GameObject>().Level == attacker.Get<GameObject>().Level, "attacker is not on the same level as defender");
			Contract.Requires<ArgumentException>(defender.Id == bodyPartTargetted.Owner.OwnerUId);

			Defender = defender;
			Weapon = weapon;
			BodyPartTargetted = bodyPartTargetted;
			TargettingPenalty = targettingPenalty;
		}

		public Entity Attacker {
			get { return Entity; }
		}

		public bool TargettingPenalty { get; private set; }

		public CombatEventResult Attack(string attackerName, string defenderName, double easeOfAttack, double easeOfDodging = World.Mean, bool dodge = true, bool block = true, bool parry = true) {
			if (Defender.Has<ActorComponent>()) {
				Defender.Get<ActorComponent>().Disturb();
			}

			double atkRoll = World.SkillRoll();

			if (atkRoll > easeOfAttack) {
				Logger.InfoFormat("{0} misses {1} (needs:{2:0.00}, rolled:{3:0.00}, difficulty: {4:0.00}%)", attackerName, defenderName, easeOfAttack, atkRoll,
				                  World.ChanceOfSuccess(easeOfAttack));
				return CombatEventResult.Miss;
			}

			Logger.InfoFormat("{0} attacks {1} (needs:{2:0.00}, rolled:{3:0.00}, difficulty: {4:0.00}%)", attackerName, defenderName, easeOfAttack, atkRoll,
			                  World.ChanceOfSuccess(easeOfAttack));

			if (dodge) {
				double defRoll = World.SkillRoll();

				Logger.InfoFormat("{1} attempts to dodge {0}'s attack (needs:{2:0.00}, rolled:{3:0.00}, difficulty: {4:0.00}%)", attackerName, defenderName, easeOfDodging, defRoll,
				                  World.ChanceOfSuccess(easeOfDodging));
				if (defRoll < easeOfDodging)
					return CombatEventResult.Dodge;
			}

			// todo defenses, parries, etc

			return CombatEventResult.Hit;
		}

		public void Damage(int damage, double penetration, DamageType type, out int damageResistance, out int damageDealt) {
			Contract.Ensures(Contract.ValueAtReturn(out damageDealt) >= 0);
			Contract.Ensures(Contract.ValueAtReturn(out damageResistance) >= 0);

			damageDealt = damage;
			damageResistance = 0;

			if (Defender.Has<EquipmentComponent>()) {
				var equipment = Defender.Get<EquipmentComponent>();


				var armors = equipment.EquippedItems.FilteredBy<ArmorComponent>().Where(e => e.Get<ArmorComponent>().Defenses.ContainsKey(BodyPartTargetted.Name)).ToList();
				if (!armors.IsEmpty()) {
					var armorEntity = armors.First();
					var armor = armorEntity.Get<ArmorComponent>();

					if (Rng.Chance(armor.Defenses[BodyPartTargetted.Name].Coverage / 100.0)) {
						damageResistance = (int) Math.Floor(armor.Defenses[BodyPartTargetted.Name].Resistances[type] / penetration);
						damageDealt = Math.Max(damage - damageResistance, 0);
						Logger.InfoFormat("Damage: {3} reduced to {0} because of {1} [DR: {2}]", damageDealt, Identifier.GetNameOrId(armorEntity), damageResistance, damage);
					} else {
						// we hit a chink in the armor
						damageResistance = 0;
						Logger.InfoFormat("Hit a chink in the armor, DR = 0");
					}
				}
			}

			if (damageDealt > BodyPartTargetted.MaxHealth) {
				damageDealt = Math.Min(damage, BodyPartTargetted.MaxHealth);
				Logger.DebugFormat("Damage reduced to {0} because of {1}'s max health", damageDealt, BodyPartTargetted.Name);
			}

			BodyPartTargetted.Health -= damageDealt;
			BodyPartTargetted.Owner.Health.Value -= damageDealt;

			Logger.InfoFormat("{0}'s {1} was hurt ({2} damage)", Identifier.GetNameOrId(Defender), BodyPartTargetted.Name, damageDealt);
		}
	}
}