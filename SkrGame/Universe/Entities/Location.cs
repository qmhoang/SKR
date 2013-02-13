using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using Level = SkrGame.Universe.Locations.Level;

namespace SkrGame.Universe.Entities {
	public class Location : Position {
		private Level level;
		public Level Level {
			get { return level; }
			set {
				level = value;
			}
		}


		public event ComponentEventHandler<EventArgs<Level>> MapChanged;

		public void OnMapChanged(EventArgs<Level> e) {
			ComponentEventHandler<EventArgs<Level>> handler = MapChanged;
			if (handler != null)
				handler(this, e);
		}

		public Location(Point position, Level level) : this(position.X, position.Y, level) { }
		public Location(int x, int y, Level level)
			: base(x, y) {
			Level = level;
		}
		public override Component Copy() {
			return new Location(Point, Level);
		}
	}
}
