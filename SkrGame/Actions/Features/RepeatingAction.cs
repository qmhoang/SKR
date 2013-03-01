using System;
using System.Diagnostics.Contracts;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;

namespace SkrGame.Actions.Features {
	public sealed class RepeatingAction : LoggedAction {
		public RepeatingAction(Entity entity, int count, Func<Entity, ActionResult> repeat, Func<Entity, int> apCostFunc) : base(entity) {
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

			if ((result == ActionResult.Success || result == ActionResult.SuccessNoTime) && (Count > 1)) {
				Entity.Get<ActorComponent>().Enqueue(new RepeatingAction(Entity, Count - 1, Repeat, APCostFunc));
			}

			return result;			
		}
	}
}