using System;
using System.Collections.Generic;
using System.Linq;
using DEngine.Actor;
using DEngine.Core;
using DEngine.Extensions;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;
using libtcod;

namespace SkrGame.Universe.Location {
	/// <summary>
	/// Tiles represent entities that are (normally immovable, but can be interacted with by actors)
	/// </summary>
	public class Terrain {
		public string Definition { get; protected set; }
		public string Asset { get; protected set; }
		public bool Transparent { get; protected set; }
		public bool Walkable { get; protected set; }
		public double WalkPenalty { get; protected set; }

		public Terrain(string definition, string asset, bool transparent, bool walkable, double walkPenalty) {
			Definition = definition;
			Asset = asset;
			Transparent = transparent;
			Walkable = walkable;
			WalkPenalty = walkPenalty;
		}
	}

	public class Level : Map {
		private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		protected string[,] Map;
		protected Dictionary<string, Terrain> TerrainDefinitions;

		public bool[,] Vision { get; protected set; }

		public string RefId { get; protected set; }
		public UniqueId Uid { get; protected set; }

		public World World { get; set; }

		private readonly List<Actor> actors;
		private readonly List<Feature> features;
		private readonly Dictionary<Item, Point> itemsByLocation;

		public IEnumerable<Actor> Actors { get { return actors; } }

		public IEnumerable<Tuple<Point, Item>> Items {
			get {
				return itemsByLocation.Select(i => new Tuple<Point, Item>(i.Value, i.Key));
			}
		}

		public IEnumerable<Feature> Features { get { return features; } }

		public Level(Size size, string fill, IEnumerable<Terrain> terrainDefinitions) : base(size) {
			Uid = new UniqueId();

			Vision = new bool[Width, Height];
			Map = new string[Width, Height];
			TerrainDefinitions = new Dictionary<string, Terrain>();

			foreach (var terrain in terrainDefinitions) {
				TerrainDefinitions.Add(terrain.Definition, terrain);
			}

			actors = new List<Actor>();
			itemsByLocation = new Dictionary<Item, Point>();
			features = new List<Feature>();

			for (int x = 0; x < Map.GetLength(0); x++)
				for (int y = 0; y < Map.GetLength(1); y++) {
					Map[x, y] = fill;

					var t = TerrainDefinitions[fill];
					if (t == null)
						SetProperties(x, y, false, false);
					else
						SetProperties(x, y, t.Transparent, t.Walkable);
				}
		}

		public Feature GetFeatureAtLocation(Point location) {
			if (!IsInBoundsOrBorder(location))
				throw new ArgumentOutOfRangeException();
			return Features.ToList().Find(f => f.Position == location);
		}

		public IEnumerable<Actor> GetActorInRadius(Point origin, double length) {
			List<Actor> actorsInRadius = new List<Actor>(Actors.Where(m => m.Position.IsInCircle(origin, length)));
			if (World.Player != null && World.Player.Position.IsInCircle(origin, length))
				actorsInRadius.Add(World.Player);
			return actorsInRadius;
		}


		/// <summary>
		/// Returns the actor (including the player's) at a level's location, if no actor exist at location, null is returned
		/// </summary>
		public Actor GetActorAtLocation(Point location) {
			if (!IsInBoundsOrBorder(location))
				throw new ArgumentOutOfRangeException();
			if (World.Player != null && World.Player.Position == location)  // TODO FIX HACK
				return World.Player;
			return actors.Find(m => m.Position == location);
		}

		/// <summary>
		/// Returns a boolean that represents if an actor (including the player) exist at the location provided in the current level
		/// </summary>
		public bool DoesActorExistAtLocation(Point location) {
			return DoesActorExistAtLocation(location.X, location.Y);
		}

		/// <summary>
		/// Returns a boolean that represents if an actor (including the player) exist at the location provided in the current level
		/// </summary>
		public bool DoesActorExistAtLocation(int x, int y) {
			if (!IsInBoundsOrBorder(x, y))
				throw new ArgumentOutOfRangeException();
			if (World.Player != null && World.Player.Position.X == x && World.Player.Position.Y == y)  // TODO FIX HACK
				return true;
			return actors.Exists(m => m.Position.X == x && m.Position.Y == y);
		}

