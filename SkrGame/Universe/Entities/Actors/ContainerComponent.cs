using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using DEngine.Core;
using DEngine.Entity;
using SkrGame.Universe.Entities.Items;
using log4net;

namespace SkrGame.Universe.Entities.Actors {
	public class ContainerComponent : EntityComponent {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private readonly List<Entity> itemContainer;

		public IEnumerable<Entity> Items {
			get { return itemContainer; }
		}

		public bool Exist(Predicate<Entity> match) {
			return itemContainer.Exists(match);
		}

		public Entity GetItem(Predicate<Entity> match) {
			return itemContainer.Find(match);
		}

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

		public ContainerComponent() {
			itemContainer = new List<Entity>();
			equippedItems = new Dictionary<string, Entity>();
			slots = new Dictionary<string, bool>()
			        {
			        		{"MainHand", true},
			        		{"OffHand", true},
			        };
		}

		/// <summary>
		/// Add item into inventory
		/// </summary>
		public void AddItem(Entity item) {
			Contract.Requires(item != null);

			if (itemContainer.Contains(item))
				return;

			Logger.DebugFormat("{0} is adding {1} to his inventory.", OwnerUId, item.Id);			

			itemContainer.Add(item);
			OnItemAdded(new EventArgs<Entity>(item));
		}

		public bool RemoveItem(Entity item) {
			Contract.Requires(item != null);

			if (!itemContainer.Contains(item))
				return false;

			Logger.DebugFormat("{0} is removing {1} to his inventory.", OwnerUId, item.Id);

			OnItemRemoved(new EventArgs<Entity>(item));
			itemContainer.Remove(item);
			return true;
		}


		private readonly Dictionary<string, Entity> equippedItems;
		private readonly Dictionary<string, bool> slots;

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
			return slots.ContainsKey(slot);
		}

		public IEnumerable<string> Slots {
			get { return slots.Keys; }
		}

		public void Equip(string slot, Entity item) {			
			Contract.Requires<ArgumentException>(item.Is<Item>());
//			Contract.Requires<ArgumentException>(slots.ContainsKey(slot), "invalid slot");

			Logger.DebugFormat("{0} is equipping {1} to {2}.", OwnerUId, item.Id, slot);
			OnItemEquipped(new EventArgs<string, Entity>(slot, item));

			if (equippedItems.ContainsKey(slot))
				Unequip(slot);

			equippedItems.Add(slot, item);		
		}
		
		public bool Unequip(string slot) {
//			Contract.Requires<ArgumentException>(slots.ContainsKey(slot), "invalid slot");
			Contract.Ensures(!equippedItems.ContainsKey(slot));

			Logger.DebugFormat("{0} is unequipping his item at {1}.", OwnerUId, slot);

			if (!equippedItems.ContainsKey(slot))
				return false;
			else {
				Entity old = equippedItems[slot];				
				equippedItems.Remove(slot);
				AddItem(old);

				OnItemUnequipped(new EventArgs<string, Entity>(slot, old));

				return true;
			}
		}

		public bool IsSlotEquipped(string slot) {
			return equippedItems.ContainsKey(slot);
		}

		public Entity GetItemAt(string slot) {
			try {
				return equippedItems[slot];				
			} catch (Exception) {
				return null;
			}
		}
	}
}