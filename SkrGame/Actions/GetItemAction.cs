using DEngine.Actions;
using DEngine.Actor;
using DEngine.Entities;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;

namespace SkrGame.Actions {
	public class GetItemAction : AbstractItemAction {
		private int amount;
		public GetItemAction(Entity entity, Entity item, int amount = 1) : base(entity, item) {
			this.amount = amount;
		}

		public override int APCost {
			get { return Item.Get<Item>().Size * 3; }
		}

		public override ActionResult OnProcess() {
			if (amount == 0)
				return ActionResult.Aborted;

			if (amount < Item.Get<Item>().Amount) {
				var ne = Universe.Entities.Items.Item.Split(Item, amount);
				Get(ne);
			} else {
				Get(Item);
			}
			return ActionResult.Success;
		}

		private void Get(Entity i) {
			// make the item we just added invisible
			if (i.Has<VisibleComponent>())
				i.Get<VisibleComponent>().VisibilityIndex = -1;

			// just in case, move the item to the entity's location
			if (i.Has<Location>())
				i.Get<Location>().Point = Entity.Get<Location>().Point;

			Entity.Get<ContainerComponent>().Add(i);

			World.Log.Normal(string.Format(""));
		}
	}
}