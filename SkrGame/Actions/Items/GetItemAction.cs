using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;

namespace SkrGame.Actions.Items {
	public class GetItemAction : AbstractItemAction {
		private int amount;
		public GetItemAction(Entity entity, Entity item, int amount = 1) : base(entity, item) {
			this.amount = amount;
		}

		public override int APCost {
			get { return Item.Get<Item>().Size * 3; }
		}

		public override ActionResult OnProcess() {
			// todo check if item is visible, still there, etc...

			if (amount == 0)
				return ActionResult.Aborted;

			var item = Item.Get<Item>();
			if (amount < item.Amount && item.StackType == StackType.Hard) {
				var temp = Universe.Entities.Items.Item.Split(Item, amount);
				Get(temp);
			} else {
				Get(Item);
			}

			World.Log.Normal(item.StackType == StackType.Hard
			                 		? string.Format("{0} picks up {1} {2}.", EntityName, amount, ItemName)
			                 		: string.Format("{0} picks up {1}.", EntityName, ItemName));

			return ActionResult.Success;
		}

		private void Get(Entity i) {
			// make the item we just added invisible
			if (i.Has<VisibleComponent>())
				i.Get<VisibleComponent>().VisibilityIndex = -1;

			// just in case, move the item to the entity's location
			if (i.Has<GameObject>())
				i.Get<GameObject>().Location = Entity.Get<GameObject>().Location;

			Entity.Get<ContainerComponent>().Add(i);

			if (!i.IsActive)
				World.EntityManager.Remove(i);
		}
	}
}