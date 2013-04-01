using System;
using System.Collections.Generic;
using System.Globalization;
using DEngine.Actions;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;

namespace SkrGame.Universe {
	public class Calendar : Controller {
		private Calendar(TimeSpan timeSpan) {
			TimeSpan = timeSpan;
		}

		public static DateTime StartingDate = new DateTime(2013, 5, 13, 9, 4, 42);

		public DateTime DateTime { get { return StartingDate + TimeSpan; } }
		public TimeSpan TimeSpan { get; private set; }

		public Calendar() : this(new TimeSpan()) { }

		public override IAction NextAction() {
			return new CalendarAction(this);
		}

		public override Controller Copy() {
			return new Calendar(TimeSpan);
		}

		private void IncreaseTime(int seconds = 1, int milliseconds = 0) {
			TimeSpan += new TimeSpan(0, 0, 0, seconds, milliseconds);
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
