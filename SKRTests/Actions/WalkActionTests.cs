using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Core;
using DEngine.Entities;
using DEngine.Extensions;
using NUnit.Framework;
using SkrGame.Actions;
using SkrGame.Actions.Movement;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;

namespace SKRTests.Actions {
	[TestFixture]
	public class WalkActionTests : SkrTests {
		protected Entity Item0;
		protected Entity Item1;

		protected Entity Slot1Item0;
		protected Entity Slot2Item0;

		[SetUp]
		public void SetUp() {
			Item0 = EntityManager.Create(new List<Component>
			                             {
			                             		new GameObject(-1, -1, Level),
			                             		new Item(new Item.Template
			                             		         {})

			                             });
			Item1 = EntityManager.Create(new List<Component>
			                             {
			                             		new GameObject(-1, -1, Level),
			                             		new Item(new Item.Template
			                             		         {})

			                             });

			Slot1Item0 = EntityManager.Create(new List<Component>
			                                  {
			                                  		new GameObject(-1, -1, Level),
			                                  		Equipable.SingleSlot("slot1")
			                                  });
			Slot2Item0 = EntityManager.Create(new List<Component>
			                                  {
			                                  		new GameObject(-1, -1, Level),
			                                  		Equipable.SingleSlot("slot2")
			                                  });


			// .....
			// .....
			// ..@#.
			// .....
			// .....

			Level.SetTerrain(3, 2, "Wall");
		}

		[Test]
		public void TestPositionChanged() {
			Assert.AreEqual(new Point(2, 2), Entity.Get<GameObject>().Location);

			Move(Direction.N);

			Assert.AreEqual(new Point(2, 1), Entity.Get<GameObject>().Location);
		}

		[Test]
		public void TestWalkIntoWall() {
			Assert.AreEqual(new Point(2, 2), Entity.Get<GameObject>().Location);

			Move(Direction.E);

			Assert.AreEqual(new Point(2, 2), Entity.Get<GameObject>().Location);
		}

		[Test]
		public void TestWalkOutOfBounds() {
			Entity.Get<GameObject>().Location = new Point(0, 0);
			Assert.AreEqual(new Point(0, 0), Entity.Get<GameObject>().Location);

			Move(Direction.N);

			Assert.AreEqual(new Point(0, 0), Entity.Get<GameObject>().Location);
		}

		[Test]
		public void TestMoveContainer() {
			Entity.Get<ContainerComponent>().Add(Item0);
			Entity.Get<ContainerComponent>().Add(Item1);

			var startingPoint = Entity.Get<GameObject>().Location;

			Move(Direction.S);

			Entity.Get<ContainerComponent>().Items.Each(e => Assert.AreEqual(startingPoint + Direction.S, e.Get<GameObject>().Location));
		}

		[Test]
		public void TestMoveContainerAlot() {
			Entity.Get<ContainerComponent>().Add(Item0);
			Entity.Get<ContainerComponent>().Add(Item1);

			var startingPoint = Entity.Get<GameObject>().Location;

			Move(Direction.S);
			Move(Direction.S);

			Entity.Get<ContainerComponent>().Items.Each(e => Assert.AreEqual(startingPoint + Direction.S + Direction.S, e.Get<GameObject>().Location));
		}

		[Test]
		public void TestRemovedItemDoesntMove() {
			Entity.Get<ContainerComponent>().Add(Item0);
			Entity.Get<ContainerComponent>().Add(Item1);

			var startingPoint = Entity.Get<GameObject>().Location;

			Move(Direction.S);

			Entity.Get<ContainerComponent>().Items.Each(e => Assert.AreEqual(startingPoint + Direction.S, e.Get<GameObject>().Location));

			Entity.Get<ContainerComponent>().Remove(Item0);

			Move(Direction.S);

			Assert.AreEqual(startingPoint + Direction.S, Item0.Get<GameObject>().Location);
			Assert.AreEqual(startingPoint + Direction.S + Direction.S, Item1.Get<GameObject>().Location);
		}

		[Test]
		public void TestMoveEquipment() {
			Entity.Get<EquipmentComponent>().Equip("slot1", Slot1Item0);
			Entity.Get<EquipmentComponent>().Equip("slot2", Slot2Item0);

			var startingPoint = Entity.Get<GameObject>().Location;

			Move(Direction.S);

			Assert.AreEqual(startingPoint + Direction.S, Entity.Get<GameObject>().Location);

			foreach (var e in Entity.Get<EquipmentComponent>().EquippedItems) {
				Assert.AreEqual(e.Get<GameObject>().Location, Entity.Get<GameObject>().Location);
			}
		}

		[Test]
		public void TestUnequippedItemDoesntMove() {
			Entity.Get<EquipmentComponent>().Equip("slot1", Slot1Item0);
			Entity.Get<EquipmentComponent>().Equip("slot2", Slot2Item0);

			var startingPoint = Entity.Get<GameObject>().Location;

			Move(Direction.S);

			Entity.Get<ContainerComponent>().Items.Each(e => Assert.AreEqual(e.Get<GameObject>().Location, startingPoint + Direction.S));

			Entity.Get<EquipmentComponent>().Unequip("slot1");

			Move(Direction.S);

			Assert.AreEqual(Slot1Item0.Get<GameObject>().Location, startingPoint + Direction.S);
			Assert.AreEqual(Slot2Item0.Get<GameObject>().Location, startingPoint + Direction.S + Direction.S);
		}
		
		private void Move(Direction d) {
			var action = new WalkAction(Entity, d);
			action.OnProcess();
		}
	}
}
