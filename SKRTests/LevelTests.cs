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
		private EntityManager _entityManager;
		private Level _level;
		private World _world;

		[SetUp]
		public void SetUp() {
			_world = new World();
			_entityManager = _world.EntityManager;

			_world.CurrentLevel = _level = new Level(new Size(5, 5),
									   _world,
									   "Floor",
									   new List<Terrain>
			                                       {
			                                       		new Terrain("Floor", "Floor", true, true, 1.0),
			                                       		new Terrain("Wall", "Wall", false, false, 0.0)
			                                       });
		}

		[Test]
		public void TestEmptyFilledLevel() {
			for (int x = 0; x < _level.Size.Width; x++) {
				for (int y = 0; y < _level.Size.Height; y++) {
					Assert.AreEqual(_level.GetTerrain(x, y).Definition, "Floor");
					Assert.IsTrue(_level.GetTerrain(x, y).Transparent);
					Assert.IsTrue(_level.GetTerrain(x, y).Walkable);
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

			_level.SetTerrain(3, 2, "Wall");

			Assert.AreEqual(_level.GetTerrain(3, 2).Definition, "Wall");
			Assert.IsFalse(_level.GetTerrain(3, 2).Transparent);
			Assert.IsFalse(_level.GetTerrain(3, 2).Walkable);
		}

		[Test]
		public void TestTerrainBlocking() {
			// .....
			// .#...
			// .....
			// .....
			// .....

			_level.SetTerrain(1, 1, "Wall");

			Assert.IsTrue(_level.IsWalkable(0, 0));
			Assert.IsTrue(_level.IsTransparent(0, 0));

			Assert.IsFalse(_level.IsWalkable(1, 1));
			Assert.IsFalse(_level.IsTransparent(1, 1));
		}

		[Test]
		public void TestSize() {
			Assert.AreEqual(_level.Size, new Size(5, 5));
			Assert.AreEqual(_level.Width, 5);
			Assert.AreEqual(_level.Height, 5);
		}

		[Test]
		public void TestEntitiesBlock() {
			Assert.IsTrue(_level.IsWalkable(1, 1));
			Assert.IsTrue(_level.IsTransparent(1, 1));

			_entityManager.Create().Add(new Scenery(false, false)).Add(new GameObject(1, 1, _level));

			Assert.AreEqual(1, _level.GetEntities().Count());

			Assert.IsFalse(_level.IsWalkable(1, 1));
			Assert.IsFalse(_level.IsTransparent(1, 1));			
		}

		[Test]
		public void TestBlockerEntityPropertyChange() {
			var blocker = _entityManager.Create().Add(new Scenery(false, false)).Add(new GameObject(1, 1, _level));

			Assert.IsFalse(_level.IsWalkable(1, 1));
			Assert.IsFalse(_level.IsTransparent(1, 1));

			blocker.Get<Scenery>().Transparent = true;

			Assert.IsFalse(_level.IsWalkable(1, 1));
			Assert.IsTrue(_level.IsTransparent(1, 1));
		}

		[Test]
		public void TestBlockerEntityMoved() {
			var blocker = _entityManager.Create().Add(new Scenery(false, false)).Add(new GameObject(1, 1, _level));

			Assert.IsFalse(_level.IsWalkable(1, 1));
			Assert.IsFalse(_level.IsTransparent(1, 1));

			blocker.Get<GameObject>().Location = new Point(0, 0);

			// old location is now walkable/transparent
			Assert.IsTrue(_level.IsWalkable(1, 1));
			Assert.IsTrue(_level.IsTransparent(1, 1));

			Assert.IsFalse(_level.IsWalkable(0, 0));
			Assert.IsFalse(_level.IsTransparent(0, 0));
		}

		[Test]
		public void TestMultipleBlockersPropertyChange() {
			var b1 = _entityManager.Create().Add(new Scenery(false, false)).Add(new GameObject(1, 1, _level));
			var b2 = _entityManager.Create().Add(new Scenery(false, false)).Add(new GameObject(1, 1, _level));

			Assert.IsFalse(_level.IsWalkable(1, 1));
			Assert.IsFalse(_level.IsTransparent(1, 1));

			b1.Get<Scenery>().Transparent = true;

			Assert.IsFalse(_level.IsWalkable(1, 1));
			Assert.IsFalse(_level.IsTransparent(1, 1));	// still false because of b2 still blocks
		}

		[Test]
		public void TestMultipleBlockersMove() {
			var b1 = _entityManager.Create().Add(new Scenery(false, false)).Add(new GameObject(1, 1, _level));
			var b2 = _entityManager.Create().Add(new Scenery(false, false)).Add(new GameObject(1, 1, _level));

			Assert.IsFalse(_level.IsWalkable(1, 1));
			Assert.IsFalse(_level.IsTransparent(1, 1));

			b1.Get<GameObject>().Location = new Point(0, 0);

			// old location is still not walkable/transparent because of b2
			Assert.IsFalse(_level.IsWalkable(1, 1));
			Assert.IsFalse(_level.IsTransparent(1, 1));

			Assert.IsFalse(_level.IsWalkable(0, 0));
			Assert.IsFalse(_level.IsTransparent(0, 0));
		}

		[Test]
		public void TestGetEntities() {
			var e1 = _entityManager.Create().Add(new Scenery(false, false)).Add(new GameObject(1, 1, _level));
			var e2 = _entityManager.Create().Add(new Scenery(false, false)).Add(new GameObject(1, 1, _level));
			var e3 = _entityManager.Create().Add(new Scenery(false, false)).Add(new GameObject(1, 2, _level));

			CollectionAssert.Contains(_level.GetEntities(), e1);
			CollectionAssert.Contains(_level.GetEntities(), e2);
			CollectionAssert.Contains(_level.GetEntities(), e3);			
		}

		[Test]
		public void TestGetEntitiesAt() {
			var e1 = _entityManager.Create().Add(new Scenery(false, false)).Add(new GameObject(1, 1, _level));
			var e2 = _entityManager.Create().Add(new Scenery(false, false)).Add(new GameObject(1, 1, _level));
			var e3 = _entityManager.Create().Add(new Scenery(false, false)).Add(new GameObject(1, 2, _level));

			CollectionAssert.Contains(_level.GetEntitiesAt(new Point(1, 1)), e1);
			CollectionAssert.Contains(_level.GetEntitiesAt(new Point(1, 1)), e2);
			CollectionAssert.Contains(_level.GetEntitiesAt(new Point(1, 2)), e3);
		}
	}
}
