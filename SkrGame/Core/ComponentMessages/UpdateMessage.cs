using DEngine.Entities;

namespace SkrGame.Core.ComponentMessages {
	public class UpdateMessage : IComponentMessage {
		public int APDifference { get; private set; }
		public Entity Entity { get; private set; }


		public UpdateMessage(Entity entity, int apDifference) {
			APDifference = apDifference;
			this.Entity = entity;
		}
	}
}