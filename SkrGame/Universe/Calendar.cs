using System;
using System.Globalization;
using DEngine.Actions;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;

namespace SkrGame.Universe {
	public class Calendar : Controller {
		public DateTime Time { get; private set; }
		
		public Calendar() {
			Time = new DateTime(2013, 5, 26);
		}

		public Calendar(DateTime timeSpan) {
			this.Time = timeSpan;
		}

		public override IAction NextAction() {
			return new CalendarAction(this);
		}

		public override Controller Copy() {
			return new Calendar(Time);
		}

		public void IncreaseTime(int seconds = World.TURN_LENGTH_IN_SECONDS) {
			Time = Time.AddSeconds(seconds);
		}

		public sealed class CalendarAction : IAction {
			private Calendar calendar;

			public CalendarAction(Calendar calendar) {
				this.calendar = calendar;
			}

			public int APCost {
				get { return World.SpeedToActionPoints(World.DEFAULT_SPEED); }
			}

			public ActionResult OnProcess() {
				calendar.IncreaseTime();
				return ActionResult.Success;
			}
		}
	}
}
