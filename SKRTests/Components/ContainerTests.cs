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

		protected ContainerComponent Container { get { return Entity.Get<ContainerComponent>(); } }

		[Test]
		public void TestAdd() {
			Container.Add(Item0);

			Assert.AreEqual(Container.Count, 1);

			Container.Add(EntityManager.Create(new List<Component>
			                                   {
			                                   		new Item(new Item.Template()
			                                   		         {})
			                                   }));
			Assert.AreEqual(Container.Count, 2);

			Assert.Throws<ArgumentNullException>(() => Container.Add(null));

			// adding an item that exist already returns false
			Assert.IsFalse(Container.Add(Item0));
			Assert.AreEqual(Container.Count, 2);
		}

		[Test]
		public void TestAddStackItem() {
			Container.Add(StackedItem0);
			Assert.AreEqual(Container.Count, 1);
			Assert.AreEqual(Container.TotalCount, 1);

			Container.Add(StackedItem1);
			Assert.AreEqual(Container.Count, 1);
			Assert.AreEqual(Container.TotalCount, 2);
			Assert.AreEqual(Container.GetItem(e => e.Get<ReferenceId>().RefId == "item").Get<Item>().Amount, 2);
		}

		[Test]
		public void TestRemove() {
			Container.Add(Item0);
			Assert.AreEqual(Container.Count, 1);

			Assert.IsTrue(Container.Remove(Item0));
			Assert.AreEqual(Container.Count, 0);

			Assert.IsFalse(Container.Remove(Item0));
		}

		[Test]
		public void TestPredicate() {
			Container.Add(Item0);

			Assert.IsTrue(Container.Contains(Item0));

			Assert.IsTrue(Container.Exist(i => i == Item0));

			// testing negative
			var item1 = EntityManager.Create(new List<Component>
			                                 {
			                                 		new Item(new Item.Template()
			                                 		         {})
			                                 });
			Assert.IsFalse(Container.Contains(item1));
			Assert.IsFalse(Container.Exist(i => i == item1));
		}

		[Test]
		public void TestGet() {
			Container.Add(Item0);
			Assert.IsNull(Container.GetItem(i => i.Id.ToString() == ""));
			Assert.AreSame(Container.GetItem(i => i.Id == Item0.Id), Item0);
		}

		[Test]
		public void TestCopy() {
			Container.Add(Item0);
			Container.Add(Item1);
			Container.Add(Item2);

			Assert.AreEqual(Container.TotalCount, 3);
			CollectionAssert.AllItemsAreUnique(Container.Items);
			CollectionAssert.Contains(Container.Items, Item0);
			CollectionAssert.Contains(Container.Items, Item1);
			CollectionAssert.Contains(Container.Items, Item2);

			var copy = Entity.Copy().Get<ContainerComponent>();
			Assert.AreNotSame(Entity, copy);
			Assert.AreEqual(copy.TotalCount, 3);
			CollectionAssert.AllItemsAreUnique(copy.Items);

			// items in conainer aren't in new container
			foreach (var e in Container.Items) {
				CollectionAssert.DoesNotContain(copy.Items, e);
			}
			// vice versa
			foreach (var e in copy.Items) {
				CollectionAssert.DoesNotContain(Container.Items, e);
			}
		}
	}
}