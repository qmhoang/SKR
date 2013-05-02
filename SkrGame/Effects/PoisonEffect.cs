using System;
using DEngine.Components;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;

namespace SkrGame.Effects {
	public class TestingPoisonEffect : Effect {
		public TimeSpan Length { get; private set; }
		public TimeSpan Interval { get; private set; }
		public TimeSpan Counter { get; private set; }

		public TestingPoisonEffect(TimeSpan length, TimeSpan interval) {
			this.Length = length;
			this.Interval = interval;
			Counter = new TimeSpan();
		}

		public override Effect Copy() {
			return new TestingPoisonEffect(Length, Interval)
			       {
			       		Counter = Counter
			       };
		}

		protected override void OnTick(int apElapsed) {
			var ts = new TimeSpan(0, 0, 0, 0, apElapsed);
			Length -= ts;
			Counter += ts;

			while (Counter.Ticks / Interval.Ticks > 0) {
				Holder.Entity.Get<BodyComponent>().Health.Value--;

				// todo ew, fix this
				Holder.Entity.Get<GameObject>().Level.World.Log.BadFormat("{0} is poisoned.", Identifier.GetNameOrId(Holder.Entity));

				Counter -= Interval;
			}

			if (Length.TotalMilliseconds <= 0) {
				End();
			}
		}
	}
}