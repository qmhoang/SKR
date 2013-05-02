using System;
using System.Diagnostics.Contracts;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;

namespace SkrGame.Actions.Items {
	public abstract class AbstractItemAction : LoggedAction {
		public Entity Item { get; protected set; }
		public string ItemName { get { return Identifier.GetNameOrId(Item); } }
		protected AbstractItemAction(Entity entity, Entity item) : base(entity) {
			Contract.Requires<ArgumentNullException>(item != null, "item");
			Contract.Requires<ArgumentException>(entity.Has<ItemContainerComponent>());
			Item = item;
		}

		protected void MoveItemToEntityLocation(Entity i) { 
			if (i.Has<GameObject>())
				i.Get<GameObject>().Location = Entity.Get<GameObject>().Location;
		}

		protected void MakeInvisible(Entity i) { 
			if (i.Has<VisibleComponent>())
				i.Get<VisibleComponent>().VisibilityIndex = -1;
		}
	}
}