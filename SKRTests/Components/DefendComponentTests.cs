using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SkrGame.Universe.Entities.Actors;

namespace SKRTests.Components {
	[TestFixture]
	public class DefendComponentTests {
		private DefendComponent defend;

		[SetUp]
		public void SetUp() {
			defend = new DefendComponent();
		}

		[Test]
		public void TestRandomBodyPart() {
			var d = defend.BodyPartsList.ToDictionary(attackablePart => attackablePart.Name, attackablePart => 0);

			for (int i = 0; i < 10000; i++) {
				d[defend.GetRandomPart().Name]++;
			}

			foreach (var i in d) {
				Console.WriteLine(i.Key + ": " + i.Value);
			}
		}
	}
}
