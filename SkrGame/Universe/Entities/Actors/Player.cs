using System.Collections.Generic;
using DEngine.Actions;
using DEngine.Components;

namespace SkrGame.Universe.Entities.Actors {
	public class Player : AbstractActor {
		public Player() { }

		protected Player(Queue<IAction> actions) : base(actions) { }

		public override IAction NextAction() {
			return Actions.Dequeue();
		}

		public override void Cancel() {
			base.Cancel();

			Actions.Dequeue();
		}

		public override AbstractActor Copy() {
			return new Player(new Queue<IAction>(Actions));
		}

		public override void Disturb() {
			base.Disturb();

			Actions.Clear();
		}		
	}
}