using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using DEngine.Core;
using DEngine.Entities;
using DEngine.Level;
using SkrGame.Universe.Locations;

namespace SkrGame.Universe.Entities.Actors {
	public sealed class SightComponent : Component {
		private VisionMap _vision;
		private Level _currLevel;

		[Pure]
		public bool IsVisible(Point p) {
			return IsVisible(p.X, p.Y);
		}

		[Pure]
		public bool IsVisible(int x, int y) {
			return _vision.IsVisible(x, y);
		}

		public void CalculateSight() {
			var level = Entity.Get<GameObject>();
			if (_currLevel != level.Level) {
				_currLevel = level.Level;
				_vision = new VisionMap(_currLevel.Size);	
			}
			ShadowCastingFOV.ComputeRecursiveShadowcasting(_vision, _currLevel, level.X, level.Y, 32, true);
		}

		public override Component Copy() {
			var sight = new SightComponent();
			// todo
			return sight;
		}
	}
}
