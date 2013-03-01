using System;
using DEngine.Entities;

namespace SkrGame.Universe.Entities {
	public sealed class OnCreation : Component {
		public Action<Entity, World> CreationFunc { get; private set; }

		public OnCreation(Action<Entity, World> creationFunc) {
			if (creationFunc == null)
				this.CreationFunc = (e, w) => { };
			else
				this.CreationFunc = creationFunc;
		}

		public override Component Copy() {
			return new OnCreation(CreationFunc);
		}
	}
}