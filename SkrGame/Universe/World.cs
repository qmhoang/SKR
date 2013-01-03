using System;
using System.Collections.Generic;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entity;
using SkrGame.Core;
using SkrGame.Gameplay.Talent;
using SkrGame.Systems;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Actors.NPC;
using SkrGame.Universe.Entities.Actors.NPC.AI;
using SkrGame.Universe.Entities.Actors.PC;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Entities.Items.Components;
using SkrGame.Universe.Factories;
using SkrGame.Universe.Locations;

namespace SkrGame.Universe {
	public class World {
		/// <summary>
		/// default speed of entities, an entity with 2x speed can act twice as before an entity with normal speed
		/// </summary>
		public const int DEFAULT_SPEED = 100; // 

		public const int TURN_LENGTH_IN_SECONDS = 1; // how long is a turn in seconds

		public const int MEAN = 50; // what is the mean score for an attribute
		public const int STANDARD_DEVIATION = 15; // what is the stddev for an attribute score

		public const int TILE_LENGTH = 1; // length of 1 square tile

		public static int SecondsToActionPoints(double seconds) {
			return (int) Math.Round((seconds * DEFAULT_SPEED) / TURN_LENGTH_IN_SECONDS);
		}

		public static double ActionPointsToSeconds(int ap) {
			return (double) (ap * TURN_LENGTH_IN_SECONDS) / DEFAULT_SPEED;
		}

		public static int SpeedToActionPoints(int speed) {
			return SecondsToActionPoints(SpeedToSeconds(speed));
		}

		public static int ActionPointsToSpeed(int ap) {
			return SecondsToSpeed(ActionPointsToSeconds(ap));
		}

		public static double SpeedToSeconds(int speed) {
			return (DEFAULT_SPEED * TURN_LENGTH_IN_SECONDS) / (double) speed;
		}

		/// <summary>
		/// Convert how fast an action in seconds to its speed, where speed represents how fast an action is
		/// </summary>
		public static int SecondsToSpeed(double seconds) {
			return (int) ((DEFAULT_SPEED * TURN_LENGTH_IN_SECONDS) / seconds);
		}

		public List<Message> MessageBuffer { get; private set; }

		public event EventHandler<EventArgs<Message>> MessageAdded;
		public Message CurrentMessage;

		public void OnMessageAdded(EventArgs<Message> e) {
			EventHandler<EventArgs<Message>> handler = MessageAdded;
			if (handler != null)
				handler(this, e);
		}


		public ItemFactory ItemFactory { get; private set; }
		private readonly FeatureFactory featureFactory;
//		private readonly TalentFactory talentFactory;
		private readonly MapFactory mapFactory;

		public static World Instance { get; private set; }

		private Level level;

		public Level CurrentLevel {
			get { return level; }
		}

		public Entity Player { get; set; }

		public EntityManager EntityManager {
			get { return level.EntityManager; }
		}

		private ActionPointSystem actionPointSystem;		

		private World() {
			MessageBuffer = new List<Message>();

//			talentFactory = new SourceTalentFactory();
			ItemFactory = new SourceItemFactory();
			featureFactory = new SourceFeatureFactory();
			mapFactory = new SourceMapFactory(featureFactory);


			Rng.Seed(0);
			level = mapFactory.Construct("TestHouse");

			Player = EntityManager.Create(new Template()
			                              {
			                              		new ActionPoint(),
			                              		new Sprite("player", 3),
			                              		new Location(0, 0, level),
			                              		new PlayerMarker(),
			                              		new Actor("player", level),			                              		
			                              		new DefendComponent(),
												new ContainerComponent()												
			                              });
			Player.Add(ItemFactory.Construct("punch"));

			EntityManager.Create(new Template
			                     {
			                     		new ActionPoint(),
			                     		new Sprite("npc", 3),
			                     		new Location(4, 3, level),
			                     		new Actor("player", level),
			                     		new DefendComponent(),
			                     });

			EntityManager.Create(ItemFactory.Construct("smallknife")).Add(new Location(1, 1, level));


		}

		public void Initialize() {
			actionPointSystem = new ActionPointSystem(EntityManager);
		}

//
//		private void Temp() {
//			var level = mapFactory.Construct("TestHouse");
//			level.GenerateFov();
//			level.World = this;
//			
//			Player = new Player(level) { Position = new Point(0, 0) };
//
//			Player.AddItem(CreateItem("largeknife"));
//
//			var i = CreateItem("brassknuckles");
//			Player.AddItem(i);
//			Player.Equip(BodySlot.OffHand, i);
//
//			Player.AddItem(CreateItem("glock22"));
//			Player.AddItem(CreateItem(".40S&W"));
//			var ammo = CreateItem(".40S&W");
//			ammo.Amount = 20;
//			level.AddItem(ammo, Player.Position);
//
//			Npc npc1 = new Npc(level) { Position = new Point(3, 4) };
//			npc1.Intelligence = new SimpleIntelligence(npc1);
//			level.AddActor(npc1);
//			AddEntity(npc1);
//		}


		public void AddMessage(string message, MessageType priority = MessageType.Normal) {
			var currentMessage = new Message(message, priority);

			MessageBuffer.Add(currentMessage);
			OnMessageAdded(new EventArgs<Message>(currentMessage));
		}

		public static World Create() {
			Instance = new World();
			Instance.Initialize();
			return Instance;
		}

		public void UpdateSystems() {
			actionPointSystem.Update();			
		}
	}
}