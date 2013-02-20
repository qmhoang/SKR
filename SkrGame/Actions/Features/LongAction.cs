using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;

namespace SkrGame.Actions.Features {
	public class LongAction : LoggedAction {
		public int TimeLeftInAP { get; private set; }
		public Func<Entity, ActionResult> OnFinish { get; private set; } 

		public LongAction(Entity entity, Func<Entity, ActionResult> onFinish, int timeInAP) : base(entity) {
			this.TimeLeftInAP = timeInAP;
			this.OnFinish = onFinish;
		}

		public override int APCost {
			get {
				var actionPointPerTurn = Entity.Get<ActorComponent>().AP.ActionPointPerTurn;
				return TimeLeftInAP < actionPointPerTurn ? TimeLeftInAP : actionPointPerTurn;
			}
		}

		public override ActionResult OnProcess() {
			var actionPointPerTurn = Entity.Get<ActorComponent>().AP.ActionPointPerTurn;

			if (TimeLeftInAP > actionPointPerTurn) {
				Entity.Get<ActorComponent>().Enqueue(new LongAction(Entity, OnFinish, TimeLeftInAP - actionPointPerTurn));
				return ActionResult.Success;
			}

			return OnFinish(Entity);			
		}
	}
}
