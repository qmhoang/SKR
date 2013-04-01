using System.Collections.Generic;
using DEngine.Actions;
using DEngine.Actor;

namespace SkrGame.Universe.Entities.Controllers {
	public class Player : Controller {
		public Player() : this(new Queue<IAction>()) { } 

		private Player(Queue<IAction> actions) {
			Actions = actions;
		}

		public Queue<IAction> Actions { get; private set; }

		public override void Enqueue(IAction action) {
			Actions.Enqueue(action);
		}

		public override IAction NextAction() {
			return Actions.Dequeue();
		}

		public override bool HasActionsQueued {
			get { return Actions.Count > 0; }
		}

		public override void Cancel() {
			base.Cancel();

			Actions.Dequeue();
		}

		public override Controller Copy() {
			return new Player(new Queue<IAction>(Actions));
		}

		public override void Disturb() {
			base.Disturb();

			Actions.Clear();
		}		
	}
}