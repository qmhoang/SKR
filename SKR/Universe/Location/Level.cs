using System;
using System.Collections.Generic;
using System.Linq;
using DEngine.Actor;
using DEngine.Actor.Components.Graphics;
using DEngine.Core;
using DEngine.Extensions;
using SKR.Universe.Entities.Actors;
using SKR.Universe.Entities.Items;
using libtcod;

namespace SKR.Universe.Location {
    public enum TileEnum {
        Unused = 0,
        Grass,
        WoodFloor,
        HorizWall,
        VertWall,
        // ReSharper disable InconsistentNaming
        NEWall,
        NWWall,
        SEWall,
        SWWall,
        TWallE,
        TWallW,
        TWallS,
        TWallN,
        // ReSharper restore InconsistentNaming
        Fence,
    }

    public class Tile : ICopy<Tile>, IObject {
        public TileEnum Type { get; private set; }        
        public bool Transparent { get; private set; }
        public bool Walkable { get; private set; }
        public double WalkPenalty { get; private set; }        

        public Image Image { get; set; }

        public Tile(TileEnum type, bool transparent, bool walkable, double walkPenalty) {
            Type = type;
            Transparent = transparent;
            Walkable = walkable;
            WalkPenalty = walkPenalty;
            RefId = Type.ToString();
        }

        public Tile Copy() {
            return new Tile(Type, Transparent, Walkable, WalkPenalty);
        }

        public string RefId { get; private set; }

        public Point Position { get; set; }
    }

    public class Level {
        protected Tile[,] Map;
        public TCODMap Fov { get; protected set; }

        public Size Size { get; protected set; }
        public bool[,] Vision { get; protected set; }

        public int Width {
            get { return Size.Width; }
        }

        public int Height {
            get { return Size.Height; }
        }

        public string RefId { get; protected set; }
        public UniqueId Uid { get; protected set; }

        public List<Person> Actors { get; protected set; }
        public List<Item> Items { get; protected set; }

        public Level(UniqueId uid, Size size, Tile fill) {
            Uid = uid;
            Size = size;

            Vision = new bool[Width,Height];
            Map = new Tile[Width,Height];
            Fov = new TCODMap(Width, Height);

            Actors = new List<Person>();
            Items = new List<Item>();            

            for (int x = 0; x < Map.GetLength(0); x++)
                for (int y = 0; y < Map.GetLength(1); y++) {
                    Map[x, y] = fill.Copy();
                }
        }

        public IEnumerable<Person> GetActorInRadius(Point origin, double length) {
            List<Person> actors = new List<Person>(Actors.Where(m => m.Position.IsInCircle(origin, length)));
            if (World.Instance.Player != null && World.Instance.Player.Position.IsInCircle(origin, length))
                actors.Add(World.Instance.Player);
            return actors;
        }


        /// <summary>
        /// Returns the actor (including the player's) at a level's location, if no actor exist at location, null is returned
        /// </summary>
        public Person GetActorAtLocation(Point location) {
            if (!IsInBoundsOrBorder(location))
                throw new ArgumentOutOfRangeException();
            if (World.Instance.Player != null && World.Instance.Player.Position == location)  // TODO FIX HACK
                return World.Instance.Player;
            return Actors.Find(m => m.Position == location);
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
            if (World.Instance.Player != null && World.Instance.Player.Position.X == x && World.Instance.Player.Position.Y == y)  // TODO FIX HACK
                return true;
            return Actors.Exists(m => m.Position.X == x && m.Position.Y == y);
        }

        public void SetTile(int x, int y, Tile t) {
            if (!IsInBoundsOrBorder(x, y))
                throw new ArgumentOutOfRangeException();
            Map[x, y] = t;
        }

        public void SetTile(Point p, Tile t) {
            SetTile(p.X, p.Y, t);
        }

        public Tile GetTile(Point p) {
            return GetTile(p.X, p.Y);
        }

        public Tile GetTile(int x, int y) {
            if (!IsInBoundsOrBorder(x, y))
                throw new ArgumentOutOfRangeException();
            return Map[x, y];
        }

        public bool IsVisible(Point p) {
            return IsVisible(p.X, p.Y);
        }

        public bool IsVisible(int x, int y) {
            if (!IsInBoundsOrBorder(x, y))
                throw new ArgumentOutOfRangeException();
            return Fov.isInFov(x, y);
        }

        public bool IsWalkable(Point v) {
            return IsWalkable(v.X, v.Y);
        }

        public bool IsWalkable(int x, int y) {
            if (!IsInBoundsOrBorder(x, y))
                throw new ArgumentOutOfRangeException();
            return Fov.isWalkable(x, y);
        }

        /// <summary>
        /// Generate FOV from tiles and features
        /// </summary>
        public void GenerateFov() {
            // tiles
            for (int x = 0; x < Map.GetLength(0); x++)
                for (int y = 0; y < Map.GetLength(1); y++) {
                    var t = GetTile(x, y);
                    if (t == null)
                        Fov.setProperties(x, y, false, false);
                    else
                        Fov.setProperties(x, y, t.Transparent, t.Walkable);                    
                }

            // features
        }

        #region Guards
        public bool IsInBounds(Point v) {
            return IsInBounds(v.X, v.Y);
        }

        public bool IsInBoundsOrBorder(Point v) {
            return IsInBoundsOrBorder(v.X, v.Y);
        }

        public bool IsOnBorder(Point v) {
            return IsOnBorder(v.X, v.Y);
        }

        public bool IsInBounds(int x, int y) {
            return x >= 1 && y >= 1 && x < Size.Width - 1 && y < Size.Height - 1;
        }

        public bool IsInBoundsOrBorder(int x, int y) {
            return x >= 0 && y >= 0 && x < Size.Width && y < Size.Height;
        }

        public bool IsOnBorder(int x, int y) {
            return x == 0 || y == 0 || x == Size.Width - 1 && y == Size.Height - 1;
        }
        #endregion
    }
}
