using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using DEngine.Core;
using DEngine.Entities;
using DEngine.Level;

namespace SkrGame.Universe.Entities.Actors {
	public sealed class SightComponent : Component {
		private VisionMap vision;
		private AbstractLevel currLevel;

		[Pure]
		public bool IsVisible(Point p) {
			return IsVisible(p.X, p.Y);
		}

		[Pure]
		public bool IsVisible(int x, int y) {
			return vision.IsVisible(x, y);
		}

		public void CalculateSight() {
			var level = Entity.Get<GameObject>();
			if (currLevel != level.Level) {
				currLevel = level.Level;
				vision = new VisionMap(currLevel.Size);	
			}
			ShadowCastingFOV.ComputeRecursiveShadowcasting(vision, currLevel, level.X, level.Y, 16, true);
		}

		public override Component Copy() {
			var sight = new SightComponent();
			// todo
			return sight;
		}
	}
}
