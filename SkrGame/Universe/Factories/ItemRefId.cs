using System;
using DEngine.Entity;

namespace SkrGame.Universe.Factories {
	public class ItemRefId : Component, IEquatable<ItemRefId> {
		public string RefId { get; private set; }

		public ItemRefId(string refId) {
			RefId = refId;
		}

		public override Component Copy() {
			return new ItemRefId(RefId);
		}

		public bool Equals(ItemRefId other) {
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;
			return Equals(other.RefId, RefId);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(ItemRefId))
				return false;
			return Equals((ItemRefId) obj);
		}

		public override int GetHashCode() {
			return (RefId != null ? RefId.GetHashCode() : 0);
		}

		public static bool operator ==(ItemRefId left, ItemRefId right) {
			return Equals(left, right);
		}

		public static bool operator !=(ItemRefId left, ItemRefId right) {
			return !Equals(left, right);
		}
	}
}