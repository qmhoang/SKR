using System;
using System.Diagnostics.Contracts;
using System.Linq;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;

namespace SkrGame.Actions.Items {
	public class DropItemAction : AbstractItemAction {
		private int amount;

		public DropItemAction(Entity entity, Entity item, int amount = 1)
				: base(entity, item) {
			Contract.Requires<ArgumentException>(entity.Get<ContainerComponent>().Contains(item));
			this.amount = amount;
		}

		public override int APCost {
			get { return Item.Get<Item>().Size * 30; }
		}

		public override ActionResult OnProcess() {
			if (amount == 0)
				return ActionResult.Aborted;			

			if (amount < Item.Get<Item>().Amount) {

				var level = Entity.Get<GameObject>().Level;
				var itemsInLevel = level.GetEntitiesAt(Entity.Get<GameObject>().Location).Where(e => e.Has<Item>() &&
				                                                                                e.Has<VisibleComponent>() &&
				                                                                                e.Get<VisibleComponent>().VisibilityIndex > 0).ToList();

				if (itemsInLevel.Exists(e => e.Get<ReferenceId>() == Item.Get<ReferenceId>())) {
					itemsInLevel.First(e => e.Get<ReferenceId>() == Item.Get<ReferenceId>()).Get<Item>().Amount += amount;
				} else {
					var ne = Universe.Entities.Items.Item.Split(Item, amount);
					ne.Get<VisibleComponent>().Reset();
				}
				World.Log.NormalFormat("{0} drops {2} {1}.", EntityName, ItemName, amount);
			} else {
				if (Item.Has<VisibleComponent>()) {
					Item.Get<VisibleComponent>().Reset();
				}
				Entity.Get<ContainerComponent>().Remove(Item);
				World.Log.NormalFormat("{0} drops {1}.", EntityName, ItemName);
			}
			return ActionResult.Success;
		}
	}
}