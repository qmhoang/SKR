using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Core;
using DEngine.Extensions;
using DEngine.Utility;
using SKR.Universe.Entities.Actors;
using SKR.Universe.Location;
using libtcod;

namespace SKR.Universe.Factories {
    public abstract class FeatureFactory : Factory<string, Feature> {
        
    }
    public class SourceFeatureFactory : FeatureFactory {
//        private static Dictionary<string, int> cells =
//                        new Dictionary<string, Tile>
//                    {
//                        {TileEnum.Unused.ToString(), },
//
//                        {TileEnum.WoodFloor, new Tile(TileEnum.WoodFloor, true, true, 0.0)},
//                        {TileEnum.Grass, new Tile(TileEnum.Grass, true, true, 0.0)},
//
//                        {TileEnum.Wall, new Tile(TileEnum.Wall, false, false, 0.0)},
//                        {TileEnum.HorizWall, new Tile(TileEnum.HorizWall, false, false, 0.0)},
//                        {TileEnum.VertWall, new Tile(TileEnum.VertWall, false, false, 0.0)},
//
//                        {TileEnum.NEWall, new Tile(TileEnum.NEWall, false, false, 0.0)},
//                        {TileEnum.NWWall, new Tile(TileEnum.NWWall, false, false, 0.0)},
//                        {TileEnum.SEWall, new Tile(TileEnum.SEWall, false, false, 0.0)},
//                        {TileEnum.SWWall, new Tile(TileEnum.SWWall, false, false, 0.0)},
//
//                        {TileEnum.TWallE, new Tile(TileEnum.TWallE, false, false, 0.0)},
//                        {TileEnum.TWallW, new Tile(TileEnum.TWallW, false, false, 0.0)},
//                        {TileEnum.TWallS, new Tile(TileEnum.TWallS, false, false, 0.0)},
//                        {TileEnum.TWallN, new Tile(TileEnum.TWallN, false, false, 0.0)},  
//
//                    };

        public override Feature Construct(string key) {
            switch (key) {
                case "BlockWall":
                    return new Feature(key, "Wall");
//                case "TestTrap":
//                    return new Feature(key, "Trap").Add(new AnotherWalk(a => a.World.InsertMessage("The tile squeaks")));
                case "Door":
                    return new Feature(key, "OpenedDoor").Add(new ActiveFeatureComponent("Open door", delegate(Feature f, Actor user)
                                                                                                    {
                                                                                                        f.Asset = "OpenedDoor";
                                                                                                        f.Transparent = f.Walkable = true;
                                                                                                    }),
                                                              new ActiveFeatureComponent("Close door", delegate(Feature f, Actor user)
                                                                                                     {
                                                                                                         f.Asset = "ClosedDoor";
                                                                                                         f.Transparent = f.Walkable = false;
                                                                                                     }));
            }

            return null;
        }
    }
}
