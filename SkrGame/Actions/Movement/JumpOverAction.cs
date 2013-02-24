using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using SkrGame.Actions.Features;
using SkrGame.Universe;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Features;

namespace SkrGame.Actions.Movement {
	public class JumpOverAction : FeatureAction {
		private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private Direction direction;

		public JumpOverAction(Entity entity, Entity feature)
			: base(entity, feature) {
			Contract.Requires<ArgumentException>(entity.Has<Person>());
			Contract.Requires<ArgumentException>(feature.Get<GameObject>().DistanceTo(entity.Get<GameObject>()) <= 1.5f);

			direction = Feature.Get<GameObject>().Location - Entity.Get<GameObject>().Location;
		}

		public override int APCost {
			get { return World.TURN_LENGTH_IN_AP * Math.Max(Feature.Get<Scenery>().JumpHeight + 3, 1); }
		}

		public override ActionResult OnProcess() {
			var level = Entity.Get<GameObject>().Level;
		
			// check to see if we land in is ok
			Point landedLocation = Feature.Get<GameObject>().Location + direction;

			if (!level.IsWalkable(landedLocation)) {
				Logger.InfoFormat("{0} cannot be walked on.  We can't jump over {1}.", landedLocation, EntityName);
				return ActionResult.Aborted;
			}

			double jumpRoll = World.SkillRoll();
			double jumpEase = Entity.Get<Person>().Skills["skill_jumping"] - Feature.Get<Scenery>().JumpHeight;

			jumpEase += World.STANDARD_DEVIATION;		// todo fix hack, need to add size for objects (NOT IN GAMEOBJECT, ANOTHER COMPONENT)

			Logger.InfoFormat("{0} tries to jump over {1} : (needs:{2:0.00}, rolled:{3:0.00}, difficulty: {4:0.00}%)", EntityName, FeatureName, jumpEase, jumpRoll, World.ChanceOfSuccess(jumpEase));

			if (jumpRoll <= jumpEase) {
				// success
				Entity.Get<GameObject>().Location = landedLocation;
				World.Log.Normal(String.Format("{0} vaults over the {1} successfully.", EntityName, FeatureName));

				return ActionResult.Success;
			} else {
				World.Log.Fail(String.Format("{0} fails to move past the {1}.", EntityName, FeatureName));
				return ActionResult.Failed;
			}
		}
	}
}
