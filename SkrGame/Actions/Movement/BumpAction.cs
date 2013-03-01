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
	public class BumpAction : ActorAction {
		private Direction direction;

		private const int BumpCost = World.OneSecondInAP / 10;

		public BumpAction(Entity entity, Direction direction) : base(entity) {
			Contract.Requires<ArgumentException>(entity.Has<GameObject>());			
			this.direction = direction;
		}

		public override int APCost {
			get { return BumpCost; }
		}

		public override ActionResult OnProcess() {
			Point newLocation = Entity.Get<GameObject>().Location + direction;

			var bumpablesAtLocation = Entity.Get<GameObject>().Level.GetEntitiesAt<OnBump>(newLocation);
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