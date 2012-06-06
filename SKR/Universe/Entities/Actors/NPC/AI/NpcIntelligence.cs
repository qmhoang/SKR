using libtcod;

namespace SKR.Universe.Entities.Actors.NPC.AI {
    public abstract class NpcIntelligence : ITCODPathCallback {
        protected Npc Actor;

        protected NpcIntelligence(Npc actor) {
            this.Actor = actor;
        }

        public virtual void Update() { }

        public override float getWalkCost(int xFrom, int yFrom, int xTo, int yTo) {
            if (!Actor.Level.IsWalkable(xTo, yTo) || Actor.Level.DoesActorExistAtLocation(xTo, yTo)) {
                return 0f;
            }
            return 1f;
        }
    }
}