using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using DEngine.Core;
using DEngine.Entities;
using DEngine.Extensions;
using NUnit.Framework;
using SkrGame.Universe;
using System.Linq;
using libtcod;


namespace SKRTests {
	[TestFixture]
	public class WorldTests {
		
		[Test]
		public static void TestAPFunctions() {
			// an action that takes 2 second should equal 200  AP
			Assert.AreEqual(World.SecondsToActionPoints(2), World.OneSecondInAP * 2);

			Assert.AreEqual(World.SecondsToSpeed(10), World.OneSecondInSpeed / 10);

			Assert.AreEqual(World.SecondsToSpeed(World.SpeedToSeconds(5)), 5);
			Assert.AreEqual(World.SecondsToSpeed(World.SpeedToSeconds(9)), 9);
			Assert.AreEqual(World.SecondsToSpeed(World.SpeedToSeconds(50)), 50);
			Assert.AreEqual(World.SecondsToSpeed(World.SpeedToSeconds(14)), 14);
		}

		[TestCase(100)]
		[TestCase(50)]
		[TestCase(200)]
		[TestCase(150)]
		public static void TestStaticConversion(int value) {
			Assert.AreEqual(World.SecondsToSpeed(World.SpeedToSeconds(value)), value);
			Assert.AreEqual(World.SecondsToActionPoints(World.SpeedToSeconds(World.ActionPointsToSpeed(value))), value);
		}

		public class MyClass {
			private static readonly int a = c + 1;
			private static readonly int b = a + 1;
			private static readonly int c = b + 1;

			public int Value { get; set; }

			public MyClass(int value) {
				Value = value;
			}
		}

		[Test]
		public static void TestMethod() {
			var list = new SortedList<MyClass, MyClass>(new LambdaComparer<MyClass>((x, y) => x.Value - y.Value));
			var m1 = new MyClass(10);
			list.Add(m1, m1);

			var m2 = new MyClass(9);
			list.Add(m2, m2);

			var m3 = new MyClass(11);
			list.Add(m3, m3);

			m2.Value = 20;
//			SortedSet<MyClass> set = new SortedSet<MyClass>(new LambdaComparer<MyClass>((x, y) => x.Value - y.Value));
//
//			set.Add(new MyClass(10));
//			set.Add(new MyClass(4));
//			set.Add(new MyClass(15));
//			set.Add(new MyClass(1));
//			set.Add(new MyClass(-4));
//
//			var m = new MyClass(6);
//			set.Add(m);
//
//			m.Value = 16;

		}
	}
}
