using SkrGame.Universe.Entities.Actors;

namespace SkrGame.Universe.Entities.Stats {
	public sealed class Attribute : Stat {
		public int Temporary { get; set; }

		private int value;
		public override int Value {
			get { return value + Temporary; }
			set { this.value = value; }
		}

		public override int MaximumValue { get; set; }		

		public Attribute(string name, int maxRank, int rank = 0) : base(name) {
			Value = rank;
			MaximumValue = maxRank;
		}
	}
}