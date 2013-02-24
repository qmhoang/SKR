using System;
using System.Diagnostics.Contracts;
using DEngine.Actions;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using DEngine.Extensions;
using SkrGame.Actions.Features;
using SkrGame.Universe;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Features;

namespace SkrGame.Actions.Movement {
	public class JumpOverAction : FeatureAction {
		public JumpOverAction(Entity entity, Entity feature) : base(entity, feature) {
			
		}
		
		public override int APCost {
			get { return World.TURN_LENGTH_IN_AP * 3; }
		}

		public override ActionResult OnProcess() {
			throw new NotImplementedException();
		}
	}

	public class BumpAction : ActorAction {
		private Direction direction;

		private const int BUMP_COST = World.TURN_LENGTH_IN_AP / 10;

		public BumpAction(Entity entity, Direction direction) : base(entity) {
			Contract.Requires<ArgumentException>(entity.Has<GameObject>());			
			this.direction = direction;
		}

		public override int APCost {
			get { return BUMP_COST; }
		}

		public override ActionResult OnProcess() {
			Point newLocation = Entity.Get<GameObject>().Location + direction;

			var bumpablesAtLocation = Entity.Get<GameObject>().Level.GetEntitiesAt(newLocation).FilteredBy<OnBump>();
			bool movementAllowed = true;

			//todo bumpables add action
			foreach (var b in bumpablesAtLocation) {
				if (b.Get<OnBump>().Action(Entity, b) == OnBump.BumpResult.BlockMovement) {
					movementAllowed = false;
				}
			}

			if (movementAllowed) {
				Entity.Get<ActorComponent>().Enqueue(new WalkAction(Entity, direction));
				return ActionResult.Success;
			} else {
				return ActionResult.Failed;				
			}
		}
	}
}