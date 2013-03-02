using DEngine.Entities;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;

namespace SkrGame.Systems {
	public class EncumbranceSubsystem {
		private FilteredCollection entities;

		public EncumbranceSubsystem(EntityManager em) {
			entities = em.Get(typeof(Creature));
		}
	}

}