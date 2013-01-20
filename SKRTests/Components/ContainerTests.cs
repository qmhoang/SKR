using System;
using System.Collections.Generic;
using DEngine.Components;
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

		private Entity singleItem;

		private Entity stack1;
		private Entity stack2;


		[SetUp]
		public void Init() {
			entityManager = new EntityManager();

			entity = entityManager.Create(new List<Component>()
			                              {
			                              		new ContainerComponent()
			                              });
			container = entity.Get<ContainerComponent>();

			singleItem = entityManager.Create(new List<Component>
			                                  {
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
			container.Add(singleItem);

			Assert.AreEqual(container.Count, 1);

			container.Add(entityManager.Create(new List<Component>
			                                   {
			                                   		new Item(new Item.Template()
			                                   		         {})
			                                   }));
			Assert.AreEqual(container.Count, 2);

			Assert.Throws<ArgumentNullException>(() => container.Add(null));

			// adding an item that exist already returns false
			Assert.IsFalse(container.Add(singleItem));
		}

		[Test]
		public void TestAddStackItem() {
			container.Add(stack1);
			Assert.AreEqual(container.Count, 1);

			container.Add(stack2);
			Assert.AreEqual(container.Count, 1);
			Assert.AreEqual(container.GetItem(e => e.Get<ReferenceId>().RefId == "stack").Get<Item>().Amount, 2);
		}

		[Test]
		public void TestRemove() {
			container.Add(singleItem);
			Assert.AreEqual(container.Count, 1);

			Assert.IsTrue(container.Remove(singleItem));
			Assert.AreEqual(container.Count, 0);

			Assert.Throws<ArgumentNullException>(() => container.Remove(null));
			Assert.IsFalse(container.Remove(singleItem));
		}

		[Test]
		public void TestPredicate() {
			container.Add(singleItem);

			Assert.IsTrue(container.Contains(singleItem));

			Assert.IsTrue(container.Exist(i => i == singleItem));

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
			container.Add(singleItem);
			Assert.IsNull(container.GetItem(i => i.Id.ToString() == ""));
			Assert.AreSame(container.GetItem(i => i.Id == singleItem.Id), singleItem);
		}

		[Test]
		public void TestCopy() {
			
		}
	}
}