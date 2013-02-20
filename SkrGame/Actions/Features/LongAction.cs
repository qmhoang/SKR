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
		public Func<Entity, ActionResult> Process { get; private set; }

		public LongAction(Entity entity,  int timeInAP, Func<Entity, ActionResult> process, Func<Entity, ActionResult> onFinish) : base(entity) {
			this.TimeLeftInAP = timeInAP;
			this.OnFinish = onFinish;
			this.Process = process;
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
				var result = Process(Entity);
				if (result == ActionResult.Aborted || result == ActionResult.Failed) {
					return result;
				} else if (result == ActionResult.SuccessNoTime) { // prevents infinite queuing
					Entity.Get<ActorComponent>().Enqueue(new LongAction(Entity, TimeLeftInAP - actionPointPerTurn, Process, OnFinish));
					return ActionResult.Success;
				} else {
					Entity.Get<ActorComponent>().Enqueue(new LongAction(Entity, TimeLeftInAP - actionPointPerTurn, Process, OnFinish));
					return result;
				}
			}

			return OnFinish(Entity);			
		}
	}
}
