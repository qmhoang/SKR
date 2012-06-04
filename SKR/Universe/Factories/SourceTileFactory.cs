using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Core;
using DEngine.Extensions;
using SKR.Universe.Location;
using libtcod;

namespace SKR.Universe.Factories {
    public class SourceTileFactory : Factory<TileEnum, Tile> {
        private static Dictionary<TileEnum, Tile> cells =
            new Dictionary<TileEnum, Tile>
                    {
                        {TileEnum.Unused, new Tile((char) TCODSpecialCharacter.Block1, false, false, 0.0, new Pigment(ColorPresets.White, ColorPresets.Black))},

                        {TileEnum.WoodFloor, new Tile(' ', true, true, 0.0, new Pigment(ColorPresets.Black, ColorPresets.Black))},
                        {TileEnum.Grass, new Tile((char) TCODSpecialCharacter.Block1, true, true, 0.0, new Pigment(ColorPresets.DarkerGreen, ColorPresets.LightGreen))},

                        {TileEnum.HorizWall, new Tile((char) TCODSpecialCharacter.HorzLine, false, false, 0.0, new Pigment(ColorPresets.White, ColorPresets.Black))},
                        {TileEnum.VertWall, new Tile((char) TCODSpecialCharacter.VertLine, false, false, 0.0, new Pigment(ColorPresets.White, ColorPresets.Black))},

                        {TileEnum.NEWall, new Tile((char) TCODSpecialCharacter.NE, false, false, 0.0, new Pigment(ColorPresets.White, ColorPresets.Black))},
                        {TileEnum.NWWall, new Tile((char) TCODSpecialCharacter.NW, false, false, 0.0, new Pigment(ColorPresets.White, ColorPresets.Black))},
                        {TileEnum.SEWall, new Tile((char) TCODSpecialCharacter.SE, false, false, 0.0, new Pigment(ColorPresets.White, ColorPresets.Black))},
                        {TileEnum.SWWall, new Tile((char) TCODSpecialCharacter.SW, false, false, 0.0, new Pigment(ColorPresets.White, ColorPresets.Black))},

                        {TileEnum.TWallE, new Tile((char) TCODSpecialCharacter.TeeEast, false, false, 0.0, new Pigment(ColorPresets.White, ColorPresets.Black))},
                        {TileEnum.TWallW, new Tile((char) TCODSpecialCharacter.TeeWest, false, false, 0.0, new Pigment(ColorPresets.White, ColorPresets.Black))},
                        {TileEnum.TWallS, new Tile((char) TCODSpecialCharacter.TeeSouth, false, false, 0.0, new Pigment(ColorPresets.White, ColorPresets.Black))},
                        {TileEnum.TWallN, new Tile((char) TCODSpecialCharacter.TeeNorth, false, false, 0.0, new Pigment(ColorPresets.White, ColorPresets.Black))},  

                    };

        public override Tile Construct(TileEnum type) {
            return cells[type].Copy();
        }
    }
}
