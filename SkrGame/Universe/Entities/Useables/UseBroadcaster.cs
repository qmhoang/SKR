using System.Collections.Generic;
using System.Linq;
using DEngine.Actor;
using DEngine.Entities;

namespace SkrGame.Universe.Entities.Useables {
	public sealed class UseBroadcaster : Component {
		public IEnumerable<UseAction> Actions {
			get {
				return Entity.Components.OfType<IUseable>().SelectMany(component => (component).Uses);
			}
		}

		public override Component Copy() {
			return new UseBroadcaster();
		}
	}

	public interface IUseable {
		IEnumerable<UseAction> Uses { get; }
	}

	public class UseAction {
		public delegate void UseDelegate(Entity user, Entity useableEntity, UseAction action);
		public string Description { get; set; }
		public UseDelegate Use { get; set; }

		public UseAction(string description, UseDelegate use) {
			Description = description;
			Use = use;
		}
	}
}