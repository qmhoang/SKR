using System;
using System.Collections.Generic;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
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
			if (TransparencyChanged != null)
				b.TransparencyChanged = (EventHandler<EventArgs>)TransparencyChanged.Clone();
			if (WalkableChanged != null)
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
		public class UseAction {
			public delegate ActionResult UseDelegate(Entity user, Entity useableEntity, UseAction action);
			public string Description { get; set; }
			public UseDelegate Use { get; set; }

			public UseAction(string description, UseDelegate use) {
				Description = description;
				Use = use;
			}
		}

		private List<UseAction> uses;

		public IEnumerable<UseAction> Uses {
			get { return uses; }
		}

		public UseableFeature(IEnumerable<UseAction> uses) {
			this.uses = new List<UseAction>(uses);
		}

		public override Component Copy() {
			return new UseableFeature(uses);
		}
	}

	public class PassiveFeature : Component {
		public delegate void NearDelegate(Entity entityNear, Entity featureEntity);
		public NearDelegate Near { get; set; }

		public PassiveFeature(NearDelegate near) {
			Near = near;            
		}

		public override Component Copy() {
			return new PassiveFeature((NearDelegate)Near.Clone());
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