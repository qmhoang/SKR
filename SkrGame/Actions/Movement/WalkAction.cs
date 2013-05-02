using System;
using System.Diagnostics.Contracts;
using System.Linq;
using DEngine.Actor;
using DEngine.Core;
using DEngine.Entities;
using DEngine.Extensions;
using SkrGame.Universe;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Features;

namespace SkrGame.Actions.Movement {
	public class WalkAction : LoggedAction {
		private Direction _direction;

		private const int WalkCost = (int) (World.OneSecondInAP * World.TileLengthInMeter) / 2;

		public WalkAction(Entity entity, Direction direction)
				: base(entity) {
			Contract.Requires<ArgumentException>(entity.Has<Creature>());
			this._direction = direction;
		}

		public override int APCost {
			get { return (int) Math.Round(WalkCost * _direction.Offset.Length / PostureModifier()); }
		}

		private double PostureModifier() {
			switch (Entity.Get<Creature>().Posture) {
				case Posture.Run:
					return 3.0f;
				case Posture.Stand:
					return 1.0f;
				case Posture.Crouch:
					return 1 / 3f;
				case Posture.Prone:
					return 1 / 6f;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public override ActionResult OnProcess() {
			// note: check for actions on illegal entities
			// todo stamina burning
			var location = Entity.Get<GameObject>();

			Point newLocation = location.Location + _direction;

			if (location.Level.IsWalkable(newLocation)) {
				location.Location = newLocation;

				// check if we're near anything
				var nearEntities = location.Level.GetEntities().FilteredBy<PassiveFeature>();

				foreach (var e in nearEntities) {
					e.Get<PassiveFeature>().Near(Entity, e);
				}
				// move all items in inventory with entity
				if (Entity.Has<ItemContainerComponent>()) {
					foreach (var item in Entity.Get<ItemContainerComponent>().Items) {
						if (item.Has<GameObject>())
							item.Get<GameObject>().Location = newLocation;
					}
				}
				// move all equipped items with entity
				if (Entity.Has<EquipmentComponent>()) {
					foreach (var item in Entity.Get<EquipmentComponent>().EquippedItems) {
						if (item.Has<GameObject>())
							item.Get<GameObject>().Location = newLocation;
					}
				}
			} else {

				World.Log.Fail("There is something in the way.");
				return ActionResult.Aborted;
			}
			return ActionResult.Success;
		}
	}
}
