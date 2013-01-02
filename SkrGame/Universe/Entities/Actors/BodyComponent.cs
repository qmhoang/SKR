using System;
using System.Collections.Generic;
using System.Reflection;
using DEngine.Actor;
using DEngine.Core;
using DEngine.Entity;
using SkrGame.Gameplay.Combat;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Entities.Items.Components;
using log4net;
using log4net.Repository.Hierarchy;

namespace SkrGame.Universe.Entities.Actors {
	public class ItemHolder : EntityComponent {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private readonly ItemContainer inventory;

		public ItemHolder() {
			inventory = new ItemContainer();
		}

		/// <summary>
		/// Add item into inventory, return true if item was added, false if item couldn't for whatever reason
		/// </summary>
		public void AddItem(Item item) {
			if (item == null)
				throw new ArgumentException("item is null", "item");

			Logger.DebugFormat("{0} is adding {1} to his inventory.", OwnerUId, item.Name);
			inventory.AddItem(item);
			OnItemAdded(new EventArgs<Item>(item));			
		}

		public void RemoveItem(Item item) {
			if (item == null)
				throw new ArgumentException("item is null", "item");
			Logger.DebugFormat("{0} is removing {1} from his inventory.", OwnerUId, item.Name);
			OnItemRemoved(new EventArgs<Item>(item));
			inventory.RemoveItem(item);
		}

		public IEnumerable<Item> Items {
			get { return inventory; }
		}

		public event EventHandler<EventArgs<Item>> ItemRemoved;
		public event EventHandler<EventArgs<Item>> ItemAdded;


		public void OnItemAdded(EventArgs<Item> e) {
			EventHandler<EventArgs<Item>> handler = ItemAdded;
			if (handler != null)
				handler(this, e);
		}

		public void OnItemRemoved(EventArgs<Item> e) {
			EventHandler<EventArgs<Item>> handler = ItemRemoved;
			if (handler != null)
				handler(this, e);
		}
	}
	public class BodyComponent : EntityComponent {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		#region Health

		private int health;
		public int Health {
			get { return health; }
			set { health = value; OnHealthChange(); }
		}

		private int maxHealth;
		public int MaxHealth {
			get { return maxHealth; }
			set { maxHealth = value; OnHealthChange(); }
		}

		public event EventHandler<EventArgs> HealthChanged;

		protected void OnHealthChange() {
			var handler = HealthChanged;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		public bool Dead {
			get { return Health < 0; }
		}

		public void OnDeath() {
			//			Level.EntityManager.Remove(OwnerUId);
			//			World.Instance.AddMessage(String.Format("{0} has died.",  Name)); //todo
		}



		#endregion

		#region Items
		
		public event EventHandler<EventArgs<Item>> ItemEquipped;
		public event EventHandler<EventArgs<Item>> ItemUnequipped;

		public void OnItemEquipped(EventArgs<Item> e) {
			EventHandler<EventArgs<Item>> handler = ItemEquipped;
			if (handler != null)
				handler(this, e);
		}

		public void OnItemUnequipped(EventArgs<Item> e) {
			EventHandler<EventArgs<Item>> handler = ItemUnequipped;
			if (handler != null)
				handler(this, e);
		}

//		/// <summary>
//		/// Equip item into body part, any item already in the slot will be unequipped
//		/// </summary>
//		/// <returns>returns success if it was successful, aborted if item already equip couldn't fit in inventory</returns>
//		/// todo rethink this
//		public ActionResult Equip(BodySlot bpslot, Item item) {
//			var bp = GetBodyPart(bpslot);
//			if (item == null)
//				throw new ArgumentException("item is null", "item");
//			if (!item.Slot.HasFlag(bpslot))
//				throw new ArgumentException("part cannot use this item", "item");
//
//			// todo check if entity can carry item, if not drop it to the level
////			if (equippedItems.ContainsKey(bpslot))
////				if (!AddItem(equippedItems[bpslot]))
////					return ActionResult.Aborted;
////
////			// we have enough space in your inventory for this
////			equippedItems[bpslot] = item;
////			RemoveItem(item);
////			OnItemEquipped(new EventArgs<Item>(item));
////
////			Logger.InfoFormat("{0} is equipping {1} to {2}.", Name, item.Name, bpslot);
//			return ActionResult.Success;
//		}
//
//		// todo rethink this, abort = nothing happen, failed = couldn't fit in inventory to dropped?
//		public ActionResult Unequip(BodySlot bpslot) {
//			if (!equippedItems.ContainsKey(bpslot))
//				return ActionResult.Aborted;
////			if (AddItem(equippedItems[bpslot])) {
////				OnItemUnequipped(new EventArgs<Item>(equippedItems[bpslot]));
////				Logger.InfoFormat("{0} is unequipping {1} from {2}.", Name, equippedItems[bpslot].Name, bpslot);
////				equippedItems.Remove(bpslot);
////
////				return ActionResult.Success;
////			}
//			return ActionResult.Aborted;
//		}

		#endregion

		private readonly Dictionary<BodySlot, BodyPart> bodyParts;

		public IEnumerable<BodyPart> BodyPartsList {
			get { return bodyParts.Values; } 
		}

		public BodyPart GetBodyPart(BodySlot bp) {
			return bodyParts[bp];
		}



		public BodyComponent() {
			//			MaxHealth = Health = GetTalent("attrb_constitution").As<AttributeComponent>().Rank;

			bodyParts = new Dictionary<BodySlot, BodyPart>
			            {
			            		{BodySlot.Torso, new BodyPart("Torso", BodySlot.Torso, this, Health, 0)},
			            		{BodySlot.Head, new BodyPart("Head", BodySlot.Head, this, Health, -World.STANDARD_DEVIATION * 5 / 3)},
			            		{BodySlot.MainArm, new BodyPart("Right Arm", BodySlot.MainArm, this, Health / 2, -World.STANDARD_DEVIATION * 4 / 3)},
			            		{BodySlot.OffArm, new BodyPart("Left Arm", BodySlot.OffArm, this, Health / 2, -World.STANDARD_DEVIATION * 4 / 3)},
			            		{BodySlot.MainHand, new BodyPart("Main Hand", BodySlot.MainHand, this, Health / 3, -World.STANDARD_DEVIATION * 4 / 3)},
			            		{BodySlot.OffHand,new BodyPart("Off Hand", BodySlot.OffHand, this, Health / 3, -World.STANDARD_DEVIATION * 4 / 3)},
			            		{BodySlot.Leg, new BodyPart("Leg", BodySlot.Leg, this, Health / 2, -2)},
			            		{BodySlot.Feet, new BodyPart("Feet", BodySlot.Feet, this, Health / 3, -4)}
			            };			

		}


	}
}