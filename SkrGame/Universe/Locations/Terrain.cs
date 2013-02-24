namespace SkrGame.Universe.Locations {
	/// <summary>
	/// Tiles represent entities that are (normally immovable, but can be interacted with by actors)
	/// </summary>
	public class Terrain {
		public string Definition { get; protected set; }
		public string Asset { get; protected set; }
		public bool Transparent { get; protected set; }
		public bool Walkable { get; protected set; }
		public double WalkCost { get; protected set; }

		public Terrain(string definition, string asset, bool transparent, bool walkable, double walkCost) {
			Definition = definition;
			Asset = asset;
			Transparent = transparent;
			Walkable = walkable;
			WalkCost = walkCost;
		}
	}
}