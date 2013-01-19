using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Universe.Entities.Features;
using SkrGame.Universe.Locations;

namespace SkrGame.Systems {
	public abstract class EventSubsystem {
		protected FilteredCollection Collection;
		protected Dictionary<UniqueId, EntityWrapper> WrapperLUT;

		protected abstract class EntityWrapper {
			protected Entity e;

			protected EntityWrapper(Entity e) {
				this.e = e;
			}
		}

		protected EventSubsystem(EntityManager entityManager, params Type[] types) {
			Collection = entityManager.Get(types);
			WrapperLUT = new Dictionary<UniqueId, EntityWrapper>();

			Collection.OnEntityAdd += CollectionOnCollectionAdd;
			Collection.OnEntityRemove += CollectionOnCollectionRemove;
		}

		protected void CollectionOnCollectionRemove(Entity entity) {
			AbstractOnRemove(entity);
			WrapperLUT.Remove(entity.Id);
		}

		protected void CollectionOnCollectionAdd(Entity entity)
		{
			var wrapper = AbstractOnAdd(entity);
			WrapperLUT.Add(entity.Id, wrapper);
		}

		protected abstract EntityWrapper AbstractOnAdd(Entity entity);
		protected abstract void AbstractOnRemove(Entity entity);
	}

	public class DoorSubsystem : EventSubsystem {

		private class DoorWrapper : EventSubsystem.EntityWrapper {
			public DoorWrapper(Entity e) : base(e) {

				if (e.Has<Blocker>()) {
					var blocker = e.Get<Blocker>();
					blocker.Walkable = e.Get<Opening>().Status != Opening.EntryStatus.Closed;
				}
				if (e.Has<Sprite>()) {
					var sprite = e.Get<Sprite>();
					sprite.Asset = e.Get<Opening>().Status == Opening.EntryStatus.Closed ? e.Get<Opening>().ClosedAsset : e.Get<Opening>().OpenedAsset;
				}
				
			}

			public void DoorWrapper_Used(object sender, DEngine.Core.EventArgs<Opening.EntryStatus> @event) {
				if (e.Has<Blocker>()) {
					var blocker = e.Get<Blocker>();
					blocker.Walkable = @event.Data != Opening.EntryStatus.Closed;
				}
				if (e.Has<Sprite>()) {
					var sprite = e.Get<Sprite>();
					sprite.Asset = e.Get<Opening>().Status == Opening.EntryStatus.Closed ? e.Get<Opening>().ClosedAsset : e.Get<Opening>().OpenedAsset;
				}
			}
			
		}

		public DoorSubsystem(EntityManager entityManager) : base(entityManager, typeof(Opening)) {
			foreach (var door in Collection) {
				CollectionOnCollectionAdd(door);
			}
		}

		protected override EntityWrapper AbstractOnAdd(Entity entity) {
			var wrapper = new DoorWrapper(entity);
			entity.Get<Opening>().Used += wrapper.DoorWrapper_Used;
			return wrapper;
		}

		protected override void AbstractOnRemove(Entity entity) {
			entity.Get<Opening>().Used -= ((DoorWrapper) WrapperLUT[entity.Id]).DoorWrapper_Used;
		}
	}
}
