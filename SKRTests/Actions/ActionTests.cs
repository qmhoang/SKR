using System.Collections.Generic;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using NUnit.Framework;
using SkrGame.Universe;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Locations;

namespace SKRTests.Actions {
	public class ActionTests {
		protected EntityManager EntityManager;
		protected Level Level;
		protected Entity Entity;
		protected World World;
		
		[SetUp]
		public void PreSetUp() {
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
			                                             		new Location(2, 2, Level),
			                                             		new ContainerComponent(),
			                                             		new ActorComponent(new Player(), new AP()),
			                                             		new Person(),
			                                             		new Identifier("Player"),
			                                             		new EquipmentComponent(new List<string>
			                                             		                       {
			                                             		                       		"slot1",
			                                             		                       		"slot2"
			                                             		                       })
			                                             });

			
		}
	}
}