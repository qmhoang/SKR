using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Core;
using DEngine.Extensions;
using NUnit.Framework;

namespace SKRTests {
    [TestFixture]
    class RngTests {
        [SetUp]
        public static void SetUp() {
            Rng.Seed(0);
        }

        [Test]
        public static void TestTriangle() {
            Assert.AreEqual(1, 1);

            int[] list = new int[10];

            for (int i = 0; i < 1000; i++) {
                list[Rng.TriangleInt(5, 3)]++;
            }

            int index = 0;
            foreach (var i in list) {
                Console.WriteLine("{0}: {1}", index, i);
                index++;
            }
        }

        [Test]
        public static void TestAddition() {
            var r = Rand.Constant(10) + Rand.Constant(10) + Rand.Constant(55);

            Assert.AreEqual(75, r.Roll());
        }
    }
}
