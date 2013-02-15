using System;
using System.Diagnostics.Contracts;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Features;

namespace SkrGame.Actions.Features {
	public class CloseDoorAction : LoggedAction {
		private Entity feature;

		public CloseDoorAction(Entity entity, Entity feature) : base(entity) {
			Contract.Requires<ArgumentNullException>(entity != null, "entity");
			Contract.Requires<ArgumentNullException>(feature != null, "feature");
			Contract.Requires<ArgumentException>(feature.Has<Opening>());
			this.feature = feature;
		}

		public override int APCost {
			get { return feature.Get<Opening>().APCost; }
		}

		public override ActionResult OnProcess() {
			var opening = feature.Get<Opening>();

			if (opening.Status == Opening.OpeningStatus.Opened) {
				if (feature.Get<Location>().Level.IsWalkable(Entity.Get<Location>().Point) || !opening.WalkableOpened) {
					if (feature.Has<Blocker>())
						feature.Get<Blocker>().Transparent = feature.Get<Blocker>().Walkable = false;
					if (feature.Has<Sprite>())
						feature.Get<Sprite>().Asset = opening.ClosedAsset;

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