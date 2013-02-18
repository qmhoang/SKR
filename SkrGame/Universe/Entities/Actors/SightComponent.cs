using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using DEngine.Core;
using DEngine.Entities;

namespace SkrGame.Universe.Entities.Actors {
	public class SightComponent : Component {
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

		public void CalculateSight(AbstractLevel level, int x, int y, int radius) {
			if (currLevel != level) {
				currLevel = level;
				vision = new VisionMap(currLevel.Size);	
			}
			ShadowCastingFOV.ComputeRecursiveShadowcasting(vision, currLevel, x, y, radius, true);
		}

		public override Component Copy() {
			var sight = new SightComponent();
			sight.currLevel = currLevel;
			sight.vision = vision.Copy();
			return sight;
		}
	}
}
