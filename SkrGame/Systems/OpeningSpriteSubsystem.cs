//using System;
//using System.Collections.Generic;
//using System.Diagnostics.Contracts;
//using System.Linq;
//using System.Text;
//using DEngine.Actor;
//using DEngine.Components;
//using DEngine.Entities;
//using SkrGame.Universe;
//using SkrGame.Universe.Entities.Features;
//using SkrGame.Universe.Locations;
//
//namespace SkrGame.Systems {
//	public class OpeningSpriteSubsystem : EventSubsystem {
//		public OpeningSpriteSubsystem(EntityManager entityManager) : base(entityManager, typeof(Opening), typeof(Sprite)) {
//			foreach (var door in Collection) {
//				EntityAddedToCollection(door);
//			}
//		}
//
//		void OpeningUsed(Component sender, DEngine.Core.EventArgs<Opening.OpeningStatus> e) {
//			Contract.Requires<ArgumentNullException>(sender != null, "sender");
//			Contract.Requires<ArgumentNullException>(e != null, "e");
//			var entity = GetEntity(sender);
//
//			var sprite = entity.Get<Sprite>();
//			sprite.Asset = entity.Get<Opening>().Status == Opening.OpeningStatus.Closed ? entity.Get<Opening>().ClosedAsset : entity.Get<Opening>().OpenedAsset;
//		}
//
//		protected override void EntityRemovedFromCollection(Entity entity) {
//			entity.Get<Opening>().Used -= OpeningUsed;
//		}
//
//		protected override sealed void EntityAddedToCollection(Entity entity) {
//			entity.Get<Opening>().Used += OpeningUsed;
//
//		}
//	}
//
//	public class OpeningBlockerSubsystem : EventSubsystem {
//		public OpeningBlockerSubsystem(EntityManager entityManager)
//			: base(entityManager, typeof(Opening), typeof(Blocker)) {
//			foreach (var door in Collection) {
//				EntityAddedToCollection(door);
//			}
//		}
//
//		void OpeningUsed(Component sender, DEngine.Core.EventArgs<Opening.OpeningStatus> e) {
//			Contract.Requires<ArgumentNullException>(sender != null, "sender");
//			Contract.Requires<ArgumentNullException>(e != null, "e");
//			var entity = GetEntity(sender);
//
//			var blocker = entity.Get<Blocker>();
//			if (e.Data == Opening.OpeningStatus.Closed) {
//				blocker.Transparent = true;
//				blocker.Walkable = entity.Get<Opening>().WalkableOpened;
//			}
//			blocker.Walkable = e.Data != Opening.OpeningStatus.Closed;
//
//		}
//
//		protected override void EntityRemovedFromCollection(Entity entity) {
//			entity.Get<Opening>().Used -= OpeningUsed;
//		}
//
//		protected override sealed void EntityAddedToCollection(Entity entity) {
//			entity.Get<Opening>().Used += OpeningUsed;
//
//		}
//	}
//}
