using System;
using System.Diagnostics.Contracts;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Universe;

namespace SkrGame.Actions.Features {
	/// <summary>
	/// Recursion like action that does something at every interval for a specified length of time.
	/// </summary>
	public sealed class IntervalAction : LoggedAction {
		public IntervalAction(Entity entity, TimeSpan length, TimeSpan interval, Func<Entity, ActionResult> onInterval, Func<Entity, ActionResult> onFinish) :
			this(entity, new TimeSpan(), length, interval, onInterval, onFinish) {

			Log.Normal("Press 'z' to cancel action.");
		}

		private IntervalAction(Entity entity, TimeSpan counter, TimeSpan length, TimeSpan interval, Func<Entity, ActionResult> onInterval, Func<Entity, ActionResult> onFinish) : base(entity) {
			Contract.Requires<ArgumentNullException>(onInterval != null, "onInterval");
			Contract.Requires<ArgumentNullException>(onFinish != null, "onFinish");
			
			this._counter = counter;
			Length = length;
			Interval = interval;
			OnInterval = onInterval;
			OnFinish = onFinish;	
		}

		public TimeSpan Length { get; private set; }
		public TimeSpan Interval { get; private set; }

		private TimeSpan _counter;

		public int LengthInAP { get { return World.SecondsToActionPoints(Length.TotalSeconds); } }
		
		public Func<Entity, ActionResult> OnInterval { get; private set; }

		// on finish will be called even if onInterval returns an abort or failed.
		public Func<Entity, ActionResult> OnFinish { get; private set; }

		public override int APCost {
			get {
				var actionPointPerTurn = Entity.Get<ControllerComponent>().AP.ActionPointPerTurn;
				return LengthInAP < actionPointPerTurn ? LengthInAP : actionPointPerTurn;
			}
		}

		public override ActionResult OnProcess() {
			var ts = new TimeSpan(0, 0, 0, 0, (int) Math.Round(World.ActionPointsToSeconds(APCost) * 1000));
			
			if (LengthInAP > APCost) {
				if (_counter > Interval) {
					var result = OnInterval(Entity);

					if (result == ActionResult.Aborted || result == ActionResult.Failed) {
						return OnFinish(Entity);
					} else if (result == ActionResult.SuccessNoTime) { // prevents infinite queuing
						Entity.Get<ControllerComponent>().Enqueue(new IntervalAction(Entity, _counter - Interval + ts, Length - ts, Interval, OnInterval, OnFinish));
						return ActionResult.Success;
					} else {
						Entity.Get<ControllerComponent>().Enqueue(new IntervalAction(Entity, _counter - Interval + ts, Length - ts, Interval, OnInterval, OnFinish));
						return result;
					}
				} else {
					Entity.Get<ControllerComponent>().Enqueue(new IntervalAction(Entity, _counter + ts, Length - ts, Interval, OnInterval, OnFinish));
					return ActionResult.Success;
				}
			}
			return OnFinish(Entity);
		}
	}
}