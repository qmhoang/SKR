using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Entities;
using SkrGame.Universe.Entities.Features;
using SkrGame.Universe.Entities.Features.Useables;

namespace SkrGame.Universe.Factories {
	public static class TestEntityFactory {
		public static void Init(EntityFactory ef) {
			ef.Inherits("testuse", "feature", 
				new UseBroadcaster(),
				new TestUseableComponent());
		}
	}
}
