using System;
using System.Diagnostics;
using System.Timers;
using DEngine.Core;
using NUnit.Framework;

namespace DEngineTests {
    [TestFixture]
    class RngTests {
        [SetUp]
        public static void SetUp() {
            Rng.Seed(0);
        }

        [Test]
        public static void TestTriangle() {
            int[] list = new int[20];

            for (int i = 0; i < 100000; i++) {
                list[Rng.TriangleInt(10, 8)]++;
            }

            int index = 0;
            foreach (var i in list) {
                Console.WriteLine("{0}: {1}", index, i);
                index++;
            }
        }

        [Test]
        public static void TestGaussian() {
            int[] list = new int[20];

            for (int i = 0; i < 100000; i++) {
                list[Rng.GaussianInt(10, 8, 3)]++;
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
