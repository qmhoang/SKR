using DEngine.Actor;
using DEngine.Core;
using SKR.Universe.Entities.Actors.PC;
using libtcod;

namespace SKR.Universe.Entities.Actors.NPC.AI {
    class SimpleIntelligence : NpcIntelligence {
        private TCODPath pf;
        
        public SimpleIntelligence(Npc monster)
                : base(monster) {
            pf = new TCODPath(monster.Level.Width, monster.Level.Height, this, 1.0f);
        }

        public override void Update() {
            Player player = World.Instance.Player;
            if (Actor.HasLineOfSight(player.Position)) {
                pf.compute(Actor.Position.X, Actor.Position.Y, player.Position.X, player.Position.Y);
                int nx = Actor.Position.X, ny = Actor.Position.Y;


                if (pf.walk(ref nx, ref ny, false)) {                    
                    Point dir = new Point(nx, ny) - Actor.Position;
                    if (Actor.Move(dir) == ActionResult.Success) {
                    }
                }
            } else {
                Actor.Wait();
            }
        }
    }
}