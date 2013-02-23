namespace SkrGame.Universe.Entities.Stats {
	public abstract class Stat {
		public string Name { get; set; }
		
		public abstract int Value { get; set; }
		public abstract int MaximumValue { get; set; }

		public static implicit operator int(Stat s) {
			return s.Value;
		}

		protected Stat(string name) {
			Name = name;			
		}
	}
}