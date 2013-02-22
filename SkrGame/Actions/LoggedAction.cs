using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using DEngine.Actions;
using DEngine.Entities;
using SkrGame.Universe;
using SkrGame.Universe.Entities;

namespace SkrGame.Actions {
	public abstract class LoggedAction : ActorAction {
		protected LoggedAction(Entity entity) : base(entity) {
			Contract.Requires<ArgumentException>(entity.Has<GameObject>());
		}

		public World World { get { return Entity.Get<GameObject>().Level.World; } }
	}
}
