using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using DEngine.Actions;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using DEngine.Extensions;
using SkrGame.Systems;
using SkrGame.Universe.Factories;
using Level = SkrGame.Universe.Locations.Level;

namespace SkrGame.Universe {
	public enum WorldStatus {
		Created,
		Initialized,
	}
	public class World {
		/// <summary>
		/// default speed, anything with 2x speed cost 1/2 AP 
		/// </summary>
		public const int DEFAULT_SPEED = 1000; // 

		public const double TURN_LENGTH_IN_SECONDS = 1;	// how long is a turn in seconds
		public const int ONE_SECOND_IN_AP = 1000;	// how long is a turn in seconds
		public const int MEAN = 50;						// what is the mean score for an attribute
		public const int STANDARD_DEVIATION = 15;		// what is the stddev for an attribute score
		public const int STANDARD_INCREMENT = STANDARD_DEVIATION / 3;
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

		private ActionSystem actionSystem;
		private VisionSubsystem visionSubsystem;

		public WorldStatus Status { get; private set; }

		public event EventHandler<World, EventArgs> ActionProcessed;

		public void OnActionProcessed() {
			var handler = ActionProcessed;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		public World() {
			Rng.Seed(0);

			Status = WorldStatus.Created;

			TagManager = new TagManager<string>();
			GroupManager = new GroupManager<string>();
			EntityFactory = new EntityFactory();
			EntityManager = new EntityManager();
			Log = new Log();

			EntityManager.EntityRemoved += EntityManager_EntityRemoved;

			ItemFactory.Init(EntityFactory);
			FeatureFactory.Init(EntityFactory);
			PersonFactory.Init(EntityFactory);

			EntityFactory.Compile();

			MapFactory = new MapFactory(this);

			Calendar = new Calendar(new DateTime(2013, 4, 2, 6, 23, 52));
			CalendarEntity = EntityManager.Create(new List<Component>
			                                      {
			                                      		new ActorComponent(Calendar, new AP(World.DEFAULT_SPEED))
			                                      });

			ActionProcessed += new EventHandler<World, EventArgs>(World_ActionProcessed);
		}

		void World_ActionProcessed(World sender, EventArgs e) {
			visionSubsystem.Update();
		}

		void EntityManager_EntityRemoved(Entity entity) {
			if (TagManager.HasTags(entity)) 
				TagManager.Remove(entity);
			
			if (GroupManager.IsGrouped(entity))
				GroupManager.Remove(entity);
		}
		
		public void Initialize() {
			actionSystem = new ActionSystem(this);
			visionSubsystem = new VisionSubsystem(this);
			Status = WorldStatus.Initialized;
		}

		// todo events when a turn is processed
		public void UpdateSystems() {
			actionSystem.Update();
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

		/// <summary>
		/// Given a difficulty, return the chance of success for it.
		/// </summary>
		/// <param name="difficulty"></param>
		/// <returns>Higher means easier</returns>
		public static double ChanceOfSuccess(double difficulty) {
			return GaussianDistribution.CumulativeTo(difficulty, World.MEAN, World.STANDARD_DEVIATION);
		}
	}
}