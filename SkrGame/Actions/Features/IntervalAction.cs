using System;
using System.Diagnostics.Contracts;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Universe;

namespace SkrGame.Actions.Features {
	public sealed class IntervalAction : LoggedAction {
		public IntervalAction(Entity entity, TimeSpan length, TimeSpan interval, Func<Entity, ActionResult> onInterval, Func<Entity, ActionResult> onFinish) :
				this(entity, new TimeSpan(), length, interval, onInterval, onFinish) {}

		private IntervalAction(Entity entity, TimeSpan counter, TimeSpan length, TimeSpan interval, Func<Entity, ActionResult> onInterval, Func<Entity, ActionResult> onFinish) : base(entity) {
			Contract.Requires<ArgumentNullException>(onInterval != null, "onInterval");
			Contract.Requires<ArgumentNullException>(onFinish != null, "onFinish");
			
			this.counter = counter;
			Length = length;
			Interval = interval;
			OnInterval = onInterval;
			OnFinish = onFinish;
		}

		public TimeSpan Length { get; private set; }
		public TimeSpan Interval { get; private set; }
		private TimeSpan counter;

		public int LengthInAP { get { return World.SecondsToActionPoints(Length.TotalSeconds); } }
		
		public Func<Entity, ActionResult> OnInterval { get; private set; }
		public Func<Entity, ActionResult> OnFinish { get; private set; }

		public override int APCost {
			get {
				var actionPointPerTurn = Entity.Get<ActorComponent>().AP.ActionPointPerTurn;
				return LengthInAP < actionPointPerTurn ? LengthInAP : actionPointPerTurn;
			}
		}

		public override ActionResult OnProcess() {
			var ts = new TimeSpan(0, 0, 0, 0, (int) World.ActionPointsToSeconds(APCost) * 1000);
			
			if (LengthInAP > APCost) {
				if (counter > Interval) {
					var result = OnInterval(Entity);

					if (result == ActionResult.Aborted || result == ActionResult.Failed) {
						return result;
					} else if (result == ActionResult.SuccessNoTime) { // prevents infinite queuing
						Entity.Get<ActorComponent>().Enqueue(new IntervalAction(Entity, counter - Interval, Length - ts, Interval, OnInterval, OnFinish));
						return ActionResult.Success;
					} else {
						Entity.Get<ActorComponent>().Enqueue(new IntervalAction(Entity, counter - Interval, Length - ts, Interval, OnInterval, OnFinish));
						return result;
					}
				} else {
					Entity.Get<ActorComponent>().Enqueue(new IntervalAction(Entity, counter + ts, Length - ts, Interval, OnInterval, OnFinish));
					return ActionResult.Success;
				}
			}
			return OnFinish(Entity);
		}
	}
}