using System;
using DEngine.Components;
using DEngine.Entities;

namespace SkrGame.Universe.Entities.Features {
	public class Scenery : Component {
		public Scenery() : this(true, true) { }

		public Scenery(bool transparent, bool walkable) : this(transparent, walkable, int.MaxValue) { }

		public Scenery(bool transparent, bool walkable, int jumpHeight) {
			this.transparent = transparent;
			this.walkable = walkable;
			JumpHeight = jumpHeight;
		}

		private bool transparent;
		private bool walkable;

		public bool Transparent {
			get { return transparent; }
			set { 
				transparent = value; 
				OnTransparencyChanged();
			}
		}

		public bool Walkable {
			get { return walkable; }
			set {
				walkable = value;
				OnWalkableChanged();
			}
		}

		/// <summary>
		/// How high a scenery is.  0 represents waist level.
		/// </summary>
		public int JumpHeight { get; set; }

		public event ComponentEventHandler<EventArgs> TransparencyChanged;
		public event ComponentEventHandler<EventArgs> WalkableChanged;

		public void OnTransparencyChanged() {
			var handler = TransparencyChanged;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}


		public void OnWalkableChanged() {
			var handler = WalkableChanged;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		
		public override Component Copy() {
			var b = new Scenery(transparent, walkable, JumpHeight);
			if (TransparencyChanged != null)
				b.TransparencyChanged = (ComponentEventHandler<EventArgs>)TransparencyChanged.Clone();
			if (WalkableChanged != null)
				b.WalkableChanged = (ComponentEventHandler<EventArgs>)WalkableChanged.Clone();

			return b;
		}

		public override string ToString() {
			return String.Format("Transparent: {0}, Walkable: {1}, Height: {2}", Transparent, Walkable, JumpHeight);
		}
	}
	
	public class FeaturePropertyChangeEvent : EventArgs {
		public bool Transparency { get; private set; }
		public bool Walkable { get; private set; }
		public double WalkPenalty { get; private set; }

		public FeaturePropertyChangeEvent(bool transparency, bool walkable, double walkPenalty) {
			Transparency = transparency;
			Walkable = walkable;
			WalkPenalty = walkPenalty;
		}
	}
}