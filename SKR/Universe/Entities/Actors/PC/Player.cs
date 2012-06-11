using System;
using DEngine.Actor;
using DEngine.Actor.Components.Graphics;
using DEngine.Core;
using DEngine.Extensions;
using SKR.Universe.Location;

namespace SKR.Universe.Entities.Actors.PC {
    public class Player : Actor {
        public override int SightRadius {
            get { return 10; }
        }

        public override int Speed {
            get { return 100; }
        }

        public override void Update() {
            
        }

        public override bool Dead {
            get { return false; }
        }

        public override void OnDeath() {
            
        }

        public Player(Level level) : base("player", "player", level) {
            Name = "player";            
        }
    }
}
