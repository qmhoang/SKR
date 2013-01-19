using System;
using DEngine.Entities;

namespace SkrGame.Systems {
	public abstract class EventSubsystem {
		protected FilteredCollection Collection;
		private EntityManager em;
		
		public Entity GetEntity(Component c) {
			return em[c.OwnerUId];
		}

		
		protected EventSubsystem(EntityManager entityManager, params Type[] types) {
			em = entityManager;
			Collection = entityManager.Get(types);

			Collection.OnEntityAdd += EntityAddedToCollection;
			Collection.OnEntityRemove += EntityRemovedFromCollection;
		}

		protected abstract void EntityAddedToCollection(Entity entity);
		protected abstract void EntityRemovedFromCollection(Entity entity);
	}
}