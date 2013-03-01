using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;

namespace SkrGame.Actions.Features {
	public sealed class LongAction : LoggedAction {
		public int LengthInAP { get; private set; }

		public Func<Entity, ActionResult> Repeat { get; private set; }
		public Func<Entity, ActionResult> OnFinish { get; private set; }

		public LongAction(Entity entity,  int lengthInAP, Func<Entity, ActionResult> repeat, Func<Entity, ActionResult> onFinish) : base(entity) {
			this.LengthInAP = lengthInAP;
			this.OnFinish = onFinish;
			this.Repeat = repeat;
		}

		public override int APCost {
			get {
				var actionPointPerTurn = Entity.Get<ActorComponent>().AP.ActionPointPerTurn;
				return LengthInAP < actionPointPerTurn ? LengthInAP : actionPointPerTurn;
			}
		}

		public override ActionResult OnProcess() {
			var actionPointPerTurn = Entity.Get<ActorComponent>().AP.ActionPointPerTurn;

			if (LengthInAP > actionPointPerTurn) {
				var result = Repeat(Entity);
				if (result == ActionResult.Aborted || result == ActionResult.Failed) {
					return result;
				} else if (result == ActionResult.SuccessNoTime) { // prevents infinite queuing
					Entity.Get<ActorComponent>().Enqueue(new LongAction(Entity, LengthInAP - actionPointPerTurn, Repeat, OnFinish));
					return ActionResult.Success;
				} else {
					Entity.Get<ActorComponent>().Enqueue(new LongAction(Entity, LengthInAP - actionPointPerTurn, Repeat, OnFinish));
					return result;
				}
			}

			return OnFinish(Entity);			
		}
	}
}
