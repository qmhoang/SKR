using System;
using DEngine.Entities;

namespace SkrGame.Universe.Entities.Features {
	public sealed class OnBump : Component {
		public enum BumpResult {
			BlockMovement,
			NormalMovement
		}
		public delegate BumpResult OnBumpAction(Entity user, Entity entity);

		public OnBumpAction Action { get; private set; }

		public OnBump(OnBumpAction action) {
			Action = action;
		}

		public override Component Copy() {
			return new OnBump(Action);
		}
	}
}