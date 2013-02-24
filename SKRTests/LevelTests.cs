using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Core;
using DEngine.Entities;
using NUnit.Framework;
using SkrGame.Universe;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Features;
using SkrGame.Universe.Locations;

namespace SKRTests {
	[TestFixture]
	public class LevelTests {
		private EntityManager entityManager;
		private Level level;
		private World world;

		[SetUp]
		public void SetUp() {
			world = new World();
			entityManager = world.EntityManager;

			world.CurrentLevel = level = new Level(new Size(5, 5),
									   world,
									   "Floor",
									   new List<Terrain>
			                                       {
			                                       		new Terrain("Floor", "Floor", true, true, 1.0),
			                                       		new Terrain("Wall", "Wall", false, false, 0.0)
			                                       });
		}

		[Test]
		public void TestEmptyFilledLevel() {
			for (int x = 0; x < level.Size.Width; x++) {
				for (int y = 0; y < level.Size.Height; y++) {
					Assert.AreEqual(level.GetTerrain(x, y).Definition, "Floor");
					Assert.IsTrue(level.GetTerrain(x, y).Transparent);
					Assert.IsTrue(level.GetTerrain(x, y).Walkable);
				}
			}
		}

		[Test]
		public void TestTerrain() {
			// .....
			// .....
			// ...#.
			// .....
			// .....

			level.SetTerrain(3, 2, "Wall");

			Assert.AreEqual(level.GetTerrain(3, 2).Definition, "Wall");
			Assert.IsFalse(level.GetTerrain(3, 2).Transparent);
			Assert.IsFalse(level.GetTerrain(3, 2).Walkable);
		}

		[Test]
		public void TestTerrainBlocking() {
			// .....
			// .#...
			// .....
			// .....
			// .....

			level.SetTerrain(1, 1, "Wall");

			Assert.IsTrue(level.IsWalkable(0, 0));
			Assert.IsTrue(level.IsTransparent(0, 0));

			Assert.IsFalse(level.IsWalkable(1, 1));
			Assert.IsFalse(level.IsTransparent(1, 1));
		}

		[Test]
		public void TestSize() {
			Assert.AreEqual(level.Size, new Size(5, 5));
			Assert.AreEqual(level.Width, 5);
			Assert.AreEqual(level.Height, 5);
		}

		[Test]
		public void TestEntitiesBlock() {
			Assert.IsTrue(level.IsWalkable(1, 1));
			Assert.IsTrue(level.IsTransparent(1, 1));

			entityManager.Create().Add(new Scenery(false, false)).Add(new GameObject(1, 1, level));

			Assert.AreEqual(1, level.GetEntities().Count());

			Assert.IsFalse(level.IsWalkable(1, 1));
			Assert.IsFalse(level.IsTransparent(1, 1));			
		}

		[Test]
		public void TestBlockerEntityPropertyChange() {
			var blocker = entityManager.Create().Add(new Scenery(false, false)).Add(new GameObject(1, 1, level));

			Assert.IsFalse(level.IsWalkable(1, 1));
			Assert.IsFalse(level.IsTransparent(1, 1));

			blocker.Get<Scenery>().Transparent = true;

			Assert.IsFalse(level.IsWalkable(1, 1));
			Assert.IsTrue(level.IsTransparent(1, 1));
		}

		[Test]
		public void TestBlockerEntityMoved() {
			var blocker = entityManager.Create().Add(new Scenery(false, false)).Add(new GameObject(1, 1, level));

			Assert.IsFalse(level.IsWalkable(1, 1));
			Assert.IsFalse(level.IsTransparent(1, 1));

			blocker.Get<GameObject>().Location = new Point(0, 0);

			// old location is now walkable/transparent
			Assert.IsTrue(level.IsWalkable(1, 1));
			Assert.IsTrue(level.IsTransparent(1, 1));

			Assert.IsFalse(level.IsWalkable(0, 0));
			Assert.IsFalse(level.IsTransparent(0, 0));
		}

		[Test]
		public void TestMultipleBlockersPropertyChange() {
			var b1 = entityManager.Create().Add(new Scenery(false, false)).Add(new GameObject(1, 1, level));
			var b2 = entityManager.Create().Add(new Scenery(false, false)).Add(new GameObject(1, 1, level));

			Assert.IsFalse(level.IsWalkable(1, 1));
			Assert.IsFalse(level.IsTransparent(1, 1));

			b1.Get<Scenery>().Transparent = true;

			Assert.IsFalse(level.IsWalkable(1, 1));
			Assert.IsFalse(level.IsTransparent(1, 1));	// still false because of b2 still blocks
		}

		[Test]
		public void TestMultipleBlockersMove() {
			var b1 = entityManager.Create().Add(new Scenery(false, false)).Add(new GameObject(1, 1, level));
			var b2 = entityManager.Create().Add(new Scenery(false, false)).Add(new GameObject(1, 1, level));

			Assert.IsFalse(level.IsWalkable(1, 1));
			Assert.IsFalse(level.IsTransparent(1, 1));

			b1.Get<GameObject>().Location = new Point(0, 0);

			// old location is still not walkable/transparent because of b2
			Assert.IsFalse(level.IsWalkable(1, 1));
			Assert.IsFalse(level.IsTransparent(1, 1));

			Assert.IsFalse(level.IsWalkable(0, 0));
			Assert.IsFalse(level.IsTransparent(0, 0));
		}

		[Test]
		public void TestGetEntities() {
			var e1 = entityManager.Create().Add(new Scenery(false, false)).Add(new GameObject(1, 1, level));
			var e2 = entityManager.Create().Add(new Scenery(false, false)).Add(new GameObject(1, 1, level));
			var e3 = entityManager.Create().Add(new Scenery(false, false)).Add(new GameObject(1, 2, level));

			CollectionAssert.Contains(level.GetEntities(), e1);
			CollectionAssert.Contains(level.GetEntities(), e2);
			CollectionAssert.Contains(level.GetEntities(), e3);			
		}

		[Test]
		public void TestGetEntitiesAt() {
			var e1 = entityManager.Create().Add(new Scenery(false, false)).Add(new GameObject(1, 1, level));
			var e2 = entityManager.Create().Add(new Scenery(false, false)).Add(new GameObject(1, 1, level));
			var e3 = entityManager.Create().Add(new Scenery(false, false)).Add(new GameObject(1, 2, level));

			CollectionAssert.Contains(level.GetEntitiesAt(new Point(1, 1)), e1);
			CollectionAssert.Contains(level.GetEntitiesAt(new Point(1, 1)), e2);
			CollectionAssert.Contains(level.GetEntitiesAt(new Point(1, 2)), e3);
		}
	}
}
