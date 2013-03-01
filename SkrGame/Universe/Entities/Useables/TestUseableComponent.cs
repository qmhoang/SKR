using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using DEngine.Actions;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Actions.Features;

namespace SkrGame.Universe.Entities.Useables {
//	public class UseApplianceAction : FeatureAction {
//		public UseApplianceAction(Entity entity, Entity feature) : base(entity, feature) {
//			Contract.Requires<ArgumentException>(feature.Has<ApplianceComponent>());
//		}
//
//		public override int APCost {
//			get { throw new NotImplementedException(); }
//		}
//
//		public override ActionResult OnProcess() {
//			var appliance = Feature.Get<ApplianceComponent>();
//			var apLength = World.SecondsToActionPoints(appliance.Length.TotalSeconds);
//			Entity.Get<ActorComponent>().Enqueue(new LongAction(Entity, apLength, e =>
//			                                                                      	{
//
//																						return ActionResult.Success;			                                                                      		
//			                                                                      	}, e =>
//			                                                                      	   	{
//			                                                                      	   		return ActionResult.Success;
//			                                                                      	   	}));
//		}
//	}

	public sealed class ApplianceComponent : Component {
		public TimeSpan Length { get; private set; }
		public TimeSpan Interval { get; private set; }



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
