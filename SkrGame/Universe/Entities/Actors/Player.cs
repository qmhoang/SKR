using System.Collections.Generic;
using DEngine.Actions;
using DEngine.Components;

namespace SkrGame.Universe.Entities.Actors {
	public class Player : AbstractActor {
		public Player() { }

		protected Player(Queue<IAction> actions) : base(actions) { }

		public override bool RequiresInput {
			get { return Actions.Count == 0; }
		}

		public override IAction NextAction() {
			return RequiresInput ? new RequiresPlayerInputAction() : Actions.Dequeue();
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

		public override void Enqueue(IAction action) {
			base.Enqueue(action);
			// todo fix this ugly hack
			if (Holder.Entity.Get<GameObject>().Level.World.CurrentAction is IPlayerInputAction)
				((IPlayerInputAction)Holder.Entity.Get<GameObject>().Level.World.CurrentAction).SetFinished();
		}
	}
}