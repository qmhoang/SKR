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
			public string Name { get; set; }
			public ItemType Type { get; set; }
			public int Weight { get; set; }
			public int Size { get; set; }
			public int Value { get; set; }
			public StackType StackType { get; set; }
			public List<string> Slot { get; set; }
		}

		public string Name { get; private set; }

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
				Contract.Requires(value > 0);
				Contract.Requires(StackType == StackType.Hard);
				Contract.Ensures(value > 0);
				amount = value;
			}
		}

		public StackType StackType { get; private set; }
		public ItemType Type { get; private set; }
		public List<string> Slots { get; private set; }

		private Item() { }

		internal Item(Template template) {
			Name = template.Name;

			StackType = template.StackType;
			Slots = template.Slot == null ? new List<string>() : new List<string>(template.Slot);			
			amount = 1;
			Type = template.Type;
			Weight = template.Weight;
			Size = template.Size;
			Value = template.Value;
		}

		public override Component Copy() {
			return new Item()
			       {
			       		Name = Name,

			       		StackType = StackType,
			       		Slots = new List<string>(Slots),
			       		Amount = Amount,
			       		Type = Type,
			       		Weight = Weight,
			       		Size = Size,
			       		Value = Value,
			       };
		}
	}

	public enum StackType {
		None,
		Soft,
		Hard
	}

}