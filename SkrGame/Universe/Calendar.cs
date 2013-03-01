using System;
using System.Collections.Generic;
using System.Globalization;
using DEngine.Actions;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;

namespace SkrGame.Universe {
	public class TimeElapsed : EventArgs {
		public int Milliseconds { get; private set; }

		public TimeElapsed(int milliseconds) {
			Milliseconds = milliseconds;
		}
	}

	public class Calendar : Controller {
		private Calendar(Queue<IAction> actions, TimeSpan timeSpan) : base(actions) {
			TimeSpan = timeSpan;
		}

		public static DateTime StartingDate = new DateTime(2013, 5, 13, 9, 4, 42);

		public DateTime DateTime { get { return StartingDate + TimeSpan; } }
		public TimeSpan TimeSpan { get; private set; }

		public event EventHandler<TimeElapsed> TimeChanged;

		private void OnTimeChanged(TimeElapsed e) {
			EventHandler<TimeElapsed> handler = TimeChanged;
			if (handler != null)
				handler(this, e);
		}

		public Calendar() : this(new Queue<IAction>(), new TimeSpan()) { }

		public override IAction NextAction() {
			return new CalendarAction(this);
		}

		public override Controller Copy() {
			return new Calendar(new Queue<IAction>(), TimeSpan);
		}

		private void IncreaseTime(int seconds = 1, int milliseconds = 0) {
			TimeSpan += new TimeSpan(0, 0, 0, seconds, milliseconds);
			OnTimeChanged(new TimeElapsed(seconds * 1000 + milliseconds));
		}

		private sealed class CalendarAction : IAction {
			private Calendar calendar;

			public CalendarAction(Calendar calendar) {
				this.calendar = calendar;
			}

			public int APCost {
				get { return World.OneSecondInAP / 10; }
			}

			public ActionResult OnProcess() {
				calendar.IncreaseTime(0, 100);
				return ActionResult.Success;
			}
		}
	}
}
