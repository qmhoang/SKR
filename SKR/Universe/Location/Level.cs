using System;
using System.Collections.Generic;
using System.Linq;
using DEngine.Actor;
using DEngine.Core;
using DEngine.Extensions;
using SKR.Universe.Entities.Actor;
using SKR.Universe.Entities.Items;
using libtcod;

namespace SKR.Universe.Location {
    public enum Tile {
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

    public class Cell {
        public Pigment Pigment { get; private set; }
        public char Ascii { get; private set; }

        public double WalkPenalty { get; private set; }
        public bool Transparent { get; private set; }

        private static Dictionary<Tile, Cell> cells =
                new Dictionary<Tile, Cell>
                    {
                        {Tile.Unused, new Cell((char) TCODSpecialCharacter.Block1, new Pigment(ColorPresets.White, ColorPresets.Black), 0.0, false)},

                        {Tile.WoodFloor, new Cell(' ', new Pigment(ColorPresets.Black, ColorPresets.Black), 0.0, true)},
                        {Tile.Grass, new Cell((char) TCODSpecialCharacter.Block1, new Pigment(ColorPresets.DarkerGreen, ColorPresets.LightGreen), 0.0, true)},

                        {Tile.HorizWall, new Cell((char) TCODSpecialCharacter.HorzLine, new Pigment(ColorPresets.White, ColorPresets.Black), 0.0, false)},
                        {Tile.VertWall, new Cell((char) TCODSpecialCharacter.VertLine, new Pigment(ColorPresets.White, ColorPresets.Black), 0.0, false)},

                        {Tile.NEWall, new Cell((char) TCODSpecialCharacter.Block1, new Pigment(ColorPresets.White, ColorPresets.Black), 0.0, false)},
                        {Tile.NWWall, new Cell((char) TCODSpecialCharacter.Block1, new Pigment(ColorPresets.White, ColorPresets.Black), 0.0, false)},
                        {Tile.SEWall, new Cell((char) TCODSpecialCharacter.Block1, new Pigment(ColorPresets.White, ColorPresets.Black), 0.0, false)},
                        {Tile.SWWall, new Cell((char) TCODSpecialCharacter.Block1, new Pigment(ColorPresets.White, ColorPresets.Black), 0.0, false)},

                        {Tile.TWallE, new Cell((char) TCODSpecialCharacter.Block1, new Pigment(ColorPresets.White, ColorPresets.Black), 0.0, false)},
                        {Tile.TWallW, new Cell((char) TCODSpecialCharacter.Block1, new Pigment(ColorPresets.White, ColorPresets.Black), 0.0, false)},
                        {Tile.TWallS, new Cell((char) TCODSpecialCharacter.Block1, new Pigment(ColorPresets.White, ColorPresets.Black), 0.0, false)},
                        {Tile.TWallN, new Cell((char) TCODSpecialCharacter.Block1, new Pigment(ColorPresets.White, ColorPresets.Black), 0.0, false)},  

                    };                                       

        private Cell(char ascii, Pigment pigment, double walkPenalty, bool transparent) {
            Pigment = pigment;
            Ascii = ascii;
            WalkPenalty = walkPenalty;
            Transparent = transparent;
        }

        public static Cell GetCell(Tile tile) {
            return cells[tile];
        }
    }

    public class Level : IGuid {
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

        public long Guid { get; protected set; }

        public List<Person> Actors { get; protected set; }
        public List<Item> Items { get; protected set; } 

        public Level(Size size) {
            Size = size;

            Vision = new bool[Width, Height];
            Map = new Tile[Width, Height];
            Fov = new TCODMap(Width, Height);

            Actors = new List<Person>();
            Items = new List<Item>();
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

        public void SetTile(Point v, Tile t) {
            Map[v.X, v.Y] = t;
        }

        public Tile GetTile(Point v) {
            return Map[v.X, v.Y];
        }

        public Tile GetTile(int x, int y) {
            if (!IsInBoundsOrBorder(x, y))
                throw new ArgumentOutOfRangeException();
            return Map[x, y];
        }

        public bool IsVisible(Point v) {
            return IsVisible(v.X, v.Y);
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
            for (int i = 0; i < Map.GetLength(0); i++)
                for (int j = 0; j < Map.GetLength(1); j++)
                    if (Cell.GetCell(GetTile(i, j)).Transparent)
                        Fov.setProperties(i, j, true, true);

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
