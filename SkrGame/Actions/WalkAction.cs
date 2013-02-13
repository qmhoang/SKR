using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using DEngine.Actions;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using DEngine.Extensions;
using SkrGame.Universe;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Features;

namespace SkrGame.Actions {
	public class BumpAction : ActorAction {
		private Direction direction;

		private static readonly int BUMP_COST = World.SpeedToActionPoints(World.DEFAULT_SPEED) / 10;

		public BumpAction(Entity entity, Direction direction) : base(entity) {
			Contract.Requires<ArgumentException>(entity.Has<Location>());			
			this.direction = direction;
		}

		public override int APCost {
			get { return BUMP_COST; }
		}

		public override ActionResult OnProcess() {
			Point newLocation = Entity.Get<Location>().Point + direction;

			var bumpablesAtLocation = Entity.Get<Location>().Level.GetEntitiesAt(newLocation).FilteredBy<OnBump>();
			bool blockedMovement = false;

			//todo bumpables add action
			foreach (var b in bumpablesAtLocation) {
				if (b.Get<OnBump>().Action(Entity, b) == OnBump.BumpResult.BlockMovement) {
					blockedMovement = true;
				}
			}

			if (!blockedMovement) {
				Entity.Get<ActorComponent>().Enqueue(new WalkAction(Entity, direction));
				return ActionResult.Success;
			} else {
				return ActionResult.Failed;				
			}
		}
	}

	public class WaitAction : ActorAction {
		public WaitAction(Entity entity) : base(entity) { }

		public override int APCost {
			get { return Entity.Get<ActorComponent>().AP.ActionPointPerTurn; }
		}

		public override ActionResult OnProcess() {
			return ActionResult.Success;
		}
	}

	public class WalkAction : ActorAction {
		private Direction direction;

		private static readonly int WALK_COST = World.SpeedToActionPoints(World.DEFAULT_SPEED);

		public WalkAction(Entity entity, Direction direction) : base(entity) {
			this.direction = direction;
		}

		public override int APCost {
			get { return WALK_COST; }
		}

		public override ActionResult OnProcess() {
			// note: check for actions on illegal entities
			var location = Entity.Get<Location>();

			Point newLocation = location.Point + direction;
			
			if (location.Level.IsWalkable(newLocation)) {
				location.Point = newLocation;
				
				// check if we're near anything
				var nearEntities = location.Level.GetEntities().Where(e => e.Has<PassiveFeature>());

				foreach (var e in nearEntities) {
					e.Get<PassiveFeature>().Near(Entity, e);
				}
			} else {

				location.Level.World.Log.Fail("There is something in the way.");
				return ActionResult.Aborted;
			}
			return ActionResult.Success;
		}
	}

//	public class TestAction : ActorAction {
//		public TestAction(Entity entity) : base(entity) {}
//
//		public override int APCost {
//			get { return 10; }
//		}
//
//		public override ActionResult OnProcess() {
//			if ()
//		}
//	}
}
