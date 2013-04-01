using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actions;
using DEngine.Actor;
using DEngine.Core;
using SkrGame.Actions.Features;
using SkrGame.Actions.Movement;

namespace SkrGame.Universe.Entities.Controllers {
	public class QueuedController : Controller {

		public override void Enqueue(IAction action) {
			Actions.Enqueue(action);
		}

		public override IAction NextAction() {
			return Actions.Count > 0 ? Actions.Dequeue() : new WaitAction(Holder.Entity);
		}

		public override bool HasActionsQueued {
			get { return Actions.Count > 0; }
		}

		public override Controller Copy() {
			return new QueuedController();
		}

		public Queue<IAction> Actions { get; private set; }

		public QueuedController() {
			Actions = new Queue<IAction>();
		}
	}
}
