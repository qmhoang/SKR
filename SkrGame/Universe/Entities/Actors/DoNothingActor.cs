using DEngine.Actions;
using DEngine.Components;
using SkrGame.Actions;

namespace SkrGame.Universe.Entities.Actors {
	public class DoNothingActor : AbstractActor {
		public override IAction NextAction() {
			return new WaitAction(Holder.Entity);
		}

		public override AbstractActor Copy() {
			return new DoNothingActor();
		}
	}
}