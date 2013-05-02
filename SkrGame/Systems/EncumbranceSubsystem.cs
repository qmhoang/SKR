using DEngine.Entities;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;

namespace SkrGame.Systems {
	public class EncumbranceSubsystem {
		private FilteredCollection entities;
		private FilteredCollection containers;
		private FilteredCollection equipments;

		public EncumbranceSubsystem(EntityManager em) {
			entities = em.Get(typeof(Creature));
			containers = em.Get<ItemContainerComponent>();
			equipments = em.Get<EquipmentComponent>();

			containers.OnEntityAdd += containers_OnEntityAdd;
		}

		void containers_OnEntityAdd(Entity entity) {
			entity.Get<ItemContainerComponent>().ItemAdded += Container_ItemChanged;
		}

		void Container_ItemChanged(ItemContainerComponent sender, DEngine.Core.EventArgs<Entity> e) {
			
		}


	}

}