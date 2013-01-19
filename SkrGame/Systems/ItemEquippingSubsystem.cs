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

			Collection.OnEntityAdd += EntityAddedToCollection;
			Collection.OnEntityRemove += EntityRemovedFromCollection;
		}

		protected override void EntityRemovedFromCollection(Entity equipment) {
			equipment.Get<EquipmentComponent>().ItemEquipped -= ItemEquipped;
			equipment.Get<EquipmentComponent>().ItemUnequipped -= ItemUnequipped;
		}

		protected override void EntityAddedToCollection(Entity equipment) {
			equipment.Get<EquipmentComponent>().ItemEquipped += ItemEquipped;
			equipment.Get<EquipmentComponent>().ItemUnequipped += ItemUnequipped;

			foreach (var equippedItem in equipment.Get<EquipmentComponent>().EquippedItems) {
				if (equippedItem.Has<Location>()) {
					equippedItem.Get<Location>().Position = equipment.Get<Location>().Position;
				}
			}
		}

		void ItemUnequipped(Component sender, EventArgs<string, Entity> e) {
			var equipment = GetEntity(sender);

			equipment.Get<Location>().PositionChanged -= PositionChanged;
		}

		void ItemEquipped(Component sender, EventArgs<string, Entity> e) {
			var equipment = GetEntity(sender);

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