		public bool DoesFeatureExistAtLocation(Point location) {
			return DoesFeaturExistAtLocation(location.X, location.Y);
		}

		public bool DoesFeaturExistAtLocation(int x, int y) {
			if (!IsInBoundsOrBorder(x, y))
				throw new ArgumentOutOfRangeException();
			return features.Exists(f => f.Position.X == x && f.Position.Y == y);
		}

		public void SetTerrain(int x, int y, string t) {
			if (!IsInBoundsOrBorder(x, y))
				throw new ArgumentOutOfRangeException();
			Map[x, y] = t;
			var terrain = GetTerrain(x, y);
			SetProperties(x, y, terrain.Transparent, terrain.Walkable);
		}

		public void SetTerrain(Point p, string t) {
			SetTerrain(p.X, p.Y, t);
		}

		public Terrain GetTerrain(Point p) {
			return GetTerrain(p.X, p.Y);
		}

		public Terrain GetTerrain(int x, int y) {
			if (!IsInBoundsOrBorder(x, y))
				throw new ArgumentOutOfRangeException();
			return TerrainDefinitions[Map[x, y]];
		}

		/// <summary>
		/// Generate FOV from tiles and features
		/// </summary>
		public void GenerateFov() {
			// tiles
			for (int x = 0; x < Map.GetLength(0); x++)
				for (int y = 0; y < Map.GetLength(1); y++) {
					var t = GetTerrain(x, y);
					if (t == null)
						SetProperties(x, y, false, false);
					else
						SetProperties(x, y, t.Transparent, t.Walkable);
				}

			// features
			Features.Each(feature => SetProperties(feature.Position.X, feature.Position.Y, feature.Transparent, feature.Walkable));
		}

		public void AddFeature(Feature feature) {
			features.Add(feature);
			feature.Level = this;
			feature.TransparencyChanged += FeatureTransparencyChanged;
			feature.WalkableChanged += FeatureWalkableChanged;
			SetProperties(feature.Position.X, feature.Position.Y, feature.Transparent, feature.Walkable);
		}

		public bool RemoveFeature(Feature feature) {
			feature.Level = null;
			feature.TransparencyChanged -= FeatureTransparencyChanged;
			feature.WalkableChanged -= FeatureWalkableChanged;

			var terrain = GetTerrain(feature.Position);
			SetProperties(feature.Position.X, feature.Position.Y, terrain.Transparent, terrain.Walkable);

			return features.Remove(feature);
		}

		// these are basically delegates that are added to every feature to detect when they change
		private void FeatureWalkableChanged(object sender, EventArgs e) {
			if (sender is Feature) {
				var feature = sender as Feature;
				SetProperties(feature.Position.X, feature.Position.Y, feature.Transparent, feature.Walkable);
			}
		}

		private void FeatureTransparencyChanged(object sender, EventArgs e) {
			if (sender is Feature) {
				var feature = sender as Feature;
				SetProperties(feature.Position.X, feature.Position.Y, feature.Transparent, feature.Walkable);
			}
		}

		public void AddActor(Actor actor) {
			actors.Add(actor);
		}

		public bool RemoveActor(Actor actor) {
			return actors.Remove(actor);
		}

		public void AddItem(Item item, Point position) {
			try {
				itemsByLocation.Add(item, position);
			} catch (ArgumentException) {
				// item already exist, lets just move it to the new position
				itemsByLocation[item] = position;
				Logger.WarnFormat("Item (name: {0}, RefId: {5}, Uid: {1}) already exist in level (RefId: {2}, Uid: {3}), moving item to given position: {4}",
					item.Name, item.UniqueId, RefId, Uid, position, item.RefId);
			}
		}

		public bool RemoveItem(Item item) {
			return itemsByLocation.Remove(item);
		}
	}
}
