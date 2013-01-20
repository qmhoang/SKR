using System;
using DEngine.Entities;

namespace SkrGame.Universe.Entities.Features {
	public class Blocker : Component {
		internal Blocker() : this(true, true) { }

		internal Blocker( bool walkable, bool transparent) {
			this.walkable = walkable;
			this.transparent = transparent;			
		}
		
		public string Description { get; set; }

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

		public double WalkPenalty { get; set; }

		public event ComponentEventHandler<EventArgs> TransparencyChanged;
		public event ComponentEventHandler<EventArgs> WalkableChanged;

		public void OnTransparencyChanged() {
			ComponentEventHandler<EventArgs> handler = TransparencyChanged;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}


		public void OnWalkableChanged() {
			ComponentEventHandler<EventArgs> handler = WalkableChanged;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}

//		public void Use(Actor user, string action) {
//			foreach (var component in components) {
//				if (component is ActiveFeatureComponent) {
//					var activefeature = (ActiveFeatureComponent) component;
//					if (activefeature.Action == action) {
//						ActionResult result = activefeature.Use(component as ActiveFeatureComponent, user);
//
//						switch (result) {
//							case ActionResult.SuccessNoTime:
//							case ActionResult.Aborted:
//								// no AP is used
//								break;
//							case ActionResult.Failed:
//							case ActionResult.Success:
//								user.ActionPoints -= activefeature.ActionPointCost;
//								break;
//						}
//					}
//				}
//					
//			}
//		}

//		public void Near(Actor user) {
//			foreach (var component in components) {
//				if (component is PassiveFeatureComponent)
//					((PassiveFeatureComponent) component).Near(component as PassiveFeatureComponent, user, user.Position.DistanceTo(Position));
//			}
//		}		
		public override Component Copy() {
			var b = new Blocker(walkable, transparent);
			if (TransparencyChanged != null)
				b.TransparencyChanged = (ComponentEventHandler<EventArgs>)TransparencyChanged.Clone();
			if (WalkableChanged != null)
				b.WalkableChanged = (ComponentEventHandler<EventArgs>)WalkableChanged.Clone();

			return b;
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