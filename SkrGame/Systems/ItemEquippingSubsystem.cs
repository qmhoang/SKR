using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using SkrGame.Universe.Entities.Actors;
using log4net;

namespace SkrGame.Systems {
	public sealed class ItemEquippingSubsystem : EventSubsystem {
		public ItemEquippingSubsystem(EntityManager entityManager) : base(entityManager, typeof(Location), typeof(EquipmentComponent)) {
			// if containers already exist, add them
			foreach (var container in Collection) {
				EntityAddedToCollection(container);
			}
		}

		protected override void EntityRemovedFromCollection(Entity equipment) {
			equipment.Get<EquipmentComponent>().ItemEquipped -= ItemEquipped;
			equipment.Get<EquipmentComponent>().ItemUnequipped -= ItemUnequipped;
			equipment.Get<Location>().PositionChanged -= PositionChanged;

		}

		protected override void EntityAddedToCollection(Entity equipment) {
			equipment.Get<EquipmentComponent>().ItemEquipped += ItemEquipped;
			equipment.Get<EquipmentComponent>().ItemUnequipped += ItemUnequipped;
			equipment.Get<Location>().PositionChanged += PositionChanged;

			foreach (var equippedItem in equipment.Get<EquipmentComponent>().EquippedItems) {
				EquipItem(equipment, equippedItem);
			}
		}

		private static void EquipItem(Entity equipment, Entity equippedItem) {
			// we make the item we're equipping invisible
			if (equippedItem.Has<VisibleComponent>())
				equippedItem.Get<VisibleComponent>().VisibilityIndex = -1;
			
			// just in case, move the item to the entity's location
			if (equippedItem.Has<Location>())
				equippedItem.Get<Location>().Position = equipment.Get<Location>().Position;
		}

		void ItemUnequipped(Component sender, EventArgs<string, Entity> e) {
			var equipment = GetEntity(sender);
			
			if (e.Data2.Has<VisibleComponent>()) {
				e.Data2.Get<VisibleComponent>().Reset();
			}

			equipment.Get<Location>().PositionChanged -= PositionChanged;
		}

		void ItemEquipped(Component sender, EventArgs<string, Entity> e) {
			var equipment = GetEntity(sender);

			EquipItem(equipment, e.Data2);

			equipment.Get<Location>().PositionChanged += PositionChanged;
		}

		void PositionChanged(Component sender, EventArgs<Point> e) {
			var equipment = GetEntity(sender);

			foreach (var entity in equipment.Get<EquipmentComponent>().EquippedItems) {
				if (entity.Has<Location>()) {
					entity.Get<Location>().Position = e.Data;
				}
			}
		}
	}
}