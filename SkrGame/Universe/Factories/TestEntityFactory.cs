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
			                                   		new ApplianceComponent.Use("Drink from",
			                                   		                           new TimeSpan(0, 0, 1, 0),
			                                   		                           new TimeSpan(0, 0, 0, 2),
			                                   		                           (e, app) =>
			                                   		                           	{
			                                   		                           		if (e.Has<Person>()) {
			                                   		                           			e.Get<Person>().Stats["stat_water"].Value++;
			                                   		                           			return ActionResult.Success;
			                                   		                           		}
			                                   		                           		return ActionResult.Aborted;
			                                   		                           	},
			                                   		                           (e, app) =>
			                                   		                           	{
			                                   		                           		if (e.Has<Person>()) {
			                                   		                           			e.Get<Person>().Stats["stat_water"].Value++;

			                                   		                           			e.Get<GameObject>().Level.World.Log.Normal(
			                                   		                           					String.Format("{0} finishes drinking from the {1}.",
			                                   		                           					              Identifier.GetNameOrId(e),
			                                   		                           					              Identifier.GetNameOrId(app.Entity)));

			                                   		                           			return ActionResult.Success;
			                                   		                           		}
			                                   		                           		return ActionResult.Aborted;
			                                   		                           	})
			                                   }));
		}
	}
}
