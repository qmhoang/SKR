using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Entities;

namespace SkrGame.Universe.Entities.Features.Useables {
	public class TestUseableComponent : Component, IUseable {
		public override Component Copy() {
			return new TestUseableComponent();
		}

		private static readonly UseAction A1 =
				new UseAction("A1", (user, featureEntity, action) =>
				                           	{
												user.Get<GameObject>().Level.World.Log.Normal("A1");
				                           		return ActionResult.Success;
				                           	});
		private static readonly UseAction A2 =
				new UseAction("A2", (user, featureEntity, action) =>
				{
					user.Get<GameObject>().Level.World.Log.Normal("A2");
					return ActionResult.Success;
				});
		private static readonly UseAction A3 =
				new UseAction("A3", (user, featureEntity, action) =>
				{
					user.Get<GameObject>().Level.World.Log.Normal("A3");
					return ActionResult.Success;
				});

		public IEnumerable<UseAction> Uses {
			get {
				yield return A1;
				yield return A2;
				yield return A3;
			}
		}
	}
}
