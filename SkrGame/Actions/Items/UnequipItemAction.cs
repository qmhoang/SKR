using System;
using System.Diagnostics.Contracts;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Entities.Items.Equipables;

namespace SkrGame.Actions.Items {
	public class UnequipItemAction : LoggedAction {
		private string _slot;
		private int _apCost;

		public UnequipItemAction(Entity entity, string slot)
				: base(entity) {
			Contract.Requires<ArgumentNullException>(entity != null, "entity");
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(slot), "string \"slot\" cannot be null or empty.");
			Contract.Requires<ArgumentException>(entity.Has<EquipmentComponent>());
			Contract.Requires<ArgumentException>(entity.Has<ItemContainerComponent>());
			Contract.Requires<ArgumentException>(entity.Get<EquipmentComponent>().ContainSlot(slot), "Entity doesn't have slot.");
			Contract.Requires<ArgumentException>(entity.Get<EquipmentComponent>().IsSlotEquipped(slot), "No item equipped in slot.");

			this._slot = slot;
			this._apCost = 1;
		}

		public override int APCost {
			get { return _apCost; }
		}

		public override ActionResult OnProcess() {
			var removed = Entity.Get<EquipmentComponent>()[_slot];

			Entity.Get<EquipmentComponent>().Unequip(_slot);

			if (removed != null) {
				Entity.Get<ItemContainerComponent>().Add(removed);
				_apCost = removed.Get<Item>().Size * 10;

				if (removed.Has<VisibleComponent>()) {
					removed.Get<VisibleComponent>().Reset();
				}
				World.Log.NormalFormat("{0} unequips {1} from {2}", EntityName, Identifier.GetNameOrId(removed), _slot);

				if (Entity.Has<Creature>() && removed.Has<EquippedBonus>()) {
					var p = Entity.Get<Creature>();
					foreach (var bonus in removed.Get<EquippedBonus>().Bonuses) {
						p.Skills[bonus.Key].Temporary -= bonus.Value;
					}
				}
			}
			
			return ActionResult.Success;
		}
	}
}