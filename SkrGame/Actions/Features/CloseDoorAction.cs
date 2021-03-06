using System;
using System.Diagnostics.Contracts;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Features;

namespace SkrGame.Actions.Features {
	public class CloseDoorAction : FeatureAction {
		public CloseDoorAction(Entity entity, Entity feature) : base(entity, feature) {
			Contract.Requires<ArgumentException>(feature.Has<Opening>());			
		}

		public override int APCost {
			get { return Feature.Get<Opening>().APCost; }
		}

		public override ActionResult OnProcess() {
			var opening = Feature.Get<Opening>();

			if (opening.Status == Opening.OpeningStatus.Opened) {
				if (Feature.Get<GameObject>().Level.IsWalkable(Entity.Get<GameObject>().Location) || !opening.WalkableOpened) {
					if (Feature.Has<Scenery>())
						Feature.Get<Scenery>().Transparent = Feature.Get<Scenery>().Walkable = false;
					if (Feature.Has<Sprite>())
						Feature.Get<Sprite>().Asset = opening.ClosedAsset;

					opening.Status = Opening.OpeningStatus.Closed;
					World.Log.NormalFormat("{0} {1}.", EntityName, opening.ClosedDescription);
					return ActionResult.Success;
				} else {
					World.Log.FailFormat("{0} tries to {1}, but can't.", EntityName, opening.ClosedDescription);
					return ActionResult.Failed;
				}
			}
			World.Log.AbortedFormat("{0} tries to {1}, but can't since it is already closed.", EntityName, opening.ClosedDescription);
			return ActionResult.Aborted;
		}
	}
}