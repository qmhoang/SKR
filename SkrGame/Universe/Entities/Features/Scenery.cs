using System;
using DEngine.Components;
using DEngine.Entities;

namespace SkrGame.Universe.Entities.Features {
	public sealed class Scenery : Component {
		public Scenery() : this(true, true) { }

		public Scenery(bool transparent, bool walkable) : this(transparent, walkable, int.MaxValue) { }

		public Scenery(bool transparent, bool walkable, int jumpHeight) {
			this._transparent = transparent;
			this._walkable = walkable;
			JumpHeight = jumpHeight;
		}

		private bool _transparent;
		private bool _walkable;

		public bool Transparent {
			get { return _transparent; }
			set { 
				_transparent = value; 
				OnTransparencyChanged();
			}
		}

		public bool Walkable {
			get { return _walkable; }
			set {
				_walkable = value;
				OnWalkableChanged();
			}
		}

		/// <summary>
		/// How high a scenery is.  0 represents waist level.
		/// </summary>
		public int JumpHeight { get; set; }

		public event ComponentEventHandler<Scenery, EventArgs> TransparencyChanged;
		public event ComponentEventHandler<Scenery, EventArgs> WalkableChanged;

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
			var b = new Scenery(_transparent, _walkable, JumpHeight);
			if (TransparencyChanged != null)
				b.TransparencyChanged = (ComponentEventHandler<Scenery, EventArgs>)TransparencyChanged.Clone();
			if (WalkableChanged != null)
				b.WalkableChanged = (ComponentEventHandler<Scenery, EventArgs>)WalkableChanged.Clone();

			return b;
		}

		public override string ToString() {
			return String.Format("Transparent: {0}, Walkable: {1}, Height: {2}", Transparent, Walkable, JumpHeight);
		}
	}
}