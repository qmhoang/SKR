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

		private FilteredCollection entities;
		private FilteredCollection blockers;

		internal struct Cell {
			public bool Transparent;
			public bool Walkable;
		}

		internal Cell[,] Cells;

		public string RefId { get; protected set; }
		public UniqueId Uid { get; protected set; }

		public World World { get; set; }

		[ContractInvariantMethod]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
		private void ObjectInvariant() {
			Contract.Invariant(TerrainDefinitions != null);
		}

		public Level(Size size, EntityManager em, string fill, IEnumerable<Terrain> terrainDefinitions)
			: base(size) {
			Uid = new UniqueId();

			Map = new string[Size.Width, Size.Height];
			TerrainDefinitions = new Dictionary<string, Terrain>();

			foreach (var terrain in terrainDefinitions) {
				TerrainDefinitions.Add(terrain.Definition, terrain);
			}

			entities = em.Get<Location>();
			blockers = em.Get(typeof(Location), typeof(Blocker));
			Cells = new Cell[size.Width, size.Height];

			for (int x = 0; x < Map.GetLength(0); x++) {
				for (int y = 0; y < Map.GetLength(1); y++) {
					Map[x, y] = fill;


					var t = TerrainDefinitions[fill];
					if (t == null) {
						Cells[x, y].Walkable = false;
						Cells[x, y].Transparent = false;
					} else {
						Cells[x, y].Walkable = t.Walkable;
						Cells[x, y].Transparent = t.Transparent;
					}
				}
			}

			foreach (var entity in entities) {
				InitializeBlocker(entity);
			}

			blockers.OnEntityAdd += InitializeBlocker;
			blockers.OnEntityRemove += OnRemoveBlocker;
		}

		private void OnRemoveBlocker(Entity entity) {
			var position = entity.Get<Location>().Position;
			SetBlocker(position);
			entity.Get<Location>().PositionChanged -= OnBlockerPositionChanged;
			entity.Get<Blocker>().WalkableChanged -= OnblockerWalkableChanged;
			entity.Get<Blocker>().TransparencyChanged -= OnBlockerTransparencyChanged;
		}

		private void InitializeBlocker(Entity entity) {
			var position = entity.Get<Location>().Position;
			SetBlocker(position);
			entity.Get<Location>().PositionChanged += OnBlockerPositionChanged;
			entity.Get<Blocker>().WalkableChanged += OnblockerWalkableChanged;
			entity.Get<Blocker>().TransparencyChanged += OnBlockerTransparencyChanged;
		}

		private void OnBlockerTransparencyChanged(Component sender, EventArgs @event) {
			var position = sender.Entity.Get<Location>().Position;
			Cells[position.X, position.Y].Transparent = blockers.Where(e => e.Get<Location>().Position == position).All(e => e.Get<Blocker>().Transparent);
		}

		private void OnblockerWalkableChanged(Component sender, EventArgs @event) {
			var position = sender.Entity.Get<Location>().Position;
			Cells[position.X, position.Y].Walkable = blockers.Where(e => e.Get<Location>().Position == position).All(e => e.Get<Blocker>().Walkable);			
		}

		private void OnBlockerPositionChanged(Component sender, PositionChangedEvent e) {
			SetBlocker(e.Current);
			SetBlocker(e.Previous);			
		}

		private void SetBlocker(Point position) {
			if (GetTerrain(position).Walkable)
				Cells[position.X, position.Y].Walkable = blockers.Where(e => e.Get<Location>().Position == position).All(e => e.Get<Blocker>().Walkable);
			if (GetTerrain(position).Transparent)
				Cells[position.X, position.Y].Transparent = blockers.Where(e => e.Get<Location>().Position == position).All(e => e.Get<Blocker>().Transparent);
		}

		public void SetTerrain(int x, int y, string t) {
			if (!IsInBoundsOrBorder(x, y))
				throw new ArgumentOutOfRangeException();
			Map[x, y] = t;
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

		public override bool IsTransparent(int x, int y) {
			return IsTransparent(new Point(x, y));
		}

		public override bool IsWalkable(int x, int y) {
			return IsWalkable(new Point(x, y));
		}

		public override bool IsWalkable(Point v) {
			return Cells[v.X, v.Y].Walkable;
		}

		public override bool IsTransparent(Point v) {
			return Cells[v.X, v.Y].Transparent;
		}

		public override IEnumerable<Entity> GetEntitiesAt(Point location) {
			return GetEntities().Where(e => e.Get<Location>().Position == location);
		}

		public override IEnumerable<Entity> GetEntities() {
			return entities;
		}
	}
}
