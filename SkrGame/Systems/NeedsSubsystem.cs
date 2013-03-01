using DEngine.Entities;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;

//namespace SkrGame.Systems {
//	public class ConditionSubsystem : ITimeElapsedSubsystem {
//		public void Update(int millisecondsElapsed) {
//			throw new System.NotImplementedException();
//		}
//	}
//
//	// faster than having individuals conditions for every human
//	public class NeedsSubsystem : ITimeElapsedSubsystem {
//		private FilteredCollection people;
//		private int staminaMilli;
//		private int needsMilli;
//
//		private const int MillisecondsPerMinute = 60000;
//
//		public NeedsSubsystem(World world) {
//			people = world.EntityManager.Get(typeof(Person));
//			staminaMilli = 0;
//		}
//
//		public void Update(int millisecondsElapsed) {
//			staminaMilli += millisecondsElapsed;
//			needsMilli += millisecondsElapsed;
//
//			while (staminaMilli / 200 > 0) {
//				foreach (var e in people) {
//					var person = e.Get<Person>();
//
//					if (person.Stats["stat_energy"] > 0 &&
//					    person.Stats["stat_food"] > 0 &&
//					    person.Stats["stat_water"] > 0) {
//
//						person.Stats["stat_stamina"].Value++;
//					}
//				}
//
//				staminaMilli -= 200;
//			}
//
//			while (needsMilli / MillisecondsPerMinute > 0) {
//				foreach (var e in people) {
//					var person = e.Get<Person>();
//					person.Stats["stat_energy"].Value--;
//					person.Stats["stat_food"].Value--;
//					person.Stats["stat_water"].Value--;
//					person.Stats["stat_bladder"].Value--;
//				}
//
//				needsMilli -= MillisecondsPerMinute;
//			}
//		}
//	}
//}