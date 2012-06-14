using System;
using System.Collections.Generic;
using DEngine.Core;
using DEngine.Extensions;
using libtcod;

namespace SKR.Universe {
    public class AssetsManager {
        private Dictionary<string, Tuple<char, Pigment>> assets;

        private static Pigment Brick = new Pigment(new Color(146, 68, 35), ColorPresets.Black);

        public AssetsManager() {
            assets = new Dictionary<string, Tuple<char, Pigment>>();

            assets.Add("player", new Tuple<char, Pigment>('@', new Pigment(ColorPresets.White, ColorPresets.Black)));
            assets.Add("npc", new Tuple<char, Pigment>('@', new Pigment(ColorPresets.GhostWhite, ColorPresets.Black)));
            assets.Add("Wall", new Tuple<char, Pigment>((char) TCODSpecialCharacter.DoubleCross, new Pigment(ColorPresets.Gray, ColorPresets.Black)));
            assets.Add("Trap", new Tuple<char, Pigment>('^', new Pigment(ColorPresets.Gray, ColorPresets.Black)));
            assets.Add("ClosedDoor", new Tuple<char, Pigment>((char) TCODSpecialCharacter.Division, new Pigment(ColorPresets.Gray, ColorPresets.Black)));
            assets.Add("OpenedDoor", new Tuple<char, Pigment>('/', new Pigment(ColorPresets.Gray, ColorPresets.Black)));
            assets.Add("Grass", new Tuple<char, Pigment>((char) TCODSpecialCharacter.Block1, new Pigment(ColorPresets.Green, ColorPresets.Brown)));
            assets.Add("StoneFloor", new Tuple<char, Pigment>((char) TCODSpecialCharacter.Block1, new Pigment(ColorPresets.SandyBrown, ColorPresets.Brown)));

            assets.Add("ROAD", new Tuple<char, Pigment>(' ', new Pigment(ColorPresets.SandyBrown, ColorPresets.DarkGray)));
            assets.Add("ROAD_DIVIDER", new Tuple<char, Pigment>('-', new Pigment(ColorPresets.Yellow, ColorPresets.DarkGray)));

            assets.Add("PAVEMENT", new Tuple<char, Pigment>((char) TCODSpecialCharacter.Block2, new Pigment(ColorPresets.DarkerGrey, ColorPresets.Gray)));
            assets.Add("FLOOR_WOOD_TWO", new Tuple<char, Pigment>((char) TCODSpecialCharacter.Block1, new Pigment(new Color(145, 104, 58), new Color(166, 120, 66))));
            assets.Add("GRASS_DARK", new Tuple<char, Pigment>((char)TCODSpecialCharacter.Block3, new Pigment(ColorPresets.Green, ColorPresets.DarkGreen)));
            assets.Add("CARPET_GREY", new Tuple<char, Pigment>((char) TCODSpecialCharacter.Block1, new Pigment(ColorPresets.LightSlateGray, ColorPresets.LightGray)));
            assets.Add("FLOOR_TILE", new Tuple<char, Pigment>((char)TCODSpecialCharacter.Block2, new Pigment(ColorPresets.White, ColorPresets.Black)));

            
            assets.Add("WALL_BRICK_DARK", new Tuple<char, Pigment>('#', Brick));
            assets.Add("WALL_BRICK_DARK_VERT", new Tuple<char, Pigment>((char) TCODSpecialCharacter.DoubleVertLine, Brick));
            assets.Add("WALL_BRICK_DARK_HORZ", new Tuple<char, Pigment>((char) TCODSpecialCharacter.DoubleHorzLine, Brick));
            assets.Add("WALL_BRICK_DARK_T_NORTH", new Tuple<char, Pigment>((char) TCODSpecialCharacter.DoubleTeeNorth, Brick));
            assets.Add("WALL_BRICK_DARK_T_SOUTH", new Tuple<char, Pigment>((char) TCODSpecialCharacter.DoubleTeeSouth, Brick));
            assets.Add("WALL_BRICK_DARK_T_EAST", new Tuple<char, Pigment>((char) TCODSpecialCharacter.DoubleTeeEast, Brick));
            assets.Add("WALL_BRICK_DARK_T_WEST", new Tuple<char, Pigment>((char) TCODSpecialCharacter.DoubleTeeWest, Brick));
            assets.Add("WALL_BRICK_DARK_NORTHEAST", new Tuple<char, Pigment>((char) TCODSpecialCharacter.DoubleNE, Brick));
            assets.Add("WALL_BRICK_DARK_NORTHWEST", new Tuple<char, Pigment>((char) TCODSpecialCharacter.DoubleNW, Brick));
            assets.Add("WALL_BRICK_DARK_SOUTHWEST", new Tuple<char, Pigment>((char) TCODSpecialCharacter.DoubleSW, Brick));
            assets.Add("WALL_BRICK_DARK_SOUTHEAST", new Tuple<char, Pigment>((char)TCODSpecialCharacter.DoubleSE, Brick));
            assets.Add("WALL_BRICK_DARK_CROSS", new Tuple<char, Pigment>((char) TCODSpecialCharacter.DoubleCross, Brick));
            assets.Add("COUNTER_WOOD_RED", new Tuple<char, Pigment>('n', new Pigment(ColorPresets.Red, ColorPresets.Black)));
            assets.Add("WALL_BRICK_DARK_DOOR_HORZ", new Tuple<char, Pigment>((char)TCODSpecialCharacter.HorzLine, new Pigment(Brick.Foreground.ScaleValue(0.5f), ColorPresets.Black)));
            assets.Add("WALL_BRICK_DARK_DOOR_VERT", new Tuple<char, Pigment>((char)TCODSpecialCharacter.VertLine, new Pigment(Brick.Foreground.ScaleValue(0.5f), ColorPresets.Black)));
            assets.Add("WINDOW_BRICK_DARK_HORZ", new Tuple<char, Pigment>((char)TCODSpecialCharacter.HorzLine, new Pigment(Brick.Foreground.ScaleSaturation(0.5f), ColorPresets.Black)));
            assets.Add("WINDOW_BRICK_DARK_VERT", new Tuple<char, Pigment>((char)TCODSpecialCharacter.VertLine, new Pigment(Brick.Foreground.ScaleSaturation(0.5f), ColorPresets.Black)));
            assets.Add("TOILET", new Tuple<char, Pigment>('p', new Pigment(ColorPresets.FloralWhite, ColorPresets.Black)));
            assets.Add("BATHROOMSINK", new Tuple<char, Pigment>('n', new Pigment(ColorPresets.FloralWhite, ColorPresets.Black)));
            assets.Add("SINK", new Tuple<char, Pigment>('n', new Pigment(ColorPresets.FloralWhite, ColorPresets.Black)));
            assets.Add("BATH", new Tuple<char, Pigment>('D', Brick));
            assets.Add("SHOWER", new Tuple<char, Pigment>('H', new Pigment(ColorPresets.FloralWhite, ColorPresets.Black)));
            assets.Add("TREE_SMALL", new Tuple<char, Pigment>((char) TCODSpecialCharacter.Spade, new Pigment(ColorPresets.Green, ColorPresets.Black)));
            assets.Add("BED_WOODEN", new Tuple<char, Pigment>('B', new Pigment(new Color(145, 104, 58), ColorPresets.Black)));
            assets.Add("SHELF_WOOD", new Tuple<char, Pigment>('n', new Pigment(new Color(145, 104, 58), ColorPresets.Black)));
            assets.Add("SHELF_METAL", new Tuple<char, Pigment>('n', new Pigment(ColorPresets.LightGray, ColorPresets.Black)));
            assets.Add("TELEVISION", new Tuple<char, Pigment>('T', new Pigment(ColorPresets.Blue, ColorPresets.Black)));
            assets.Add("FRIDGE", new Tuple<char, Pigment>('B', new Pigment(ColorPresets.LightGray, ColorPresets.Black)));
            assets.Add("DESK_WOODEN", new Tuple<char, Pigment>('m', new Pigment(new Color(145, 104, 58), ColorPresets.Black)));
            assets.Add("CHAIR_WOODEN", new Tuple<char, Pigment>('b', new Pigment(new Color(145, 104, 58), ColorPresets.Black)));
            assets.Add("CASH_REGISTER", new Tuple<char, Pigment>('c', new Pigment(ColorPresets.Red, ColorPresets.Black)));
            assets.Add("SOFA", new Tuple<char, Pigment>('w', new Pigment(ColorPresets.Green, ColorPresets.Black)));
            assets.Add("OVEN", new Tuple<char, Pigment>('n', new Pigment(ColorPresets.LightGray, ColorPresets.Black)));
            assets.Add("STAIR_WOODEN_UP", new Tuple<char, Pigment>('<', new Pigment(new Color(145, 104, 58), ColorPresets.Black)));
            assets.Add("STAIR_WOODEN_DOWN", new Tuple<char, Pigment>('>', new Pigment(new Color(145, 104, 58), ColorPresets.Black)));
            assets.Add("DOOR_GARAGE", new Tuple<char, Pigment>('N', new Pigment(ColorPresets.Red, ColorPresets.Black)));
            assets.Add("FENCE_WOODEN", new Tuple<char, Pigment>('#', new Pigment(ColorPresets.White, ColorPresets.Black)));
            assets.Add("LAMP_STANDARD", new Tuple<char, Pigment>('P', new Pigment(ColorPresets.Beige, ColorPresets.Black)));
            assets.Add("TABLE_WOODEN", new Tuple<char, Pigment>('m', new Pigment(new Color(145, 104, 58), ColorPresets.Black)));
            assets.Add("PLANTPOT_FIXED", new Tuple<char, Pigment>('t', new Pigment(ColorPresets.Green, ColorPresets.Black)));
        }

        public Tuple<char, Pigment> this[string asset] {
            get { return assets[asset]; }
        }
    }
}