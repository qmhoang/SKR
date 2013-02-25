using System;
using System.Diagnostics.Contracts;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;

namespace SkrGame.Actions.Items {
	public class EquipItemAction : AbstractItemAction {
		private string slot;
		private bool force;

		public EquipItemAction(Entity entity, Entity item, string slot, bool force = false) : base(entity, item) {
			Contract.Requires<ArgumentNullException>(entity != null, "entity");
			Contract.Requires<ArgumentNullException>(item != null, "item");
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(slot), "string \"slot\" cannot be null or empty.");
			Contract.Requires<ArgumentException>(entity.Has<EquipmentComponent>());
			Contract.Requires<ArgumentException>(entity.Has<ContainerComponent>());
			Contract.Requires<ArgumentException>(item.Has<Item>());
			Contract.Requires<ArgumentException>(item.Has<Equipable>());
			Contract.Requires<ArgumentException>(item.Get<Equipable>().SlotsOccupied.ContainsKey(slot));
			Contract.Requires<ArgumentException>(entity.Get<EquipmentComponent>().ContainSlot(slot), "Entity doesn't have slot.");
			Contract.Requires<ArgumentException>(force || entity.Get<ContainerComponent>().Contains(item), "Entity doesn't have item in inventory.");

			this.slot = slot;
			this.force = force;
		}


		public override int APCost {
			get { return Item.Get<Item>().Size * 100; }
		}

		public override ActionResult OnProcess() {
			if (!Entity.Get<ContainerComponent>().Contains(Item) && !force) {
				World.Log.Aborted("You cannot equip an item you don't have in your inventory");
				return ActionResult.Aborted;
			}

			Entity.Get<ContainerComponent>().Remove(Item);
			Entity.Get<EquipmentComponent>().Equip(slot, Item);

			// skill bonuses
			if (Entity.Has<Person>() && Item.Has<EquippedBonus>()) {
				var p = Entity.Get<Person>();
				foreach (var bonus in Item.Get<EquippedBonus>().Bonuses) {
					p.Skills[bonus.Key].Temporary += bonus.Value;
				}
			}

			MakeInvisible(Item);
			MoveItemToEntityLocation(Item);

			World.Log.Normal(String.Format("{0} equips {1} to {2}", EntityName, ItemName, slot));
			
			return force ? ActionResult.SuccessNoTime : ActionResult.Success;
		}
	}

	public class UnequipItemAction : LoggedAction {
		private string slot;
		private int apCost;

		public UnequipItemAction(Entity entity, string slot)
			: base(entity) {
			Contract.Requires<ArgumentNullException>(entity != null, "entity");
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(slot), "string \"slot\" cannot be null or empty.");
			Contract.Requires<ArgumentException>(entity.Has<EquipmentComponent>());
			Contract.Requires<ArgumentException>(entity.Has<ContainerComponent>());
			Contract.Requires<ArgumentException>(entity.Get<EquipmentComponent>().ContainSlot(slot), "Entity doesn't have slot.");
			Contract.Requires<ArgumentException>(entity.Get<EquipmentComponent>().IsSlotEquipped(slot), "No item equipped in slot.");

			this.slot = slot;
			this.apCost = 1;
		}


		public override int APCost {
			get { return apCost; }
		}

		public override ActionResult OnProcess() {
			var removed = Entity.Get<EquipmentComponent>()[slot];

			Entity.Get<EquipmentComponent>().Unequip(slot);

			if (removed != null) {
				Entity.Get<ContainerComponent>().Add(removed);
				apCost = removed.Get<Item>().Size * 10;

				if (removed.Has<VisibleComponent>()) {
					removed.Get<VisibleComponent>().Reset();
				}
				World.Log.Normal(String.Format("{0} unequips {1} from {2}", EntityName, Identifier.GetNameOrId(removed), slot));

				if (Entity.Has<Person>() && removed.Has<EquippedBonus>()) {
					var p = Entity.Get<Person>();
					foreach (var bonus in removed.Get<EquippedBonus>().Bonuses) {
						p.Skills[bonus.Key].Temporary -= bonus.Value;
					}
				}
			}

			
			
			return ActionResult.Success;
		}
	}
}