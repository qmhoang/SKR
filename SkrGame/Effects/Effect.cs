using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using DEngine.Actions;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Universe;
using SkrGame.Universe.Entities;
using log4net;

namespace SkrGame.Effects {
	public abstract class Effect {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private EntityConditions holder;
		public EntityConditions Holder {
			get { return holder; }
			internal set {
				Contract.Requires<ArgumentNullException>(value != null, "value");
				holder = value;
			}
		}

		public void Update(int ap) {
			Contract.Requires<ArgumentException>(Holder != null);

			OnTick((int) Math.Round(World.ActionPointsToSeconds(ap) * 1000));
		}

		public void End() {
			Contract.Requires<ArgumentException>(Holder != null);

			var result = Holder.Entity.Get<EntityConditions>().Remove(this);
			if (!result)
				Logger.ErrorFormat("Condition was attempted to be removed from an entity: {0} that doesn't contain it.", Identifier.GetNameOrId(Holder.Entity));
			OnEnd();
		}

		public abstract Effect Copy();

		protected abstract void OnTick(int millisecondsElapsed);
		protected virtual void OnEnd() { }
	}
}
