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
			Point newPosition = Entity.Get<Location>().Position + direction;

			var bumpablesAtLocation = Entity.Get<Location>().Level.GetEntitiesAt(newPosition).FilteredBy<OnBump>();
			bool blockedMovement = false;

			//todo bumpables add action
			foreach (var b in bumpablesAtLocation) {
				if (b.Get<OnBump>().Action(Entity, b) == OnBump.BumpResult.BlockMovement) {
					blockedMovement = true;
				}
			}

			if (!blockedMovement) {
//				Holder.Actions.Enqueue(new WalkAction(Entity, direction));
				return ActionResult.Success;
			} else {
				return ActionResult.Failed;				
			}
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
			Point newPosition = Entity.Get<Location>().Position + direction;
			
			if (Entity.Get<Location>().Level.IsWalkable(newPosition)) {
				Entity.Get<Location>().Position = newPosition;
				
				// check if we're near anything
				var nearEntities = Entity.Get<Location>().Level.GetEntities().Where(e => e.Has<PassiveFeature>());

				foreach (var e in nearEntities) {
					e.Get<PassiveFeature>().Near(Entity, e);
				}
			} else {
//				World.Instance.Log.Fail("There is something in the way.");
				return ActionResult.Aborted;
			}
			return ActionResult.Success;
		}
	}

	public class TestAction : ActionRequiresPrompt<int> {
		public TestAction(Entity entity, int apcost) : base(entity) {}

		public override int APCost {
			get { return 10; }
		}

		public override ActionResult OnProcess() {
			throw new NotImplementedException();
		}

		public override IEnumerable<int> Options {
			get { yield return 1; yield return 10; yield return 100; }
		}
	}
}
