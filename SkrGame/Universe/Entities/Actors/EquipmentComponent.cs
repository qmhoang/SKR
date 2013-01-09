using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;
using DEngine.Core;
using DEngine.Entity;
using SkrGame.Universe.Entities.Items;
using log4net;

namespace SkrGame.Universe.Entities.Actors {
	public class EquipmentComponent : Component {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private readonly Dictionary<string, Entity> equippedItems;
		private List<string> slots;

		public event EventHandler<EventArgs<string, Entity>> ItemEquipped;
		public event EventHandler<EventArgs<string, Entity>> ItemUnequipped;

		public void OnItemEquipped(EventArgs<string, Entity> e) {
			EventHandler<EventArgs<string, Entity>> handler = ItemEquipped;
			if (handler != null)
				handler(this, e);
		}

		public void OnItemUnequipped(EventArgs<string, Entity> e) {
			EventHandler<EventArgs<string, Entity>> handler = ItemUnequipped;
			if (handler != null)
				handler(this, e);
		}

		public bool ContainSlot(string slot) {
			return slots.Contains(slot);
		}

		public IEnumerable<string> Slots {
			get { return slots; }
		}

		internal EquipmentComponent()
			: this(new List<string>
			        {
			        		"Main Hand",
			        		"Off Hand",
							"Torso"
			        }) { }

		internal EquipmentComponent(IEnumerable<string> slots) {
			equippedItems = new Dictionary<string, Entity>();
			this.slots = new List<string>(slots);
		}

		public void Equip(string slot, Entity item) {
			Contract.Requires<ArgumentNullException>(item != null);
			Contract.Requires<ArgumentException>(item.Has<Item>());
			Contract.Requires<ArgumentException>(ContainSlot(slot), "invalid slot");
			Contract.Requires<ArgumentException>(!IsSlotEquipped(slot), "slot already equipped");
			Contract.Requires<ArgumentException>(item.Get<Item>().Slots.Contains(slot));
			Contract.Ensures(equippedItems.ContainsKey(slot), "item is not equipped");

			Logger.DebugFormat("{0} is equipping {1} to {2}.", OwnerUId, item.Id, slot);
			OnItemEquipped(new EventArgs<string, Entity>(slot, item));

//			if (equippedItems.ContainsKey(slot))
//				Unequip(slot, out removed);
//			else
//				removed = null;

			equippedItems.Add(slot, item);			
		}

		public bool Unequip(string slot, out Entity removed) {
			Contract.Requires<ArgumentException>(ContainSlot(slot), "invalid slot");
			Contract.Ensures(!equippedItems.ContainsKey(slot), "item is still equipped");

			Logger.DebugFormat("{0} is unequipping his item at {1}.", OwnerUId, slot);

			if (!equippedItems.ContainsKey(slot)) {
				removed = null;
				return false;
			} else {
				Entity old = equippedItems[slot];
				equippedItems.Remove(slot);
				removed = old;

				OnItemUnequipped(new EventArgs<string, Entity>(slot, old));

				return true;
			}
		}

		public bool IsSlotEquipped(string slot) {
			return equippedItems.ContainsKey(slot);
		}

		public Entity GetEquippedItemAt(string slot) {
			Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(slot));
			Contract.Requires<ArgumentException>(ContainSlot(slot));
			try {
				return equippedItems[slot];
			} catch (Exception) {
				return null;
			}
		}

		public Entity this[string slot] {
			get { return GetEquippedItemAt(slot); }			
		}

		public override Component Copy() {
			var equipment = new EquipmentComponent()
			{
				slots = new List<string>(slots)
			};

			foreach (var equippedItem in equippedItems) {
				equipment.equippedItems.Add(equippedItem.Key, equippedItem.Value.Copy());
			}

			if (ItemEquipped != null)
				equipment.ItemEquipped = (EventHandler<EventArgs<string, Entity>>)ItemEquipped.Clone();
			if (ItemUnequipped != null)
				equipment.ItemUnequipped = (EventHandler<EventArgs<string, Entity>>)ItemUnequipped.Clone();

			return equipment;
		}
	}
}