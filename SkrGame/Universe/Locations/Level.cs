using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
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

	public class Level : DEngine.Core.Level {
		private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		protected string[,] Map;
		protected Dictionary<string, Terrain> TerrainDefinitions;
		
		public string RefId { get; protected set; }
		public UniqueId Uid { get; protected set; }

		public World World { get; set; }

		private FilteredCollection entities;

		[ContractInvariantMethod]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
		private void ObjectInvariant() {
			Contract.Invariant(TerrainDefinitions != null);
		}
		
		public Level(Size size, string fill, IEnumerable<Terrain> terrainDefinitions) : base(size) {
			Uid = new UniqueId();

			Map = new string[Size.Width, Size.Height];
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

		[Pure]
		public Terrain GetTerrain(int x, int y) {
			Contract.Requires<ArgumentOutOfRangeException>(IsInBoundsOrBorder(x, y));			
			return TerrainDefinitions[Map[x, y]];
		}

		private void entities_OnEntityRemove(Entity entity) {
			Contract.Requires<ArgumentNullException>(entity != null, "entity");
			if (entity.Has<Blocker>()) {
				ResetTransparency(entity.Get<Location>().Position);
				ResetWalkable(entity.Get<Location>().Position);
				entity.Get<Blocker>().WalkableChanged -= FeatureWalkableChanged;
				entity.Get<Blocker>().TransparencyChanged -= FeatureTransparencyChanged;
			}
		}

		private void entities_OnEntityAdd(Entity entity) {
			Contract.Requires<ArgumentNullException>(entity != null, "entity");
			if (entity.Has<Blocker>()) {
				ResetTransparency(entity.Get<Location>().Position);
				ResetWalkable(entity.Get<Location>().Position);
				entity.Get<Blocker>().WalkableChanged += FeatureWalkableChanged;
				entity.Get<Blocker>().TransparencyChanged += FeatureTransparencyChanged;
			}
		}

		private void FeatureWalkableChanged(Component sender, EventArgs e) {
			Contract.Requires<ArgumentNullException>(sender != null, "sender");
			Contract.Requires<ArgumentNullException>(e != null, "e");

			var p = EntityManager[sender.OwnerUId].Get<Location>().Position;

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

		private void FeatureTransparencyChanged(Component sender, EventArgs e) {
			Contract.Requires<ArgumentNullException>(sender != null, "sender");
			Contract.Requires<ArgumentNullException>(e != null, "e");

			var p = EntityManager[sender.OwnerUId].Get<Location>().Position;

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
