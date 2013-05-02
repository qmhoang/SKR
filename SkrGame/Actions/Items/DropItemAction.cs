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
		private int _amount;

		public DropItemAction(Entity entity, Entity item, int amount = 1)
				: base(entity, item) {
			Contract.Requires<ArgumentException>(entity.Get<ItemContainerComponent>().Contains(item));
			this._amount = amount;
		}

		public override int APCost {
			get { return Item.Get<Item>().Size * 30; }
		}

		public override ActionResult OnProcess() {
			if (_amount == 0)
				return ActionResult.Aborted;			

			if (_amount < Item.Get<Item>().Amount) {

				var level = Entity.Get<GameObject>().Level;
				var itemsInLevel = level.GetEntitiesAt(Entity.Get<GameObject>().Location).Where(e => e.Has<Item>() &&
				                                                                                e.Has<VisibleComponent>() &&
				                                                                                e.Get<VisibleComponent>().VisibilityIndex > 0).ToList();

				if (itemsInLevel.Exists(e => e.Get<ReferenceId>() == Item.Get<ReferenceId>())) {
					itemsInLevel.First(e => e.Get<ReferenceId>() == Item.Get<ReferenceId>()).Get<Item>().Amount += _amount;
				} else {
					var ne = Universe.Entities.Items.Item.Split(Item, _amount);
					ne.Get<VisibleComponent>().Reset();
				}
				World.Log.NormalFormat("{0} drops {2} {1}.", EntityName, ItemName, _amount);
			} else {
				if (Item.Has<VisibleComponent>()) {
					Item.Get<VisibleComponent>().Reset();
				}
				Entity.Get<ItemContainerComponent>().Remove(Item);
				World.Log.NormalFormat("{0} drops {1}.", EntityName, ItemName);
			}
			return ActionResult.Success;
		}
	}
}