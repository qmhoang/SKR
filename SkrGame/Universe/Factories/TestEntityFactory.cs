using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Features;
using SkrGame.Universe.Entities.Useables;

namespace SkrGame.Universe.Factories {
	public static class TestEntityFactory {
		public static void Init(EntityFactory ef) {
			ef.Inherits("testuse", "feature",
			            new UseBroadcaster(),
			            new TestUseableComponent(),
			            new ApplianceComponent(new List<ApplianceComponent.Use>()
			                                   {
			                                   		ApplianceComponent.Use.UseAppliance("Use",
			                                   		                                    "stat_bladder",
			                                   		                                    new TimeSpan(0, 0, 1, 0),
			                                   		                                    new TimeSpan(0, 0, 0, 1, 150),
			                                   		                                    (e, app) => e.Get<GameObject>().Level.World.Log.NormalFormat("{0} finishes using the {1}.",
			                                   		                                                                                                         Identifier.GetNameOrId(e),
			                                   		                                                                                                         Identifier.GetNameOrId(app.Entity)),
			                                   		                                    (e, app) => e.Get<GameObject>().Level.World.Log.NormalFormat("{0} is unable to use the {1}.",
			                                   		                                                                                                         Identifier.GetNameOrId(e),
			                                   		                                                                                                         Identifier.GetNameOrId(app.Entity)))
			                                   }));
		}
	}
}
