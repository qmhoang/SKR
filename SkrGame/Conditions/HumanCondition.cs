using System.Reflection;
using DEngine.Components;
using SkrGame.Universe.Entities.Actors;
using log4net;

namespace SkrGame.Conditions {
	public class HumanCondition : Condition {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private int staminaMilli;
		private int needsMilli;

		private const int MillisecondsPerMinute = 60000;

		public HumanCondition() {
			staminaMilli = 0;
			needsMilli = 0;
		}


		public override Condition Copy() {
			return new HumanCondition()
			       {
			       		staminaMilli = staminaMilli,
						needsMilli = needsMilli
			       };
		}

		protected override void ConditionUpdate(int millisecondsElapsed) {
			staminaMilli += millisecondsElapsed;
			needsMilli += millisecondsElapsed;

			if (Holder.Entity.Has<Person>()) {
				var person = Holder.Entity.Get<Person>();

				while (staminaMilli / 200 > 0) {
					if (person.Stats["stat_energy"] > 0 &&
					    person.Stats["stat_food"] > 0 &&
					    person.Stats["stat_water"] > 0) {

						person.Stats["stat_stamina"].Value++;
					}
					
					staminaMilli -= 200;
				}

				while (needsMilli / MillisecondsPerMinute > 0) {

					person.Stats["stat_energy"].Value--;
					person.Stats["stat_food"].Value--;
					person.Stats["stat_water"].Value--;
					person.Stats["stat_bladder"].Value--;

					needsMilli -= MillisecondsPerMinute;
				}
			} else {
				Logger.WarnFormat("{0} doesn't have person component.", Identifier.GetNameOrId(Holder.Entity));
			}
		}
	}
}