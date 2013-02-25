using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;

namespace SkrGame.Actions.Features {
	public sealed class RepeatAction : LoggedAction {
		public RepeatAction(Entity entity, int count, Func<Entity, ActionResult> repeat, Func<Entity, int> apCostFunc) : base(entity) {
			Contract.Requires<ArgumentException>(count > 0);
			Count = count;
			Repeat = repeat;
			APCostFunc = apCostFunc;
		}

		public int Count { get; private set; }

		public Func<Entity, ActionResult> Repeat { get; private set; }
		public Func<Entity, int> APCostFunc { get; private set; }

		public override int APCost {
			get { return APCostFunc(Entity); }
		}

		public override ActionResult OnProcess() {
			var result = Repeat(Entity);

			if ((result == ActionResult.Success || result == ActionResult.SuccessNoTime) && (Count > 0)) {
				Entity.Get<ActorComponent>().Enqueue(new RepeatAction(Entity, Count - 1, Repeat, APCostFunc));
			}

			return result;			
		}
	}

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
