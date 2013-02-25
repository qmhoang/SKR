using SkrGame.Universe.Entities.Actors;

namespace SkrGame.Universe.Entities.Stats {
	public sealed class Attribute : Stat {
		public int Temporary { get; set; }
		public string Abbreviation { get; set; }

		private int value;
		public override int Value {
			get { return value + Temporary; }
			set { this.value = value; }
		}

		public override int MaximumValue { get; set; }		

		public Attribute(string name, string abbreviation, int rank, int maxRank) : base(name) {
			Value = rank;
			MaximumValue = maxRank;
			Abbreviation = abbreviation;
		}
	}
}