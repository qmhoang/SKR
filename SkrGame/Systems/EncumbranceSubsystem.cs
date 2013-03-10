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
			containers = em.Get<ContainerComponent>();
			equipments = em.Get<EquipmentComponent>();
		}
	}

}