using System;
using System.Reflection;
using DEngine.Components;
using SkrGame.Universe.Entities.Actors;
using log4net;

namespace SkrGame.Effects {
	public class HumanNeedsEffect : Effect {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private int staminaMSCount;
		private int needsMSCount;

		private static readonly int StaminaRequirementInMS = (int) new TimeSpan(0, 0, 10, 0).TotalMilliseconds;
		private static readonly int NeedsRequirementInMS = (int) new TimeSpan(0, 0, 9, 30).TotalMilliseconds;

		public HumanNeedsEffect() {
			staminaMSCount = 0;
			needsMSCount = 0;
		}


		public override Effect Copy() {
			return new HumanNeedsEffect()
			       {
			       		staminaMSCount = staminaMSCount,
						needsMSCount = needsMSCount
			       };
		}

		protected override void OnTick(int millisecondsElapsed) {
			// todo need tuning
			staminaMSCount += millisecondsElapsed;
			needsMSCount += millisecondsElapsed;

			if (Holder.Entity.Has<Person>()) {
				var person = Holder.Entity.Get<Person>();

				while (staminaMSCount / StaminaRequirementInMS > 0) {
					if (person.Stats["stat_energy"] > 0 &&
					    person.Stats["stat_food"] > 0 &&
					    person.Stats["stat_water"] > 0) {

						person.Stats["stat_stamina"].Value++;
					}

					staminaMSCount -= StaminaRequirementInMS;
				}

				while (needsMSCount / NeedsRequirementInMS > 0) {

					person.Stats["stat_energy"].Value--;
					person.Stats["stat_food"].Value--;
					person.Stats["stat_water"].Value--;
					person.Stats["stat_bladder"].Value--;

					needsMSCount -= NeedsRequirementInMS;
				}
			} else {
				Logger.WarnFormat("{0} doesn't have person component.", Identifier.GetNameOrId(Holder.Entity));
			}
		}
	}
}