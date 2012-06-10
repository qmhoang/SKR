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
        private readonly Dictionary<RefId, TCODImage> images =
                new Dictionary<RefId, TCODImage>
                    {
                            {new RefId("player"), new TCODImage('@', new Pigment(ColorPresets.White, ColorPresets.Black))},
                            {new RefId("human"), new TCODImage('@', new Pigment(ColorPresets.LightGray, ColorPresets.Black))},
                            {new RefId("largeknife"), new TCODImage('(', new Pigment(ColorPresets.Gray, ColorPresets.Black))},
                            {new RefId("axe"), new TCODImage('(', new Pigment(ColorPresets.Gray, ColorPresets.Black))},
                            {new RefId("brassknuckles"), new TCODImage('(', new Pigment(ColorPresets.Gray, ColorPresets.Black))},

                            
                            {new RefId(TileEnum.Unused.ToString()), new TCODImage((char) TCODSpecialCharacter.Block1, new Pigment(ColorPresets.White, ColorPresets.Black))},

                            {new RefId(TileEnum.WoodFloor.ToString()), new TCODImage(' ', new Pigment(ColorPresets.Black, ColorPresets.SandyBrown))},
                            {new RefId(TileEnum.Grass.ToString()), new TCODImage((char) TCODSpecialCharacter.Block1, new Pigment(ColorPresets.DarkerGreen, ColorPresets.LightGreen))},

                            {new RefId(TileEnum.Wall.ToString()), new TCODImage('#', new Pigment(ColorPresets.DarkerGrey, ColorPresets.Black))},
                            {new RefId(TileEnum.HorizWall.ToString()), new TCODImage((char) TCODSpecialCharacter.HorzLine, new Pigment(ColorPresets.White, ColorPresets.Black))},
                            {new RefId(TileEnum.VertWall.ToString()), new TCODImage((char) TCODSpecialCharacter.VertLine, new Pigment(ColorPresets.White, ColorPresets.Black))},

                            {new RefId(TileEnum.NEWall.ToString()), new TCODImage((char) TCODSpecialCharacter.NE, new Pigment(ColorPresets.White, ColorPresets.Black))},
                            {new RefId(TileEnum.NWWall.ToString()), new TCODImage((char) TCODSpecialCharacter.NW, new Pigment(ColorPresets.White, ColorPresets.Black))},
                            {new RefId(TileEnum.SEWall.ToString()), new TCODImage((char) TCODSpecialCharacter.SE, new Pigment(ColorPresets.White, ColorPresets.Black))},
                            {new RefId(TileEnum.SWWall.ToString()), new TCODImage((char) TCODSpecialCharacter.SW, new Pigment(ColorPresets.White, ColorPresets.Black))},

                            {new RefId(TileEnum.TWallE.ToString()), new TCODImage((char) TCODSpecialCharacter.TeeEast, new Pigment(ColorPresets.White, ColorPresets.Black))},
                            {new RefId(TileEnum.TWallW.ToString()), new TCODImage((char) TCODSpecialCharacter.TeeWest, new Pigment(ColorPresets.White, ColorPresets.Black))},
                            {new RefId(TileEnum.TWallS.ToString()), new TCODImage((char) TCODSpecialCharacter.TeeSouth, new Pigment(ColorPresets.White, ColorPresets.Black))},
                            {new RefId(TileEnum.TWallN.ToString()), new TCODImage((char) TCODSpecialCharacter.TeeNorth, new Pigment(ColorPresets.White, ColorPresets.Black))},


                    };

        public TCODImage Transform(IObject @object) {
            return images[@object.RefId];
        }
    }
}
