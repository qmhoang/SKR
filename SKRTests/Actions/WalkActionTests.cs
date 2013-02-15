using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using DEngine.Extensions;
using NUnit.Framework;
using SkrGame.Actions;
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

		protected Entity Item0;
		protected Entity Item1;

		protected Entity Slot1Item0;
		protected Entity Slot2Item0;

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
			
			// .....
			// .....
			// ..@#.
			// .....
			// .....

			Level.SetTerrain(3, 2, "Wall");

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

			Item0 = EntityManager.Create(new List<Component>
			                                 {
			                                 		new Location(-1, -1, null),
			                                 		new Item(new Item.Template
			                                 		         {})

			                                 });
			Item1 = EntityManager.Create(new List<Component>
			                                 {
			                                 		new Location(-1, -1, null),
			                                 		new Item(new Item.Template
			                                 		         {})

			                                 });

			Slot1Item0 = EntityManager.Create(new List<Component>
			                                      {
			                                      		new Location(-1, -1, null),
			                                      		new Equipable(new Equipable.Template
			                                      		              {
			                                      		              		Slot = new List<string>
			                                      		              		       {
			                                      		              		       		"slot1"
			                                      		              		       }
			                                      		              })
			                                      });
			Slot2Item0 = EntityManager.Create(new List<Component>
			                                      {
			                                      		new Location(-1, -1, null),
			                                      		new Equipable(new Equipable.Template
			                                      		              {
			                                      		              		Slot = new List<string>
			                                      		              		       {
			                                      		              		       		"slot2"
			                                      		              		       }
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

			Move(Direction.N);

			Assert.AreEqual(Entity.Get<Location>().Point, new Point(2, 1));
		}

		[Test]
		public void WalkIntoWall() {
			Assert.AreEqual(Entity.Get<Location>().Point, new Point(2, 2));

			Move(Direction.E);
			
			Assert.AreEqual(Entity.Get<Location>().Point, new Point(2, 2));
		}

		[Test]
		public void MoveContainer() {
			Entity.Get<ContainerComponent>().Add(Item0);
			Entity.Get<ContainerComponent>().Add(Item1);

			var startingPoint = Entity.Get<Location>().Point;

			Move(Direction.S);
			
			Entity.Get<ContainerComponent>().Items.Each(e => Assert.AreEqual(e.Get<Location>().Point, startingPoint + Direction.S));
		}

		[Test]
		public void MoveContainerAlot() {
			Entity.Get<ContainerComponent>().Add(Item0);
			Entity.Get<ContainerComponent>().Add(Item1);

			var startingPoint = Entity.Get<Location>().Point;

			Move(Direction.S);
			Move(Direction.S);

			Entity.Get<ContainerComponent>().Items.Each(e => Assert.AreEqual(e.Get<Location>().Point, startingPoint + Direction.S + Direction.S));
		}

		[Test]
		public void RemovedItemDoesntMove() {
			Entity.Get<ContainerComponent>().Add(Item0);
			Entity.Get<ContainerComponent>().Add(Item1);

			var startingPoint = Entity.Get<Location>().Point;

			Move(Direction.S);

			Entity.Get<ContainerComponent>().Items.Each(e => Assert.AreEqual(e.Get<Location>().Point, startingPoint + Direction.S));

			Entity.Get<ContainerComponent>().Remove(Item0);

			Move(Direction.S);

			Assert.AreEqual(Item0.Get<Location>().Point, startingPoint + Direction.S);
			Assert.AreEqual(Item1.Get<Location>().Point, startingPoint + Direction.S + Direction.S);
		}

		[Test]
		public void MoveEquipment() {
			Entity.Get<EquipmentComponent>().Equip("slot1", Slot1Item0);
			Entity.Get<EquipmentComponent>().Equip("slot2", Slot2Item0);

			var startingPoint = Entity.Get<Location>().Point;

			Move(Direction.S);

			Assert.AreEqual(startingPoint + Direction.S, Entity.Get<Location>().Point);

			foreach (var e in Entity.Get<EquipmentComponent>().EquippedItems) {
				Assert.AreEqual(e.Get<Location>().Point, Entity.Get<Location>().Point);
			}
		}

		[Test]
		public void UnequippedItemDoesntMove() {
			Entity.Get<EquipmentComponent>().Equip("slot1", Slot1Item0);
			Entity.Get<EquipmentComponent>().Equip("slot2", Slot2Item0);

			var startingPoint = Entity.Get<Location>().Point;

			Move(Direction.S);

			Entity.Get<ContainerComponent>().Items.Each(e => Assert.AreEqual(e.Get<Location>().Point, startingPoint + Direction.S));

			Entity.Get<EquipmentComponent>().Unequip("slot1");

			Move(Direction.S);

			Assert.AreEqual(Slot1Item0.Get<Location>().Point, startingPoint + Direction.S);
			Assert.AreEqual(Slot2Item0.Get<Location>().Point, startingPoint + Direction.S + Direction.S);
		}
		
		private void Move(Direction d) {
			var action = new WalkAction(Entity, d);
			action.OnProcess();
		}
	}
}
