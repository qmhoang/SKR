using System.Collections.Generic;
using DEngine.Actor;
using DEngine.Entities;

namespace SkrGame.Universe.Entities.Features {
	public class UseableFeature : Component {
		public class UseAction {
			public delegate ActionResult UseDelegate(Entity user, Entity useableEntity, UseAction action);
			public string Description { get; set; }
			public UseDelegate Use { get; set; }

			public UseAction(string description, UseDelegate use) {
				Description = description;
				Use = use;
			}
		}

		private List<UseAction> uses;

		public IEnumerable<UseAction> Uses {
			get { return uses; }
		}

		public UseableFeature(IEnumerable<UseAction> uses) {
			this.uses = new List<UseAction>(uses);
		}

		public override Component Copy() {
			return new UseableFeature(uses);
		}
	}
}