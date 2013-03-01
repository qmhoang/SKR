using DEngine.Entities;

namespace SkrGame.Universe.Entities.Actors {
	public sealed class VisibleComponent : Component {
		/// <summary>
		/// How difficulty is it to see the item, -1 means its impossible to see
		/// </summary>
		public int VisibilityIndex { get; set; }
		public int DefaultIndex { get; set; }

		public void Reset() {
			VisibilityIndex = DefaultIndex;
		}

		public VisibleComponent(int defaultIndex) {
			VisibilityIndex = DefaultIndex = defaultIndex;
		}

		public VisibleComponent() {}
		public override Component Copy() {
			return new VisibleComponent()
			       {
			       		VisibilityIndex = VisibilityIndex,
						DefaultIndex = DefaultIndex,						
			       };
		}
	}
}