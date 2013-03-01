using System;
using DEngine.Actions;
using DEngine.Actor;
using SkrGame.Universe;

namespace SkrGame.Actions {
	public sealed class RealTimeAction : IAction {
		private int apCost;
		private Action action;

		public RealTimeAction(int seconds, Action action) {
			apCost = World.SecondsToActionPoints(seconds);
			this.action = action;
		}

		public int APCost {
			get { return apCost; }
		}

		public ActionResult OnProcess() {
			action();
			return ActionResult.Success;
		}
	}
}