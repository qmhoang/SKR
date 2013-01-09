using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entity;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;
using log4net;

namespace SkrGame.Systems {	
	public class ItemEquippingSubsystem {
		private FilteredCollection containers;
		private Dictionary<UniqueId, EquipmentHelper> equipments;


		private class EquipmentHelper {
			private Entity equipment;

			public EquipmentHelper(Entity equipment) {
				this.equipment = equipment;
			}

			public void InventoryHelper_ItemEquipped(object sender, EventArgs<string, Entity> itemEquippedEvent) {

			}

			public void InventoryHelper_ItemUnequipped(object sender, EventArgs<string, Entity> itemUnequippedEvent) {

			}
		}

		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


		public ItemEquippingSubsystem(EntityManager entityManager) {
			containers = entityManager.Get(typeof(EquipmentComponent), typeof(DefendComponent));
			equipments = new Dictionary<UniqueId, EquipmentHelper>();

			foreach (var container in containers) {
				Containers_OnContainerAddToManager(container);
			}

			containers.OnEntityAdd += Containers_OnContainerAddToManager;
			containers.OnEntityRemove += Containers_OnContainerRemoveFromManager;
		}

		private void Containers_OnContainerRemoveFromManager(Entity container) {
			Contract.Requires(equipments.ContainsKey(container.Id));

			var helper = equipments[container.Id];

			container.Get<EquipmentComponent>().ItemEquipped -= helper.InventoryHelper_ItemEquipped;
			container.Get<EquipmentComponent>().ItemUnequipped -= helper.InventoryHelper_ItemUnequipped;
			equipments.Remove(container.Id);
		}

		private void Containers_OnContainerAddToManager(Entity container) {
			Contract.Requires(!equipments.ContainsKey(container.Id));

			var helper = new EquipmentHelper(container);

			container.Get<EquipmentComponent>().ItemEquipped += helper.InventoryHelper_ItemEquipped;
			container.Get<EquipmentComponent>().ItemUnequipped += helper.InventoryHelper_ItemUnequipped;
			equipments.Add(container.Id, helper);
		}

	}
	public class ItemContainerInteractionSubsystem {
		
		private FilteredCollection containers;
		private Dictionary<UniqueId, InventoryHelper> inventories;

		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private class InventoryHelper {
			private Entity inventory;
			private Dictionary<UniqueId, ItemHelper> items;

			public class ItemHelper {
				Entity item;

				public ItemHelper(Entity item) {
					this.item = item;
				}

				public void ItemHelper_PositionChanged(object sender, EventArgs<Point> positionChangedEvent) {
					if (item.Has<Location>())
						item.Get<Location>().Position = positionChangedEvent.Data;
				}
			}

			public InventoryHelper(Entity inventory) {
				this.inventory = inventory;
				items = new Dictionary<UniqueId, ItemHelper>();
			}			

			[Pure]
			public void InventoryHelper_ItemAdded(object sender, EventArgs<Entity> itemAddedEvent) {
				AddListener(itemAddedEvent.Data);
			}

			[Pure]
			public void InventoryHelper_ItemRemoved(object sender, EventArgs<Entity> itemRemovedEvent) {
				Contract.Requires(Contain(itemRemovedEvent.Data.Id));

				var itemHelper = items[itemRemovedEvent.Data.Id];
				inventory.Get<Location>().PositionChanged -= itemHelper.ItemHelper_PositionChanged;
				items.Remove(itemRemovedEvent.Data.Id);
			}

			public void AddListener(Entity item) {
				Contract.Requires(!Contain(item.Id));

				if (item.Has<Location>())
					item.Get<Location>().Position = inventory.Get<Location>().Position;

				var itemHelper = new ItemHelper(item);
				inventory.Get<Location>().PositionChanged += itemHelper.ItemHelper_PositionChanged;
				items.Add(item.Id, itemHelper);
			}

			public bool Contain(UniqueId id) {
				return items.ContainsKey(id);
			}
		}

		public ItemContainerInteractionSubsystem(EntityManager entityManager) {
			containers = entityManager.Get(typeof(ContainerComponent), typeof(Location));
			inventories = new Dictionary<UniqueId, InventoryHelper>();

			// if containers already exist, add them
			foreach (var container in containers) {
				Containers_OnContainerAddToManager(container);
			}

			containers.OnEntityAdd += Containers_OnContainerAddToManager;
			containers.OnEntityRemove += Containers_OnContainerRemoveFromManager;
		}

		private void Containers_OnContainerRemoveFromManager(Entity container) {
			Contract.Requires(inventories.ContainsKey(container.Id));

			var helper = inventories[container.Id];
			container.Get<ContainerComponent>().ItemAdded -= helper.InventoryHelper_ItemAdded;
			container.Get<ContainerComponent>().ItemRemoved -= helper.InventoryHelper_ItemRemoved;

			inventories.Remove(container.Id);
		}

		private void Containers_OnContainerAddToManager(Entity container) {
			Contract.Requires(!inventories.ContainsKey(container.Id));

			var helper = new InventoryHelper(container);
			container.Get<ContainerComponent>().ItemAdded += helper.InventoryHelper_ItemAdded;
			container.Get<ContainerComponent>().ItemRemoved += helper.InventoryHelper_ItemRemoved;

			inventories.Add(container.Id, helper);

			// we need to add  listeners that are already inside the container
			foreach (var item in container.Get<ContainerComponent>().Items) {
				helper.AddListener(item);
			}
		}
		
	}

	
}
