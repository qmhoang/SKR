using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using SkrGame.Actions;
using SkrGame.Actions.Features;
using SkrGame.Actions.Items;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Features;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Entities.Items.Tools;

namespace SkrGame.Actions.Skills {
	public class LockpickAction : AbstractItemAction {
		private Entity feature;

		protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public LockpickAction(Entity entity, Entity item, Entity feature) : base(entity, item) {
			Contract.Requires<ArgumentNullException>(feature != null, "feature");
			Contract.Requires<ArgumentException>(item.Has<Lockpick>());
			Contract.Requires<ArgumentException>(feature.Has<LockedFeature>());

			this.feature = feature;
			lengthRequired = new TimeSpan(0, 0, 1, 0);
		}

		public override int APCost {
			get { return 1; }
		}

		private TimeSpan lengthRequired;

		public override ActionResult OnProcess() {
			if (feature.Get<LockedFeature>().Status == LockStatus.Opened) {
				World.Log.AbortedFormat("{0} isn't locked.", Identifier.GetNameOrId(feature));
				return ActionResult.Aborted;
			}

			var person = Entity.Get<Creature>();
			double easeOfLock = Item.Get<Lockpick>().Quality + person.Skills["skill_lockpicking"].Value - feature.Get<LockedFeature>().Quality;

			double roll = World.SkillRoll();

			// intellect roll, reduces times
			var bonus = -(roll - easeOfLock);

			if (roll <= easeOfLock) {
				lengthRequired -= new TimeSpan(0, 0, 0, 0, (int) (bonus * 1000));
			}

			Logger.InfoFormat("{0} attempts in Lockpicking/Intellect on {1} (needs:{2:0.00}, rolled:{3:0.00}, difficulty: {4:0.00}%)",
			                  EntityName,
			                  Identifier.GetNameOrId(feature),
			                  easeOfLock,
			                  roll,
			                  World.ChanceOfSuccess(easeOfLock));

			Entity.Get<ActorComponent>().Enqueue(
					new LongAction(Entity,
					               lengthRequired,
					               e => ActionResult.Success,
					               e =>
					               	{
					               		double agiRoll = World.SkillRoll();
					               		double agiDiff = Item.Get<Lockpick>().Quality +
					               		                 person.Skills["skill_lockpicking"].Rank +
					               		                 person.Attributes["attribute_agility"] -
					               		                 feature.Get<LockedFeature>().Quality +
					               		                 bonus;

					               		Logger.InfoFormat("{0} attempts in Lockpicking/Agility on {1} (needs:{2:0.00}, rolled:{3:0.00}, difficulty: {4:0.00}%)",
					               		                  EntityName, Identifier.GetNameOrId(feature), agiDiff, agiRoll, World.ChanceOfSuccess(agiDiff));

					               		if (agiRoll <= agiDiff) {
					               			feature.Get<LockedFeature>().Status = LockStatus.Opened;
					               			World.Log.GoodFormat("{0} succeeds in lockpicking {1}.", EntityName, Identifier.GetNameOrId(feature));
					               			return ActionResult.Success;
					               		}
					               		World.Log.FailFormat("{0} fails in lockpicking {1}.", EntityName, Identifier.GetNameOrId(feature));
					               		return ActionResult.Failed;
					               	}));

			return ActionResult.Success;
		}
	}
}
