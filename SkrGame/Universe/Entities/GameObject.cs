using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using Level = SkrGame.Universe.Locations.Level;

namespace SkrGame.Universe.Entities {
	public sealed class GameObject : Component {
		private Level _level;
		private Point _location;

		public Level Level {
			get { return _level; }
			set {
				_level = value;
			}
		}

		public Point Location {
			get { return _location; }
			set {
				var eventArgs = new PositionChangedEvent(_location, value);
				_location = value;

				OnPositionChanged(eventArgs);
			}
		}

		public int X {
			get { return Location.X; }			
		}

		public int Y {
			get { return Location.Y; }			
		}

		public event ComponentEventHandler<GameObject, EventArgs<Level>> MapChanged;

		public void OnMapChanged(EventArgs<Level> e) {
			var handler = MapChanged;
			if (handler != null)
				handler(this, e);
		}

		public GameObject(Point position, Level level) {
			Location = position;
			Level = level;
		}
		public GameObject(int x, int y, Level level)
			: this(new Point(x, y), level) { }

		public override Component Copy() {
			return new GameObject(Location, Level);
		}

		public event ComponentEventHandler<GameObject, PositionChangedEvent> PositionChanged;

		public void OnPositionChanged(PositionChangedEvent e) {
			var handler = PositionChanged;
			if (handler != null)
				handler(this, e);
		}

		public double DistanceTo(GameObject obj) {
			Contract.Requires<ArgumentNullException>(obj != null, "loc");
			return Location.DistanceTo(obj.Location);
		}

		public double DistanceTo(Point p) {
			return Location.DistanceTo(p);
		}				
	}
}
