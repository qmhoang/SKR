using System.Collections.Generic;
using DEngine.Actions;
using DEngine.Actor;

namespace SkrGame.Universe.Entities.Controllers {
	public class Player : Controller {
		public Player() { }

		protected Player(Queue<IAction> actions) : base(actions) { }

		public override IAction NextAction() {
			return Actions.Dequeue();
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