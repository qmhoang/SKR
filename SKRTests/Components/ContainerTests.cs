using System;
using System.Collections.Generic;
using System.Linq;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using DEngine.Extensions;
using NUnit.Framework;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;

namespace SKRTests.Components {
	[TestFixture]
	internal class ContainerTests : ItemTestsHelper {

		protected ItemContainerComponent ItemContainer { get { return Entity.Get<ItemContainerComponent>(); } }

		[Test]
		public void TestAdd() {
			ItemContainer.Add(Item0);

			Assert.AreEqual(ItemContainer.Count, 1);

			ItemContainer.Add(EntityManager.Create(new List<Component>
			                                   {
			                                   		new Item(new Item.Template()
			                                   		         {})
			                                   }));
			Assert.AreEqual(ItemContainer.Count, 2);

			Assert.Throws<ArgumentNullException>(() => ItemContainer.Add(null));

			// adding an item that exist already returns false
			Assert.IsFalse(ItemContainer.Add(Item0));
			Assert.AreEqual(ItemContainer.Count, 2);
		}

		[Test]
		public void TestAddStackItem() {
			ItemContainer.Add(StackedItem0);
			Assert.AreEqual(ItemContainer.Count, 1);
			Assert.AreEqual(ItemContainer.TotalCount, 1);

			ItemContainer.Add(StackedItem1);
			Assert.AreEqual(ItemContainer.Count, 1);
			Assert.AreEqual(ItemContainer.TotalCount, 2);
			Assert.AreEqual(ItemContainer.GetItem(e => e.Get<ReferenceId>() == StackedItem0.Get<ReferenceId>()).Get<Item>().Amount, 2);
		}

		[Test]
		public void TestRemove() {
			ItemContainer.Add(Item0);
			Assert.AreEqual(ItemContainer.Count, 1);

			Assert.IsTrue(ItemContainer.Remove(Item0));
			Assert.AreEqual(ItemContainer.Count, 0);

			Assert.IsFalse(ItemContainer.Remove(Item0));
		}

		[Test]
		public void TestPredicate() {
			ItemContainer.Add(Item0);

			Assert.IsTrue(ItemContainer.Contains(Item0));

			Assert.IsTrue(ItemContainer.Exist(i => i == Item0));

			// testing negative
			var item1 = EntityManager.Create(new List<Component>
			                                 {
			                                 		new Item(new Item.Template()
			                                 		         {})
			                                 });
			Assert.IsFalse(ItemContainer.Contains(item1));
			Assert.IsFalse(ItemContainer.Exist(i => i == item1));
		}

		[Test]
		public void TestGet() {
			ItemContainer.Add(Item0);
			Assert.IsNull(ItemContainer.GetItem(i => i.Id.ToString() == ""));
			Assert.AreSame(ItemContainer.GetItem(i => i.Id == Item0.Id), Item0);
		}

		[Test]
		public void TestCopy() {
			ItemContainer.Add(Item0);
			ItemContainer.Add(Item1);
			ItemContainer.Add(Item2);

			Assert.AreEqual(ItemContainer.TotalCount, 3);
			CollectionAssert.AllItemsAreUnique(ItemContainer.Items);
			CollectionAssert.Contains(ItemContainer.Items, Item0);
			CollectionAssert.Contains(ItemContainer.Items, Item1);
			CollectionAssert.Contains(ItemContainer.Items, Item2);

			var copy = Entity.Copy().Get<ItemContainerComponent>();
			Assert.AreNotSame(Entity, copy);
			Assert.AreEqual(copy.TotalCount, 3);
			CollectionAssert.AllItemsAreUnique(copy.Items);

			// items in conainer aren't in new container
			foreach (var e in ItemContainer.Items) {
				CollectionAssert.DoesNotContain(copy.Items, e);
			}
			// vice versa
			foreach (var e in copy.Items) {
				CollectionAssert.DoesNotContain(ItemContainer.Items, e);
			}
		}
	}
}