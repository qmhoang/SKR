using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Entities;

namespace SkrGame.Universe.Entities.Items {
	public sealed class Lockpick : Component {
		public int Quality { get; private set; }

		public Lockpick(int quality) {
			Quality = quality;
		}

		public override Component Copy() {
			return new Lockpick(Quality);
		}
	}
}
