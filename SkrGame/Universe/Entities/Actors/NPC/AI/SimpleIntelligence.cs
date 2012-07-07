using DEngine.Actor;
using DEngine.Core;
using SKR.Universe.Entities.Actors.PC;
using libtcod;

namespace SKR.Universe.Entities.Actors.NPC.AI {
    class SimpleIntelligence : NpcIntelligence {
        private TCODPath pf;
        
        public SimpleIntelligence(Npc monster)
                : base(monster) {
            pf = new TCODPath(monster.Level.Width, monster.Level.Height, this, 1.41f);
        }

        public override void Update() {
            Player player = World.Instance.Player;
            if (Actor.HasLineOfSight(player.Position)) {
                var distance = Actor.Position.DistanceTo(player.Position);
                if (distance <= 1) {
                    Actor.GetTalent(Skill.Attack).InvokeAction(player.Position);
                } else if (distance <= 1.5) // we are diagonally next to the player
                {
                    
                } else {
                    pf.compute(Actor.Position.X, Actor.Position.Y, player.Position.X, player.Position.Y);
                    int nx = Actor.Position.X, ny = Actor.Position.Y;


                    if (pf.walk(ref nx, ref ny, false)) {
                        Point dir = new Point(nx, ny) - Actor.Position;
                        if (Actor.Move(dir) == ActionResult.Success) {
                        }
                    }
                }

            } else {
                Actor.Wait();
            }
        }
    }

    class BasicHumanIntelligence : SimpleIntelligence {


        public BasicHumanIntelligence(Npc actor) : base(actor) {
            
        }

        public override void Update() {
            
        }
    }

    class FightOrFlightIntelligence : SimpleIntelligence {
        private Actor target;


        public FightOrFlightIntelligence(Npc monster) : base(monster) {
            target = World.Instance.Player;
        }
    }
}