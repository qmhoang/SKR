using DEngine.Entities;
using SkrGame.Core.ComponentMessages;
using SkrGame.Systems;

namespace SkrGame.Universe.Entities.Actors.NPC.AI {

	public class NpcIntelligence : Component {
		public abstract class AI {
			private Entity e;
			public Entity Entity {
				get { return e; }
				set { 
					e = value;
					OnEntitySet();
				}
			}

			protected abstract void OnEntitySet();
			public abstract void Update();
		}

		private AI ai;

		public NpcIntelligence(AI ai) {
			this.ai = ai;
		}

		protected override void OnSetOwner(Entity e) {
			base.OnSetOwner(e);
			ai.Entity = e;
		}

		public override void Receive(IComponentMessage data) {
			base.Receive(data);

			if (data is UpdateMessage) {
				var message = (UpdateMessage) data;

				if (message.APDifference >= World.TURN_LENGTH_IN_AP) {
					ai.Update();
				}
			}
		}
		
		public override Component Copy() {
			//todo
			return new NpcIntelligence(ai);
		}
	}
}