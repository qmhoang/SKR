using System.Linq;
using DEngine.Actions;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;

namespace SkrGame.Actions {
	public class DropItemAction : AbstractItemAction {
		private int amount;

		public DropItemAction(Entity entity, Entity item, int amount)
				: base(entity, item) {
			this.amount = amount;
		}

		public override int APCost {
			get { return Item.Get<Item>().Size * 3; }
		}

		public override ActionResult OnProcess() {
			if (amount == 0)
				return ActionResult.Aborted;

			var inventory = Entity.Get<ContainerComponent>();

			if (amount < Item.Get<Item>().Amount) {
				var ne = Universe.Entities.Items.Item.Split(Item, amount);

				var level = Entity.Get<Location>().Level;
				var itemsInLevel = level.GetEntitiesAt(Entity.Get<Location>().Point).Where(e => e.Has<Item>() &&
				                                                                                e.Has<VisibleComponent>() &&
				                                                                                e.Get<VisibleComponent>().VisibilityIndex > 0).ToList();

				if (itemsInLevel.Exists(e => e.Get<ReferenceId>() == ne.Get<ReferenceId>())) {
					itemsInLevel.First(e => e.Get<ReferenceId>() == ne.Get<ReferenceId>()).Get<Item>().Amount += amount;
				} else {
					ne.Get<VisibleComponent>().Reset();
				}

			} else {
				if (Item.Has<VisibleComponent>()) {
					Item.Get<VisibleComponent>().Reset();
				}
				Entity.Get<ContainerComponent>().Remove(Item);
			}
			return ActionResult.Success;
		}
	}
}