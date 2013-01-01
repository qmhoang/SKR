using System;
using System.Collections.Generic;
using DEngine.Core;
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
			if (obj.GetType() != typeof(DamageType))
				return false;
			return Equals((DamageType)obj);
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

		public static BodyPart GetRandomBodyPart(Actor target) {
			int roll = Rng.Roll(3, 6);

			Logger.InfoFormat("Random body part roll: {0}", roll);

			//todo unfinished
			if (roll <= 4)
				return target.GetBodyPart(BodySlot.Head);
			if (roll <= 5)
				return target.GetBodyPart(BodySlot.OffHand);
			if (roll <= 7)
				return target.GetBodyPart(BodySlot.Leg);            // left leg
			if (roll <= 8)
				return target.GetBodyPart(BodySlot.OffArm);
			if (roll <= 11)
				return target.GetBodyPart(BodySlot.Torso);
			if (roll <= 12)
				return target.GetBodyPart(BodySlot.MainArm);
			if (roll <= 14)
				return target.GetBodyPart(BodySlot.Leg);            // right leg
			if (roll <= 15)
				return target.GetBodyPart(BodySlot.MainHand);
			if (roll <= 16)
				return target.GetBodyPart(BodySlot.Feet);
			else
				return target.GetBodyPart(BodySlot.Head);
		}

		private static double ChanceOfSuccess(double difficulty) {
			return GaussianDistribution.CumulativeTo(difficulty + World.MEAN, World.MEAN, World.STANDARD_DEVIATION);
		}

		public static CombatEventResult Attack(Actor attacker, Actor defender, double attackDifficulty, bool dodge = true, bool block = true, bool parry = true) {
			double atkRoll = Rng.Double();
			double chanceToHit = ChanceOfSuccess(attackDifficulty);

			if (atkRoll > chanceToHit) {
				Logger.InfoFormat("{0} misses {1} (needs:{2:0.00}, rolled:{3:0.00}, difficulty: {4:0.0})", attacker.Name, defender.Name, chanceToHit, atkRoll, attackDifficulty);
				return CombatEventResult.Miss;
			}

			Logger.InfoFormat("{0} attacks {1} (needs:{2:0.00}, rolled:{3:0.00}, difficulty: {4:0.0})", attacker.Name, defender.Name, chanceToHit, atkRoll, attackDifficulty);

			if (dodge) {
				double defRoll = Rng.Double();
				double chanceToDodge = 0.5; //TODO
				Logger.InfoFormat("{0} attempts to dodge {1}'s attack (needs:{2:0.00}, rolled:{3:0.00}, difficulty: {4:0.0})", attacker.Name, defender.Name, chanceToHit, atkRoll, attackDifficulty);
				if (defRoll > chanceToDodge) {
					return CombatEventResult.Dodge;
				}
			}

			// todo defenses, parries, etc

			// todo wounding
			return CombatEventResult.Hit;
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

				if (currentLevel.DoesActorExistAtLocation(location))
					yield return location;
			}
		}
	}

}
