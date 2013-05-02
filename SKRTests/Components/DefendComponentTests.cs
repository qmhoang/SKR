using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SkrGame.Universe.Entities.Actors;

namespace SKRTests.Components {
	[TestFixture]
	public class DefendComponentTests {
		private BodyComponent _body;

		[SetUp]
		public void SetUp() {
			_body = new BodyComponent(100, new List<BodyComponent.Appendage>
			                                  {
			                                  		new BodyComponent.Appendage("Part", 10, 1, -3)
			                                  });
		}

		[Test]
		public void TestPartMaxDamage() {
			
		}
		
	}
}
