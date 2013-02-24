using System;
using System.Collections.Generic;
using Ogui.Core;
using libtcod;

namespace SKR.UI {
	public class AssetsManager {
		private Dictionary<string, Tuple<char, Pigment>> assets;

		private static Pigment Brick = new Pigment(ColorPresets.Firebrick, ColorPresets.Black);

		public AssetsManager() {
			assets = new Dictionary<string, Tuple<char, Pigment>>();

			assets.Add("player", new Tuple<char, Pigment>('@', new Pigment(ColorPresets.White, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("npc", new Tuple<char, Pigment>('@', new Pigment(ColorPresets.GhostWhite, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("Wall", new Tuple<char, Pigment>((char)TCODSpecialCharacter.DoubleCross, new Pigment(ColorPresets.Gray, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("Trap", new Tuple<char, Pigment>('^', new Pigment(ColorPresets.Gray, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("ClosedDoor", new Tuple<char, Pigment>((char)TCODSpecialCharacter.Division, new Pigment(ColorPresets.Gray, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("OpenedDoor", new Tuple<char, Pigment>('/', new Pigment(ColorPresets.Gray, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("Grass", new Tuple<char, Pigment>((char)TCODSpecialCharacter.Block1, new Pigment(ColorPresets.Green, ColorPresets.Brown, TCODBackgroundFlag.Alpha)));
			assets.Add("StoneFloor", new Tuple<char, Pigment>((char)TCODSpecialCharacter.Block1, new Pigment(ColorPresets.SandyBrown, ColorPresets.Brown, TCODBackgroundFlag.Alpha)));

			assets.Add("OpenedWindow", new Tuple<char, Pigment>('/', new Pigment(ColorPresets.Gray, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("ClosedWindow", new Tuple<char, Pigment>('\\', new Pigment(ColorPresets.Gray, ColorPresets.Black, TCODBackgroundFlag.Alpha)));

			assets.Add("ROAD", new Tuple<char, Pigment>(' ', new Pigment(ColorPresets.SandyBrown, ColorPresets.DarkGray)));
			assets.Add("ROAD_DIVIDER", new Tuple<char, Pigment>('-', new Pigment(ColorPresets.Yellow, ColorPresets.DarkGray)));

			assets.Add("PAVEMENT", new Tuple<char, Pigment>((char) TCODSpecialCharacter.Block2, new Pigment(ColorPresets.DarkerGrey, ColorPresets.Gray)));
			assets.Add("FLOOR_WOOD_TWO", new Tuple<char, Pigment>((char) TCODSpecialCharacter.Block1, new Pigment(new Color(145, 104, 58), new Color(166, 120, 66))));
			assets.Add("GRASS_DARK", new Tuple<char, Pigment>((char) TCODSpecialCharacter.Block3, new Pigment(ColorPresets.Green, ColorPresets.DarkGreen)));
			assets.Add("CARPET_GREY", new Tuple<char, Pigment>((char)TCODSpecialCharacter.Block1, new Pigment(ColorPresets.LightGray, ColorPresets.Gray)));
			assets.Add("FLOOR_TILE", new Tuple<char, Pigment>((char) TCODSpecialCharacter.Block2, new Pigment(ColorPresets.White, ColorPresets.Black)));
			assets.Add("FLOOR_TILE_YORKSTONE", new Tuple<char, Pigment>((char) TCODSpecialCharacter.Block1, new Pigment(ColorPresets.White, ColorPresets.Black)));
			assets.Add("CONCRETE_FLOOR", new Tuple<char, Pigment>(' ', new Pigment(ColorPresets.White, ColorPresets.DarkerGrey)));
			assets.Add("CONCRETE_CRACK_1", new Tuple<char, Pigment>((char) TCODSpecialCharacter.Block1, new Pigment(ColorPresets.White, ColorPresets.DarkerGrey)));
			assets.Add("GRASS_ONE", new Tuple<char, Pigment>((char) TCODSpecialCharacter.Block3, new Pigment(ColorPresets.DarkGreen, ColorPresets.Green)));

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
			assets.Add("WALL_BRICK_DARK_SOUTHEAST", new Tuple<char, Pigment>((char) TCODSpecialCharacter.DoubleSE, Brick));
			assets.Add("WALL_BRICK_DARK_CROSS", new Tuple<char, Pigment>((char) TCODSpecialCharacter.DoubleCross, Brick));
			assets.Add("WALL_BRICK_DARK_DOOR_HORZ", new Tuple<char, Pigment>((char) TCODSpecialCharacter.HorzLine, new Pigment(Brick.Foreground.ScaleValue(0.5f), ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("WALL_BRICK_DARK_DOOR_VERT", new Tuple<char, Pigment>((char) TCODSpecialCharacter.VertLine, new Pigment(Brick.Foreground.ScaleValue(0.5f), ColorPresets.Black, TCODBackgroundFlag.Alpha)));

			assets.Add("WINDOW_BRICK_DARK_HORZ", new Tuple<char, Pigment>((char) TCODSpecialCharacter.HorzLine, new Pigment(Brick.Foreground.ScaleSaturation(0.5f), ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("WINDOW_BRICK_DARK_VERT", new Tuple<char, Pigment>((char) TCODSpecialCharacter.VertLine, new Pigment(Brick.Foreground.ScaleSaturation(0.5f), ColorPresets.Black, TCODBackgroundFlag.Alpha)));

			assets.Add("COUNTER_WOOD_RED", new Tuple<char, Pigment>('n', new Pigment(ColorPresets.Red, ColorPresets.Black, TCODBackgroundFlag.Alpha)));

			assets.Add("TOILET", new Tuple<char, Pigment>('p', new Pigment(ColorPresets.FloralWhite, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("BATHROOMSINK", new Tuple<char, Pigment>('n', new Pigment(ColorPresets.FloralWhite, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("SINK", new Tuple<char, Pigment>('n', new Pigment(ColorPresets.FloralWhite, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("BATH", new Tuple<char, Pigment>('D', new Pigment(ColorPresets.FloralWhite, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("SHOWER", new Tuple<char, Pigment>('H', new Pigment(ColorPresets.FloralWhite, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("TREE_SMALL", new Tuple<char, Pigment>((char)TCODSpecialCharacter.Spade, new Pigment(ColorPresets.Green, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("BED_WOODEN", new Tuple<char, Pigment>('B', new Pigment(new Color(145, 104, 58), ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("SHELF_WOOD", new Tuple<char, Pigment>('n', new Pigment(new Color(145, 104, 58), ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("SHELF_METAL", new Tuple<char, Pigment>('n', new Pigment(ColorPresets.LightGray, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("TELEVISION", new Tuple<char, Pigment>('T', new Pigment(ColorPresets.Blue, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("FRIDGE", new Tuple<char, Pigment>('B', new Pigment(ColorPresets.LightGray, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("DESK_WOODEN", new Tuple<char, Pigment>('m', new Pigment(new Color(145, 104, 58), ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("CHAIR_WOODEN", new Tuple<char, Pigment>('b', new Pigment(new Color(145, 104, 58), ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("CASH_REGISTER", new Tuple<char, Pigment>('c', new Pigment(ColorPresets.Red, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("SOFA", new Tuple<char, Pigment>('w', new Pigment(ColorPresets.Green, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("OVEN", new Tuple<char, Pigment>('n', new Pigment(ColorPresets.LightGray, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("STAIR_WOODEN_UP", new Tuple<char, Pigment>('<', new Pigment(new Color(145, 104, 58), ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("STAIR_WOODEN_DOWN", new Tuple<char, Pigment>('>', new Pigment(new Color(145, 104, 58), ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("DOOR_GARAGE", new Tuple<char, Pigment>('N', new Pigment(ColorPresets.Red, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("FENCE_WOODEN", new Tuple<char, Pigment>('#', new Pigment(ColorPresets.White, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("LAMP_STANDARD", new Tuple<char, Pigment>('P', new Pigment(ColorPresets.Beige, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("TABLE_WOODEN", new Tuple<char, Pigment>('m', new Pigment(new Color(145, 104, 58), ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("PLANTPOT_FIXED", new Tuple<char, Pigment>('t', new Pigment(ColorPresets.Green, ColorPresets.Black, TCODBackgroundFlag.Alpha)));

			assets.Add("DOOR_APART_1_HORZ", new Tuple<char, Pigment>((char)TCODSpecialCharacter.HorzLine, new Pigment(ColorPresets.White.ScaleValue(0.5f), ColorPresets.Black)));
			assets.Add("DOOR_APART_1_VERT", new Tuple<char, Pigment>((char)TCODSpecialCharacter.VertLine, new Pigment(ColorPresets.White.ScaleValue(0.5f), ColorPresets.Black)));
			assets.Add("WINDOW_HOUSE_VERT", new Tuple<char, Pigment>('|', new Pigment(ColorPresets.White.ScaleSaturation(0.5f), ColorPresets.Black)));
			assets.Add("WINDOW_HOUSE_HORZ", new Tuple<char, Pigment>('-', new Pigment(ColorPresets.White.ScaleSaturation(0.5f), ColorPresets.Black)));

			assets.Add("SAFE_SIMPLE", new Tuple<char, Pigment>('+', new Pigment(ColorPresets.DarkGray, ColorPresets.Black, TCODBackgroundFlag.Alpha)));

			assets.Add("WALL_DRY", new Tuple<char, Pigment>('#', new Pigment(ColorPresets.White, ColorPresets.Black)));

			#region Items
			assets.Add("ITEM", new Tuple<char, Pigment>('!', new Pigment(ColorPresets.SteelBlue, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("WEAPON", new Tuple<char, Pigment>('[', new Pigment(ColorPresets.SteelBlue, ColorPresets.Black, TCODBackgroundFlag.Alpha)));

			assets.Add("LARGE_KNIFE", new Tuple<char, Pigment>('[', new Pigment(ColorPresets.SteelBlue, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("SMALL_KNIFE", new Tuple<char, Pigment>('[', new Pigment(ColorPresets.SteelBlue, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("AXE", new Tuple<char, Pigment>('[', new Pigment(ColorPresets.SteelBlue, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("HATCHET", new Tuple<char, Pigment>('[', new Pigment(ColorPresets.SteelBlue, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("BRASS_KNUCKLES", new Tuple<char, Pigment>('[', new Pigment(ColorPresets.SteelBlue, ColorPresets.Black, TCODBackgroundFlag.Alpha)));

			assets.Add("GUN", new Tuple<char, Pigment>('[', new Pigment(ColorPresets.SteelBlue, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("GLOCK17", new Tuple<char, Pigment>('[', new Pigment(ColorPresets.SteelBlue, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("GLOCK22", new Tuple<char, Pigment>('[', new Pigment(ColorPresets.SteelBlue, ColorPresets.Black, TCODBackgroundFlag.Alpha)));

			assets.Add("BULLET", new Tuple<char, Pigment>('.', new Pigment(ColorPresets.Brass, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("MAGAZINE_GLOCK17", new Tuple<char, Pigment>('.', new Pigment(ColorPresets.Brass, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("BULLET_9x19MM", new Tuple<char, Pigment>('.', new Pigment(ColorPresets.Brass, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("MAGAZINE_GLOCK22", new Tuple<char, Pigment>('.', new Pigment(ColorPresets.Brass, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			assets.Add("BULLET_.40S&W", new Tuple<char, Pigment>('.', new Pigment(ColorPresets.Brass, ColorPresets.Black, TCODBackgroundFlag.Alpha)));

			assets.Add("FOOTBALL_SHOULDER_PADS", new Tuple<char, Pigment>(']', new Pigment(ColorPresets.LightGray, ColorPresets.Black, TCODBackgroundFlag.Alpha)));
			#endregion
		}

		public Tuple<char, Pigment> this[string asset] {
			get { return assets[asset]; }
		}
	}
}