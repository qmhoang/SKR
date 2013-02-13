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
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Entities.Items.Components;

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

		public static ActionResult MeleeAttack(Entity attacker, Entity weapon, Entity defender, DefendComponent.AttackablePart bodyPartTargetted, double hitBonus = 0, bool targettingPenalty = false) {
			Contract.Requires<ArgumentNullException>(attacker != null, "attacker");
			Contract.Requires<ArgumentNullException>(weapon != null, "weapon");
			Contract.Requires<ArgumentNullException>(defender != null, "defender");
			Contract.Requires<ArgumentNullException>(bodyPartTargetted != null, "bodyPartTargetted");

			Contract.Requires<ArgumentException>(weapon.Has<MeleeComponent>(), "weapon cannot melee attack");
			Contract.Requires<ArgumentException>(attacker.Has<ActionPoint>(), "attacker doesn't have a location");
			Contract.Requires<ArgumentException>(attacker.Has<Location>(), "attacker doesn't have a location");
			Contract.Requires<ArgumentException>(defender.Has<DefendComponent>(), "defender cannot be attacked");
			Contract.Requires<ArgumentException>(defender.Has<Location>(), "defender doesn't have a location");
			Contract.Requires<ArgumentException>(defender.Get<DefendComponent>() == bodyPartTargetted.Owner, "defender does not contain supplied body part");
			Contract.Requires<ArgumentException>(defender.Get<Location>().Level == attacker.Get<Location>().Level, "attacker is not on the same level as defender");
			Contract.Requires<ArgumentException>(defender.Id == bodyPartTargetted.Owner.OwnerUId);

			var melee = weapon.Get<MeleeComponent>();			
			//apply skill
			if (attacker.Has<ActorComponent>()) {
				hitBonus += attacker.Get<Person>().GetSkill(melee.Skill);				
			} else {
				hitBonus += World.MEAN;
			}

			var defenderName = Identifier.GetNameOrId(defender);
			var attackerName = Identifier.GetNameOrId(attacker);

			var result = Combat.Attack(attackerName, defenderName, hitBonus + melee.HitBonus - World.MEAN - (targettingPenalty ? bodyPartTargetted.TargettingPenalty : 0));

			if (result == CombatEventResult.Hit) {
				const int TEMP_STR_BONUS = World.MEAN;
				var damage = Math.Max(melee.Damage.Roll() + GetStrengthDamage(TEMP_STR_BONUS).Roll(), 1);
				int damageResistance, realDamage;

				Combat.Damage(damage, melee.Penetration, melee.DamageType, defender, bodyPartTargetted, out damageResistance, out realDamage);

//				if (World.Instance.Player == attacker) {
//					World.Instance.Log.Good(String.Format("{0} {1} {2}'s {3}.... and inflict {4} wounds.",
//					                                     attackerName, melee.ActionDescriptionPlural, defenderName, bodyPartTargetted.Name, "todo-description"));
//				} else {
//					World.Instance.Log.Bad(String.Format("{0} {1} {2}'s {3}.... and inflict {4} wounds.",
//					                                     attackerName, melee.ActionDescriptionPlural, defenderName, bodyPartTargetted.Name, "todo-description"));
//				}



				Combat.ProcessCombat(new CombatEventArgs(attacker, defender, weapon, bodyPartTargetted, CombatEventResult.Hit, damage,
				                                         damageResistance, realDamage));
			} else if (result == CombatEventResult.Miss) {
//				if (World.Instance.Player == attacker) {
//					World.Instance.Log.Bad(String.Format("{0} {1} {2}'s {3}.... and misses.",
//					                                     attackerName, melee.ActionDescriptionPlural, defenderName, bodyPartTargetted.Name));
//				} else {
//					World.Instance.Log.Good(String.Format("{0} {1} {2}'s {3}.... and misses.",
//					                                      attackerName, melee.ActionDescriptionPlural, defenderName, bodyPartTargetted.Name));
//				}


				Combat.ProcessCombat(new CombatEventArgs(attacker, defender, weapon, bodyPartTargetted));
			} else if (result == CombatEventResult.Dodge) {
//				if (World.Instance.Player == attacker) {
//					World.Instance.Log.Bad(String.Format("{0} {1} {2}'s {3}.... and {2} dodges.",
//					                                     attackerName, melee.ActionDescriptionPlural, defenderName, bodyPartTargetted.Name));
//				} else {
//					World.Instance.Log.Good(String.Format("{0} {1} {2}'s {3}.... and {2} dodges.",
//					                                      attackerName, melee.ActionDescriptionPlural, defenderName, bodyPartTargetted.Name));
//				}
				

				Combat.ProcessCombat(new CombatEventArgs(attacker, defender, weapon, bodyPartTargetted, CombatEventResult.Dodge));
			}

			attacker.Get<ActionPoint>().ActionPoints -= melee.APToAttack;
			return ActionResult.Success;
		}

		public static ActionResult RangeAttack(Entity attacker, Entity rangeWeapon, Entity defender, DefendComponent.AttackablePart bodyPartTargetted, double hitBonus = 0, bool targettingPenalty = false) {
			Contract.Requires<ArgumentNullException>(attacker != null, "attacker");
			Contract.Requires<ArgumentNullException>(rangeWeapon != null, "rangeWeapon");
			Contract.Requires<ArgumentNullException>(defender != null, "defender");
			Contract.Requires<ArgumentNullException>(bodyPartTargetted != null, "bodyPartTargetted");

			Contract.Requires<ArgumentException>(rangeWeapon.Has<RangeComponent>(), "rangeWeapon cannot range attack");
			Contract.Requires<ArgumentException>(attacker.Has<ActionPoint>(), "attacker doesn't have a location");
			Contract.Requires<ArgumentException>(attacker.Has<Location>(), "attacker doesn't have a location");
			Contract.Requires<ArgumentException>(defender.Has<DefendComponent>(), "defender cannot be attacked");
			Contract.Requires<ArgumentException>(defender.Has<Location>(), "defender doesn't have a location");
			Contract.Requires<ArgumentException>(defender.Get<DefendComponent>() == bodyPartTargetted.Owner, "defender does not contain supplied body part");
			Contract.Requires<ArgumentException>(defender.Get<Location>().Level == attacker.Get<Location>().Level, "attacker is not on the same level as defender");
			
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
//				World.Instance.Log.Normal(String.Format("{0} attempts to use the only to realize the weapon is not loaded",
//				                                        attackerName));
				attacker.Get<ActionPoint>().ActionPoints -= weapon.APToAttack;
				return ActionResult.Failed;
			}

			weapon.ShotsRemaining--;
			attacker.Get<ActionPoint>().ActionPoints -= weapon.APToAttack;

