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
//    public enum TileEnum {
//        Unused = 0,
//        Grass,
//        WoodFloor,
//        Wall,
//        HorizWall,
//        VertWall,
//        // ReSharper disable InconsistentNaming
//        NEWall,
//        NWWall,
//        SEWall,
//        SWWall,
//        TWallE,
//        TWallW,
//        TWallS,
//        TWallN,
//        // ReSharper restore InconsistentNaming
//        Fence,
//    }

    /// <summary>
    /// Tiles represent entities that are (normally immovable, but interactive with actors)
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

    public class Level {
        protected string[,] Map;
        protected Dictionary<string, Terrain> TerrainDefinitions; 
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

        private List<Actor> actors;
        private List<Item> items;
        private List<Feature> features;

        public IEnumerable<Actor> Actors { get { return actors; } }
        public IEnumerable<Item> Items { get { return items; } }
        public IEnumerable<Feature> Features { get { return features; } }        

        public Level(Size size, string fill) {
            Uid = new UniqueId();
            Size = size;

            Vision = new bool[Width,Height];
            Map = new string[Width,Height];
            Fov = new TCODMap(Width, Height);
            TerrainDefinitions = new Dictionary<string, Terrain>();

            actors = new List<Actor>();
            items = new List<Item>();   
            features = new List<Feature>();

            for (int x = 0; x < Map.GetLength(0); x++)
                for (int y = 0; y < Map.GetLength(1); y++) {
                    Map[x, y] = fill;
                }
        }

        public Feature GetFeatureAtLocation(Point location) {
            if (!IsInBoundsOrBorder(location))
                throw new ArgumentOutOfRangeException();
            return Features.ToList().Find(f => f.Position == location);
        }

        public IEnumerable<Actor> GetActorInRadius(Point origin, double length) {
            List<Actor> actors = new List<Actor>(Actors.Where(m => m.Position.IsInCircle(origin, length)));
            if (World.Instance.Player != null && World.Instance.Player.Position.IsInCircle(origin, length))
                actors.Add(World.Instance.Player);
            return actors;
        }


        /// <summary>
        /// Returns the actor (including the player's) at a level's location, if no actor exist at location, null is returned
        /// </summary>
        public Actor GetActorAtLocation(Point location) {
            if (!IsInBoundsOrBorder(location))
                throw new ArgumentOutOfRangeException();
            if (World.Instance.Player != null && World.Instance.Player.Position == location)  // TODO FIX HACK
                return World.Instance.Player;
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
            if (World.Instance.Player != null && World.Instance.Player.Position.X == x && World.Instance.Player.Position.Y == y)  // TODO FIX HACK
                return true;
            return actors.Exists(m => m.Position.X == x && m.Position.Y == y);
        }

        public void SetTerrain(int x, int y, string t) {
            if (!IsInBoundsOrBorder(x, y))
                throw new ArgumentOutOfRangeException();
            Map[x, y] = t;
            var terrain = GetTerrain(x, y);
            Fov.setProperties(x, y, terrain.Transparent, terrain.Walkable);
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
                return false;
            return Fov.isWalkable(x, y);
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
                        Fov.setProperties(x, y, false, false);
                    else
                        Fov.setProperties(x, y, t.Transparent, t.Walkable);                    
                }

            // features           
            Features.Each(feature => Fov.setProperties(feature.Position.X, feature.Position.Y, feature.Transparent, feature.Walkable));
        }

        public void AddTerrain(Terrain t) {
            TerrainDefinitions.Add(t.Definition, t);
        }

        public void AddFeature(Feature feature) {
            features.Add(feature);
            feature.Level = this;
            feature.TransparencyChanged += FeatureTransparencyChanged;
            feature.WalkableChanged += FeatureWalkableChanged;
            Fov.setProperties(feature.Position.X, feature.Position.Y, feature.Transparent, feature.Walkable);            
        }

        void FeatureWalkableChanged(object sender, EventArgs<bool> e) {
            if (sender is Feature) {
                var feature = sender as Feature;
                Fov.setProperties(feature.Position.X, feature.Position.Y, feature.Transparent, feature.Walkable);
            }
        }

        void  FeatureTransparencyChanged(object sender, EventArgs<bool> e)
        {
            if (sender is Feature) {
                var feature = sender as Feature;
                Fov.setProperties(feature.Position.X, feature.Position.Y, feature.Transparent, feature.Walkable);
            }
        }

        public void AddActor(Actor actor) {
            actors.Add(actor);
        }

        public void AddItem(Item item) {
            items.Add(item);
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
