using System.Linq;
using NUnit.Framework;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;

namespace SKRTests.Actions {
	[TestFixture]
	public class DropItemActionTests : ItemTestsHelper {
		[Test]
		public void DropItem() {
			PickUp(Item);

			Assert.AreEqual(Entity.Get<ContainerComponent>().Count, 1);
			Assert.AreEqual(Item.Get<Location>().Point, Entity.Get<Location>().Point);
			Assert.AreEqual(Item.Get<VisibleComponent>().VisibilityIndex, -1);
			CollectionAssert.Contains(Entity.Get<ContainerComponent>().Items, Item);

			Drop(Item);

			Assert.AreEqual(Entity.Get<ContainerComponent>().Count, 0);
			Assert.AreEqual(Item.Get<VisibleComponent>().VisibilityIndex, 10);
			CollectionAssert.DoesNotContain(Entity.Get<ContainerComponent>().Items, Item);
		}

		[Test]
		public void DropStackedItem() {
			StackedItem0.Get<Item>().Amount = 5;

			PickUp(StackedItem0, 5);

			Assert.AreEqual(1, Entity.Get<ContainerComponent>().Count);
			Assert.AreEqual(5, Entity.Get<ContainerComponent>().TotalCount);

			Drop(StackedItem0, 2);

			Assert.AreEqual(1, Entity.Get<ContainerComponent>().Count);
			Assert.AreEqual(3, Entity.Get<ContainerComponent>().TotalCount);			

			var dropped = Level.GetEntitiesAt(Entity.Get<Location>().Point).Where(e => e.Has<Item>() && !Entity.Get<ContainerComponent>().Contains(e)).First();
			Assert.AreEqual(2, dropped.Get<Item>().Amount);
			Assert.AreNotSame(dropped, StackedItem0);
		}


	}
}