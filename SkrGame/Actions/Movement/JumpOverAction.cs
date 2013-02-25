using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using DEngine.Extensions;
using SkrGame.Actions.Features;
using SkrGame.Universe;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Features;

namespace SkrGame.Actions.Movement {
	// todo needs TLC refactoring
	public class JumpOverAction : LoggedAction {
		private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private Direction direction;
		private Entity feature;
		private Point landedLocation;
		private int apCost;

		public JumpOverAction(Entity entity, Direction direction)
				: base(entity) {
			Contract.Requires<ArgumentException>(entity.Has<Person>());
			//			Contract.Requires<ArgumentException>(feature.Get<GameObject>().DistanceTo(entity.Get<GameObject>()) <= 1.5f);

			this.direction = direction;
			landedLocation = Entity.Get<GameObject>().Location + direction + direction;

			var features = World.CurrentLevel.GetEntitiesAt<Scenery>(entity.Get<GameObject>().Location + direction).OrderByDescending(e => e.Get<Scenery>().JumpHeight);

			if (features.IsEmpty()) {
				feature = null;
				Logger.InfoFormat("{0} is jumping over nothing.", EntityName);
				apCost = (int) Math.Round(World.ONE_SECOND_IN_AP * direction.Offset.Length);
			} else {
				feature = features.First();
				apCost = (int) Math.Round(World.ONE_SECOND_IN_AP * direction.Offset.Length * Math.Max(feature.Get<Scenery>().JumpHeight + 3, 1.0));
			}
		}

		public override int APCost {
			get { return apCost; }
		}

		public override ActionResult OnProcess() {
			// todo stamina burning
			var level = Entity.Get<GameObject>().Level;

			if (!level.IsWalkable(landedLocation)) {
				Logger.InfoFormat("{0} cannot be walked on.  We can't jump over {1}.", landedLocation, EntityName);
				World.Log.Aborted(String.Format("Can't jump over location."));
				return ActionResult.Aborted;
			}

			double jumpRoll = World.SkillRoll();
			double jumpEase = Entity.Get<Person>().Skills["skill_jumping"] - (feature == null ? 0 : feature.Get<Scenery>().JumpHeight);

			jumpEase += World.STANDARD_DEVIATION; // todo fix hack, need to add size for objects (NOT IN GAMEOBJECT, ANOTHER COMPONENT)

			var thingJumpedOver = feature == null ? "location" : Identifier.GetNameOrId(feature);

			Logger.InfoFormat("{0} tries to jump over the {1} : (needs:{2:0.00}, rolled:{3:0.00}, difficulty: {4:0.00}%)",
			                  EntityName,
			                  thingJumpedOver,
			                  jumpEase,
			                  jumpRoll,
			                  World.ChanceOfSuccess(jumpEase));

			if (jumpRoll <= jumpEase) {
				// success
				Entity.Get<GameObject>().Location = landedLocation;

				// move all items in inventory with entity
				if (Entity.Has<ContainerComponent>()) {
					foreach (var item in Entity.Get<ContainerComponent>().Items) {
						if (item.Has<GameObject>())
							item.Get<GameObject>().Location = landedLocation;
					}
				}
				// move all equipped items with entity
				if (Entity.Has<EquipmentComponent>()) {
					foreach (var item in Entity.Get<EquipmentComponent>().EquippedItems) {
						if (item.Has<GameObject>())
							item.Get<GameObject>().Location = landedLocation;
					}
				}

				World.Log.Normal(String.Format("{0} vaults over the {1} successfully.", EntityName, thingJumpedOver));

				return ActionResult.Success;
			} else {
				World.Log.Fail(String.Format("{0} fails to move past the {1}.", EntityName, thingJumpedOver));
				return ActionResult.Failed;
			}
		}
	}
}
