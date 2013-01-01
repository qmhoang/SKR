using System;
using System.Collections.Generic;
using System.Linq;
using DEngine.Actor;
using DEngine.Core;
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

	public class Item {
		public string Name { get; private set; }

		// Asset string is a unique string that represents what each item looks like.  A GUI uses this to draw the item depending on each item
		public string Asset { get; private set; }
		public string RefId { get; private set; }
		public UniqueId UniqueId { get; private set; }
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
		public BodySlot Slot { get; private set; }


		private readonly Dictionary<Type, IItemComponent> components;

		public bool Is(Type action) {
			return components.ContainsKey(action);
		}

		/// <summary>
		/// Get Item's Component
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T As<T>() where T : IItemComponent {
			return (T)components.Values.First(c => c is T);
		}

		public T As<T>(Type action) where T : IItemComponent {
			if (Is(action))
				return (T)components[action];
			else
				throw new ArgumentException("This item has no component for this", "action");
		}

		public Item(ItemTemplate template) {
			Name = template.Name;
			RefId = template.RefId;
			Asset = template.Asset;

			StackType = template.StackType;
			Slot = template.Slot;
			amount = 1;
			Type = template.Type;
			UniqueId = new UniqueId();
			Weight = template.Weight;
			Size = template.Size;
			Value = template.Value;
			components = new Dictionary<Type, IItemComponent>();

			foreach (var componentTemplate in template.Components) {
				IItemComponent comp = componentTemplate.Construct(this);

				components.Add(comp.GetType(), comp);
			}
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
		public BodySlot Slot { get; set; }

		public IEnumerable<IItemComponentTemplate> Components { set; get; }
	}
}