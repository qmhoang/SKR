using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Actions.Features;

namespace SkrGame.Universe.Entities.Useables {
	// todo breakage, needs repairing
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

		private readonly List<Use> uses;

		public ApplianceComponent(List<Use> uses) {
			this.uses = uses;

			Uses = uses.Select(use => new UseAction(use.Description,
			                                        (user, featureEntity) => user.Get<ActorComponent>().Enqueue(new IntervalAction(user,
			                                                                                                                       use.Length,
			                                                                                                                       use.Interval,
			                                                                                                                       e => use.OnInterval(e, this),
			                                                                                                                       e => use.OnFinish(e, this)))));
		}

		public override Component Copy() {
			return new ApplianceComponent(new List<Use>(uses));
		}

		public IEnumerable<UseAction> Uses { get; private set; }
	}
}