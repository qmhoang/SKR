using System;
using System.Diagnostics.Contracts;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Universe.Entities.Features;

namespace SkrGame.Actions.Features {
	public class ToggleDoorAction : LoggedAction {
		private Entity feature;

		public ToggleDoorAction(Entity entity, Entity feature) : base(entity) {
			Contract.Requires<ArgumentNullException>(entity != null, "entity");
			Contract.Requires<ArgumentNullException>(feature != null, "feature");
			Contract.Requires<ArgumentException>(feature.Has<Opening>());
			this.feature = feature;
		}

		public override int APCost {
			get { return 0; }
		}

		public override ActionResult OnProcess() {
			if (feature.Get<Opening>().Status == Opening.OpeningStatus.Closed)
				Entity.Get<ActorComponent>().Enqueue(new OpenDoorAction(Entity, feature));
			else
				Entity.Get<ActorComponent>().Enqueue(new CloseDoorAction(Entity, feature));
			return ActionResult.SuccessNoTime;
		}
	}
}