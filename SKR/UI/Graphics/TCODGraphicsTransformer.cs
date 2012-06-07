using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Actor.Components.Graphics;
using DEngine.Core;
using DEngine.Extensions;
using SKR.Universe.Location;
using libtcod;

namespace SKR.UI.Graphics {
    public class TCODGraphicsTransformer : IGraphicsTransformer<TCODImage> {
        //todo make faster, probably should have each class contain an image and is set 
        private readonly Dictionary<string, TCODImage> images =
                new Dictionary<string, TCODImage>
                    {
                            {"player", new TCODImage('@', new Pigment(ColorPresets.White, ColorPresets.Black))},
                            {"human", new TCODImage('@', new Pigment(ColorPresets.LightGray, ColorPresets.Black))},
                            {"largeknife", new TCODImage('(', new Pigment(ColorPresets.Gray, ColorPresets.Black))},
                            {"axe", new TCODImage('(', new Pigment(ColorPresets.Gray, ColorPresets.Black))},
                            {"brassknuckles", new TCODImage('(', new Pigment(ColorPresets.Gray, ColorPresets.Black))},

                            
                            {TileEnum.Unused.ToString(), new TCODImage((char) TCODSpecialCharacter.Block1, new Pigment(ColorPresets.White, ColorPresets.Black))},

                            {TileEnum.WoodFloor.ToString(), new TCODImage(' ', new Pigment(ColorPresets.Black, ColorPresets.SandyBrown))},
                            {TileEnum.Grass.ToString(), new TCODImage((char) TCODSpecialCharacter.Block1, new Pigment(ColorPresets.DarkerGreen, ColorPresets.LightGreen))},

                            {TileEnum.Wall.ToString(), new TCODImage('#', new Pigment(ColorPresets.DarkerGrey, ColorPresets.Black))},
                            {TileEnum.HorizWall.ToString(), new TCODImage((char) TCODSpecialCharacter.HorzLine, new Pigment(ColorPresets.White, ColorPresets.Black))},
                            {TileEnum.VertWall.ToString(), new TCODImage((char) TCODSpecialCharacter.VertLine, new Pigment(ColorPresets.White, ColorPresets.Black))},

                            {TileEnum.NEWall.ToString(), new TCODImage((char) TCODSpecialCharacter.NE, new Pigment(ColorPresets.White, ColorPresets.Black))},
                            {TileEnum.NWWall.ToString(), new TCODImage((char) TCODSpecialCharacter.NW, new Pigment(ColorPresets.White, ColorPresets.Black))},
                            {TileEnum.SEWall.ToString(), new TCODImage((char) TCODSpecialCharacter.SE, new Pigment(ColorPresets.White, ColorPresets.Black))},
                            {TileEnum.SWWall.ToString(), new TCODImage((char) TCODSpecialCharacter.SW, new Pigment(ColorPresets.White, ColorPresets.Black))},

                            {TileEnum.TWallE.ToString(), new TCODImage((char) TCODSpecialCharacter.TeeEast, new Pigment(ColorPresets.White, ColorPresets.Black))},
                            {TileEnum.TWallW.ToString(), new TCODImage((char) TCODSpecialCharacter.TeeWest, new Pigment(ColorPresets.White, ColorPresets.Black))},
                            {TileEnum.TWallS.ToString(), new TCODImage((char) TCODSpecialCharacter.TeeSouth, new Pigment(ColorPresets.White, ColorPresets.Black))},
                            {TileEnum.TWallN.ToString(), new TCODImage((char) TCODSpecialCharacter.TeeNorth, new Pigment(ColorPresets.White, ColorPresets.Black))},


                    };

        public TCODImage Transform(IObject @object) {
            return images[@object.RefId];
        }
    }
}
