using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Universe;

namespace SkrGame.Actions.Features {
	public sealed class LongAction : LoggedAction {
		public int LengthInAP { get; private set; }

		public Func<Entity, ActionResult> OnRepeat { get; private set; }
		public Func<Entity, ActionResult> OnFinish { get; private set; }

		public LongAction(Entity entity, TimeSpan length, Func<Entity, ActionResult> onRepeat, Func<Entity, ActionResult> onFinish) :
				this(entity, World.SecondsToActionPoints(length.TotalSeconds), onRepeat, onFinish) {
			Log.Normal("Press 'z' to cancel action.");			
		}

		private LongAction(Entity entity, int lengthInAP, Func<Entity, ActionResult> onRepeat, Func<Entity, ActionResult> onFinish) : base(entity) {
			this.LengthInAP = lengthInAP;
			this.OnFinish = onFinish;
			this.OnRepeat = onRepeat;
		}

		public override int APCost {
			get {
				var actionPointPerTurn = Entity.Get<ActorComponent>().AP.ActionPointPerTurn;
				return LengthInAP < actionPointPerTurn ? LengthInAP : actionPointPerTurn;
			}
		}

		public override ActionResult OnProcess() {
			if (LengthInAP > APCost) {
				var result = OnRepeat(Entity);
				if (result == ActionResult.Aborted || result == ActionResult.Failed) {
					return result;
				} else if (result == ActionResult.SuccessNoTime) { // prevents infinite queuing
					Entity.Get<ActorComponent>().Enqueue(new LongAction(Entity, LengthInAP - APCost, OnRepeat, OnFinish));
					return ActionResult.Success;
				} else {
					Entity.Get<ActorComponent>().Enqueue(new LongAction(Entity, LengthInAP - APCost, OnRepeat, OnFinish));
					return result;
				}
			}

			return OnFinish(Entity);			
		}
	}
}
