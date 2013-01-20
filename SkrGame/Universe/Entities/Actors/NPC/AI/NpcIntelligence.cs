using DEngine.Entities;

namespace SkrGame.Universe.Entities.Actors.NPC.AI {

	public class NpcIntelligence : Component {
		public abstract class AI {
			
			public abstract void Update(Entity user);
		}

		private AI ai;

		public NpcIntelligence(AI ai) {
			this.ai = ai;
		}

		public void Update(Entity actor) {
			ai.Update(actor);
		}
		public override Component Copy() {
			//todo
			return new NpcIntelligence(ai);
		}
	}
}