using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;

namespace SkrGame.Actions.Features {
	public class LongAction : FeatureAction {
		public int TimeLeftInAP { get; set; }

		public LongAction(Entity entity, Entity feature, int timeInAP) : base(entity, feature) {
			this.TimeLeftInAP = timeInAP;
		}

		public override int APCost {
			get {
				var actionPointPerTurn = Entity.Get<ActorComponent>().AP.ActionPointPerTurn;
				return TimeLeftInAP < actionPointPerTurn ? TimeLeftInAP : actionPointPerTurn;
			}
		}

		public override ActionResult OnProcess() {
			var actionPointPerTurn = Entity.Get<ActorComponent>().AP.ActionPointPerTurn;

			World.Log.Normal(String.Format("{0} uses entity.  {1} left.", Identifier.GetNameOrId(Entity), TimeLeftInAP - actionPointPerTurn));

			if (TimeLeftInAP > actionPointPerTurn) {
				Entity.Get<ActorComponent>().Enqueue(new LongAction(Entity, Feature, TimeLeftInAP - actionPointPerTurn));
				return ActionResult.Success;
			}
			return ActionResult.Success;
		}
	}
}
