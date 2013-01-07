using System;
using DEngine.Actor;
using DEngine.Entity;
using SkrGame.Universe.Entities.Actors;

namespace SkrGame.Universe.Locations {
//	public class MovementBlocker : Component {
//		public string Description { get; set; }
//
//		private bool walkable;
//		public bool Walkable {
//			get { return walkable; }
//			set {
//				walkable = value;
//				OnWalkableChanged();
//			}
//		}
//
//		public double WalkPenalty { get; set; }
//
//		public event EventHandler<EventArgs> WalkableChanged;
//
//		public void OnWalkableChanged() {
//			EventHandler<EventArgs> handler = WalkableChanged;
//			if (handler != null)
//				handler(this, EventArgs.Empty);
//		}
//	}
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

		public event EventHandler<EventArgs> TransparencyChanged;
		public event EventHandler<EventArgs> WalkableChanged;

		public void OnTransparencyChanged() {
			EventHandler<EventArgs> handler = TransparencyChanged;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}


		public void OnWalkableChanged() {
			EventHandler<EventArgs> handler = WalkableChanged;
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
			b.TransparencyChanged = (EventHandler<EventArgs>)TransparencyChanged.Clone();
			b.WalkableChanged = (EventHandler<EventArgs>)WalkableChanged.Clone();

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

	public class UseableFeature : Component {        
		public string Action { get; set; }
		public int ActionPointCost { get; set; }
		public Func<UseableFeature, Actor, ActionResult> Use { get; set; }

		public UseableFeature(string action, int apcost, Func<UseableFeature, Actor, ActionResult> use) {
			Use = use;
			ActionPointCost = apcost;
			Action = action;
		}

		public override Component Copy() {
			return new UseableFeature(Action, ActionPointCost, (Func<UseableFeature, Actor, ActionResult>) Use.Clone());
		}
	}

	public class PassiveFeature : Component {
		public Action<PassiveFeature, Actor, double> Near { get; set; }        

		public PassiveFeature(Action<PassiveFeature, Actor, double> near) {
			Near = near;            
		}

		public override Component Copy() {
			return new PassiveFeature((Action<PassiveFeature, Actor, double>) Near.Clone());
		}
	}

	public class SwitchFeature : Component {
		public bool Switch { get; set; }

		public SwitchFeature(bool @switch = false) {
			Switch = @switch;
		}

		public override Component Copy() {
			return new SwitchFeature(Switch);
		}
	}
}