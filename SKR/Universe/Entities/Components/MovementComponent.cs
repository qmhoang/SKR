using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Core;
using DEngine.Entities;
using DEngine.Extensions;
using SKR.Universe.Location;
using libtcod;
using TCODImage = SKR.UI.Graphics.TCODImage;

namespace SKR.Universe.Entities.Components {
    public class TileComponent : EntityComponent {
        public enum TileEnum {
            Unused = 0,
            Grass,
            WoodFloor,
            Wall,
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

        public static readonly ComponentType ComponentType = new ComponentType("tileComponent");
        public override ComponentType Type {
            get { return ComponentType; }
        }

        public TileEnum Tile { get; set; }

        public TileComponent(TileEnum tile) {
            Tile = tile;
        }
    }
    public class TCODTileVisual : TCODVisualComponent {
        private readonly Dictionary<TileComponent.TileEnum, Tuple<char, Pigment>> images =
                new Dictionary<TileComponent.TileEnum, Tuple<char, Pigment>>
                    {

                            {TileComponent.TileEnum.Unused, new Tuple<char, Pigment>((char) TCODSpecialCharacter.Block1, new Pigment(ColorPresets.White, ColorPresets.Black))},

                            {TileComponent.TileEnum.WoodFloor, new Tuple<char, Pigment>(' ', new Pigment(ColorPresets.Black, ColorPresets.SandyBrown))},
                            {TileComponent.TileEnum.Grass, new Tuple<char, Pigment>((char) TCODSpecialCharacter.Block1, new Pigment(ColorPresets.DarkerGreen, ColorPresets.LightGreen))},

                            {TileComponent.TileEnum.Wall, new Tuple<char, Pigment>('#', new Pigment(ColorPresets.DarkerGrey, ColorPresets.Black))},
                            {TileComponent.TileEnum.HorizWall, new Tuple<char, Pigment>((char) TCODSpecialCharacter.HorzLine, new Pigment(ColorPresets.White, ColorPresets.Black))},
                            {TileComponent.TileEnum.VertWall, new Tuple<char, Pigment>((char) TCODSpecialCharacter.VertLine, new Pigment(ColorPresets.White, ColorPresets.Black))},

                            {TileComponent.TileEnum.NEWall, new Tuple<char, Pigment>((char) TCODSpecialCharacter.NE, new Pigment(ColorPresets.White, ColorPresets.Black))},
                            {TileComponent.TileEnum.NWWall, new Tuple<char, Pigment>((char) TCODSpecialCharacter.NW, new Pigment(ColorPresets.White, ColorPresets.Black))},
                            {TileComponent.TileEnum.SEWall, new Tuple<char, Pigment>((char) TCODSpecialCharacter.SE, new Pigment(ColorPresets.White, ColorPresets.Black))},
                            {TileComponent.TileEnum.SWWall, new Tuple<char, Pigment>((char) TCODSpecialCharacter.SW, new Pigment(ColorPresets.White, ColorPresets.Black))},

                            {TileComponent.TileEnum.TWallE, new Tuple<char, Pigment>((char) TCODSpecialCharacter.TeeEast, new Pigment(ColorPresets.White, ColorPresets.Black))},
                            {TileComponent.TileEnum.TWallW, new Tuple<char, Pigment>((char) TCODSpecialCharacter.TeeWest, new Pigment(ColorPresets.White, ColorPresets.Black))},
                            {TileComponent.TileEnum.TWallS, new Tuple<char, Pigment>((char) TCODSpecialCharacter.TeeSouth, new Pigment(ColorPresets.White, ColorPresets.Black))},
                            {TileComponent.TileEnum.TWallN, new Tuple<char, Pigment>((char) TCODSpecialCharacter.TeeNorth, new Pigment(ColorPresets.White, ColorPresets.Black))},


                    };

        private TileComponent.TileEnum Tile {
            get { return OwnerEntity.GetComponent<TileComponent>(TileComponent.ComponentType).Tile; }
        }

        public override char Ascii {
            get { return images[Tile].Item1; }
        }

        public override Pigment Color {
            get { return images[Tile].Item2; }
        }        
    }

    public abstract class TCODVisualComponent : EntityComponent {
        public static readonly ComponentType ComponentType = new ComponentType("tcodvisualComponent");
        public override ComponentType Type {
            get { return ComponentType; }
        }

        public abstract char Ascii { get; }
        public abstract Pigment Color { get; }
        public Point Location { get { return OwnerEntity.Position; } }
    }

    public class MovementComponent : EntityComponent {
        public static readonly ComponentType ComponentType = new ComponentType("moveComponent");

        public override ComponentType Type {
            get { return ComponentType; }
        }

        public virtual ActionResult Move(Point p) {
            return Move(p.X, p.Y);
        }

        public ActionResult Move(int dx, int dy) {
            throw new NotImplementedException();
        }
        public ActionResult Wait() {
            OwnerEntity.GetComponent<ActionPointComponent>(ActionPointComponent.ComponentType).ActionPoints -= World.DefaultTurnSpeed;            
            return ActionResult.Success;
        }

        public bool IsNear(int x, int y, int radius) {
            return IsNear(new Point(x, y), radius);
        }

        public bool IsNear(Point point, int radius) {
            return point.IsInCircle(OwnerEntity.Position, radius);
        }
    }
}
