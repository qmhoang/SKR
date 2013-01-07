using DEngine.Core;
using DEngine.Entity;

namespace SkrGame.Universe.Entities.Actors.PC {
	public class Player : Component {

		public Player() {}
		
		public override Component Copy() {
			return new Player();
		}
	}
}
