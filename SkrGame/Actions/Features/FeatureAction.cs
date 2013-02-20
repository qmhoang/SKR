using System;
using System.Diagnostics.Contracts;
using DEngine.Entities;

namespace SkrGame.Actions.Features {
	public abstract class FeatureAction : LoggedAction {
		public Entity Feature { get; private set; }
		protected FeatureAction(Entity entity, Entity feature)
			: base(entity) {
			Contract.Requires<ArgumentNullException>(feature != null, "feature");
			Feature = feature;
		}
	}
}