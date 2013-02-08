using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Features;
using SkrGame.Universe.Entities.Items.Components;
using log4net;

namespace SkrGame.Gameplay {
	public static class Movement {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public static bool BumpDirection(Entity entity, Direction direction) {
			Contract.Requires<ArgumentNullException>(entity != null, "entity");

			Point newPosition = entity.Get<Location>().Position + direction;

			var level = entity.Get<Location>().Level;

			// first we see if there are any entities that we "bump" into when we walk onto new location
			var bumpablesAtNewLocation = level.GetEntitiesAt(newPosition).Where(e => e.Has<OnBump>());
			bool blockedMovement = false;

			foreach (var b in bumpablesAtNewLocation) {
				if (b.Get<OnBump>().Action(entity, b) == OnBump.BumpResult.BlockMovement) {
					blockedMovement = true;
				}
			}

			return !blockedMovement;
		}

		public static void MoveEntity(Entity entity, Point newPosition) {
			Contract.Requires<ArgumentNullException>(entity != null, "entity");
			Contract.Requires<ArgumentException>(entity.Has<Location>());
			Contract.Requires<ArgumentException>(entity.Has<ActionPoint>());

			// finally move onto the new location
			if (entity.Get<Location>().Level.IsWalkable(newPosition)) {
				entity.Get<Location>().Position = newPosition;
				entity.Get<ActionPoint>().ActionPoints -= World.SpeedToActionPoints(World.DEFAULT_SPEED);

				// check if we're near anything
				var nearEntities = entity.Get<Location>().Level.GetEntities().Where(e => e.Has<PassiveFeature>());

				foreach (var e in nearEntities) {
					e.Get<PassiveFeature>().Near(entity, e);
				}

			} else
				World.Instance.Log.Normal("There is something in the way.");
		}

		public static void Wait(Entity e) {
			Contract.Requires<ArgumentNullException>(e != null, "entity");
			Contract.Requires<ArgumentException>(e.Has<ActionPoint>());

			e.Get<ActionPoint>().ActionPoints -= e.Get<ActionPoint>().ActionPointPerTurn;
		}

//		public static void Move(Entity entity, Point direction, Func<Entity, Entity> selectWeapon, Func<IEnumerable<Entity>, Entity> selectTarget) {
//			Contract.Requires<ArgumentNullException>(entity != null, "entity");
//
//			Point newPosition = entity.Get<Location>().Position + direction;
//
//			var level = entity.Get<Location>().Level;
//
//			// first we see if there are any entities that we "bump" into when we walk onto new location
//			var bumpablesAtNewLocation = level.GetEntitiesAt(newPosition, typeof(OnBump)).ToList();
//			bool blockedMovement = false;
//
//			foreach (var b in bumpablesAtNewLocation) {
//				if (b.Get<OnBump>().Action(entity, b) == OnBump.BumpResult.BlockMovement) {
//					blockedMovement = true;
//				}
//			}
//
//			if (blockedMovement)
//				return;
//
//			// then we check for attackables
//			var actorsAtNewLocation = level.GetEntitiesAt(newPosition, typeof(DefendComponent)).ToList();
//
//			if (actorsAtNewLocation.Count > 0) {
//				var target = selectTarget(actorsAtNewLocation);
//				Combat.Combat.MeleeAttack(entity, selectWeapon(entity), target, target.Get<DefendComponent>().DefaultPart, World.MEAN);
//
//				return;	// attacking doesn't move you
//			}
//
//			// finally move onto the new location
//			if (level.IsWalkable(newPosition)) {
//				entity.Get<Location>().Position = newPosition;
//				entity.Get<ActionPoint>().ActionPoints -= World.SpeedToActionPoints(World.DEFAULT_SPEED);
//
//				// check if we're near anything
//				var nearEntities = level.EntityManager.Get(typeof(PassiveFeature)).ToList();
//
//				foreach (var e in nearEntities) {
//					e.Get<PassiveFeature>().Near(entity, e);
//				}
//
//			} else
//				World.Instance.AddMessage("There is something in the way.");
//
//		}
	}
}
