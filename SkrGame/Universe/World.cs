﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;
using DEngine.Actions;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using DEngine.Extensions;
using DEngine.Random;
using SkrGame.Systems;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Factories;
using log4net;
using Level = SkrGame.Universe.Locations.Level;

namespace SkrGame.Universe {
	public enum WorldStatus {
		Created,
		Initialized,
	}

	public class World {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// default speed, anything with 2x speed cost 1/2 AP 
		/// </summary>
		public const int OneSecondInSpeed = 1000; // 
		public const double TurnLengthInSeconds = 1;	// how long is a turn in seconds
		public const int OneSecondInAP = 1000;	// how long is a turn in seconds
		public const int Mean = 50;						// what is the mean score for an attribute
		public const int StandardDeviation = 15;		// what is the stddev for an attribute score
		public const int StandardIncrement = StandardDeviation / 3;
		public const double TileLengthInMeter = 1f;	// length of 1 square tile
		
		public static double SkillRoll() {
			return Rng.GaussianDouble(Mean, 5 * StandardDeviation, StandardDeviation);
		}

		public MapFactory MapFactory { get; private set; }

		public Level CurrentLevel { get; set; }

		public Entity Player {
			get { return TagManager.GetEntity("player"); }
			set {				
				Contract.Requires<ArgumentNullException>(value != null, "value");
				TagManager.Register("player", value);
			}
		}

		public TagManager<string> TagManager { get; private set; }
		public GroupManager<string> GroupManager { get; private set; }
		public EntityFactory EntityFactory { get; private set; }
		public EntityManager EntityManager { get; private set; }
		public GameLog Log { get; private set; }

		public Calendar Calendar { get; private set; }
		public Entity CalendarEntity { get; private set; }

		private ActionSystem _actionSystem;
		private VisionSubsystem _visionSubsystem;

		public WorldStatus Status { get; private set; }

		public event ComplexEventHandler<World, EventArgs> ActionProcessed;

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
			Log = new GameLog();

			EntityManager.EntityAdded += EntityManager_EntityAdded;
			EntityManager.EntityRemoved += EntityManager_EntityRemoved;

			ItemFactory.Init(EntityFactory);
			FeatureFactory.Init(EntityFactory);
			PersonFactory.Init(EntityFactory);
			TestEntityFactory.Init(EntityFactory);

			EntityFactory.Compile();

			MapFactory = new MapFactory(this);

			Calendar = new Calendar();
			CalendarEntity = EntityManager.Create(new List<Component>
			                                      {
			                                      		new ControllerComponent(Calendar, new AP(World.OneSecondInSpeed / 2))
			                                      });

//			ActionProcessed += World_ActionProcessed;
		}

		void EntityManager_EntityAdded(Entity entity) {
			if (entity.Has<OnCreation>()) {
				entity.Get<OnCreation>().CreationFunc(entity, this);
			}
		}

//		void World_ActionProcessed(World sender, EventArgs e) {
//			visionSubsystem.Update();
//		}

		void EntityManager_EntityRemoved(Entity entity) {
			if (TagManager.HasTags(entity)) 
				TagManager.Remove(entity);
			
			if (GroupManager.IsGrouped(entity))
				GroupManager.Remove(entity);
		}
		
		public void Initialize() {
			_actionSystem = new ActionSystem(this);
			_visionSubsystem = new VisionSubsystem(this);
			Status = WorldStatus.Initialized;
		}

		// todo events when a turn is processed
		public void UpdateSystems() {
			_actionSystem.Update();
		}

		public static int SecondsToActionPoints(double seconds) {
			return (int) Math.Round((seconds * OneSecondInSpeed) / TurnLengthInSeconds);
		}

		public static double ActionPointsToSeconds(int ap) {
			return ap * TurnLengthInSeconds / OneSecondInSpeed;
		}

		public static int SpeedToActionPoints(double speed) {
			return SecondsToActionPoints(SpeedToSeconds(speed));
		}

		public static double ActionPointsToSpeed(int ap) {
			return SecondsToSpeed(ActionPointsToSeconds(ap));
		}

		public static double SpeedToSeconds(double speed) {
			return (OneSecondInSpeed * TurnLengthInSeconds) / speed;
		}

		/// <summary>
		/// Convert how fast an action in seconds to its speed, where speed represents how fast an action is
		/// </summary>
		public static double SecondsToSpeed(double seconds) {
			return ((OneSecondInSpeed * TurnLengthInSeconds) / seconds);
		}

		/// <summary>
		/// Given a difficulty, return the chance of success for it.
		/// </summary>
		/// <param name="difficulty"></param>
		/// <returns>Higher means easier</returns>
		public static double ChanceOfSuccess(double difficulty) {
			return GaussianDistribution.CumulativeTo(difficulty, World.Mean, World.StandardDeviation);
		}
	}
}