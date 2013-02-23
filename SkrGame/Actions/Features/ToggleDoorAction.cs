using System;
using System.Diagnostics.Contracts;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Universe.Entities.Features;

namespace SkrGame.Actions.Features {
	public class ToggleDoorAction : FeatureAction {
		public ToggleDoorAction(Entity entity, Entity feature) : base(entity, feature) {
			Contract.Requires<ArgumentException>(feature.Has<Opening>());
		}

		public override int APCost {
			get { return 1; }
		}

		public override ActionResult OnProcess() {
			if (Feature.Get<Opening>().Status == Opening.OpeningStatus.Closed)
				Entity.Get<ActorComponent>().Enqueue(new OpenDoorAction(Entity, Feature));
			else
				Entity.Get<ActorComponent>().Enqueue(new CloseDoorAction(Entity, Feature));
			return ActionResult.SuccessNoTime;
		}
	}
}