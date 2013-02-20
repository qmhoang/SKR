using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using SkrGame.Systems;
using SkrGame.Universe.Factories;
using Level = SkrGame.Universe.Locations.Level;

namespace SkrGame.Universe {
	public class World {
		/// <summary>
		/// default speed of entities, an entity with 2x speed gains AP 2x as fast
		/// </summary>
		public const int DEFAULT_SPEED = 100; // 

		public const int TURN_LENGTH_IN_SECONDS = 1;	// how long is a turn in seconds
		public const int TURN_LENGTH_IN_AP = DEFAULT_SPEED;	// how long is a turn in seconds
		public const int MEAN = 50;						// what is the mean score for an attribute
		public const int STANDARD_DEVIATION = 15;		// what is the stddev for an attribute score
		public const double TILE_LENGTH_IN_METER = 1f;	// length of 1 square tile

		public static double SkillRoll() {
			return Rng.GaussianDouble(MEAN, 5 * STANDARD_DEVIATION, STANDARD_DEVIATION);
		}

		public MapFactory MapFactory { get; private set; }

		public Level CurrentLevel { get; set; }

		public Entity Player {
			get { return TagManager.GetEntity("player"); }
			set {				
				Contract.Requires<ArgumentNullException>(value != null, "value");
				TagManager.Register(value, "player");
			}
		}

		public TagManager<string> TagManager { get; private set; }
		public GroupManager<string> GroupManager { get; private set; }
		public EntityFactory EntityFactory { get; private set; }
		public EntityManager EntityManager { get; private set; }
		public Log Log { get; private set; }

		public Calendar Calendar { get; private set; }
		public Entity CalendarEntity { get; private set; }

		private ActionPointSystem actionPointSystem;
		private VisionSubsystem visionSubsystem;

		public World() {
			Rng.Seed(0);

			TagManager = new TagManager<string>();
			GroupManager = new GroupManager<string>();
			EntityFactory = new EntityFactory();
			EntityManager = new EntityManager();
			Log = new Log();

			ItemFactory.Init(EntityFactory);
			FeatureFactory.Init(EntityFactory);

			EntityFactory.Compile();

			MapFactory = new MapFactory(this);

			Calendar = new Calendar();
			CalendarEntity = EntityManager.Create(new List<Component>
			                                      {
			                                      		new ActorComponent(Calendar, new AP())
			                                      });
		}

		public void Initialize() {
			actionPointSystem = new ActionPointSystem(Player, EntityManager);
			visionSubsystem = new VisionSubsystem(EntityManager);
		}

		public void UpdateSystems() {
			visionSubsystem.Update();
			actionPointSystem.Update();
		}

		public static int SecondsToActionPoints(double seconds) {
			return  (int) Math.Round((seconds * DEFAULT_SPEED) / TURN_LENGTH_IN_SECONDS);
		}

		public static double ActionPointsToSeconds(int ap) {
			return (double) (ap * TURN_LENGTH_IN_SECONDS) / DEFAULT_SPEED;
		}

		public static int SpeedToActionPoints(double speed) {
			return SecondsToActionPoints(SpeedToSeconds(speed));
		}

		public static double ActionPointsToSpeed(int ap) {
			return SecondsToSpeed(ActionPointsToSeconds(ap));
		}

		public static double SpeedToSeconds(double speed) {
			return (DEFAULT_SPEED * TURN_LENGTH_IN_SECONDS) / speed;
		}

		/// <summary>
		/// Convert how fast an action in seconds to its speed, where speed represents how fast an action is
		/// </summary>
		public static double SecondsToSpeed(double seconds) {
			return ((DEFAULT_SPEED * TURN_LENGTH_IN_SECONDS) / seconds);
		}

		public static int ScaleSkill(int rank) {
			return rank * World.STANDARD_DEVIATION / 3;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="difficulty"></param>
		/// <returns>Higher means easier</returns>
		public static double ChanceOfSuccess(double difficulty) {
			return GaussianDistribution.CumulativeTo(difficulty, World.MEAN, World.STANDARD_DEVIATION);
		}
	}
}