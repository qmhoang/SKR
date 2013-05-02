using DEngine.Actions;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;

namespace SkrGame.Actions.Movement {
	public class WaitAction : ActorAction {
		public WaitAction(Entity entity) : base(entity) { }

		public override int APCost {
			get { return Entity.Get<ControllerComponent>().AP.ActionPointPerTurn; }
		}

		public override ActionResult OnProcess() {
			return ActionResult.Success;
		}
	}
}