using System;
using System.Collections.Generic;
using System.Linq;
using DEngine.Actor;
using DEngine.Core;
using SKR.Universe.Entities.Actors;

namespace SKR.Universe.Location {
    /// <summary>
    /// Features are useable "items" that reside on the map, they contain the logic in themselves on how to use them
    /// </summary>
    public class Feature {
        public Feature(string refid, string asset) : this(refid, asset, true, true) {            
        }

        public Feature(string refId, string asset, bool walkable, bool transparent) {
            RefId = refId;
            Asset = asset;
            this.walkable = walkable;
            this.transparent = transparent;
            components = new List<FeatureComponent>();
            Uid = new UniqueId();

        }

        public string Asset { get; set; }
        public Level Level { get; set; }

        public string RefId { get; private set; }
        public UniqueId Uid { get; private set; }
        public Point Position { get; set; }

        private bool transparent;
        private bool walkable;

        public bool Transparent {
            get { return transparent; }
            set { 
                transparent = value; 
                OnTransparencyChanged(new EventArgs<bool>(Transparent));
            }
        }

        public bool Walkable {
            get { return walkable; }
            set {
                walkable = value;
                OnWalkableChanged(new EventArgs<bool>(Walkable));
            }
        }

        public double WalkPenalty { get; set; }

        public event EventHandler<EventArgs<bool>> TransparencyChanged;
        public event EventHandler<EventArgs<bool>> WalkableChanged;

        public void OnTransparencyChanged(EventArgs<bool> e) {
            EventHandler<EventArgs<bool>> handler = TransparencyChanged;
            if (handler != null)
                handler(this, e);
        }


        public void OnWalkableChanged(EventArgs<bool> e) {
            EventHandler<EventArgs<bool>> handler = WalkableChanged;
            if (handler != null)
                handler(this, e);
        }

        private List<FeatureComponent> components;

        public Feature Add(params FeatureComponent[] comps)
        {
            foreach (var comp in comps) {
                comp.Owner = this;
                components.Add(comp);
            }

            return this;
        }

        public Feature Remove(FeatureComponent component)
        {
            component.Owner = null;
            components.Remove(component);
            return this;
        }

        public void Use(Actor user, string action) {
            foreach (var component in components) {
                if (component is ActiveFeatureComponent)
                    if (((ActiveFeatureComponent)component).Action == action)
                        ((ActiveFeatureComponent)component).Use(component as ActiveFeatureComponent, user);
            }
        }

        public void Near(Actor user) {
            foreach (var component in components) {
                if (component is PassiveFeatureComponent)
                    ((PassiveFeatureComponent) component).Near(component as PassiveFeatureComponent, user, user.Position.DistanceTo(Position));
            }
        }

        public IEnumerable<string> ActiveUsages { get { return components.Where(fc => fc is ActiveFeatureComponent).Select(fc => ((ActiveFeatureComponent)fc).Action); } }
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

    public abstract class FeatureComponent {
        public Feature Owner { get; set; }

    }

    public class ActiveFeatureComponent : FeatureComponent {        
        public string Action { get; set; }
        public Action<ActiveFeatureComponent, Actor> Use { get; set; }

        public ActiveFeatureComponent(string action, Action<ActiveFeatureComponent, Actor> use)
        {
            Use = use;
            Action = action;
        }
    }

    public class PassiveFeatureComponent : FeatureComponent {
        public Action<PassiveFeatureComponent, Actor, double> Near { get; set; }        

        public PassiveFeatureComponent(Action<PassiveFeatureComponent, Actor, double> near) {
            Near = near;            
        }
    }

    public class SwitchFeaturecomponent : FeatureComponent {
        public bool Switch { get; set; }

        public SwitchFeaturecomponent(bool @switch = false) {
            Switch = @switch;
        }
    }
}