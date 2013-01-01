namespace SkrGame.Universe.Entities.Actors.NPC.AI {
	public abstract class NpcIntelligence {
		protected Npc Actor;

		protected NpcIntelligence(Npc actor) {
			this.Actor = actor;
		}

		public virtual void Update() { }
	}
}