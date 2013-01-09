using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entity;
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

		public static ActionResult MeleeAttack(Entity attacker, Entity defender, DefendComponent.AttackablePart bodyPartTargetted, double hitBonus = 0, bool targettingPenalty = false) {
			Contract.Requires<ArgumentException>(attacker.Has<MeleeComponent>());
			Contract.Requires<ArgumentException>(attacker.Has<ActionPoint>());
			Contract.Requires<ArgumentException>(defender.Has<DefendComponent>());
			Contract.Requires<ArgumentException>(bodyPartTargetted.Owner.OwnerUId == defender.Id);

			var weapon = attacker.Get<MeleeComponent>();
			
			var result = Combat.Attack(attacker.Id.ToString(), defender.Id.ToString(), hitBonus + weapon.HitBonus - World.MEAN - (targettingPenalty ? bodyPartTargetted.TargettingPenalty : 0));

			if (result == CombatEventResult.Hit) {
				var damage = Math.Max(weapon.Damage.Roll(), 1);
				int damageResistance, realDamage;

				Combat.Damage(damage, weapon.DamageType, defender, bodyPartTargetted, out damageResistance, out realDamage);

				Logger.InfoFormat("{0} {1} {2}'s {3}.... and inflict {4} wounds.",
				                  attacker.Id, weapon.ActionDescriptionPlural, defender.Id, bodyPartTargetted.Name, "todo-description");


				Combat.ProcessCombat(new CombatEventArgs(attacker, defender, bodyPartTargetted, CombatEventResult.Hit, damage,
				                                         damageResistance, realDamage));
			} else if (result == CombatEventResult.Miss) {
				Logger.InfoFormat("{0} {1} {2}'s {3}.... and misses.",
				                  attacker.Id, weapon.ActionDescriptionPlural, defender.Id, bodyPartTargetted.Name);

				Combat.ProcessCombat(new CombatEventArgs(attacker, defender, bodyPartTargetted));
			} else if (result == CombatEventResult.Dodge) {
				Logger.InfoFormat("{0} {1} {2}'s {3}.... and {2} dodges.",
				                  attacker.Id, weapon.ActionDescriptionPlural, defender.Id, bodyPartTargetted.Name);

				Combat.ProcessCombat(new CombatEventArgs(attacker, defender, bodyPartTargetted, CombatEventResult.Dodge));
			}

			attacker.Get<ActionPoint>().ActionPoints -= weapon.APToAttack;
			return ActionResult.Success;
		}

		public static ActionResult AttackRange(Entity attacker, Entity rangeWeapon, Entity defender, DefendComponent.AttackablePart bodyPartTargetted, double hitBonus = 0, bool targettingPenalty = false) {
			if (!rangeWeapon.Has<RangeComponent>())
				throw new ArgumentException("rangeWeapon cannot range attack", "rangeWeapon");
			if (!attacker.Has<ActionPoint>())
				throw new ArgumentException("entity cannot act", "attacker");
			if (!attacker.Has<Location>())
				throw new ArgumentException("entity doesn't have a location", "attacker");

			if (!defender.Has<DefendComponent>())
				throw new ArgumentException("entity cannot be attacked", "defender");
			if (!defender.Has<Location>())
				throw new ArgumentException("entity doesn't have a location", "defender");
			if (defender.Get<DefendComponent>() != bodyPartTargetted.Owner)
				throw new ArgumentException("entity does not contain body part", "bodyPartTargetted");
			if (defender.Get<Location>().Level != attacker.Get<Location>().Level)
				throw new ArgumentException("attacker is not on the same level as target");
			Contract.EndContractBlock();

			var weapon = rangeWeapon.Get<RangeComponent>();

//			if (attackerLocation.DistanceTo(targettedPosition) > (weapon.Range + t.Range + 1)) {
//				World.Instance.AddMessage("Too far to attack.");
//				return ActionResult.Aborted;
//			}

			if (weapon.ShotsRemaining <= 0) {
				World.Instance.AddMessage(String.Format("{0} attempts to use the only to realize the weapon is not loaded",
				                                        attacker.Id));
				attacker.Get<ActionPoint>().ActionPoints -= weapon.APToAttack;
				return ActionResult.Failed;
			}

			weapon.ShotsRemaining--;
			attacker.Get<ActionPoint>().ActionPoints -= weapon.APToAttack;

//			var hitBonusFromSkill = attacker.GetTalent(weapon.Skill).As<SkillComponent>().Rank - World.MEAN;
			var attackerLocation = attacker.Get<Location>();
			var targetLocation = defender.Get<Location>();
			
			var locationsOnPath = Combat.GetTargetsOnPath(attackerLocation.Position, targetLocation.Position).ToList();


			for (int i = 0; i < locationsOnPath.Count; i++) {
				var point = locationsOnPath[i];

				var possibleTargetsAtLocation = targetLocation.Level.GetEntitiesAt(point).ToList();

				// todo fix hack
				var defenderEntity = possibleTargetsAtLocation.First();
//				var defender = defenderEntity.Get<>()<Actor>();
				var defenderLocation = defenderEntity.Get<Location>();


				double range = defenderLocation.DistanceTo(attackerLocation) * World.TILE_LENGTH_IN_METER;
				double rangePenalty = Math.Min(0, -World.STANDARD_DEVIATION * Combat.RANGE_PENALTY_STD_DEV_MULT * Math.Log(range) + World.STANDARD_DEVIATION * 2 / 3);
				Logger.InfoFormat("Target: {2}, range to target: {0}, penalty: {1}", range, rangePenalty, defender.Id);

				// not being targetted gives a sigma (std dev) penalty
				rangePenalty -= targetLocation == defenderLocation ? 0 : World.STANDARD_DEVIATION;

				double difficultyOfShot = hitBonus + rangePenalty + (i * Combat.RANGE_PENALTY_TILE_OCCUPIED) - World.MEAN - (targettingPenalty ? bodyPartTargetted.TargettingPenalty : 0);
				Logger.InfoFormat("Shot difficulty: {0}, targetting penalty: {1}, weapon bonus: {2}, is target: {3}",
								  difficultyOfShot, bodyPartTargetted.TargettingPenalty, hitBonus,
				                  targetLocation == defenderLocation);

				var result = Combat.Attack(attacker.Id.ToString(), defender.Id.ToString(), difficultyOfShot);

				if (result == CombatEventResult.Hit) {
					var damage = Math.Max(weapon.Damage.Roll(), 1);
					int damageResistance, realDamage;

					Combat.Damage(weapon.Damage.Roll(), weapon.DamageType, defender, bodyPartTargetted, out damageResistance, out realDamage);

					World.Instance.AddMessage(String.Format("{0} {1} {2}'s {3}.... and inflict {4} wounds.",
					                                        attacker.Id, weapon.ActionDescriptionPlural, defender.Id, bodyPartTargetted.Name, "todo-description"));


					Combat.ProcessCombat(new CombatEventArgs(attacker, defender, bodyPartTargetted, CombatEventResult.Hit, damage,
					                                         damageResistance, realDamage));
				} else if (result == CombatEventResult.Miss) {
					if (point == targetLocation.Position) // if this is where the actor targetted
						World.Instance.AddMessage(String.Format("{0} {1} {2}'s {3}.... and misses.",
																attacker.Id, weapon.ActionDescriptionPlural, defender.Id, bodyPartTargetted.Name));

					Combat.ProcessCombat(new CombatEventArgs(attacker, defender, bodyPartTargetted));
				} else if (result == CombatEventResult.Dodge) {
					if (point == targetLocation.Position) // if this is where the actor targetted
						World.Instance.AddMessage(String.Format("{0} {1} {2}'s {3}.... and {2} dodges.",
																attacker.Id, weapon.ActionDescriptionPlural, defender.Id, bodyPartTargetted.Name));

					Combat.ProcessCombat(new CombatEventArgs(attacker, defender, bodyPartTargetted, CombatEventResult.Dodge));
				}

				return ActionResult.Success;
			}

			// todo drop ammo casing

			World.Instance.AddMessage(String.Format("{0} {1} and hits nothing", attacker.Id, weapon.ActionDescriptionPlural));
			return ActionResult.Failed;
		}

		public static Rand GetStrengthDamage(int strength) {
			return Rand.Gaussian(strength / 3, strength / 3 - 1, Math.Min(World.STANDARD_DEVIATION / 3, strength / 3));
		}

		public const double RANGE_PENALTY_STD_DEV_MULT = 0.87;
		public const double RANGE_PENALTY_TILE_OCCUPIED = -World.MEAN * 4 / 3;

		public static IEnumerable<Point> GetTargetsOnPath(Point start, Point end) {
			var currentLevel = World.Instance.CurrentLevel;
			if (!currentLevel.IsWalkable(start))
				throw new ArgumentException("starting point has to be walkable", "start");

			var pointsOnPath = Bresenham.GeneratePointsFromLine(start, end);

			for (int index = 0; index < pointsOnPath.Count; index++) {
				var location = pointsOnPath[index];
				if (!currentLevel.IsWalkable(location)) {
					Logger.InfoFormat("We hit a location:({0}) where it is not walkable, returning previous location({1}).", location, pointsOnPath[index - 1]);
					yield return pointsOnPath[index - 1];
					yield break;
				}

				//				if (currentLevel.DoesActorExistAtLocation(location))
				//					yield return location;
			}
		}

		public static void Damage(int damage, DamageType type, Entity defender, DefendComponent.AttackablePart bodyPart, out int damageResistance, out int damageDealt) {
			Contract.Requires(bodyPart.Owner.OwnerUId == defender.Id);
			damageDealt = damage;
			damageResistance = 0;

			//TODO: FIX EQUIP/PROTECTION SLOT MISTACH!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			if (defender.Has<EquipmentComponent>()) {
				var armorEntity = defender.Get<EquipmentComponent>().GetEquippedItemAt(bodyPart.Name);
				if (armorEntity != null && armorEntity.Has<ArmorComponent>()) {
					var armor = armorEntity.Get<ArmorComponent>();

					if (armor.Defenses.ContainsKey(bodyPart.Name)) {

						if (Rng.Chance(armor.Defenses[bodyPart.Name].Coverage / 100.0)) {
							damageResistance = armor.Defenses[bodyPart.Name].Resistances[type];
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
//			e.Attacker.OnAttacking(e);
//			e.Defender.OnDefending(e);
		}
	}
}