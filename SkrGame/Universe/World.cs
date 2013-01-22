using System;
using System.Collections.Generic;
using System.Linq;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using SkrGame.Core;
using SkrGame.Gameplay.Combat;
using SkrGame.Gameplay.Talent;
using SkrGame.Systems;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Actors.NPC.AI;
using SkrGame.Universe.Entities.Actors.PC;
using SkrGame.Universe.Entities.Features;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Entities.Items.Components;
using SkrGame.Universe.Factories;
using SkrGame.Universe.Locations;
using Level = SkrGame.Universe.Locations.Level;

namespace SkrGame.Universe {
	public class World {
		/// <summary>
		/// default speed of entities, an entity with 2x speed gains AP 2x as fast
		/// </summary>
		public const int DEFAULT_SPEED = 100; // 

		public const int TURN_LENGTH_IN_SECONDS = 1;	// how long is a turn in seconds

		public const int MEAN = 50;						// what is the mean score for an attribute
		public const int STANDARD_DEVIATION = 15;		// what is the stddev for an attribute score

		public const double TILE_LENGTH_IN_METER = 1f;	// length of 1 square tile

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

		public List<Message> MessageBuffer { get; private set; }

		public event EventHandler<EventArgs<Message>> MessageAdded;
		public Message CurrentMessage;

		public void OnMessageAdded(EventArgs<Message> e) {
			EventHandler<EventArgs<Message>> handler = MessageAdded;
			if (handler != null)
				handler(this, e);
		}

		private readonly MapFactory mapFactory;

		public TagManager TagManager { get; private set; }
		public GroupManager GroupManager { get; private set; }

		public EntityFactory EntityFactory { get; private set; }

		public static World Instance { get; private set; }

		private Level level;

		public Level CurrentLevel {
			get { return level; }
		}

		public Entity Player {
			get { return TagManager.GetEntity("player"); }
			set { TagManager.Register("player", value); }
		}

		public EntityManager EntityManager {
			get { return level.EntityManager; }
		}

		private ActionPointSystem actionPointSystem;
		private ItemContainerInteractionSubsystem itemContainerInteractionSubsystem;
		private ItemEquippingSubsystem itemEquippingSubsystem;
//		private OpeningSpriteSubsystem openingSpriteSubsystem;

		private World() {
			Rng.Seed(0);

			TagManager = new TagManager();
			GroupManager = new GroupManager();
			EntityFactory = new EntityFactory();

			MessageBuffer = new List<Message>();

			mapFactory = new SourceMapFactory(EntityFactory);

			level = mapFactory.Construct("TestMap");

			Player = EntityManager.Create(new List<Component>
			                              {
			                              		new ActionPoint(),
			                              		new Sprite("player", Sprite.PLAYER_LAYER),
												new Identifier("Player"),
			                              		new Location(0, 0, level),
			                              		new Player(),
			                              		new Actor(),			                              		
			                              		new DefendComponent(),
												new ContainerComponent(),
												new EquipmentComponent(),
												new VisibleComponent(10),
			                              });


			Player.Add(new MeleeComponent(
			           		new MeleeComponent.Template
			           		{
			           				ComponentId = "punch",
			           				ActionDescription = "punch",
			           				ActionDescriptionPlural = "punches",
			           				Skill = "skill_unarmed",
			           				HitBonus = 0,
			           				Damage = Rand.Constant(-5),
			           				DamageType = Combat.DamageTypes["crush"],
			           				Penetration = 1,
			           				WeaponSpeed = 100,
			           				Reach = 0,
			           				Strength = 0,
			           				Parry = 0
			           		}));

			var npc = EntityManager.Create(new List<Component>
			                     {
			                     		new ActionPoint(),
			                     		new Sprite("npc", Sprite.ACTOR_LAYER),
										new Identifier("npc"),
			                     		new Location(7, 2, level),
			                     		new Actor(),
			                     		new DefendComponent(),
										new VisibleComponent(10),
										new ContainerComponent(),
										new EquipmentComponent(),
										new NpcIntelligence(new SimpleIntelligence())
			                     });

			EntityManager.Create(EntityFactory.Get("smallknife")).Add(new Location(1, 1, level));
			EntityManager.Create(EntityFactory.Get("axe")).Add(new Location(1, 1, level));
			EntityManager.Create(EntityFactory.Get("glock17")).Add(new Location(1, 1, level));
			var ammo = EntityManager.Create(EntityFactory.Get("9x9mm")).Add(new Location(1, 1, level));
			ammo.Get<Item>().Amount = 30;
			EntityManager.Create(EntityFactory.Get("bullet")).Add(new Location(1, 1, level));

			var armor = EntityManager.Create(EntityFactory.Get("footballpads")).Add(new Location(1, 1, level));
//			npc.Get<ContainerComponent>().Add(armor);
			npc.Get<EquipmentComponent>().Equip("Torso", armor);
			npc.Add(new MeleeComponent(
				new MeleeComponent.Template
				{
					ComponentId = "punch",
					ActionDescription = "punch",
					ActionDescriptionPlural = "punches",
					Skill = "skill_unarmed",
					HitBonus = 0,
					Damage = Rand.Constant(-5),
					DamageType = Combat.DamageTypes["crush"],
					Penetration = 1,
					WeaponSpeed = 100,
					Reach = 0,
					Strength = 0,
					Parry = 0
				}));
		}

		public void Initialize() {
			actionPointSystem = new ActionPointSystem(EntityManager);
			itemContainerInteractionSubsystem = new ItemContainerInteractionSubsystem(EntityManager);
			itemEquippingSubsystem = new ItemEquippingSubsystem(EntityManager);
//			openingSpriteSubsystem = new OpeningSpriteSubsystem(EntityManager);
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