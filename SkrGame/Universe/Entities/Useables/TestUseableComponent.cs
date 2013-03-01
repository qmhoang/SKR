using System.Collections.Generic;
using DEngine.Actor;
using DEngine.Entities;

namespace SkrGame.Universe.Entities.Useables {
	public sealed class ApplianceComponent : Component {
		

		public override Component Copy() {
			throw new System.NotImplementedException();
		}
	}

	public sealed class TestUseableComponent : Component, IUseable {
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
