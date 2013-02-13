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
using SkrGame.Universe.Entities.Features;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Entities.Items.Components;
using SkrGame.Universe.Factories;
using SkrGame.Universe.Locations;
using Level = SkrGame.Universe.Locations.Level;

namespace SkrGame.Universe {
	public class World : AbstractWorld {
		private readonly MapFactory mapFactory;

		private Level level;

		public Level CurrentLevel {
			get { return level; }
		}

		public override Entity Player {
			get { return TagManager.GetEntity("player"); }
			set {				
				Contract.Requires<ArgumentNullException>(value != null, "value");
				TagManager.Register(value, "player");
			}
		}

		private ActionPointSystem actionPointSystem;
		private VisionSubsystem visionSubsystem;
		
		public World() : base(new TagManager<string>(), new GroupManager<string>(), new EntityFactory(), new EntityManager(), new Log()) {
			Rng.Seed(0);

			ItemFactory.Init(EntityFactory);
			FeatureFactory.Init(EntityFactory);

			EntityFactory.Compile();

			mapFactory = new MapFactory(EntityManager, EntityFactory);

			level = mapFactory.Construct("TestMap");

			var player = EntityManager.Create(new List<Component>
			                                  {
			                                  		new ActionPoint(),
			                                  		new Sprite("player", Sprite.PLAYER_LAYER),
			                                  		new Identifier("Player"),
			                                  		new Location(0, 0, level),
//			                                  		new Player(),
			                                  		new Person(),
			                                  		new DefendComponent(),
			                                  		new ContainerComponent(),
			                                  		new EquipmentComponent(),
			                                  		new VisibleComponent(10),
			                                  		new SightComponent()
			                                  });

			TagManager.Register(player, "player");
			
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

			player.Add(new MeleeComponent(punch));

			var npc = EntityManager.Create(new List<Component>
			                               {
			                               		new ActionPoint(),
			                               		new Sprite("npc", Sprite.ACTOR_LAYER),
			                               		new Identifier("npc"),
			                               		new Location(6, 2, level),
			                               		new Person(),
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

			actionPointSystem = new ActionPointSystem(player, EntityManager);
			visionSubsystem = new VisionSubsystem(EntityManager);
		}

		public void Process() {			
		}

		public void UpdateSystems() {
			visionSubsystem.Update();
			actionPointSystem.Update();
		}
	}
}