using DEngine.Actions;
using DEngine.Entities;

namespace SkrGame.Actions {
	public abstract class AbstractItemAction : LoggedAction {
		public Entity Item { get; protected set; }
		protected AbstractItemAction(Entity entity, Entity item) : base(entity) {
			Item = item;
		}
	}
}