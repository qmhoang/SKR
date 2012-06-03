using System;
using DEngine.Actor;
using DEngine.Core;
using DEngine.Extensions;
using SKR.Universe.Location;

namespace SKR.Universe.Entities.Actor.PC {
    public class Player : Person {
        public override ActionResult Move(int dx, int dy) {
            throw new NotImplementedException();
        }

        public override int SightRadius {
            get { return 10; }
        }

        public override bool Spot(Point position) {
            return Level.IsVisible(position);
        }

        public override char Ascii { get { return '@'; } }

        public override Color Color { get { return ColorPresets.White; } }

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
        }
    }
}
