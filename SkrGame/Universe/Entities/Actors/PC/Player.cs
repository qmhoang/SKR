using DEngine.Core;
using DEngine.Entities;

namespace SkrGame.Universe.Entities.Actors.PC {
	public class Player : Component {

		public Player() {}
		
		public override Component Copy() {
			return new Player();
		}
	}
}
