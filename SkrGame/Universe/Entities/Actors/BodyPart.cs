using System;
using System.Collections.Generic;
using SkrGame.Universe.Entities.Items;

namespace SkrGame.Universe.Entities.Actors {
//	[Flags]
//	public enum BodySlot {
//		None = 1 << 0,
//		Torso = 1 << 1,
//		Head = 1 << 2,
//		MainArm = 1 << 3,
//		OffArm = 1 << 4,
//		MainHand = 1 << 5,
//		OffHand = 1 << 6,
//		Leg = 1 << 7,
//		Feet = 1 << 8,
//		Neck = 1 << 9,
//		Eyes = 1 << 10,
//		Groin = 1 << 11,
//	}
//
//	public class BodyPart {
//		public string Name { get; private set; }
//		public BodySlot Type { get; private set; }
//		public DefendComponent Owner { get; private set; }
//		public int Health { get; set; }
//		public int MaxHealth { get; protected set; }
//		public int TargettingPenalty { get; protected set; }
//
//		public BodyPart(string name, BodySlot type, DefendComponent owner, int health, int attackPenalty) {
//			Name = name;
//			Type = type;
//			Owner = owner;
//
//			if (health <= 0)
//				health = 1;
//			Health = health;
//			MaxHealth = health;
//
//			TargettingPenalty = attackPenalty;
//		}
//
//		public bool Crippled { get { return Health < 0; } }
//
//		public bool CanUseItem(Item i) {
//			return i.Slot.HasFlag(Type);
//		}
//
//		public Item Item { get; set; }
//
//		public bool Equipped {
//			get { return Item != null; }
//		}
//
//		public override string ToString() {
//			return Name;
//		}
//	}
}