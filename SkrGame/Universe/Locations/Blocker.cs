using System;
using DEngine.Actor;
using DEngine.Entity;
using SkrGame.Universe.Entities.Actors;

namespace SkrGame.Universe.Locations {
	public class Blocker : EntityComponent {
		public Blocker() : this(true, true) { }

		public Blocker( bool walkable, bool transparent) {
			this.walkable = walkable;
			this.transparent = transparent;			
		}
		
		public Level Level { get; set; }		
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

	public class UseableFeature : EntityComponent {        
		public string Action { get; set; }
		public int ActionPointCost { get; set; }
		public Func<UseableFeature, Actor, ActionResult> Use { get; set; }

		public UseableFeature(string action, int apcost, Func<UseableFeature, Actor, ActionResult> use) {
			Use = use;
			ActionPointCost = apcost;
			Action = action;
		}
	}

	public class PassiveFeature : EntityComponent {
		public Action<PassiveFeature, Actor, double> Near { get; set; }        

		public PassiveFeature(Action<PassiveFeature, Actor, double> near) {
			Near = near;            
		}
	}

	public class SwitchFeature : EntityComponent {
		public bool Switch { get; set; }

		public SwitchFeature(bool @switch = false) {
			Switch = @switch;
		}
	}
}