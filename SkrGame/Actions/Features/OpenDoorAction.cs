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
	public class OpenDoorAction : FeatureAction {
		public OpenDoorAction(Entity entity, Entity feature) : base(entity, feature) {
			Contract.Requires<ArgumentException>(feature.Has<Opening>());			
		}
		
		public override int APCost {
			get { return Feature.Get<Opening>().APCost; }
		}

		public override ActionResult OnProcess() {
			var opening = Feature.Get<Opening>();

			if (opening.Status == Opening.OpeningStatus.Closed) {
				if (Feature.Has<LockedFeature>()) {
					if (Feature.Get<LockedFeature>().Status == LockStatus.Locked) {
						World.Log.Fail(String.Format("{0} tries to {1}, but can't since it is locked.", Identifier.GetNameOrId(Entity), opening.OpenedDescription));
						return ActionResult.Failed;
					}
				}
				if (Feature.Has<Blocker>()) {
					Feature.Get<Blocker>().Transparent = true;
					Feature.Get<Blocker>().Walkable = opening.WalkableOpened;
				}					
				if (Feature.Has<Sprite>())
					Feature.Get<Sprite>().Asset = opening.OpenedAsset;

				
				opening.Status = Opening.OpeningStatus.Opened;

				World.Log.Normal(String.Format("{0} {1}.", Identifier.GetNameOrId(Entity), opening.OpenedDescription));
				return ActionResult.Success;
			}

			World.Log.Fail(String.Format("{0} tries to {1}, but can't since it is already open.", Identifier.GetNameOrId(Entity), opening.OpenedDescription));
			return ActionResult.Aborted;
		}
	}
}
