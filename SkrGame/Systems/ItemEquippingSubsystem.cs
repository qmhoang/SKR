using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;
using DEngine.Actor;
using DEngine.Core;
using DEngine.Entities;
using SkrGame.Universe.Entities.Actors;
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
}