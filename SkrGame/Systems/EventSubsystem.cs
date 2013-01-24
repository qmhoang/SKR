using System;
using System.Diagnostics.Contracts;
using DEngine.Entities;

namespace SkrGame.Systems {
	public abstract class EventSubsystem {
		protected FilteredCollection Collection;
		private EntityManager em;
		
		public Entity GetEntity(Component c) {
			Contract.Requires<ArgumentNullException>(c != null, "c");
			return em[c.OwnerUId];
		}

		
		protected EventSubsystem(EntityManager entityManager, params Type[] types) {
			Contract.Requires<ArgumentNullException>(entityManager != null, "entityManager");
			Contract.Requires<ArgumentNullException>(types != null, "types");

			em = entityManager;
			Collection = entityManager.Get(types);

			Collection.OnEntityAdd += EntityAddedToCollection;
			Collection.OnEntityRemove += EntityRemovedFromCollection;
		}

		protected abstract void EntityAddedToCollection(Entity entity);
		protected abstract void EntityRemovedFromCollection(Entity entity);
	}
}