﻿using System;
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
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Features;
using SkrGame.Universe.Entities.Items;

namespace SkrGame.Actions.Skills {
	public class LockpickAction : AbstractItemAction {
		private Entity feature;

		protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public LockpickAction(Entity entity, Entity item, Entity feature) : base(entity, item) {
			Contract.Requires<ArgumentNullException>(feature != null, "feature");
			Contract.Requires<ArgumentException>(item.Has<Lockpick>());
			Contract.Requires<ArgumentException>(feature.Has<LockedFeature>());

			this.feature = feature;
			apRequired = World.SecondsToActionPoints(60);
		}

		public override int APCost {
			get { return 1; }
		}

		private int apRequired;

		public override ActionResult OnProcess() {
			if (feature.Get<LockedFeature>().Status == LockStatus.Opened) {
				World.Log.Fail(String.Format("{0} isn't locked.", Identifier.GetNameOrId(feature)));
				return ActionResult.Failed;
			}
			
			double difficulty = Item.Get<Lockpick>().Quality + Entity.Get<Person>().GetSkill("skill_lockpicking").Rank - feature.Get<LockedFeature>().Quality;

			double roll = World.SkillRoll();

			// intellect roll, reduces times
			double chanceOfSuccess = World.ChanceOfSuccess(difficulty);
			double skillRoll = Rng.Double();
			var bonus = difficulty - roll;

			if (skillRoll <= difficulty) {
				apRequired += World.SecondsToActionPoints(bonus);				
			}

			Logger.InfoFormat("{0} attempts in Lockpicking/Intellect on {1} (needs:{2:0.00}, rolled:{3:0.00}, difficulty: {4:0.00}%)", Identifier.GetNameOrId(Entity), Identifier.GetNameOrId(feature), difficulty, roll, chanceOfSuccess);

			Entity.Get<ActorComponent>().Enqueue(
					new LongAction(Entity,
					               e =>
					               	{
					               		double agiRoll = World.SkillRoll();
					               		double agiDiff = Item.Get<Lockpick>().Quality + Entity.Get<Person>().GetSkill("skill_lockpicking").RawRank + Entity.Get<Person>().Agility -
					               		                 feature.Get<LockedFeature>().Quality + bonus;

					               		Logger.InfoFormat("{0} attempts in Lockpicking/Agility on {1} (needs:{2:0.00}, rolled:{3:0.00}, difficulty: {4:0.00}%)",
					               		                  Identifier.GetNameOrId(Entity), Identifier.GetNameOrId(feature), agiDiff, agiRoll, World.ChanceOfSuccess(agiDiff));

					               		if (agiRoll <= agiDiff) {
					               			feature.Get<LockedFeature>().Status = LockStatus.Opened;
					               			World.Log.Good(String.Format("{0} succeeds in lockpicking {1}.", Identifier.GetNameOrId(Entity), Identifier.GetNameOrId(feature)));
					               			return ActionResult.Success;
					               		}
					               		World.Log.Fail(String.Format("{0} fails in lockpicking {1}.", Identifier.GetNameOrId(Entity), Identifier.GetNameOrId(feature)));
					               		return ActionResult.Failed;
					               	},
					               apRequired));

			return ActionResult.Success;
		}
	}
}