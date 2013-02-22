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
					if (Feature.Has<Blocker>())
						Feature.Get<Blocker>().Transparent = Feature.Get<Blocker>().Walkable = false;
					if (Feature.Has<Sprite>())
						Feature.Get<Sprite>().Asset = opening.ClosedAsset;

					opening.Status = Opening.OpeningStatus.Closed;
					World.Log.Normal(String.Format("{0} {1}.", Identifier.GetNameOrId(Entity), opening.ClosedDescription));
					return ActionResult.Success;
				} else {
					World.Log.Normal(String.Format("{0} tries to {1}, but can't.", Identifier.GetNameOrId(Entity), opening.ClosedDescription));
					return ActionResult.Failed;
				}
			}
			World.Log.Fail(String.Format("{0} tries to {1}, but can't since it is already closed.", Identifier.GetNameOrId(Entity), opening.ClosedDescription));
			return ActionResult.Aborted;
		}
	}
}