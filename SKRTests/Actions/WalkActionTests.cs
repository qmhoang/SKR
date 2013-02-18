using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Core;
using DEngine.Entities;
using DEngine.Extensions;
using NUnit.Framework;
using SkrGame.Actions;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;

namespace SKRTests.Actions {
	[TestFixture]
	public class WalkActionTests : ActionTests {
		protected Entity Item0;
		protected Entity Item1;

		protected Entity Slot1Item0;
		protected Entity Slot2Item0;

		[SetUp]
		public void SetUp() {
			Item0 = EntityManager.Create(new List<Component>
			                             {
			                             		new Location(-1, -1, Level),
			                             		new Item(new Item.Template
			                             		         {})

			                             });
			Item1 = EntityManager.Create(new List<Component>
			                             {
			                             		new Location(-1, -1, Level),
			                             		new Item(new Item.Template
			                             		         {})

			                             });

			Slot1Item0 = EntityManager.Create(new List<Component>
			                                  {
			                                  		new Location(-1, -1, Level),
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
			                                  		new Location(-1, -1, Level),
			                                  		new Equipable(new Equipable.Template
			                                  		              {
			                                  		              		Slot = new List<string>
			                                  		              		       {
			                                  		              		       		"slot2"
			                                  		              		       }
			                                  		              })
			                                  });


			// .....
			// .....
			// ..@#.
			// .....
			// .....

			Level.SetTerrain(3, 2, "Wall");
		}

		[Test]
		public void PositionChanged() {
			Assert.AreEqual(new Point(2, 2), Entity.Get<Location>().Point);

			Move(Direction.N);

			Assert.AreEqual(new Point(2, 1), Entity.Get<Location>().Point);
		}

		[Test]
		public void WalkIntoWall() {
			Assert.AreEqual(new Point(2, 2), Entity.Get<Location>().Point);

			Move(Direction.E);

			Assert.AreEqual(new Point(2, 2), Entity.Get<Location>().Point);
		}

		[Test]
		public void WalkOutOfBounds() {
			Entity.Get<Location>().Point = new Point(0, 0);
			Assert.AreEqual(new Point(0, 0), Entity.Get<Location>().Point);

			Move(Direction.N);

			Assert.AreEqual(new Point(0, 0), Entity.Get<Location>().Point);
		}

		[Test]
		public void MoveContainer() {
			Entity.Get<ContainerComponent>().Add(Item0);
			Entity.Get<ContainerComponent>().Add(Item1);

			var startingPoint = Entity.Get<Location>().Point;

			Move(Direction.S);

			Entity.Get<ContainerComponent>().Items.Each(e => Assert.AreEqual(startingPoint + Direction.S, e.Get<Location>().Point));
		}

		[Test]
		public void MoveContainerAlot() {
			Entity.Get<ContainerComponent>().Add(Item0);
			Entity.Get<ContainerComponent>().Add(Item1);

			var startingPoint = Entity.Get<Location>().Point;

			Move(Direction.S);
			Move(Direction.S);

			Entity.Get<ContainerComponent>().Items.Each(e => Assert.AreEqual(startingPoint + Direction.S + Direction.S, e.Get<Location>().Point));
		}

		[Test]
		public void RemovedItemDoesntMove() {
			Entity.Get<ContainerComponent>().Add(Item0);
			Entity.Get<ContainerComponent>().Add(Item1);

			var startingPoint = Entity.Get<Location>().Point;

			Move(Direction.S);

			Entity.Get<ContainerComponent>().Items.Each(e => Assert.AreEqual(startingPoint + Direction.S, e.Get<Location>().Point));

			Entity.Get<ContainerComponent>().Remove(Item0);

			Move(Direction.S);

			Assert.AreEqual(startingPoint + Direction.S, Item0.Get<Location>().Point);
			Assert.AreEqual(startingPoint + Direction.S + Direction.S, Item1.Get<Location>().Point);
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
