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
	public class GameObject : Component, IEquatable<GameObject> {
		private Level level;
		private Point location;

		public Level Level {
			get { return level; }
			set {
				level = value;
			}
		}

		public Point Location {
			get { return location; }
			set {
				var eventArgs = new PositionChangedEvent(location, value);
				location = value;

				OnPositionChanged(eventArgs);
			}
		}

		public int X {
			get { return Location.X; }			
		}

		public int Y {
			get { return Location.Y; }			
		}
		
		public event ComponentEventHandler<EventArgs<Level>> MapChanged;

		public void OnMapChanged(EventArgs<Level> e) {
			ComponentEventHandler<EventArgs<Level>> handler = MapChanged;
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

		public event ComponentEventHandler<PositionChangedEvent> PositionChanged;

		public void OnPositionChanged(PositionChangedEvent e) {
			ComponentEventHandler<PositionChangedEvent> handler = PositionChanged;
			if (handler != null)
				handler(this, e);
		}

		public double DistanceTo(GameObject loc) {
			Contract.Requires<ArgumentNullException>(loc != null, "loc");
			return Location.DistanceTo(loc.Location);
		}

		public double DistanceTo(Point p) {
			return Location.DistanceTo(p);
		}

		public bool IsNear(int x, int y, int radius) {
			return IsNear(new Point(x, y), radius);
		}

		public bool IsNear(Point p, int radius) {
			return p.IsInCircle(Location, radius);
		}

		public bool Equals(GameObject other) {
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;
			return Equals(other.level, level) && other.location.Equals(location);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(GameObject))
				return false;
			return Equals((GameObject) obj);
		}

		public static bool operator ==(GameObject left, GameObject right) {
			return Equals(left, right);
		}

		public static bool operator !=(GameObject left, GameObject right) {
			return !Equals(left, right);
		}

		public override int GetHashCode() {
			unchecked {
				return ((level != null ? level.GetHashCode() : 0) * 397) ^ location.GetHashCode();
			}
		}
	}
}
