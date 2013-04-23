using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;

namespace SkrGame.Actions.Items {
	public class GetItemAction : AbstractItemAction {
		private int _amount;

		public GetItemAction(Entity entity, Entity item, int amount = 1) : base(entity, item) {
			this._amount = amount;
		}

		public override int APCost {
			get { return (Item.Get<Item>().Size + 1 ) * 30; }
		}

		public override ActionResult OnProcess() {
			// todo check if item is visible, still there, etc...

			if (_amount == 0)
				return ActionResult.Aborted;

			var item = Item.Get<Item>();
			if (_amount < item.Amount && item.StackType == StackType.Hard) {
				var temp = Universe.Entities.Items.Item.Split(Item, _amount);
				Get(temp);
			} else {
				Get(Item);
			}

			World.Log.Normal(item.StackType == StackType.Hard
			                 		? string.Format("{0} picks up {1} {2}.", EntityName, _amount, ItemName)
			                 		: string.Format("{0} picks up {1}.", EntityName, ItemName));

			return ActionResult.Success;
		}

		private void Get(Entity i) {
			// make the item we just added invisible
			MakeInvisible(i);

			// just in case, move the item to the entity's location
			MoveItemToEntityLocation(i);

			Entity.Get<ContainerComponent>().Add(i);

			if (!i.IsActive)
				World.EntityManager.Remove(i);
		}
	}
}