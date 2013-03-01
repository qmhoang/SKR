using System;
using DEngine.Entities;

namespace SkrGame.Universe.Entities.Features {
	public sealed class PassiveFeature : Component {
		public delegate void NearDelegate(Entity entityNear, Entity featureEntity);
		public NearDelegate Near { get; set; }

		public PassiveFeature(NearDelegate near) {
			Near = near;            
		}

		public override Component Copy() {
			return new PassiveFeature((NearDelegate)Near.Clone());
		}
	}
}