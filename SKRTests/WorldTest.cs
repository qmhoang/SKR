using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SkrGame.Universe;

namespace SKRTests {
	[TestFixture]
	class WorldTest {
		private World world;

		[SetUp]
		public void SetUp() {
			world = World.Create();
		}

		[Test]
		public static void TestAPFunctions() {
			// an action that takes 2 second should equal 200  AP
			Assert.AreEqual(World.SecondsToActionPoints(2), 200);

			Assert.AreEqual(World.SecondsToSpeed(10), 10);

			Assert.AreEqual(World.SecondsToSpeed(World.SpeedToSeconds(5)), 5);
			Assert.AreEqual(World.SecondsToSpeed(World.SpeedToSeconds(9)), 9);
			Assert.AreEqual(World.SecondsToSpeed(World.SpeedToSeconds(50)), 50);
			Assert.AreEqual(World.SecondsToSpeed(World.SpeedToSeconds(14)), 14);
		}

		[TestCase(100)]
		[TestCase(50)]
		[TestCase(200)]
		[TestCase(150)]
		public void TestStaticConversion(int value) {
			Assert.AreEqual(World.SecondsToSpeed(World.SpeedToSeconds(value)), value);
		}
	}
}
