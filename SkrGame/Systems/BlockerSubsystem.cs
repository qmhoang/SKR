//using System;
//using DEngine.Components;
//using DEngine.Entities;
//using SkrGame.Universe.Entities.Features;
//
//namespace SkrGame.Systems {
//	public class BlockerSubsystem : EventSubsystem {
//		public BlockerSubsystem(EntityManager entityManager) : base(entityManager, typeof(Blocker), typeof(Location)) {
//			
//		}
//
//		protected override void EntityAddedToCollection(Entity entity) {
//			entity.Get<Blocker>().WalkableChanged += new Component.ComponentEventHandler<EventArgs>(BlockerSubsystem_WalkableChanged);
//		}
//
//		void BlockerSubsystem_WalkableChanged(Component sender, EventArgs e) {
//			GetEntity(sender).Get<Location>().Level.
//		}
//
//		protected override void EntityRemovedFromCollection(Entity entity) {
//			throw new NotImplementedException();
//		}
//	}
//}