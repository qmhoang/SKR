using DEngine.Core;
using NUnit.Framework;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;

namespace SKRTests.Actions.Items {
	[TestFixture]
	public class GetItemActionTests : ItemTestsHelper {
		[Test]
		public void TestPickUpItem() {
			Assert.AreEqual(Entity.Get<ItemContainerComponent>().Count, 0);
			Assert.AreEqual(Item0.Get<VisibleComponent>().VisibilityIndex, 10);
			Assert.AreEqual(Item0.Get<GameObject>().Location, new Point(-1, -1));

			PickUp(Item0);

			Assert.AreEqual(Entity.Get<ItemContainerComponent>().Count, 1);
			Assert.AreEqual(Item0.Get<GameObject>().Location, Entity.Get<GameObject>().Location);
			Assert.AreEqual(Item0.Get<VisibleComponent>().VisibilityIndex, -1);
			CollectionAssert.Contains(Entity.Get<ItemContainerComponent>().Items, Item0);
		}

		[Test]
		public void TestPickUpStacked() {
			StackedItem0.Get<Item>().Amount = 3;
			StackedItem1.Get<Item>().Amount = 3;

			PickUp(StackedItem0, 3);

			Assert.AreEqual(1, Entity.Get<ItemContainerComponent>().Count);
			Assert.AreEqual(3, Entity.Get<ItemContainerComponent>().TotalCount);

			PickUp(StackedItem1);

			Assert.AreEqual(1, Entity.Get<ItemContainerComponent>().Count);
			Assert.AreEqual(4, Entity.Get<ItemContainerComponent>().TotalCount);
			Assert.AreEqual(4, StackedItem0.Get<Item>().Amount);
			Assert.AreEqual(2, StackedItem1.Get<Item>().Amount);

			PickUp(StackedItem1, 2);

			Assert.AreEqual(6, StackedItem0.Get<Item>().Amount);			
		}
	}
}