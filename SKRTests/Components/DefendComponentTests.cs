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
			defend = new DefendComponent(100, new List<DefendComponent.Appendage>
			                                  {
			                                  		new DefendComponent.Appendage("Part", 10, 1, -3)
			                                  });
		}

		[Test]
		public void TestPartMaxDamage() {
			
		}
		
	}
}
