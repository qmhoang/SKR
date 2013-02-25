using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using DEngine.Level;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Features;

namespace SkrGame.Universe.Locations {
	public class Level : AbstractLevel, IEquatable<Level> {
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

		public Level(Size size, World world, string fill, IEnumerable<Terrain> terrainDefinitions)
			: base(size) {
			Uid = new UniqueId();

			Map = new string[Size.Width, Size.Height];
			TerrainDefinitions = new Dictionary<string, Terrain>();

			foreach (var terrain in terrainDefinitions) {
				TerrainDefinitions.Add(terrain.Definition, terrain);
			}

			this.World = world;
			entities = world.EntityManager.Get<GameObject>();
			blockers = world.EntityManager.Get(typeof(GameObject), typeof(Scenery));
			Cells = new Cell[size.Width, size.Height];

			for (int x = 0; x < Map.GetLength(0); x++) {
				for (int y = 0; y < Map.GetLength(1); y++) {
					Map[x, y] = fill;

					Cells[x, y].Walkable = true;
					Cells[x, y].Transparent = true;
				}
			}

			foreach (var entity in entities) {
				InitializeBlocker(entity);
			}

			blockers.OnEntityAdd += InitializeBlocker;
			blockers.OnEntityRemove += OnRemoveBlocker;
		}

		private void OnRemoveBlocker(Entity entity) {
			var position = entity.Get<GameObject>().Location;
			SetBlocker(position);
			entity.Get<GameObject>().PositionChanged -= OnBlockerPositionChanged;
			entity.Get<Scenery>().WalkableChanged -= OnblockerWalkableChanged;
			entity.Get<Scenery>().TransparencyChanged -= OnBlockerTransparencyChanged;
		}

		private void InitializeBlocker(Entity entity) {
			var position = entity.Get<GameObject>().Location;
			SetBlocker(position);
			entity.Get<GameObject>().PositionChanged += OnBlockerPositionChanged;
			entity.Get<Scenery>().WalkableChanged += OnblockerWalkableChanged;
			entity.Get<Scenery>().TransparencyChanged += OnBlockerTransparencyChanged;
		}

		private void OnBlockerTransparencyChanged(Component sender, EventArgs @event) {
			var position = sender.Entity.Get<GameObject>().Location;
			Cells[position.X, position.Y].Transparent = blockers.Where(e => e.Get<GameObject>().Location == position).All(e => e.Get<Scenery>().Transparent);
		}

		private void OnblockerWalkableChanged(Component sender, EventArgs @event) {
			var position = sender.Entity.Get<GameObject>().Location;
			Cells[position.X, position.Y].Walkable = blockers.Where(e => e.Get<GameObject>().Location == position).All(e => e.Get<Scenery>().Walkable);			
		}

		private void OnBlockerPositionChanged(Component sender, PositionChangedEvent e) {
			SetBlocker(e.Current);
			SetBlocker(e.Previous);			
		}

		private void SetBlocker(Point position) {
			if (GetTerrain(position).Walkable)
				Cells[position.X, position.Y].Walkable = blockers.Where(e => e.Get<GameObject>().Location == position).All(e => e.Get<Scenery>().Walkable);
			if (GetTerrain(position).Transparent)
				Cells[position.X, position.Y].Transparent = blockers.Where(e => e.Get<GameObject>().Location == position).All(e => e.Get<Scenery>().Transparent);
		}

		public void SetTerrain(int x, int y, string t) {
			if (!IsInBounds(x, y))
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
			Contract.Requires<ArgumentOutOfRangeException>(IsInBounds(x, y));
			return TerrainDefinitions[Map[x, y]];
		}

		public override bool IsTransparent(int x, int y) {
			return IsTransparent(new Point(x, y));
		}

		public override bool IsWalkable(int x, int y) {
			return IsWalkable(new Point(x, y));
		}

		public override bool IsWalkable(Point v) {
			return IsInBounds(v) && TerrainDefinitions[Map[v.X, v.Y]].Walkable && Cells[v.X, v.Y].Walkable;
		}

		public override bool IsTransparent(Point v) {
			return IsInBounds(v) && TerrainDefinitions[Map[v.X, v.Y]].Transparent && Cells[v.X, v.Y].Transparent;
		}

		public override IEnumerable<Entity> GetEntitiesAt(Point location) {
			return GetEntities().Where(e => e.Get<GameObject>().Location == location);
		}

		public override IEnumerable<Entity> GetEntitiesAt<T>(Point location) {
			return World.EntityManager.Get(typeof(T), typeof(GameObject)).Where(e => e.Get<GameObject>().Location == location);
		}

		public override IEnumerable<Entity> GetEntitiesAt<T1, T2>(Point location) {
			return World.EntityManager.Get(typeof(T1), typeof(T2), typeof(GameObject)).Where(e => e.Get<GameObject>().Location == location);
		}

		public override IEnumerable<Entity> GetEntitiesAt<T1, T2, T3>(Point location) {
			return World.EntityManager.Get(typeof(T1), typeof(T2), typeof(T3), typeof(GameObject)).Where(e => e.Get<GameObject>().Location == location);
		}

		public override IEnumerable<Entity> GetEntities() {
			return entities;
		}

		public bool Equals(Level other) {
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;
			return Equals(other.Uid, Uid);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(Level))
				return false;
			return Equals((Level) obj);
		}

		public override int GetHashCode() {
			return (Uid != null ? Uid.GetHashCode() : 0);
		}

		public static bool operator ==(Level left, Level right) {
			return Equals(left, right);
		}

		public static bool operator !=(Level left, Level right) {
			return !Equals(left, right);
		}
	}
}
