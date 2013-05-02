using System;
using System.Diagnostics.Contracts;
using DEngine.Actor;
using DEngine.Entities;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Entities.Items.Equipables;

namespace SkrGame.Actions.Items {
	public class EquipItemAction : AbstractItemAction {
		private string _slot;
		private bool _force;

		public EquipItemAction(Entity entity, Entity item, string slot, bool force = false) : base(entity, item) {
			Contract.Requires<ArgumentNullException>(entity != null, "entity");
			Contract.Requires<ArgumentNullException>(item != null, "item");
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(slot), "string \"slot\" cannot be null or empty.");
			Contract.Requires<ArgumentException>(entity.Has<EquipmentComponent>());
			Contract.Requires<ArgumentException>(entity.Has<ItemContainerComponent>());
			Contract.Requires<ArgumentException>(item.Has<Item>());
			Contract.Requires<ArgumentException>(item.Has<Equipable>());
			Contract.Requires<ArgumentException>(item.Get<Equipable>().SlotsOccupied.ContainsKey(slot));
			Contract.Requires<ArgumentException>(entity.Get<EquipmentComponent>().ContainSlot(slot), "Entity doesn't have slot.");
			Contract.Requires<ArgumentException>(force || entity.Get<ItemContainerComponent>().Contains(item), "Entity doesn't have item in inventory.");

			this._slot = slot;
			this._force = force;
		}

		public override int APCost {
			get { return Item.Get<Item>().Size * 100; }
		}

		public override ActionResult OnProcess() {
			if (!Entity.Get<ItemContainerComponent>().Contains(Item) && !_force) {
				World.Log.Aborted("You cannot equip an item you don't have in your inventory");
				return ActionResult.Aborted;
			}

			Entity.Get<ItemContainerComponent>().Remove(Item);
			Entity.Get<EquipmentComponent>().Equip(_slot, Item);

			// skill bonuses
			if (Entity.Has<Creature>() && Item.Has<EquippedBonus>()) {
				var p = Entity.Get<Creature>();
				foreach (var bonus in Item.Get<EquippedBonus>().Bonuses) {
					p.Skills[bonus.Key].Temporary += bonus.Value;
				}
			}

			MakeInvisible(Item);
			MoveItemToEntityLocation(Item);

			World.Log.NormalFormat("{0} equips {1} to {2}", EntityName, ItemName, _slot);
			
			return _force ? ActionResult.SuccessNoTime : ActionResult.Success;
		}
	}
}