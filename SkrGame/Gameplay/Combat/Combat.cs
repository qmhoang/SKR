using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using SkrGame.Gameplay.Talent.Components;
using SkrGame.Universe;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Locations;

namespace SkrGame.Gameplay.Combat {
	public class DamageType {
		public int Id { get; private set; }
		public string Name { get; private set; }

		private static int idCounter = 0;

		internal DamageType(string name) {
			Name = name;
			Id = idCounter++;
		}

		public bool Equals(DamageType other) {
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;
			return other.Id == Id;
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof (DamageType))
				return false;
			return Equals((DamageType) obj);
		}

		public override int GetHashCode() {
			return Id;
		}

		public static bool operator ==(DamageType left, DamageType right) {
			return Equals(left, right);
		}

		public static bool operator !=(DamageType left, DamageType right) {
			return !Equals(left, right);
		}
	}

	public static class Combat {
		private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public static readonly Dictionary<string, DamageType> DamageTypes = new Dictionary<string, DamageType>
		                                                                    {
		                                                                    		{"true", new DamageType("True")},
		                                                                    		{"cut", new DamageType("Cutting")},
		                                                                    		{"impale", new DamageType("Impaling")},
		                                                                    		{"crush", new DamageType("Crushing")},
		                                                                    		{"pierce_small", new DamageType("Small Piercing")},
		                                                                    		{"pierce", new DamageType("Piercing")},
		                                                                    		{"pierce_large", new DamageType("Large Piercing")},
		                                                                    		{"pierce_huge", new DamageType("Huge Piercing")},
		                                                                    		{"burn", new DamageType("Burning")},
		                                                                    };


		/// <summary>
		/// Higher means easier
		/// </summary>
		/// <param name="difficulty"></param>
		/// <returns></returns>
		private static double ChanceOfSuccess(double difficulty) {
			return GaussianDistribution.CumulativeTo(difficulty + World.MEAN, World.MEAN, World.STANDARD_DEVIATION);
		}

		public static CombatEventResult Attack(string attackerName, string defenderName, double attackDifficulty, double dodgeDifficulty = 0.5, bool dodge = true, bool block = true, bool parry = true) {
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

		public static Rand GetStrengthDamage(int strength) {
			return Rand.Gaussian(strength / 3, strength / 3 - 1, Math.Min(World.STANDARD_DEVIATION / 3, strength / 3));
		}

		public const double RANGE_PENALTY_STD_DEV_MULT = 0.87;
		public const double RANGE_PENALTY_TILE_OCCUPIED = -World.MEAN * 4 / 3;

		public static IEnumerable<Entity> GetTargetsOnPath(Level currentLevel, Point start, Point end) {
			
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

		public static void Damage(int damage, double penetration, DamageType type, Entity defender, DefendComponent.AttackablePart bodyPart, out int damageResistance, out int damageDealt) {
			Contract.Requires<ArgumentNullException>(bodyPart != null, "bodyPart");
			Contract.Requires<ArgumentNullException>(defender != null, "defender");
			Contract.Requires<ArgumentException>(defender.Has<DefendComponent>() && defender.Get<DefendComponent>() != null);
			Contract.Requires<ArgumentException>(defender.Get<DefendComponent>() == bodyPart.Owner, "defender does not contain supplied body part");
			Contract.Ensures(Contract.ValueAtReturn(out damageDealt) >= 0);
			Contract.Ensures(Contract.ValueAtReturn(out damageResistance) >= 0);
			
			damageDealt = damage;
			damageResistance = 0;

			if (defender.Has<EquipmentComponent>()) {
				var equipment = defender.Get<EquipmentComponent>();

				if (equipment.ContainSlot(bodyPart.Name) && equipment.IsSlotEquipped(bodyPart.Name)) {
					var armorEntity = equipment.GetEquippedItemAt(bodyPart.Name);

					if (armorEntity.Has<ArmorComponent>()) {
						var armor = armorEntity.Get<ArmorComponent>();

						if (armor.Defenses.ContainsKey(bodyPart.Name)) {

							if (Rng.Chance(armor.Defenses[bodyPart.Name].Coverage / 100.0)) {
								damageResistance = (int) Math.Floor(armor.Defenses[bodyPart.Name].Resistances[type] / penetration);
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

			if (damageDealt > bodyPart.MaxHealth) {
				damageDealt = Math.Min(damage, bodyPart.MaxHealth);
				Logger.DebugFormat("Damage reduced to {0} because of {1}'s max health", damageDealt, bodyPart.Name);
			}

			bodyPart.Health -= damageDealt;
			bodyPart.Owner.Health -= damageDealt;

			Logger.InfoFormat("{0}'s {1} was hurt ({2} damage)", bodyPart.Owner.OwnerUId, bodyPart.Name, damageDealt);
		}

		public static void Heal(DefendComponent.AttackablePart bodyPart, int amount) {
			amount = Math.Min(amount, bodyPart.Owner.MaxHealth - bodyPart.Owner.Health);
			bodyPart.Owner.Health += amount;
			Logger.DebugFormat("{0} was healed {1} health", bodyPart.Owner.OwnerUId, amount);
		}

		public static void ProcessCombat(CombatEventArgs e) {
			Contract.Requires<ArgumentNullException>(e != null, "e");
//			e.Attacker.OnAttacking(e);
//			e.Defender.OnDefending(e);			
			Logger.Info(e.ToString());
		}
	}
}