using System;
using System.Reflection;
using DEngine.Components;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;
using log4net;

namespace SkrGame.Effects {
	public class HumanNeedsEffect : Effect {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private int _staminaMSCount;
		private int _needsMSCount;

		private static readonly int StaminaRequirementInMS = (int) new TimeSpan(0, 0, 10, 0).TotalMilliseconds;
		private static readonly int NeedsRequirementInMS = (int) new TimeSpan(0, 0, 9, 30).TotalMilliseconds;

		public HumanNeedsEffect() {
			_staminaMSCount = 0;
			_needsMSCount = 0;
		}


		public override Effect Copy() {
			return new HumanNeedsEffect()
			       {
			       		_staminaMSCount = _staminaMSCount,
						_needsMSCount = _needsMSCount
			       };
		}

		protected override void OnTick(int apElapsed) {
			// todo need tuning
			int millisecondsElapsed = (int) Math.Round(World.ActionPointsToSeconds(apElapsed) * 1000);

			_staminaMSCount += millisecondsElapsed;
			_needsMSCount += millisecondsElapsed;

			if (Holder.Entity.Has<Creature>()) {
				var person = Holder.Entity.Get<Creature>();

				while (_staminaMSCount / StaminaRequirementInMS > 0) {
					if (person.Stats["stat_energy"] > 0 &&
					    person.Stats["stat_food"] > 0 &&
					    person.Stats["stat_water"] > 0) {

						person.Stats["stat_stamina"].Value++;
					}

					_staminaMSCount -= StaminaRequirementInMS;
				}

				while (_needsMSCount / NeedsRequirementInMS > 0) {

					person.Stats["stat_energy"].Value--;
					person.Stats["stat_food"].Value--;
					person.Stats["stat_water"].Value--;
					person.Stats["stat_bladder"].Value--;

					_needsMSCount -= NeedsRequirementInMS;
				}
			} else {
				Logger.WarnFormat("{0} doesn't have person component.", Identifier.GetNameOrId(Holder.Entity));
			}
		}
	}
}