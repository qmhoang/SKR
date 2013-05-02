using System;
using System.Linq;
using NUnit.Framework;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;

namespace SKRTests.Actions.Items {
	[TestFixture]
	public class DropItemActionTests : ItemTestsHelper {
		[Test]
		public void TestDropItem() {
			PickUp(Item0);

			Assert.AreEqual(Entity.Get<ItemContainerComponent>().Count, 1);
			Assert.AreEqual(Item0.Get<GameObject>().Location, Entity.Get<GameObject>().Location);
			Assert.AreEqual(Item0.Get<VisibleComponent>().VisibilityIndex, -1);
			CollectionAssert.Contains(Entity.Get<ItemContainerComponent>().Items, Item0);

			Drop(Item0);

			Assert.AreEqual(Entity.Get<ItemContainerComponent>().Count, 0);
			Assert.AreEqual(Item0.Get<VisibleComponent>().VisibilityIndex, 10);
			CollectionAssert.DoesNotContain(Entity.Get<ItemContainerComponent>().Items, Item0);
		}

		[Test]
		public void TestDropStackedItem() {
			StackedItem0.Get<Item>().Amount = 5;

			PickUp(StackedItem0, 5);

			Assert.AreEqual(1, Entity.Get<ItemContainerComponent>().Count);
			Assert.AreEqual(5, Entity.Get<ItemContainerComponent>().TotalCount);

			Drop(StackedItem0, 2);

			Assert.AreEqual(1, Entity.Get<ItemContainerComponent>().Count);
			Assert.AreEqual(3, Entity.Get<ItemContainerComponent>().TotalCount);			

			var dropped = Level.GetEntitiesAt(Entity.Get<GameObject>().Location).Where(e => e.Has<Item>() && !Entity.Get<ItemContainerComponent>().Contains(e)).First();
			Assert.AreEqual(2, dropped.Get<Item>().Amount);
			Assert.AreNotSame(dropped, StackedItem0);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void TestItemNotInInventory() {
			Assert.IsFalse(Entity.Get<ItemContainerComponent>().Contains(Item0));

			Drop(Item0);
		}
	}
}