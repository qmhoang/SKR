﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DEngine.Core;
using DEngine.Entities;
using NUnit.Framework;
using SkrGame.Universe;
using System.Linq;


namespace SKRTests {
	[TestFixture]
	public class WorldTests {
		
		[Test]
		public static void APFunctions() {
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
		public void StaticConversion(int value) {
			Assert.AreEqual(World.SecondsToSpeed(World.SpeedToSeconds(value)), value);
			Assert.AreEqual(World.SecondsToActionPoints(World.SpeedToSeconds(World.ActionPointsToSpeed(value))), value);
		}

		[Test]
		public void TestMethod() {

		}
	}
}