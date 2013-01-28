using System;
using System.Collections.Generic;
using System.Linq;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using NUnit.Framework;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;

namespace SKRTests.Components {
	[TestFixture]
	internal class ContainerTests {
		private EntityManager entityManager;

		private Entity entity;
		private ContainerComponent container;

		private Entity item0;
		private Entity item1;
		private Entity item2;

		private Entity stack1;
		private Entity stack2;


		[SetUp]
		public void Init() {
			entityManager = new EntityManager();

			entity = entityManager.Create(new List<Component>()
			                              {
			                             		new Location(0, 0, null),
			                              		new ContainerComponent()
			                              });
			container = entity.Get<ContainerComponent>();

			item0 = entityManager.Create(new List<Component>
			                             {
			                             		new Location(-1, -1, null),
			                             		new Item(new Item.Template
			                             		         {})

			                             });
			item1 = entityManager.Create(new List<Component>
			                             {
			                             		new Location(-1, -1, null),
			                             		new Item(new Item.Template
			                             		         {})
			                             });
			item2 = entityManager.Create(new List<Component>
			                             {
			                             		new Location(-1, -1, null),
			                             		new Item(new Item.Template
			                             		         {})
			                             });

			stack1 = entityManager.Create(new List<Component>
			                              {
			                              		new Item(new Item.Template
			                              		         {
			                              		         		StackType = StackType.Hard
			                              		         }),
			                              		new ReferenceId("stack")
			                              });

			stack2 = entityManager.Create(new List<Component>
			                              {
			                              		new Item(new Item.Template
			                              		         {
			                              		         		StackType = StackType.Hard
			                              		         }),
			                              		new ReferenceId("stack")
			                              });
		}

		[Test]
		public void TestAdd() {
			container.Add(item0);

			Assert.AreEqual(container.Count, 1);

			container.Add(entityManager.Create(new List<Component>
			                                   {
			                                   		new Item(new Item.Template()
			                                   		         {})
			                                   }));
			Assert.AreEqual(container.Count, 2);

			Assert.Throws<ArgumentNullException>(() => container.Add(null));

			// adding an item that exist already returns false
			Assert.IsFalse(container.Add(item0));
			Assert.AreEqual(container.Count, 2);
		}

		[Test]
		public void TestAddStackItem() {
			container.Add(stack1);
			Assert.AreEqual(container.Count, 1);
			Assert.AreEqual(container.TotalCount, 1);

			container.Add(stack2);
			Assert.AreEqual(container.Count, 1);
			Assert.AreEqual(container.TotalCount, 2);
			Assert.AreEqual(container.GetItem(e => e.Get<ReferenceId>().RefId == "stack").Get<Item>().Amount, 2);
		}

		[Test]
		public void TestRemove() {
			container.Add(item0);
			Assert.AreEqual(container.Count, 1);

			Assert.IsTrue(container.Remove(item0));
			Assert.AreEqual(container.Count, 0);

			Assert.IsFalse(container.Remove(item0));
		}

		[Test]
		public void TestPredicate() {
			container.Add(item0);

			Assert.IsTrue(container.Contains(item0));

			Assert.IsTrue(container.Exist(i => i == item0));

			// testing negative
			var item1 = entityManager.Create(new List<Component>
			                                 {
			                                 		new Item(new Item.Template()
			                                 		         {})
			                                 });
			Assert.IsFalse(container.Contains(item1));
			Assert.IsFalse(container.Exist(i => i == item1));
		}

		[Test]
		public void TestGet() {
			container.Add(item0);
			Assert.IsNull(container.GetItem(i => i.Id.ToString() == ""));
			Assert.AreSame(container.GetItem(i => i.Id == item0.Id), item0);
		}

		[Test]
		public void TestCopy() {
			container.Add(item0);
			container.Add(item1);
			container.Add(item2);

			Assert.AreEqual(container.TotalCount, 3);
			CollectionAssert.AllItemsAreUnique(container.Items);
			CollectionAssert.Contains(container.Items, item0);
			CollectionAssert.Contains(container.Items, item1);
			CollectionAssert.Contains(container.Items, item2);

			var nc = entity.Copy().Get<ContainerComponent>();
			Assert.AreEqual(nc.TotalCount, 3);
			CollectionAssert.AllItemsAreUnique(nc.Items);

			// items in conainer aren't in new container
			foreach (var e in container.Items) {
				CollectionAssert.DoesNotContain(nc.Items, e);
			}
			// vice versa
			foreach (var e in nc.Items) {
				CollectionAssert.DoesNotContain(container.Items, e);
			}
		}
	}
}