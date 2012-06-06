using System;
using DEngine.Actor;
using DEngine.Core;
using DEngine.Extensions;
using SKR.Universe.Location;

namespace SKR.Universe.Entities.Actors.PC {
    public class Player : Person {
        public override int SightRadius {
            get { return 10; }
        }

        public override bool HasLineOfSight(Point position) {
            return Level.IsVisible(position);
        }

        public override bool CanSpot(Actor actor) {
            throw new NotImplementedException();
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

        public Player(Level level)
            : base("player", level) {
            RefId = "player";
        }
    }
}
