using System;
using System.Diagnostics.Contracts;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using SkrGame.Gameplay.Combat;
using SkrGame.Universe;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;

namespace SkrGame.Actions {
	public abstract class AttackAction : LoggedAction {
		public Entity Defender { get; private set; }
		public Entity Weapon { get; private set; }
		public DefendComponent.AttackablePart BodyPartTargetted { get; private set; }

		protected AttackAction(Entity attacker, Entity defender, Entity weapon, DefendComponent.AttackablePart bodyPartTargetted, bool targettingPenalty) : base(attacker) {
			Contract.Requires<ArgumentNullException>(attacker != null, "attacker");
			Contract.Requires<ArgumentNullException>(weapon != null, "weapon");
			Contract.Requires<ArgumentNullException>(defender != null, "defender");
			Contract.Requires<ArgumentNullException>(bodyPartTargetted != null, "bodyPartTargetted");

			Contract.Requires<ArgumentException>(attacker.Has<ActorComponent>(), "attacker doesn't have a location");
			Contract.Requires<ArgumentException>(attacker.Has<Location>(), "attacker doesn't have a location");
			Contract.Requires<ArgumentException>(defender.Has<DefendComponent>(), "defender cannot be attacked");
			Contract.Requires<ArgumentException>(defender.Has<Location>(), "defender doesn't have a location");
			Contract.Requires<ArgumentException>(defender.Get<DefendComponent>() == bodyPartTargetted.Owner, "defender does not contain supplied body part");
			Contract.Requires<ArgumentException>(defender.Get<Location>().Level == attacker.Get<Location>().Level, "attacker is not on the same level as defender");
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="difficulty"></param>
		/// <returns>Higher means easier</returns>
		public double ChanceOfSuccess(double difficulty) {
			return GaussianDistribution.CumulativeTo(difficulty + World.MEAN, World.MEAN, World.STANDARD_DEVIATION);
		}

		public CombatEventResult Attack(string attackerName, string defenderName, double attackDifficulty, double dodgeDifficulty = 0.5, bool dodge = true, bool block = true, bool parry = true) {
			double atkRoll = Rng.Double();
			double chanceToHit = ChanceOfSuccess(attackDifficulty);

			if (atkRoll > chanceToHit) {
				Logger.InfoFormat("{0} misses {1} (needs:{2:0.00}, rolled:{3:0.00}, difficulty: {4:0.0})", attackerName, defenderName, chanceToHit, atkRoll, attackDifficulty);
				return CombatEventResult.Miss;
			}

			Logger.InfoFormat("{0} attacks {1} (needs:{2:0.00}, rolled:{3:0.00}, difficulty: {4:0.0})", attackerName, defenderName, chanceToHit, atkRoll, attackDifficulty);

			if (dodge) {
				double defRoll = Rng.Double();
				double chanceToDodge = ChanceOfSuccess(dodgeDifficulty);
				Logger.InfoFormat("{1} attempts to dodge {0}'s attack (needs:{2:0.00}, rolled:{3:0.00}, difficulty: {4:0.0})", attackerName, defenderName, chanceToDodge, defRoll, dodgeDifficulty);
				if (defRoll < chanceToDodge)
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

				if (equipment.ContainSlot(BodyPartTargetted.Name) && equipment.IsSlotEquipped(BodyPartTargetted.Name)) {
					var armorEntity = equipment.GetEquippedItemAt(BodyPartTargetted.Name);

					if (armorEntity.Has<ArmorComponent>()) {
						var armor = armorEntity.Get<ArmorComponent>();

						if (armor.Defenses.ContainsKey(BodyPartTargetted.Name)) {

							if (Rng.Chance(armor.Defenses[BodyPartTargetted.Name].Coverage / 100.0)) {
								damageResistance = (int)Math.Floor(armor.Defenses[BodyPartTargetted.Name].Resistances[type] / penetration);
								damageDealt = Math.Max(damage - damageResistance, 0);
								Logger.InfoFormat("Damage: {3} reduced to {0} because of {1} [DR: {2}]", damageDealt, armor.OwnerUId, damageResistance, damage);
							} else {
								// we hit a chink in the armor
								damageResistance = 0;
								Logger.InfoFormat("Hit a chink in the armor, DR = 0");
							}
						}
					}
				}
			}

			if (Defender.Has<ActorComponent>()) {
				Defender.Get<ActorComponent>().Disturb();
			}

			if (damageDealt > BodyPartTargetted.MaxHealth) {
				damageDealt = Math.Min(damage, BodyPartTargetted.MaxHealth);
				Logger.DebugFormat("Damage reduced to {0} because of {1}'s max health", damageDealt, BodyPartTargetted.Name);
			}

			BodyPartTargetted.Health -= damageDealt;
			BodyPartTargetted.Owner.Health -= damageDealt;

			Logger.InfoFormat("{0}'s {1} was hurt ({2} damage)", Identifier.GetNameOrId(Defender), BodyPartTargetted.Name, damageDealt);
		}
	}
}