//			var locationsOnPath = Bresenham.GeneratePointsFromLine(attackerLocation.Position, targetLocation.Position).ToList();

			var entitiesOnPath = Combat.GetTargetsOnPath(attackerLocation.Position, targetLocation.Position).ToList();

			int targetsInTheWay = 0;
			foreach (var currentEntity in entitiesOnPath) {			
				var defenderLocation = currentEntity.Get<Location>();

				double range = defenderLocation.DistanceTo(attackerLocation) * World.TILE_LENGTH_IN_METER;
				double rangePenalty = Math.Min(0, -World.STANDARD_DEVIATION * Combat.RANGE_PENALTY_STD_DEV_MULT * Math.Log(range) + World.STANDARD_DEVIATION * 2 / 3);				
				Logger.InfoFormat("Target: {2}, range to target: {0}, penalty: {1}", range, rangePenalty, defenderName);

				// not being targetted gives a sigma (std dev) penalty
				rangePenalty -= defender.Id == currentEntity.Id ? 0 : World.STANDARD_DEVIATION;

				double difficultyOfShot = hitBonus + rangePenalty + (targetsInTheWay * Combat.RANGE_PENALTY_TILE_OCCUPIED) - World.MEAN - (targettingPenalty ? bodyPartTargetted.TargettingPenalty : 0);
				Logger.InfoFormat("Shot difficulty: {0}, targetting penalty: {1}, weapon bonus: {2}, is target: {3}",
								  difficultyOfShot, bodyPartTargetted.TargettingPenalty, hitBonus,
								  defender.Id == currentEntity.Id);

				var result = Combat.Attack(attackerName, defenderName, difficultyOfShot);

				if (result == CombatEventResult.Hit) {
					var damage = Math.Max(weapon.Damage.Roll(), 1);
					int damageResistance, realDamage;

					Combat.Damage(weapon.Damage.Roll(), weapon.Penetration, weapon.DamageType, defender, bodyPartTargetted, out damageResistance, out realDamage);
//
//					World.Instance.Log.Normal(String.Format("{0} {1} {2}'s {3}.... and inflict {4} wounds.",
//					                                        attackerName, weapon.ActionDescriptionPlural, defenderName, bodyPartTargetted.Name, "todo-description"));

					Combat.ProcessCombat(new CombatEventArgs(attacker, defender, rangeWeapon, bodyPartTargetted, CombatEventResult.Hit, damage,
					                                         damageResistance, realDamage));
					return ActionResult.Success;
				} else if (result == CombatEventResult.Miss) {
					if (defender.Id == currentEntity.Id) // if this is where the actor targetted
//						World.Instance.Log.Normal(String.Format("{0} {1} {2}'s {3}.... and misses.",
//																attackerName, weapon.ActionDescriptionPlural, defenderName, bodyPartTargetted.Name));

					Combat.ProcessCombat(new CombatEventArgs(attacker, defender, rangeWeapon, bodyPartTargetted));
				} else if (result == CombatEventResult.Dodge) {
					if (defender.Id == currentEntity.Id) // if this is where the actor targetted
//						World.Instance.Log.Normal(String.Format("{0} {1} {2}'s {3}.... and {2} dodges.",
//																attackerName, weapon.ActionDescriptionPlural, defenderName, bodyPartTargetted.Name));

					Combat.ProcessCombat(new CombatEventArgs(attacker, defender, rangeWeapon, bodyPartTargetted, CombatEventResult.Dodge));
				}
				targetsInTheWay++;
			}

			// todo drop ammo casing

