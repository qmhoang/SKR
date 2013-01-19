using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Features;
using SkrGame.Universe.Locations;

namespace SkrGame.Systems {
	public class OpeningSpriteSubsystem : EventSubsystem {
		public OpeningSpriteSubsystem(EntityManager entityManager) : base(entityManager, typeof(Opening)) {
			foreach (var door in Collection) {
				EntityAddedToCollection(door);
			}
		}

		void EntryUsed(Component sender, DEngine.Core.EventArgs<Opening.OpeningStatus> @event) {
			var e = GetEntity(sender);
			if (e.Has<Blocker>()) {
				var blocker = e.Get<Blocker>();
				blocker.Walkable = @event.Data != Opening.OpeningStatus.Closed;
			}
			if (e.Has<Sprite>()) {
				var sprite = e.Get<Sprite>();
				sprite.Asset = e.Get<Opening>().Status == Opening.OpeningStatus.Closed ? e.Get<Opening>().ClosedAsset : e.Get<Opening>().OpenedAsset;
			}
		}

		protected override void EntityRemovedFromCollection(Entity entity) {
			entity.Get<Opening>().Used -= EntryUsed;
		}

		protected override sealed void EntityAddedToCollection(Entity entity) {
			entity.Get<Opening>().Used += EntryUsed;

		}
	}
}
