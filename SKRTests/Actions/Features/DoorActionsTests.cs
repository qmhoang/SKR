using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Components;
using DEngine.Entities;
using NUnit.Framework;
using SkrGame.Actions.Features;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Features;

namespace SKRTests.Actions.Features {
	public class OpeningActions : ActionTests {
		protected Entity Door;

		[SetUp]
		public void SetUp() {
			// .....
			// .....
			// ..@+.
			// .....
			// .....
			Door = EntityManager.Create(new List<Component>
			                            {
			                            		new Location(3, 2, Level),
			                            		new Sprite("closed", Sprite.FEATURES_LAYER),
			                            		new Opening("opened", "closed", "open", "close"),
												new Blocker(false, false)
			                            });
		}

		protected void Open(Entity door) {
			var action = new OpenDoorAction(Entity, door);
			action.OnProcess();
		}

		protected void Close(Entity door) {
			var action = new CloseDoorAction(Entity, door);
			action.OnProcess();
		}

		protected void Toggle(Entity door) {
			Entity.Get<ActorComponent>().Enqueue(new ToggleDoorAction(Entity, door));

			while (Entity.Get<ActorComponent>().Actor.Actions.Count > 0) {
				Entity.Get<ActorComponent>().Actor.Actions.Dequeue().OnProcess();
			}
		}
	}

	[TestFixture]
	public class DoorActionsTests : OpeningActions {

		[Test]
		public void TestOpenDoor() {
			Assert.AreEqual(Opening.OpeningStatus.Closed, Door.Get<Opening>().Status);
			Assert.AreEqual("closed", Door.Get<Sprite>().Asset);
			Assert.IsFalse(Level.IsWalkable(Door.Get<Location>().Point));
			Assert.IsFalse(Level.IsTransparent(Door.Get<Location>().Point));
			
			Open(Door);

			Assert.AreEqual(Opening.OpeningStatus.Opened, Door.Get<Opening>().Status);
			Assert.AreEqual("opened", Door.Get<Sprite>().Asset);
			Assert.IsTrue(Level.IsWalkable(Door.Get<Location>().Point));
			Assert.IsTrue(Level.IsTransparent(Door.Get<Location>().Point));
		}

		[Test]
		public void TestCloseDoor() {
			Open(Door);

			Assert.AreEqual(Opening.OpeningStatus.Opened, Door.Get<Opening>().Status);
			Assert.AreEqual("opened", Door.Get<Sprite>().Asset);
			Assert.IsTrue(Level.IsWalkable(Door.Get<Location>().Point));
			Assert.IsTrue(Level.IsTransparent(Door.Get<Location>().Point));

			Close(Door);

			Assert.AreEqual(Opening.OpeningStatus.Closed, Door.Get<Opening>().Status);
			Assert.AreEqual("closed", Door.Get<Sprite>().Asset);
			Assert.IsFalse(Level.IsWalkable(Door.Get<Location>().Point));
			Assert.IsFalse(Level.IsTransparent(Door.Get<Location>().Point));
		}

		[Test]
		public void TestToggleDoor() {
			Assert.AreEqual(Opening.OpeningStatus.Closed, Door.Get<Opening>().Status);
			Assert.AreEqual("closed", Door.Get<Sprite>().Asset);
			Assert.IsFalse(Level.IsWalkable(Door.Get<Location>().Point));
			Assert.IsFalse(Level.IsTransparent(Door.Get<Location>().Point));

			Toggle(Door);

			Assert.AreEqual(Opening.OpeningStatus.Opened, Door.Get<Opening>().Status);
			Assert.AreEqual("opened", Door.Get<Sprite>().Asset);
			Assert.IsTrue(Level.IsWalkable(Door.Get<Location>().Point));
			Assert.IsTrue(Level.IsTransparent(Door.Get<Location>().Point));

			Toggle(Door);

			Assert.AreEqual(Opening.OpeningStatus.Closed, Door.Get<Opening>().Status);
			Assert.AreEqual("closed", Door.Get<Sprite>().Asset);
			Assert.IsFalse(Level.IsWalkable(Door.Get<Location>().Point));
			Assert.IsFalse(Level.IsTransparent(Door.Get<Location>().Point));
		}
	}
}