//			World.Instance.Log.Normal(String.Format("{0} {1} and hits nothing", attackerName, weapon.ActionDescriptionPlural));
			return ActionResult.Failed;
		}

		public static ActionResult ReloadWeapon(Entity user, Entity weaponEntity, Entity ammoEntity) {
			Contract.Requires<ArgumentException>(weaponEntity.Has<RangeComponent>());
			Contract.Requires<ArgumentException>(ammoEntity.Has<AmmoComponent>());
			Contract.Requires<ArgumentException>(user.Has<Location>());
			Contract.Requires<ArgumentException>(user.Has<ActionPoint>());
			Contract.Requires<ArgumentException>(weaponEntity.Get<RangeComponent>().AmmoType == ammoEntity.Get<AmmoComponent>().Type);

			var weapon = weaponEntity.Get<RangeComponent>();
			var ammo = ammoEntity.Get<Item>();

			// todo revolvers and single load weapons

			// first we unload all ammos currently in the gun to the group, semi-simulating dropping the magazine
			if (weapon.ShotsRemaining > 0) {
				var droppedAmmo = ammoEntity.Copy();

				droppedAmmo.Get<Item>().Amount = weapon.ShotsRemaining;
				weapon.ShotsRemaining = 0;
				droppedAmmo.Get<VisibleComponent>().Reset();

//				World.Instance.Log.Normal(String.Format("{0} reloads {1} with {2}, dropping all excess ammo.", user.Get<Identifier>().Name, weaponEntity.Get<Identifier>().Name, ammoEntity.Get<Identifier>().Name));
			} else {
//				World.Instance.Log.Normal(String.Format("{0} reloads {1} with {2}.", user.Get<Identifier>().Name, weaponEntity.Get<Identifier>().Name, ammoEntity.Get<Identifier>().Name));
			}

			if (ammo.StackType == StackType.Hard) {
				if (ammo.Amount >= weapon.Shots) {
					ammo.Amount -= weapon.Shots;
					weapon.ShotsRemaining = weapon.Shots;
				} else {
					ammo.Amount -= weaponEntity.Get<Item>().Amount;

					if (user.Has<ContainerComponent>()) {
						user.Get<ContainerComponent>().Remove(ammoEntity);
					}

//					World.Instance.EntityManager.Remove(ammoEntity);
				}
			}

			user.Get<ActionPoint>().ActionPoints -= weapon.APToReload;
			return ActionResult.Success;
		}

		public static Rand GetStrengthDamage(int strength) {
			return Rand.Gaussian(strength / 3, strength / 3 - 1, Math.Min(World.STANDARD_DEVIATION / 3, strength / 3));
		}

		public const double RANGE_PENALTY_STD_DEV_MULT = 0.87;
		public const double RANGE_PENALTY_TILE_OCCUPIED = -World.MEAN * 4 / 3;

		public static IEnumerable<Entity> GetTargetsOnPath(Point start, Point end) {
//			var currentLevel = World.Instance.CurrentLevel;

//			if (!currentLevel.IsWalkable(start))
//				throw new ArgumentException("starting point has to be walkable", "start");

			var pointsOnPath = Bresenham.GeneratePointsFromLine(start, end);

//			foreach (var location in pointsOnPath) {
//				if (!currentLevel.IsWalkable(location)) {
//					Logger.InfoFormat("We hit a location:({0}) where it is not walkable.", location);
//					yield break;
//				}
//
//				var entitiesAt = currentLevel.GetEntitiesAt(location).Where(e => e.Has<DefendComponent>()).ToList();
//				if (entitiesAt.Count() > 0) {					
//					foreach (var entity in entitiesAt) {
//						yield return entity;
//					}
//				}
//			}
			yield return null;
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