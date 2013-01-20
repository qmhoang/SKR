using System;
using System.Collections.Generic;
using System.Linq;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using SkrGame.Universe.Entities.Features;

namespace SkrGame.Universe.Locations {
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
		
		public string RefId { get; protected set; }
		public UniqueId Uid { get; protected set; }

		public World World { get; set; }

		private FilteredCollection entities;
		
		public Level(Size size, string fill, IEnumerable<Terrain> terrainDefinitions) : base(size) {
			Uid = new UniqueId();

			Map = new string[Width, Height];
			TerrainDefinitions = new Dictionary<string, Terrain>();

			foreach (var terrain in terrainDefinitions) {
				TerrainDefinitions.Add(terrain.Definition, terrain);
			}

			for (int x = 0; x < Map.GetLength(0); x++) {
				for (int y = 0; y < Map.GetLength(1); y++) {
					Map[x, y] = fill;

					var t = TerrainDefinitions[fill];
					if (t == null)
						SetProperties(x, y, false, false);
					else
						SetProperties(x, y, t.Transparent, t.Walkable);
				}
			}

			EntityManager = new EntityManager();

			entities = EntityManager.Get(typeof(Blocker), typeof(Location));
			entities.OnEntityAdd += entities_OnEntityAdd;
			entities.OnEntityRemove += entities_OnEntityRemove;
		}

		void entities_OnEntityRemove(Entity entity) {
			if (entity.Has<Blocker>()) {
				ResetTransparency(entity.Get<Location>().Position);
				ResetWalkable(entity.Get<Location>().Position);
				entity.Get<Blocker>().WalkableChanged -= FeatureWalkableChanged;
				entity.Get<Blocker>().TransparencyChanged -= FeatureTransparencyChanged;
			}
		}

		void entities_OnEntityAdd(Entity entity) {
			if (entity.Has<Blocker>()) {
				ResetTransparency(entity.Get<Location>().Position);
				ResetWalkable(entity.Get<Location>().Position);
				entity.Get<Blocker>().WalkableChanged += FeatureWalkableChanged;
				entity.Get<Blocker>().TransparencyChanged += FeatureTransparencyChanged;
			}
		}
		
		public void SetTerrain(int x, int y, string t) {
			if (!IsInBoundsOrBorder(x, y))
				throw new ArgumentOutOfRangeException();
			Map[x, y] = t;			
			ResetTransparency(new Point(x, y));
			ResetWalkable(new Point(x, y));
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

//		/// <summary>
//		/// Generate FOV from tiles and features
//		/// </summary>
//		public void GenerateData() {
//			// tiles
//			for (int x = 0; x < Map.GetLength(0); x++)
//				for (int y = 0; y < Map.GetLength(1); y++) {
//					var t = GetTerrain(x, y);
//					if (t == null)
//						SetProperties(x, y, false, false);
//					else
//						SetProperties(x, y, t.Transparent, t.Walkable);
//				}
//
//			// features
////			Features.Each(feature => SetProperties(feature.Position.X, feature.Position.Y, feature.Transparent, feature.Walkable));
//		}
		
		// these are basically delegates that are added to every feature to detect when they change
		private void FeatureWalkableChanged(object sender, EventArgs e) {
			var p = EntityManager[((Component) sender).OwnerUId].Get<Location>().Position;

			ResetWalkable(p);
		}

		private void ResetWalkable(Point p) {
			if (GetTerrain(p.X, p.Y).Walkable) {
				var entitiesAt = GetEntitiesAt(p, typeof(Blocker));
				var isWalkable = entitiesAt.All(entity => entity.Get<Blocker>().Walkable);

				SetWalkable(p.X, p.Y, isWalkable);
			} else
				SetWalkable(p.X, p.Y, false);
		}

		private void FeatureTransparencyChanged(object sender, EventArgs e) {
			var p = EntityManager[((Component)sender).OwnerUId].Get<Location>().Position;

			ResetTransparency(p);
		}

		private void ResetTransparency(Point p) {
			if (GetTerrain(p.X, p.Y).Transparent) {
				var entitiesAt = GetEntitiesAt(p, typeof(Blocker));
				var isTransparent = entitiesAt.All(entity => entity.Get<Blocker>().Transparent);

				SetTransparency(p.X, p.Y, isTransparent);
			} else
				SetTransparency(p.X, p.Y, false);
		}
	}
}
