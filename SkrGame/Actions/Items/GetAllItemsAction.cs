using System.Collections.Generic;
using System.Linq;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Universe.Entities.Items;

namespace SkrGame.Actions.Items {
	public class GetAllItemsAction : LoggedAction {
		private List<Entity> items;

		public GetAllItemsAction(Entity entity, IEnumerable<Entity> items)
				: base(entity) {
			this.items = items.ToList();
		}

		public override int APCost {
			get { return 1; }
		}

		public override ActionResult OnProcess() {
			if (items.Count > 0) {
				foreach (var entity in items) {
					Entity.Get<ActorComponent>().Enqueue(new GetItemAction(Entity, entity, entity.Get<Item>().Amount));
				}
				return ActionResult.SuccessNoTime;
			} else {
				World.Log.Fail("No items here to pick up.");
				return ActionResult.Aborted;
			}
		}
	}
}