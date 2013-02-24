using System;
using DEngine.Components;
using DEngine.Entities;

namespace SkrGame.Universe.Entities.Features {
	public class Scenery : Component {
		public Scenery() : this(true, true) { }

		public Scenery(bool walkable, bool transparent) {
			this.walkable = walkable;
			this.transparent = transparent;			
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
			var b = new Scenery(walkable, transparent);
			if (TransparencyChanged != null)
				b.TransparencyChanged = (ComponentEventHandler<EventArgs>)TransparencyChanged.Clone();
			if (WalkableChanged != null)
				b.WalkableChanged = (ComponentEventHandler<EventArgs>)WalkableChanged.Clone();

			return b;
		}

		public override string ToString() {
			return String.Format("Transparent: {0}, Walkable: {1}", Transparent, Walkable);
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