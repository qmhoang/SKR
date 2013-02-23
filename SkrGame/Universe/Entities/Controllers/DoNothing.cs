using DEngine.Actions;
using DEngine.Actor;
using SkrGame.Actions;
using SkrGame.Actions.Movement;

namespace SkrGame.Universe.Entities.Controllers {
	public class DoNothing : Controller {
		public override IAction NextAction() {
			return new WaitAction(Holder.Entity);
		}

		public override Controller Copy() {
			return new DoNothing();
		}
	}
}