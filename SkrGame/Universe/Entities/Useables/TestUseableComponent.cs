using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using DEngine.Actions;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Actions.Features;

namespace SkrGame.Universe.Entities.Useables {
//	public class UseApplianceAction : FeatureAction {
//		private ApplianceComponent.Use use;
//
//		public UseApplianceAction(Entity entity, Entity feature, ApplianceComponent.Use use) : base(entity, feature) {
//			Contract.Requires<ArgumentException>(feature.Has<ApplianceComponent>());
//			this.use = use;
//		}
//
//		public override int APCost {
//			get { throw new NotImplementedException(); }
//		}
//
//		public override ActionResult OnProcess() {
//			var appliance = Feature.Get<ApplianceComponent>();
//
//			Entity.Get<ActorComponent>().Enqueue(new IntervalAction(Entity,
//			                                                        use.Length,
//			                                                        use.Interval,
//			                                                        e => use.OnInterval(e, appliance),
//																	e => use.OnFinish(e, appliance)));
//		}
//	}

	public sealed class ApplianceComponent : Component, IUseable {
		public class Use {
			public string Description { get; private set; }
			public TimeSpan Length { get; private set; }
			public TimeSpan Interval { get; private set; }
			public Func<Entity, ApplianceComponent, ActionResult> OnInterval { get; private set; }
			public Func<Entity, ApplianceComponent, ActionResult> OnFinish { get; private set; }

			public Use(string description, TimeSpan length, TimeSpan interval, Func<Entity, ApplianceComponent, ActionResult> onInterval, Func<Entity, ApplianceComponent, ActionResult> onFinish) {
				Contract.Requires<ArgumentNullException>(onFinish != null, "onFinish");
				Contract.Requires<ArgumentNullException>(onInterval != null, "onInterval");
				Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(description), "string \"description\" cannot be null or empty");

				Description = description;
				Length = length;
				Interval = interval;
				OnInterval = onInterval;
				OnFinish = onFinish;
			}
		}

		private List<Use> uses;

		public ApplianceComponent(List<Use> uses) {
			this.uses = uses;
		}

		public override Component Copy() {
			return new ApplianceComponent(new List<Use>(uses));
		}

		public IEnumerable<UseAction> Uses {
			get {
				return uses.Select(use => new UseAction(use.Description, (user, featureEntity, action) => user.Get<ActorComponent>().Enqueue(new IntervalAction(user,
				                                                                                                                                                use.Length,
				                                                                                                                                                use.Interval,
				                                                                                                                                                e => use.OnInterval(e, this),
				                                                                                                                                                e => use.OnFinish(e, this)))));
			}
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
				                           	});
		private static readonly UseAction A2 =
				new UseAction("A2", (user, featureEntity, action) =>
				{
					user.Get<GameObject>().Level.World.Log.Normal("A2");
				});
		private static readonly UseAction A3 =
				new UseAction("A3", (user, featureEntity, action) =>
				{
					user.Get<GameObject>().Level.World.Log.Normal("A3");
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
