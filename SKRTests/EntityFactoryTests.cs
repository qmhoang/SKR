using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Components;
using DEngine.Entities;
using NUnit.Framework;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;

namespace SKRTests {
	[TestFixture]
	public class EntityFactoryTests {
		private EntityFactory ef;
		private EntityManager em;

		[SetUp]
		public void SetUp() {
			ef = new EntityFactory();
			em = new EntityManager();
		}

		[Test]
		public void TestInheritance() {
			ef.Add("base", new Identifier("Item"), new VisibleComponent(-100));
			ef.Inherits("inherited", "base", new VisibleComponent(0));
			ef.Compile();

			var anObject = ef.Create("base", em);
			Assert.NotNull(anObject);
			Assert.IsTrue(anObject.Has<ReferenceId>());
			Assert.AreEqual(anObject.Get<ReferenceId>().RefId, "base");
			Assert.AreEqual(anObject.Get<Identifier>().Name, "Item");
			Assert.AreEqual(anObject.Get<VisibleComponent>().VisibilityIndex, -100);


			var anotherObject = ef.Create("inherited", em);
			Assert.NotNull(anotherObject);
			Assert.IsTrue(anotherObject.Has<ReferenceId>());
			Assert.AreEqual(anotherObject.Get<ReferenceId>().RefId, "inherited");
			Assert.AreEqual(anotherObject.Get<Identifier>().Name, "Item");
			Assert.AreEqual(anotherObject.Get<VisibleComponent>().VisibilityIndex, 0);
		}

		[Test]
		public void TestIdentity() {
			var i1 = ef.Create("item", em);
			var i2 = ef.Create("item", em);

			Assert.AreNotSame(i1, i2);
		}
	}
}
