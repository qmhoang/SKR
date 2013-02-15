using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using NUnit.Framework;
using SkrGame.Actions;
using SkrGame.Universe;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Locations;

namespace SKRTests.Actions {
	public class ActionTests {
		protected EntityManager EntityManager;
		protected Level Level;
		protected Entity Entity;
		protected World World;

		public void PreSetUp() {
			World = new World();
			EntityManager = World.EntityManager;

			Level = new Level(new Size(5, 5), World, "Floor", new List<Terrain>
			                                                  {
			                                                  		new Terrain("Floor", "Floor", true, true, 1.0),
			                                                  		new Terrain("Wall", "Wall", false, false, 0.0)
			                                                  });

			// .....
			// .....
			// ..@#.
			// .....
			// .....

			Level.SetTerrain(3, 2, "Wall");

			Entity = EntityManager.Create(new List<Component>
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

	[TestFixture]
	public class WalkActionTests : ActionTests {
		[SetUp]
		public void SetUp() {
			PreSetUp();
		}

		[Test]
		public void PositionChanged() {
			Assert.AreEqual(Entity.Get<Location>().Point, new Point(2, 2));
			Entity.Get<ActorComponent>().Enqueue(new WalkAction(Entity, Direction.N));

			Assert.AreEqual(Entity.Get<Location>().Point, new Point(2, 1));
		}

		[Test]
		public void WalkIntoWall() {
			Assert.AreEqual(Entity.Get<Location>().Point, new Point(2, 2));
			Entity.Get<ActorComponent>().Enqueue(new WalkAction(Entity, Direction.E));

			Assert.AreEqual(Entity.Get<Location>().Point, new Point(2, 2));
		}
	}
}
