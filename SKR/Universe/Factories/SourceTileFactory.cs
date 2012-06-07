using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Core;
using DEngine.Extensions;
using DEngine.Utility;
using SKR.Universe.Location;
using libtcod;

namespace SKR.Universe.Factories {
    public abstract class TileFactory : Factory<TileEnum, Tile> {
        
    }
    public class SourceTileFactory : TileFactory {
        private static Dictionary<TileEnum, Tile> cells =
                        new Dictionary<TileEnum, Tile>
                    {
                        {TileEnum.Unused, new Tile(TileEnum.Unused, false, false, 0.0)},

                        {TileEnum.WoodFloor, new Tile(TileEnum.WoodFloor, true, true, 0.0)},
                        {TileEnum.Grass, new Tile(TileEnum.Grass, true, true, 0.0)},

                        {TileEnum.Wall, new Tile(TileEnum.Wall, false, false, 0.0)},
                        {TileEnum.HorizWall, new Tile(TileEnum.HorizWall, false, false, 0.0)},
                        {TileEnum.VertWall, new Tile(TileEnum.VertWall, false, false, 0.0)},

                        {TileEnum.NEWall, new Tile(TileEnum.NEWall, false, false, 0.0)},
                        {TileEnum.NWWall, new Tile(TileEnum.NWWall, false, false, 0.0)},
                        {TileEnum.SEWall, new Tile(TileEnum.SEWall, false, false, 0.0)},
                        {TileEnum.SWWall, new Tile(TileEnum.SWWall, false, false, 0.0)},

                        {TileEnum.TWallE, new Tile(TileEnum.TWallE, false, false, 0.0)},
                        {TileEnum.TWallW, new Tile(TileEnum.TWallW, false, false, 0.0)},
                        {TileEnum.TWallS, new Tile(TileEnum.TWallS, false, false, 0.0)},
                        {TileEnum.TWallN, new Tile(TileEnum.TWallN, false, false, 0.0)},  

                    };

        public override Tile Construct(TileEnum type) {
            return cells[type].Copy();
        }
    }
}
