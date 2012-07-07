using DEngine.Core;
using SkrGame.Universe.Entities.Actors.NPC.AI;
using SkrGame.Universe.Location;

namespace SkrGame.Universe.Entities.Actors.NPC {
    public class Npc : Actor {
        public NpcIntelligence Intelligence { get; set; }

        public override int SightRadius {
            get { return 10; }
        }

        public override bool HasLineOfSight(Point position) {            
            return Level.IsVisible(position);
        }

        public override bool CanSpot(DEngine.Actor.Entity actor) {
            throw new System.NotImplementedException();
        }

        public override int Speed {
            get { return 100; }
        }

        public override void Update() {
            Intelligence.Update();
        }


        public override void OnDeath() {
        }

        public Npc(Level level) : base("npc", "npc", level) {
            RefId = Name = "npc";            
        }
    }
}