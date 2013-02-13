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
		public void Add() {
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
		public void AddStackItem() {
			container.Add(stack1);
			Assert.AreEqual(container.Count, 1);
			Assert.AreEqual(container.TotalCount, 1);

			container.Add(stack2);
			Assert.AreEqual(container.Count, 1);
			Assert.AreEqual(container.TotalCount, 2);
			Assert.AreEqual(container.GetItem(e => e.Get<ReferenceId>().RefId == "stack").Get<Item>().Amount, 2);
		}

		[Test]
		public void Remove() {
			container.Add(item0);
			Assert.AreEqual(container.Count, 1);

			Assert.IsTrue(container.Remove(item0));
			Assert.AreEqual(container.Count, 0);

			Assert.IsFalse(container.Remove(item0));
		}

		[Test]
		public void Predicate() {
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
		public void Get() {
			container.Add(item0);
			Assert.IsNull(container.GetItem(i => i.Id.ToString() == ""));
			Assert.AreSame(container.GetItem(i => i.Id == item0.Id), item0);
		}

		[Test]
		public void Move() {
			container.Add(item0);
			container.Add(item1);

			entity.Get<ContainerComponent>().Items.Each(e => Assert.AreEqual(e.Get<Location>().Point, entity.Get<Location>().Point));

			var p1 = new Point(4, 8);
			entity.Get<Location>().Point = p1;
			entity.Get<ContainerComponent>().Items.Each(e => Assert.AreEqual(e.Get<Location>().Point, p1));

			var p2 = new Point(9, -8);
			entity.Get<Location>().Point = p2;
			entity.Get<ContainerComponent>().Items.Each(e => Assert.AreEqual(e.Get<Location>().Point, p2));
		}

		[Test]
		public void MoveAlot() {
			container.Add(item0);
			container.Add(item1);
			container.Add(item2);

			entity.Get<ContainerComponent>().Items.Each(e => Assert.AreEqual(e.Get<Location>().Point, entity.Get<Location>().Point));

			entity.Get<Location>().Point = new Point(4, 8);
			entity.Get<Location>().Point = new Point(51, -9);

			var Location = new Point(0, 3812);
			entity.Get<Location>().Point = Location;
			entity.Get<ContainerComponent>().Items.Each(e => Assert.AreEqual(e.Get<Location>().Point, Location));
		}

		[Test]
		public void CopyMove() {
			container.Add(item0);
			container.Add(item1);
			container.Add(item2);

			var copy = entity.Copy();

			Assert.AreEqual(copy.Get<ContainerComponent>().Count, entity.Get<ContainerComponent>().Count);


			copy.Get<Location>().Point = new Point(934, -83);

			Assert.AreNotEqual(entity.Get<Location>().Point, copy.Get<Location>().Point);
			copy.Get<ContainerComponent>().Items.Each(e => Assert.AreEqual(e.Get<Location>().Point, copy.Get<Location>().Point));
		}

		[Test]
		public void RemovedNotMoved() {
			container.Add(item0);
			container.Add(item1);

			var oldLoc = new Point(0, 3812);
			entity.Get<Location>().Point = oldLoc;

			entity.Get<ContainerComponent>().Items.Each(e => Assert.AreEqual(e.Get<Location>().Point, oldLoc));

			container.Remove(item1);
			var newLoc = new Point(51, -9);
			entity.Get<Location>().Point = newLoc;

			entity.Get<ContainerComponent>().Items.Each(e => Assert.AreEqual(e.Get<Location>().Point, newLoc));
			Assert.AreEqual(item1.Get<Location>().Point, oldLoc);
		}

		[Test]
		public void Copy() {
			container.Add(item0);
			container.Add(item1);
			container.Add(item2);

			Assert.AreEqual(container.TotalCount, 3);
			CollectionAssert.AllItemsAreUnique(container.Items);
			CollectionAssert.Contains(container.Items, item0);
			CollectionAssert.Contains(container.Items, item1);
			CollectionAssert.Contains(container.Items, item2);

			var copy = entity.Copy().Get<ContainerComponent>();
			Assert.AreEqual(copy.TotalCount, 3);
			CollectionAssert.AllItemsAreUnique(copy.Items);

			// items in conainer aren't in new container
			foreach (var e in container.Items) {
				CollectionAssert.DoesNotContain(copy.Items, e);
			}
			// vice versa
			foreach (var e in copy.Items) {
				CollectionAssert.DoesNotContain(container.Items, e);
			}
		}
	}
}