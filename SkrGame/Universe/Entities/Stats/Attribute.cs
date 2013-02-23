using SkrGame.Universe.Entities.Actors;

namespace SkrGame.Universe.Entities.Stats {
	public sealed class Attribute : Stat {
		public override int Value { get; set; }
		public override int MaximumValue { get; set; }		

		public Attribute(string name, int maxRank, int rank = 0) : base(name) {
			Value = rank;
			MaximumValue = maxRank;
		}
	}
}