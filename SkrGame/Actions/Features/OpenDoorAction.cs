using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using DEngine.Actions;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Features;

namespace SkrGame.Actions.Features {
	public class OpenDoorAction : LoggedAction {
		private Entity feature;

		public OpenDoorAction(Entity entity, Entity feature) : base(entity) {
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

			if (opening.Status == Opening.OpeningStatus.Closed) {
				if (feature.Has<Blocker>()) {
					feature.Get<Blocker>().Transparent = true;
					feature.Get<Blocker>().Walkable = opening.WalkableOpened;
				}					
				if (feature.Has<Sprite>())
					feature.Get<Sprite>().Asset = opening.OpenedAsset;

				opening.Status = Opening.OpeningStatus.Opened;

				World.Log.Normal(String.Format("{0} {1}.", Identifier.GetNameOrId(Entity), opening.OpenedDescription));
				return ActionResult.Success;
			}

			World.Log.Fail(String.Format("{0} tries to {1}, but can't since it is already open.", Identifier.GetNameOrId(Entity), opening.OpenedDescription));
			return ActionResult.Aborted;
		}
	}
}
