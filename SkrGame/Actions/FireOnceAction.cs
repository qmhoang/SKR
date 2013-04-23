using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actions;
using DEngine.Actor;

namespace SkrGame.Actions {
	public sealed class FireOnceAction : IAction {
		public FireOnceAction(int apCost, Func<ActionResult> process) {
			this._process = process;
			APCost = apCost;
		}

		private readonly Func<ActionResult> _process;

		public int APCost { get; private set; }

		public ActionResult OnProcess() {
			return _process();
		}
	}
}
