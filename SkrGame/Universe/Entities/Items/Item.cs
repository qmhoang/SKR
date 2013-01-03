using System;
using System.Collections.Generic;
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
	}

	public class Item : EntityComponent {
		public string Name { get; private set; }

		public string RefId { get; private set; }
		public int Weight { get; private set; }
		public int Size { get; private set; }
		public int Value { get; private set; }

		public int NormalDurability { get; set; }
		public int CurrentDurability { get; set; }
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
				if (value <= 0)
					throw new ArgumentException("cannot be 0 or negative, remove instead", "value");
				if (StackType != StackType.Hard)
					throw new ArgumentException("Cannot modify count of non-stacked item");
				amount = value;
			}
		}

		public StackType StackType { get; private set; }
		public ItemType Type { get; private set; }
		public List<string> Slots { get; private set; }


		public Item(ItemTemplate template) {
			Name = template.Name;
			RefId = template.RefId;

			StackType = template.StackType;
			Slots = template.Slot == null ? new List<string>() : new List<string>(template.Slot);			
			amount = 1;
			Type = template.Type;
			Weight = template.Weight;
			Size = template.Size;
			Value = template.Value;
		}
	}

	public enum StackType {
		None,
		Soft,
		Hard
	}

	public class ItemTemplate {
		public string Name { get; set; }
		public string RefId { get; set; }
		public ItemType Type { get; set; }
		public int Weight { get; set; }
		public int Size { get; set; }
		public int Value { get; set; }
		public string Asset { get; set; }
		public StackType StackType { get; set; }
		public List<string> Slot { get; set; }

		public IEnumerable<IItemComponentTemplate> Components { set; get; }
	}
}