using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SkrGame.Universe;

namespace SKRTests {
    [TestFixture]
    class WorldTest {
        [TestCase(100)]
        [TestCase(50)]
        [TestCase(200)]
        [TestCase(150)]
        public void TestStaticConversion(int value) {
            Assert.AreEqual(World.SecondsToSpeed(World.SpeedToSeconds(value)), value);
        }
    }
}
