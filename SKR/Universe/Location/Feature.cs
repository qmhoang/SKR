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
        public Feature(string refid, string asset) {            
            RefId = refid;
            components = new List<ActiveFeatureComponent>();
            Asset = asset;
            Uid = new UniqueId();
            Transparent = true;
            Walkable = true;
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

        private List<ActiveFeatureComponent> components;       

        public Feature Add(params ActiveFeatureComponent[] comps) {
            foreach (var comp in comps) {
                comp.Owner = this;
                components.Add(comp);
            }

            return this;
        }

        public Feature Remove(ActiveFeatureComponent component) {
            component.Owner = null;
            components.Remove(component);
            return this;
        }

        public void Use(Actor user, string action) {
            foreach (var component in components) {
                if (component.Action == action)
                    component.Use(this, user);
            }
        }

        public IEnumerable<string> ActiveUsages { get { return components.Select(fc => fc.Action); } }
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

    public class ActiveFeatureComponent {
        public Feature Owner { get; set; }
        public string Action { get; set; }
        public Action<Feature, Actor> Use;

        public ActiveFeatureComponent(string action, Action<Feature, Actor> use) {
            Use = use;
            Action = action;
        }
    }
}