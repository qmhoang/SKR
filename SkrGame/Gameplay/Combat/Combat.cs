using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
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

		public static readonly StaticDictionary<string, DamageType> DamageTypes =
				new StaticDictionary<string, DamageType>(new Dictionary<string, DamageType>
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
				                                         });
		
		public static void Heal(DefendComponent.AttackablePart bodyPart, int amount) {
			amount = Math.Min(amount, bodyPart.Owner.Health.MaximumValue - bodyPart.Owner.Health);
			bodyPart.Owner.Health.Value += amount;
			Logger.DebugFormat("{0} was healed {1} health", bodyPart.Owner.OwnerUId, amount);
		}
	}
}