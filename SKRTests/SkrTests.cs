using System.Collections.Generic;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using NUnit.Framework;
using SkrGame.Gameplay.Combat;
using SkrGame.Universe;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Controllers;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Entities.Items.Equipables;
using SkrGame.Universe.Locations;

namespace SKRTests {
	public class SkrTests {
		protected EntityManager EntityManager;
		protected Level Level;
		protected Entity Entity;
		protected World World;

		[SetUp]
		public void Init() {
			World = new World();
			EntityManager = World.EntityManager;

			World.CurrentLevel = Level = new Level(new Size(5, 5),
			                                       World,
			                                       "Floor",
			                                       new List<Terrain>
			                                       {
			                                       		new Terrain("Floor", "Floor", true, true, 1.0),
			                                       		new Terrain("Wall", "Wall", false, false, 0.0)
			                                       });

			World.Player = Entity = EntityManager.Create(new List<Component>
			                                             {
			                                             		new Sprite("player", Sprite.ActorLayer),
			                                             		DefendComponent.CreateHuman(50),
			                                             		new VisibleComponent(10),
			                                             		new SightComponent(),
			                                             		new MeleeComponent(new MeleeComponent.Template
			                                             		                   {
			                                             		                   		ActionDescription = "punch",
			                                             		                   		ActionDescriptionPlural = "punches",
			                                             		                   		Skill = "skill_unarmed",
			                                             		                   		HitBonus = 0,
			                                             		                   		Damage = Rand.Constant(-5),
			                                             		                   		DamageType = Combat.DamageTypes["crush"],
			                                             		                   		Penetration = 1,
			                                             		                   		AttackSpeed = World.OneSecondInSpeed,
			                                             		                   		APToReady = 1,
			                                             		                   		Reach = 0,
			                                             		                   		Strength = 1,
			                                             		                   		Parry = 0
			                                             		                   }),
			                                             		new GameObject(2, 2, Level),
			                                             		new ContainerComponent(),
			                                             		new ActorComponent(new Player(), new AP(World.OneSecondInSpeed)),
			                                             		new Creature(),
			                                             		new Identifier("Player"),
			                                             		new EquipmentComponent(new List<string>
			                                             		                       {
			                                             		                       		"slot1",
			                                             		                       		"slot2",
																							"slot3",
																							"slot4",
			                                             		                       })
			                                             });
		}
	}
}