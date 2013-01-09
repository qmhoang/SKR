using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using DEngine.Actor;
using DEngine.Core;
using DEngine.Entity;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items.Components;

namespace SkrGame.Universe.Entities.Items {
	public enum ItemType {
		// weapons
		OneHandedWeapon,
		TwoHandedWeapon,
		Armor,
		Shield,
		Ammo,
		BodyPart,
		Misc,
	}

	public class Item : Component {
		internal class Template {
			public ItemType Type { get; set; }
			public int Weight { get; set; }
			public int Size { get; set; }
			public int Value { get; set; }
			public StackType StackType { get; set; }
			public List<string> Slot { get; set; }
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
				Contract.Requires<ArgumentException>(value > 0);
				Contract.Requires<ArgumentException>(StackType == StackType.Hard);
				Contract.Ensures(value > 0);
				amount = value;				
			}
		}

		public StackType StackType { get; private set; }
		public ItemType Type { get; private set; }
		public List<string> Slots { get; private set; }

		private Item() { }

		internal Item(Template template) {
			StackType = template.StackType;
			Slots = template.Slot == null ? new List<string>() : new List<string>(template.Slot);			
			amount = 1;
			Type = template.Type;
			Weight = template.Weight;
			Size = template.Size;
			Value = template.Value;
		}

		public override Component Copy() {

			var copy = new Item()
			           {
			           		StackType = StackType,
			           		Slots = new List<string>(Slots),
			           		Type = Type,
			           		Weight = Weight,
			           		Size = Size,
			           		Value = Value,
			           };
			if (copy.StackType == StackType.Hard)
				copy.Amount = Amount;
			return copy;
		}
		
	}

	public enum StackType {
		None,
		Soft,
		Hard
	}

}