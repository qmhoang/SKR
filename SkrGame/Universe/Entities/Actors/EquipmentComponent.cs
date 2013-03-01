using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using DEngine.Extensions;
using SkrGame.Actions;
using SkrGame.Universe.Entities.Items;
using log4net;

namespace SkrGame.Universe.Entities.Actors {
	public sealed class EquipmentComponent : Component {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private readonly Dictionary<string, Entity> equippedItems;
		private List<string> slots;

		public event ComponentEventHandler<EventArgs<string, Entity>> ItemEquipped;
		public event ComponentEventHandler<EventArgs<string, Entity>> ItemUnequipped;

		public void OnItemEquipped(EventArgs<string, Entity> e) {
			ComponentEventHandler<EventArgs<string, Entity>> handler = ItemEquipped;
			if (handler != null)
				handler(this, e);
		}

		public void OnItemUnequipped(EventArgs<string, Entity> e) {
			ComponentEventHandler<EventArgs<string, Entity>> handler = ItemUnequipped;
			if (handler != null)
				handler(this, e);
		}

		[Pure]
		public bool ContainSlot(string slot) {
			return slots.Contains(slot);
		}

		public IEnumerable<Entity> EquippedItems {
			get { return equippedItems.Values; }
		}

		public IEnumerable<string> Slots {
			get { return slots; }
		}

		public EquipmentComponent(IEnumerable<string> slots) {
			equippedItems = new Dictionary<string, Entity>();
			this.slots = new List<string>(slots);
		}

		[ContractInvariantMethod]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
		private void ObjectInvariant() {
			Contract.Invariant(slots != null);
			Contract.Invariant(equippedItems != null);
		}

		public void Equip(string slot, Entity item) {
			Contract.Requires<ArgumentNullException>(item != null);
			Contract.Requires<ArgumentException>(item.Has<Equipable>() && item.Get<Equipable>() != null);
			Contract.Requires<ArgumentException>(ContainSlot(slot), "Not a valid slot.");
			Contract.Requires<ArgumentException>(!IsSlotEquipped(slot), "slot already equipped");
			Contract.Requires<ArgumentException>(item.Get<Equipable>().SlotsOccupied.ContainsKey(slot));
			Contract.Ensures(equippedItems.ContainsKey(slot), "item is not equipped");

			var slotsOccupied = item.Get<Equipable>().SlotsOccupied[slot];

			Logger.DebugFormat("{0} is equipping {1} to {2}.  Slots \"used\": {3}", OwnerUId, item.Id, slot, slotsOccupied.GetEnumeratedString());
			
			OnItemEquipped(new EventArgs<string, Entity>(slot, item));

			foreach (var s in slotsOccupied) {
				equippedItems.Add(s, item);				
			}

		}

		public bool Unequip(string slot) {
			Contract.Requires<ArgumentException>(ContainSlot(slot), "Not a valid slot.");
			Contract.Ensures(!equippedItems.ContainsKey(slot), "item is still equipped");

			if (!equippedItems.ContainsKey(slot)) {
				return false;
			} else {
				Entity old = equippedItems[slot];

				var slotsOccuped = old.Get<Equipable>().SlotsOccupied[slot];

				Logger.DebugFormat("{0} is unequipping his item at {1}. Slots freed: {2}", OwnerUId, slot, slotsOccuped.GetEnumeratedString());

				foreach (var s in slotsOccuped) {
					equippedItems.Remove(s);					
				}
				
				OnItemUnequipped(new EventArgs<string, Entity>(slot, old));

				return true;
			}
		}

		[Pure]
		public bool IsSlotEquipped(string slot) {
			return equippedItems.ContainsKey(slot);
		}

		public Entity GetEquippedItemAt(string slot) {
			Contract.Requires<ArgumentException>(IsSlotEquipped(slot), "No item is equipped at slot.");
			Contract.Requires<ArgumentException>(ContainSlot(slot), "Not a valid slot.");
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(slot), "string \"slot\" cannot be null or empty");			

			return equippedItems[slot];					
		}

		public Entity this[string slot] {
			get {				
				return GetEquippedItemAt(slot);
			}			
		}

		public override Component Copy() {
			var equipment = new EquipmentComponent(new List<string>(slots));

			foreach (var equippedItem in equippedItems) {
				equipment.equippedItems.Add(equippedItem.Key, equippedItem.Value.Copy());
			}

			if (ItemEquipped != null)
				equipment.ItemEquipped = (ComponentEventHandler<EventArgs<string, Entity>>)ItemEquipped.Clone();
			if (ItemUnequipped != null)
				equipment.ItemUnequipped = (ComponentEventHandler<EventArgs<string, Entity>>)ItemUnequipped.Clone();

			return equipment;
		}
	}
}