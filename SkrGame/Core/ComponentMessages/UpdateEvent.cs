using System;
using DEngine.Entities;

namespace SkrGame.Core.ComponentMessages {
	public class UpdateEvent : EventArgs {
		public int APDifference { get; private set; }
		public Entity Entity { get; private set; }


		public UpdateEvent(Entity entity, int apDifference) {
			APDifference = apDifference;
			this.Entity = entity;
		}
	}
}