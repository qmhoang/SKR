using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entity;
using SkrGame.Universe.Entities.Items;
using log4net;

namespace SkrGame.Universe.Entities.Actors {
	public class ContainerComponent : Component, ICollection<Entity> {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private readonly List<Entity> itemContainer;

		public event EventHandler<EventArgs<Entity>> ItemRemoved;
		public event EventHandler<EventArgs<Entity>> ItemAdded;


		public void OnItemAdded(EventArgs<Entity> e) {
			EventHandler<EventArgs<Entity>> handler = ItemAdded;
			if (handler != null)
				handler(this, e);
		}

		public void OnItemRemoved(EventArgs<Entity> e) {
			EventHandler<EventArgs<Entity>> handler = ItemRemoved;
			if (handler != null)
				handler(this, e);
		}

		internal ContainerComponent()
			: this(new List<string>
			        {
			        		"MainHand",
			        		"OffHand",
							"Torso"
			        }) { }

		internal ContainerComponent(IEnumerable<string> slots) {
			itemContainer = new List<Entity>();
			equippedItems = new Dictionary<string, Entity>();
			this.slots = new List<string>(slots);
		}

		public IEnumerator<Entity> GetEnumerator() {
			return itemContainer.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		
		public void Clear() {
			itemContainer.Clear();
		}

		public bool Contains(Entity item) {
			return itemContainer.Contains(item);
		}

		public void CopyTo(Entity[] array, int arrayIndex) {
			itemContainer.CopyTo(array, arrayIndex);
		}

		public int Count {
			get { return itemContainer.Count; }
		}

		public bool IsReadOnly {
			get { return false; }
		}
		
		public IEnumerable<Entity> Items {
			get { return itemContainer; }
		}

		public bool Exist(Predicate<Entity> match) {
			return itemContainer.Exists(match);
		}

		/// <summary>
		/// Get item that matches preditcate
		/// </summary>
		/// <param name="match"></param>
		/// <returns></returns>
		public Entity GetItem(Predicate<Entity> match) {
			return itemContainer.Find(match);
		}

		void ICollection<Entity>.Add(Entity item) {
			Add(item);
		}

		/// <summary>
		/// Add item into inventory
		/// </summary>
		public bool Add(Entity item) {
			Contract.Requires<ArgumentNullException>(item != null);

			if (Contains(item))
				return false;

			Logger.DebugFormat("{0} is adding {1} to his inventory.", OwnerUId, item.Id);						

			itemContainer.Add(item);
			OnItemAdded(new EventArgs<Entity>(item));

			return true;
		}

		public bool Remove(Entity item) {
			Contract.Requires<ArgumentNullException>(item != null);

			if (!itemContainer.Contains(item))
				return false;

			Logger.DebugFormat("{0} is removing {1} to his inventory.", OwnerUId, item.Id);

			OnItemRemoved(new EventArgs<Entity>(item));
			itemContainer.Remove(item);
			
			return true;
		}
		
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

		public void Equip(string slot, Entity item) {
			Contract.Requires<ArgumentNullException>(item != null);
			Contract.Requires<ArgumentException>(item.Has<Item>());
			Contract.Requires<ArgumentException>(ContainSlot(slot), "invalid slot");
			Contract.Requires<ArgumentException>(Contains(item));

			Contract.Ensures(equippedItems.ContainsKey(slot), "item is not equipped");			
			
			Logger.DebugFormat("{0} is equipping {1} to {2}.", OwnerUId, item.Id, slot);
			OnItemEquipped(new EventArgs<string, Entity>(slot, item));

			if (equippedItems.ContainsKey(slot))
				Unequip(slot);

			equippedItems.Add(slot, item);
			itemContainer.Remove(item);			
		}
		
		public bool Unequip(string slot) {
			Contract.Requires<ArgumentException>(ContainSlot(slot), "invalid slot");
			Contract.Ensures(!equippedItems.ContainsKey(slot), "item is still equipped");			

			Logger.DebugFormat("{0} is unequipping his item at {1}.", OwnerUId, slot);

			if (!equippedItems.ContainsKey(slot))
				return false;
			else {
				Entity old = equippedItems[slot];				
				equippedItems.Remove(slot);
				Add(old);

				OnItemUnequipped(new EventArgs<string, Entity>(slot, old));

				return true;
			}
		}

		public bool IsSlotEquipped(string slot) {
			return equippedItems.ContainsKey(slot);
		}
		
		public Entity GetEquippedItemAt(string slot) {
			Contract.Requires<ArgumentException>(string.IsNullOrEmpty(slot));
			try {
				return equippedItems[slot];				
			} catch (Exception) {
				return null;
			}
		}

		public override Component Copy() {
			var container = new ContainerComponent()
			                {
			                		slots = new List<string>(slots)
			                };

			foreach (var entity in itemContainer) {
				container.itemContainer.Add(entity.Copy());
			}

			foreach (var equippedItem in equippedItems) {
				container.equippedItems.Add(equippedItem.Key, equippedItem.Value.Copy());
			}

			if (ItemAdded != null)
				container.ItemAdded = (EventHandler<EventArgs<Entity>>)ItemAdded.Clone();
			if (ItemRemoved != null)
				container.ItemRemoved = (EventHandler<EventArgs<Entity>>)ItemRemoved.Clone();
			if (ItemEquipped != null)
				container.ItemEquipped = (EventHandler<EventArgs<string, Entity>>)ItemEquipped.Clone();
			if (ItemUnequipped != null)
				container.ItemUnequipped = (EventHandler<EventArgs<string, Entity>>)ItemUnequipped.Clone();
			
			return container;
		}
	}
}