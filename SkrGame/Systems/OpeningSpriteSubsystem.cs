using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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

		void EntryUsed(Component sender, DEngine.Core.EventArgs<Opening.OpeningStatus> e) {
			Contract.Requires<ArgumentNullException>(sender != null, "sender");
			Contract.Requires<ArgumentNullException>(e != null, "e");
			var entity = GetEntity(sender);
			if (entity.Has<Blocker>()) {
				var blocker = entity.Get<Blocker>();
				blocker.Walkable = e.Data != Opening.OpeningStatus.Closed;
			}
			if (entity.Has<Sprite>()) {
				var sprite = entity.Get<Sprite>();
				sprite.Asset = entity.Get<Opening>().Status == Opening.OpeningStatus.Closed ? entity.Get<Opening>().ClosedAsset : entity.Get<Opening>().OpenedAsset;
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
