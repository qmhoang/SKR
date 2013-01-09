using DEngine.Components;
using DEngine.Core;
using DEngine.Entity;
using NUnit.Framework;
using SkrGame.Systems;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;

namespace SkrGame.Tests.Systems {
	[TestFixture]
	class ItemContainerInteractionSubsystemTests {
		private EntityManager entityManager;
		private ItemContainerInteractionSubsystem subsystem;
		private Entity container;

		[SetUp]
		public void Init() {
			entityManager = new EntityManager();
			subsystem = new ItemContainerInteractionSubsystem(entityManager);

			container = entityManager.Create(new Template()
			                                 {
			                                 		new ContainerComponent(),
			                                 		new Location(0, 0, null)
			                                 });

			container.Get<ContainerComponent>().Add(
					entityManager.Create(new Template
					                     {
					                     		new Item(new Item.Template()
					                     		         {}),
					                     		new Location(4, 2, null)
					                     }));
			container.Get<ContainerComponent>().Add(
					entityManager.Create(new Template
					                     {
					                     		new Item(new Item.Template()
					                     		         {}),
					                     		new Location(4, 2, null)
					                     }));
		}


		[Test]
		public void TestContainersWithItemExistBeforeSubsystemInit() {
			subsystem = new ItemContainerInteractionSubsystem(entityManager);			

			foreach (var entity in container.Get<ContainerComponent>().Items) {
				Assert.AreEqual(entity.Get<Location>().Position, container.Get<Location>().Position);
			}

			container.Get<Location>().Position = new Point(4, 8);

			foreach (var entity in container.Get<ContainerComponent>().Items) {
				Assert.AreEqual(entity.Get<Location>().Position, container.Get<Location>().Position);
			}

			container.Get<Location>().Position = new Point(9, -8);

			foreach (var entity in container.Get<ContainerComponent>().Items) {
				Assert.AreEqual(entity.Get<Location>().Position, container.Get<Location>().Position);
			}
		}

		[Test]
		public void TestMovingContainer() {
			container.Get<Location>().Position = new Point(4, 8);
			container.Get<Location>().Position = new Point(51, -9);
			container.Get<Location>().Position = new Point(0, 3812);

			foreach (var entity in container.Get<ContainerComponent>().Items) {
				Assert.AreEqual(entity.Get<Location>().Position, container.Get<Location>().Position);
			}
		}

		[Test]
		public void TestDeepCopyEvents() {
			var newContainer = container.Copy();

			Assert.AreEqual(newContainer.Get<ContainerComponent>().Count, container.Get<ContainerComponent>().Count);

			newContainer.Get<Location>().Position = new Point(934, -83);

			Assert.AreNotEqual(newContainer.Get<Location>(), container.Get<Location>());			

			foreach (var entity in newContainer.Get<ContainerComponent>()) {
				Assert.AreEqual(entity.Get<Location>(), newContainer.Get<Location>());
			}
		}

		[Test]
		public void TestItemRemovedFromContainer() {
			var itemToBeRemoved = entityManager.Create(new Template
			                                  {
			                                  		new Item(new Item.Template()
			                                  		         {}),
			                                  		new Location(4, 2, null)
			                                  });
			container.Get<ContainerComponent>().Add(itemToBeRemoved);

			var location1stMove = new Point(0, 3812);
			container.Get<Location>().Position = location1stMove;
		
			foreach (var entity in container.Get<ContainerComponent>().Items) {
				Assert.AreEqual(entity.Get<Location>().Position, container.Get<Location>().Position);
			}

			container.Get<ContainerComponent>().Remove(itemToBeRemoved);

			container.Get<Location>().Position = new Point(51, -9);
			foreach (var entity in container.Get<ContainerComponent>().Items) {
				Assert.AreEqual(entity.Get<Location>().Position, container.Get<Location>().Position);
			}

			Assert.AreEqual(itemToBeRemoved.Get<Location>().Position, location1stMove);			
		}
	}
}
