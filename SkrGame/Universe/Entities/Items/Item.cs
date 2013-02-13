using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using DEngine.Actor;
using DEngine.Core;
using DEngine.Entities;
using SkrGame.Universe.Entities.Actors;

namespace SkrGame.Universe.Entities.Items {
//	public enum ItemType {
//		// weapons
//		OneHandedWeapon,
//		TwoHandedWeapon,
//		Armor,
//		Shield,
//		Ammo,
//		BodyPart,
//		Misc,
//	}

	public class Equipable : Component {
		public class Template {
			public List<string> Slot { get; set; }
			public bool TwoHanded { get; set; }
	
		}
		public List<string> Slots { get; private set; }
		public bool TwoHanded { get; private set; }

		private Equipable() {}

		public Equipable(Template template) {
			Slots = template.Slot == null ? new List<string>() : new List<string>(template.Slot);
			TwoHanded = template.TwoHanded;
		}

		public override Component Copy() {
			return new Equipable()
			       {
					   Slots = new List<string>(Slots),
					   TwoHanded = TwoHanded
			       };
		}
	}

	public class Item : Component {
		public class Template {
//			public ItemType Type { get; set; }
			public int Weight { get; set; }
			public int Size { get; set; }
			public int Value { get; set; }
			public StackType StackType { get; set; }
		}

		public int Weight { get; private set; }
		public int Size { get; private set; }
		public int Value { get; private set; }

		public int Hardness { get; set; }

		private int amount;
		public int Amount {
			get {
				if (StackType != StackType.Hard)
					return 1;
				if (amount <= 0)
					return 1;
				return amount;
			}
			set {
				Contract.Requires<ArgumentException>(StackType == StackType.Hard);
				Contract.Requires<ArgumentException>(value > 0);
				amount = value;				
			}
		}

		public StackType StackType { get; private set; }
//		public ItemType Type { get; private set; }

		private Item() {
			amount = 1;			
		}

		public Item(Template template) {
			StackType = template.StackType;
			amount = 1;
//			Type = template.Type;
			Weight = template.Weight;
			Size = template.Size;
			Value = template.Value;
		}

		public override Component Copy() {

			var copy = new Item()
			           {
			           		StackType = StackType,
//			           		Type = Type,
			           		Weight = Weight,
			           		Size = Size,
			           		Value = Value,
			           };
			if (copy.StackType == StackType.Hard)
				copy.Amount = Amount;
			return copy;
		}
		
		public static Entity Split(Entity e, int amount) {
			Contract.Requires<ArgumentNullException>(e != null, "e");
			Contract.Requires<ArgumentException>(e.Has<Item>());
			Contract.Requires<ArgumentException>(e.Get<Item>().StackType == StackType.Hard);
			Contract.Requires<ArgumentException>(amount > 0 && amount < e.Get<Item>().Amount);

			e.Get<Item>().Amount -= amount;

			var copy = e.Copy();
			copy.Get<Item>().Amount = amount;
			return copy;
		}

		[ContractInvariantMethod]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
		private void ObjectInvariant() {
			Contract.Invariant(amount > 0);			
		}
	}

	public enum StackType {
		None,
		Soft,
		Hard
	}

}