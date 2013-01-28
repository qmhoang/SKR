using System.Collections.Generic;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using NUnit.Framework;
using SkrGame.Systems;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;

namespace SKRTests.Systems {
	[TestFixture]
	class ItemContainerSubsystemTests {
		private EntityManager entityManager;
		private ItemContainerSubsystem subsystem;
		private Entity entity;

		private Entity item0;
		private Entity item1;

		[SetUp]
		public void Init() {
			entityManager = new EntityManager();
			subsystem = new ItemContainerSubsystem(entityManager);

			entity = entityManager.Create(new List<Component>()
			                                 {
			                                 		new ContainerComponent(),
			                                 		new Location(0, 0, null)
			                                 });

			item0 = entityManager.Create(new List<Component>
			                                 {
			                                 		new Item(new Item.Template()
			                                 		         {}),
			                                 		new Location(4, 2, null)
			                                 });

			item1 = entityManager.Create(new List<Component>
			                                 {
			                                 		new Item(new Item.Template()
			                                 		         {}),
			                                 		new Location(4, 2, null)
			                                 });
			entity.Get<ContainerComponent>().Add(item0);
			entity.Get<ContainerComponent>().Add(item1);
		}


		[Test]
		public void TestContainersWithItem() {
			entity.Get<ContainerComponent>().Add(item0);
			entity.Get<ContainerComponent>().Add(item1);

			subsystem = new ItemContainerSubsystem(entityManager);			

			foreach (var e in entity.Get<ContainerComponent>().Items) {
				Assert.AreEqual(e.Get<Location>().Position, this.entity.Get<Location>().Position);
			}

			entity.Get<Location>().Position = new Point(4, 8);

			foreach (var e in entity.Get<ContainerComponent>().Items) {
				Assert.AreEqual(e.Get<Location>().Position, this.entity.Get<Location>().Position);
			}

			entity.Get<Location>().Position = new Point(9, -8);

			foreach (var e in entity.Get<ContainerComponent>().Items) {
				Assert.AreEqual(e.Get<Location>().Position, this.entity.Get<Location>().Position);
			}
		}

		[Test]
		public void TestMovingContainer() {
			foreach (var e in entity.Get<ContainerComponent>().Items) {
				Assert.AreEqual(e.Get<Location>().Position, entity.Get<Location>().Position);
			}

			entity.Get<Location>().Position = new Point(4, 8);
			entity.Get<Location>().Position = new Point(51, -9);
			entity.Get<Location>().Position = new Point(0, 3812);

			foreach (var e in entity.Get<ContainerComponent>().Items) {
				Assert.AreEqual(e.Get<Location>().Position, this.entity.Get<Location>().Position);
			}
		}

		[Test]
		public void TestDeepCopyEvents() {
			var newContainer = entity.Copy();

			Assert.AreEqual(newContainer.Get<ContainerComponent>().Count, entity.Get<ContainerComponent>().Count);

			newContainer.Get<Location>().Position = new Point(934, -83);

			Assert.AreNotEqual(newContainer.Get<Location>(), entity.Get<Location>());			

			foreach (var e in newContainer.Get<ContainerComponent>().Items) {
				Assert.AreEqual(e.Get<Location>(), newContainer.Get<Location>());
			}
		}

		[Test]
		public void TestItemRemovedFromContainer() {
			var itemToBeRemoved = entityManager.Create(new List<Component>
			                                           {
			                                           		new Item(new Item.Template()
			                                           		         {}),
			                                           		new Location(4, 2, null)
			                                           });
			entity.Get<ContainerComponent>().Add(itemToBeRemoved);

			var loc = new Point(0, 3812);
			entity.Get<Location>().Position = loc;

			foreach (var e in entity.Get<ContainerComponent>().Items) {
				Assert.AreEqual(e.Get<Location>().Position, entity.Get<Location>().Position);
			}

			entity.Get<ContainerComponent>().Remove(itemToBeRemoved);

			entity.Get<Location>().Position = new Point(51, -9);
			foreach (var e in entity.Get<ContainerComponent>().Items) {
				Assert.AreEqual(e.Get<Location>().Position, entity.Get<Location>().Position);
			}

			Assert.AreEqual(itemToBeRemoved.Get<Location>().Position, loc);		
		}
	}
}
