using System;
using System.Diagnostics.Contracts;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Universe.Entities.Actors;

namespace SkrGame.Actions.Items {
	public abstract class AbstractItemAction : LoggedAction {
		public Entity Item { get; protected set; }
		public string ItemName { get { return Identifier.GetNameOrId(Item); } }
		protected AbstractItemAction(Entity entity, Entity item) : base(entity) {
			Contract.Requires<ArgumentNullException>(item != null, "item");
			Contract.Requires<ArgumentException>(entity.Has<ContainerComponent>());
			Item = item;
		}
	}
}