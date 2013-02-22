using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actions;
using DEngine.Actor;

namespace SkrGame.Actions {
	public sealed class FireOnceAction : IAction {
		public FireOnceAction(int apCost, Func<ActionResult> process) {
			this.process = process;
			APCost = apCost;
		}

		private readonly Func<ActionResult> process;

		public int APCost { get; private set; }

		public PromptRequired RequiresPrompt { get { return PromptRequired.None; } }

		public ActionResult OnProcess() {
			return process();
		}
	}
}
