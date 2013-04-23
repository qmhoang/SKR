using System;
using DEngine.Actions;
using DEngine.Actor;
using SkrGame.Universe;

namespace SkrGame.Actions {
	public sealed class RealTimeAction : IAction {
		private readonly int _apCost;
		private readonly Action _action;

		public RealTimeAction(int seconds, Action action) {
			_apCost = World.SecondsToActionPoints(seconds);
			this._action = action;
		}

		public int APCost {
			get { return _apCost; }
		}

		public ActionResult OnProcess() {
			_action();
			return ActionResult.Success;
		}
	}
}