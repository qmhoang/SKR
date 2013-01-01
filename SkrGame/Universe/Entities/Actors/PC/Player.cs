using DEngine.Core;
using SkrGame.Universe.Location;

namespace SkrGame.Universe.Entities.Actors.PC {
	public class Player : Actor {
		public override int SightRadius {
			get { return 10; }
		}

		public override int Speed {
			get { return 100; }
		}

		public override bool Dead {
			get { return false; }
		}

		public Player(Level level) : base("Player", "player", "player", level) {
			Name = "player";
		}
	}
}
