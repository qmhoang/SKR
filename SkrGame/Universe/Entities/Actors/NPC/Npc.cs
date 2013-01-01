using DEngine.Core;
using SkrGame.Universe.Entities.Actors.NPC.AI;
using SkrGame.Universe.Location;

namespace SkrGame.Universe.Entities.Actors.NPC {
	public class Npc : Actor {
		public NpcIntelligence Intelligence { get; set; }

		public override int SightRadius {
			get { return 10; }
		}

		public override int Speed {
			get { return 100; }
		}

		public override void Update() {
			base.Update();

			Intelligence.Update();
		}

		public Npc(Level level) : base("npc", "npc", "npc", level) { }
	}
}