//using System;
//using System.Collections.Generic;
//using System.Diagnostics.Contracts;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using DEngine.Actor;
//using DEngine.Components;
//using DEngine.Core;
//using DEngine.Entities;
//using SkrGame.Universe.Entities.Actors;
//using SkrGame.Universe.Entities.Items;
//using log4net;
//
//namespace SkrGame.Systems {
//	public sealed class ItemContainerInteractionSubsystem : EventSubsystem {
//		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
//
//		public ItemContainerInteractionSubsystem(EntityManager entityManager) : base(entityManager, typeof(ContainerComponent), typeof(Location)) {			
//			// if containers already exist, add them
//			foreach (var container in Collection) {
//				EntityAddedToCollection(container);
//			}
//		}
//
//		protected override void EntityRemovedFromCollection(Entity container) {
//			container.Get<ContainerComponent>().ItemAdded -= ItemAdded;
//			container.Get<ContainerComponent>().ItemRemoved -= ItemRemoved;
//			container.Get<Location>().PositionChanged -= PositionChanged;
//		}
//
//		protected override void EntityAddedToCollection(Entity container) {
//			container.Get<ContainerComponent>().ItemAdded += ItemAdded;
//			container.Get<ContainerComponent>().ItemRemoved += ItemRemoved;
//			container.Get<Location>().PositionChanged += PositionChanged;
//			
//			foreach (var entity in container.Get<ContainerComponent>()) {
//				if (entity.Has<VisibleComponent>()) {
//					entity.Get<VisibleComponent>().VisibilityIndex = -1;
//				}
//				if (entity.Has<Location>()) {
//					entity.Get<Location>().Position = container.Get<Location>().Position;
//				}
//			}
//		}
//
//		void ItemRemoved(Component sender, EventArgs<Entity> e) {
//			Contract.Requires<ArgumentNullException>(sender != null, "sender");
//			Contract.Requires<ArgumentNullException>(e != null, "e");
//
//			var inventory = GetEntity(sender);
//			
//			if (e.Data.Has<VisibleComponent>()) {
//				e.Data.Get<VisibleComponent>().Reset();
//			}
//
//			inventory.Get<Location>().PositionChanged -= PositionChanged;
//		}
//
//		void ItemAdded(Component sender, EventArgs<Entity> e) {
//			Contract.Requires<ArgumentNullException>(sender != null, "sender");
//			Contract.Requires<ArgumentNullException>(e != null, "e");
//
//			var inventory = GetEntity(sender);
//
//			if (e.Data.Has<VisibleComponent>()) {
//				e.Data.Get<VisibleComponent>().VisibilityIndex = -1;
//			}
//
//			inventory.Get<Location>().PositionChanged += PositionChanged;
//		}
//
//		void PositionChanged(Component sender, EventArgs<Point> e) {
//			Contract.Requires<ArgumentNullException>(sender != null, "sender");
//			Contract.Requires<ArgumentNullException>(e != null, "e");
//
//			var inventory = GetEntity(sender);
//
//			foreach (var entity in inventory.Get<ContainerComponent>()) {
//				if (entity.Has<Location>()) {
//					entity.Get<Location>().Position = e.Data;
//				}
//			}
//		}
//
//	}
//}
