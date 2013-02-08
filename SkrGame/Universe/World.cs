using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
		public const int TURN_LENGTH_IN_AP = DEFAULT_SPEED;	// how long is a turn in seconds

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

		public Log Log { get; private set; }

		private readonly MapFactory mapFactory;

		public TagManager<string> TagManager { get; private set; }
		public GroupManager<string> GroupManager { get; private set; }

		public EntityFactory EntityFactory { get; private set; }

		public static World Instance { get; private set; }

		private Level level;

		public Level CurrentLevel {
			get { return level; }
		}

		public Entity Player {
			get { return TagManager.GetEntity("player"); }
			set {				
				Contract.Requires<ArgumentNullException>(value != null, "value");
				TagManager.Register(value, "player");
			}
		}

		public EntityManager EntityManager { get; private set; }

		private ActionPointSystem actionPointSystem;
		private VisionSubsystem visionSubsystem;
		
		private World() {
			Rng.Seed(0);

			TagManager = new TagManager<string>();
			GroupManager = new GroupManager<string>();
			EntityManager = new EntityManager();
			EntityFactory = new EntityFactory();
			Log = new Log();

			mapFactory = new MapFactory(EntityManager, EntityFactory);

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
												new SightComponent()
			                              });


			var punch =
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
							APToReady = 1,
							Reach = 0,
							Strength = 1,
							Parry = 0
					};

			Player.Add(new MeleeComponent(punch));

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
												new SightComponent()
			                               });
			npc.Add(new NpcIntelligence(new SimpleAI()));

			EntityManager.Create(EntityFactory.Get("smallknife")).Add(new Location(1, 1, level));
			EntityManager.Create(EntityFactory.Get("axe")).Add(new Location(1, 1, level));
			EntityManager.Create(EntityFactory.Get("glock17")).Add(new Location(1, 1, level));
			var ammo = EntityManager.Create(EntityFactory.Get("9x9mm")).Add(new Location(1, 1, level));
			ammo.Get<Item>().Amount = 30;
			EntityManager.Create(EntityFactory.Get("bullet")).Add(new Location(1, 1, level));

			var armor = EntityManager.Create(EntityFactory.Get("footballpads")).Add(new Location(1, 1, level));
//			npc.Get<ContainerComponent>().Add(armor);
			npc.Get<EquipmentComponent>().Equip("Torso", armor);
			npc.Add(new MeleeComponent(punch));
		}

		public void Initialize() {
			actionPointSystem = new ActionPointSystem(EntityManager);
			visionSubsystem = new VisionSubsystem(EntityManager);
		}


		public static World Create() {
			Instance = new World();
			Instance.Initialize();
			return Instance;
		}

		public void UpdateSystems() {
			visionSubsystem.Update();
			actionPointSystem.Update();
		}
	}
}