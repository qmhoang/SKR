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
	public class Inventory : EntityComponent {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private readonly ItemContainer inventory;

		public Inventory() {
			inventory = new ItemContainer();
			equippedItems = new Dictionary<string, Item>();
			slots = new Dictionary<string, bool>()
			        {
			        		{"MainHand", true},
			        		{"OffHand", true},
			        };
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

		private readonly Dictionary<string, Item> equippedItems;
		private readonly Dictionary<string, bool> slots; 

		public event EventHandler<EventArgs<string, Item>> ItemEquipped;
		public event EventHandler<EventArgs<string, Item>> ItemUnequipped;

		public void OnItemEquipped(EventArgs<string, Item> e) {
			EventHandler<EventArgs<string, Item>> handler = ItemEquipped;
			if (handler != null)
				handler(this, e);
		}

		public void OnItemUnequipped(EventArgs<string, Item> e) {
			EventHandler<EventArgs<string, Item>> handler = ItemUnequipped;
			if (handler != null)
				handler(this, e);
		}

		public void Equip(string slot, Item item) {
			if (!slots.ContainsKey(slot))
				throw new ArgumentException("does not contain this slot");

			Logger.DebugFormat("{0} is equipping {1} to {2}.", OwnerUId, item.Name, slot);
			OnItemEquipped(new EventArgs<string, Item>(slot, item));

			if (equippedItems.ContainsKey(slot))
				Unequip(slot);

			equippedItems.Add(slot, item);		
		}

		public bool Unequip(string slot) {
			if (!slots.ContainsKey(slot))
				throw new ArgumentException("does not contain this slot");

			Logger.DebugFormat("{0} is unequipping his item at {1}.", OwnerUId, slot);

			if (!equippedItems.ContainsKey(slot))
				return false;
			else {
				Item old = equippedItems[slot];				
				equippedItems.Remove(slot);
				AddItem(old);

				OnItemUnequipped(new EventArgs<string, Item>(slot, old));

				return true;
			}
		}

		public bool IsSlotEquipped(string slot) {
			return equippedItems.ContainsKey(slot);
		}

	}
	/// <summary>
	/// Add this to an entity if the entity can be attacked, it contains its defense information
	/// </summary>
	public class DefendComponent : EntityComponent {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public class AttackablePart {
			public string Name { get; private set; }
			public DefendComponent Owner { get; private set; }
			public int Health { get; set; }
			public int MaxHealth { get; private set; }
			public int TargettingPenalty { get; private set; }

			public AttackablePart(string name, int maxHealth, int targettingPenalty, DefendComponent owner) {
				Name = name;
				Health = maxHealth;
				MaxHealth = maxHealth;
				TargettingPenalty = targettingPenalty;
				Owner = owner;
			}
		}

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
		}

		#endregion

		private readonly List<AttackablePart> bodyParts;

		public IEnumerable<AttackablePart> BodyPartsList {
			get { return bodyParts; } 
		}

		public AttackablePart GetRandomPart() {
//			int roll = Rng.Roll(3, 6);
//
//			Logger.InfoFormat("Random body part roll: {0}", roll);
			//todo unfinished
//			if (roll <= 4)
//				return target.GetBodyPart(BodySlot.Head);
//			if (roll <= 5)
//				return target.GetBodyPart(BodySlot.OffHand);
//			if (roll <= 7)
//				return target.GetBodyPart(BodySlot.Leg); // left leg
//			if (roll <= 8)
//				return target.GetBodyPart(BodySlot.OffArm);
//			if (roll <= 11)
//				return target.GetBodyPart(BodySlot.Torso);
//			if (roll <= 12)
//				return target.GetBodyPart(BodySlot.MainArm);
//			if (roll <= 14)
//				return target.GetBodyPart(BodySlot.Leg); // right leg
//			if (roll <= 15)
//				return target.GetBodyPart(BodySlot.MainHand);
//			if (roll <= 16)
//				return target.GetBodyPart(BodySlot.Feet);
//			else
//				return target.GetBodyPart(BodySlot.Head);

			return bodyParts[Rng.Int(bodyParts.Count)];
		}


		public DefendComponent() {		
			maxHealth = health = 50;
			bodyParts = new List<AttackablePart>()
			            {
			            		new AttackablePart("Torso", health, 0, this),
			            		new AttackablePart("Head", health, -World.STANDARD_DEVIATION * 5 / 3, this),
			            		new AttackablePart("Right Arm", health / 2, -World.STANDARD_DEVIATION * 4 / 3, this),
			            		new AttackablePart("Left Arm", health / 2, -World.STANDARD_DEVIATION * 4 / 3, this),
			            		new AttackablePart("Main Hand", health / 3, -World.STANDARD_DEVIATION * 4 / 3, this),
			            		new AttackablePart("Off Hand", health / 3, -World.STANDARD_DEVIATION * 4 / 3, this),
			            		new AttackablePart("Leg", health / 2, -World.STANDARD_DEVIATION * 2 / 3, this),
			            		new AttackablePart("Feet", health / 3, -World.STANDARD_DEVIATION * 2 / 3, this),
			            };


		}
	}
}