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
	public class Location : Component, IEquatable<Location> {
		private Level level;
		private Point point;

		public Level Level {
			get { return level; }
			set {
				level = value;
			}
		}

		public Point Point {
			get { return point; }
			set {
				var eventArgs = new PositionChangedEvent(point, value);
				point = value;

				OnPositionChanged(eventArgs);
			}
		}

		public int X {
			get { return Point.X; }			
		}

		public int Y {
			get { return Point.Y; }			
		}
		
		public event ComponentEventHandler<EventArgs<Level>> MapChanged;

		public void OnMapChanged(EventArgs<Level> e) {
			ComponentEventHandler<EventArgs<Level>> handler = MapChanged;
			if (handler != null)
				handler(this, e);
		}

		public Location(Point position, Level level) {
			Point = position;
			Level = level;
		}
		public Location(int x, int y, Level level)
			: this(new Point(x, y), level) { }

		public override Component Copy() {
			return new Location(Point, Level);
		}

		public event ComponentEventHandler<PositionChangedEvent> PositionChanged;

		public void OnPositionChanged(PositionChangedEvent e) {
			ComponentEventHandler<PositionChangedEvent> handler = PositionChanged;
			if (handler != null)
				handler(this, e);
		}

		public double DistanceTo(Location loc) {
			Contract.Requires<ArgumentNullException>(loc != null, "loc");
			return Point.DistanceTo(loc.Point);
		}

		public double DistanceTo(Point p) {
			return Point.DistanceTo(p);
		}

		public bool IsNear(int x, int y, int radius) {
			return IsNear(new Point(x, y), radius);
		}

		public bool IsNear(Point p, int radius) {
			return p.IsInCircle(Point, radius);
		}

		public bool Equals(Location other) {
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;
			return Equals(other.level, level) && other.point.Equals(point);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(Location))
				return false;
			return Equals((Location) obj);
		}

		public static bool operator ==(Location left, Location right) {
			return Equals(left, right);
		}

		public static bool operator !=(Location left, Location right) {
			return !Equals(left, right);
		}

		public override int GetHashCode() {
			unchecked {
				return ((level != null ? level.GetHashCode() : 0) * 397) ^ point.GetHashCode();
			}
		}
	}
}
