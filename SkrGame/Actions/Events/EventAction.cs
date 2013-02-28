using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actions;
using DEngine.Actor;
using DEngine.Entities;

namespace SkrGame.Actions.Events {
	public abstract class EventAction : LoggedAction {
		protected EventAction(Entity entity) : base(entity) { }

		public override int APCost {
			get { return 1; }
		}

		public override ActionResult OnProcess() {
			Fire();
			return ActionResult.SuccessNoTime;
		}

		protected abstract void Fire();
	}
}
