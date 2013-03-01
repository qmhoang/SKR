using System;
using DEngine.Components;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;

namespace SkrGame.Effects {
	public class PoisonEffect : Effect {
		public TimeSpan Length { get; private set; }
		public TimeSpan Interval { get; private set; }
		public TimeSpan Counter { get; private set; }

		public PoisonEffect(TimeSpan length, TimeSpan interval) {
			this.Length = length;
			this.Interval = interval;
			Counter = new TimeSpan();
		}

		public override Effect Copy() {
			return new PoisonEffect(Length, Interval)
			       {
			       		Counter = Counter
			       };
		}

		protected override void OnTick(int millisecondsElapsed) {
			var ts = new TimeSpan(0, 0, 0, 0, millisecondsElapsed);
			Length -= ts;
			Counter += ts;

			while (Counter.Ticks / Interval.Ticks > 0) {
				Holder.Entity.Get<DefendComponent>().Health.Value--;

				Holder.Entity.Get<GameObject>().Level.World.Log.Bad(String.Format("{0} is poisoned.", Identifier.GetNameOrId(Holder.Entity)));

				Counter -= Interval;
			}

			if (Length.TotalMilliseconds <= 0) {
				End();
			}
		}
	}
}