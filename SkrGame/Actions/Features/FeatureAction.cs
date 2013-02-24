using System;
using System.Diagnostics.Contracts;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Universe.Entities;

namespace SkrGame.Actions.Features {
	public abstract class FeatureAction : LoggedAction {
		public Entity Feature { get; private set; }
		public string FeatureName { get { return Identifier.GetNameOrId(Feature); } }

		protected FeatureAction(Entity entity, Entity feature)
			: base(entity) {
			Contract.Requires<ArgumentNullException>(feature != null, "feature");
			Contract.Requires<ArgumentException>(feature.Has<GameObject>());
			Feature = feature;
		}
	}
}