using System;
using SkrGame.Universe.Entities.Actors;

namespace SkrGame.Conditions {
	public class PoisonCondition : Condition {
		public TimeSpan Length { get; private set; }
		public TimeSpan Interval { get; private set; }
		public TimeSpan Counter { get; private set; }

		public PoisonCondition(TimeSpan length, TimeSpan interval) {
			this.Length = length;
			this.Interval = interval;
			Counter = new TimeSpan();
		}

		public override Condition Copy() {
			return new PoisonCondition(Length, Interval)
			       {
			       		Counter = Counter
			       };
		}

		protected override void ConditionUpdate(int millisecondsElapsed) {
			var ts = new TimeSpan(0, 0, 0, 0, millisecondsElapsed);
			Length -= ts;
			Counter += ts;

			while (Counter.Ticks / Interval.Ticks > 0) {
				Holder.Entity.Get<DefendComponent>().Health.Value--;

				Counter -= Interval;
			}

			if (Length.TotalMilliseconds <= 0) {
				End();
			}
		}
	}
}