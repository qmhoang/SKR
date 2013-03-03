using System;
using System.Collections.Generic;
using Ogui.Core;
using libtcod;

namespace SKR.UI {
	public class AssetsManager {
		private Dictionary<string, Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>> assets;

		private static Tuple<TCODColor, TCODColor, TCODBackgroundFlag> Brick = new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.Firebrick.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Set);

		public AssetsManager() {
			assets = new Dictionary<string, Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>>();

			assets.Add("player",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('@',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.White.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("npc",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('@',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.GhostWhite.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("Wall",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>((char) TCODSpecialCharacter.DoubleCross,
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.Gray.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("Trap",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('^', new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.Gray.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("ClosedDoor",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>((char) TCODSpecialCharacter.Division,
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.Gray.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("OpenedDoor",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('/', new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.Gray.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("Grass",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>((char) TCODSpecialCharacter.Block1,
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.Green.TCODColor, ColorPresets.Brown.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("StoneFloor",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>((char) TCODSpecialCharacter.Block1,
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.SandyBrown.TCODColor, ColorPresets.Brown.TCODColor, TCODBackgroundFlag.Alpha)));

			assets.Add("OpenedWindow",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('/', new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.Gray.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("ClosedWindow",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('\\',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.Gray.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));

			assets.Add("ROAD", new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>(' ', new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.SandyBrown.TCODColor, ColorPresets.DarkGray.TCODColor, TCODBackgroundFlag.Set)));
			assets.Add("ROAD_DIVIDER", new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('-', new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.Yellow.TCODColor, ColorPresets.DarkGray.TCODColor, TCODBackgroundFlag.Set)));

			assets.Add("PAVEMENT",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>((char) TCODSpecialCharacter.Block2,
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.DarkerGrey.TCODColor, ColorPresets.Gray.TCODColor, TCODBackgroundFlag.Set)));
			assets.Add("FLOOR_WOOD_TWO",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>((char) TCODSpecialCharacter.Block1,
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(new TCODColor(145, 104, 58), new TCODColor(166, 120, 66), TCODBackgroundFlag.Set)));
			assets.Add("GRASS_DARK",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>((char) TCODSpecialCharacter.Block3,
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.Green.TCODColor, ColorPresets.DarkGreen.TCODColor, TCODBackgroundFlag.Set)));
			assets.Add("CARPET_GREY",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>((char) TCODSpecialCharacter.Block1,
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.LightGray.TCODColor, ColorPresets.Gray.TCODColor, TCODBackgroundFlag.Set)));
			assets.Add("FLOOR_TILE",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>((char) TCODSpecialCharacter.Block2,
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.White.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Set)));
			assets.Add("FLOOR_TILE_YORKSTONE",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>((char) TCODSpecialCharacter.Block1,
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.White.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Set)));
			assets.Add("CONCRETE_FLOOR", new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>(' ', new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.White.TCODColor, ColorPresets.DarkerGrey.TCODColor, TCODBackgroundFlag.Set)));
			assets.Add("CONCRETE_CRACK_1",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>((char) TCODSpecialCharacter.Block1,
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.White.TCODColor, ColorPresets.DarkerGrey.TCODColor, TCODBackgroundFlag.Set)));
			assets.Add("GRASS_ONE",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>((char) TCODSpecialCharacter.Block3,
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.DarkGreen.TCODColor, ColorPresets.Green.TCODColor, TCODBackgroundFlag.Set)));

			assets.Add("FEATURE",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('#',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.BurlyWood.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));

			assets.Add("WALL_BRICK_DARK", new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('#', Brick));
			assets.Add("WALL_BRICK_DARK_VERT", new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>((char) TCODSpecialCharacter.DoubleVertLine, Brick));
			assets.Add("WALL_BRICK_DARK_HORZ", new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>((char) TCODSpecialCharacter.DoubleHorzLine, Brick));
			assets.Add("WALL_BRICK_DARK_T_NORTH", new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>((char) TCODSpecialCharacter.DoubleTeeNorth, Brick));
			assets.Add("WALL_BRICK_DARK_T_SOUTH", new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>((char) TCODSpecialCharacter.DoubleTeeSouth, Brick));
			assets.Add("WALL_BRICK_DARK_T_EAST", new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>((char) TCODSpecialCharacter.DoubleTeeEast, Brick));
			assets.Add("WALL_BRICK_DARK_T_WEST", new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>((char) TCODSpecialCharacter.DoubleTeeWest, Brick));
			assets.Add("WALL_BRICK_DARK_NORTHEAST", new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>((char) TCODSpecialCharacter.DoubleNE, Brick));
			assets.Add("WALL_BRICK_DARK_NORTHWEST", new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>((char) TCODSpecialCharacter.DoubleNW, Brick));
			assets.Add("WALL_BRICK_DARK_SOUTHWEST", new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>((char) TCODSpecialCharacter.DoubleSW, Brick));
			assets.Add("WALL_BRICK_DARK_SOUTHEAST", new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>((char) TCODSpecialCharacter.DoubleSE, Brick));
			assets.Add("WALL_BRICK_DARK_CROSS", new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>((char) TCODSpecialCharacter.DoubleCross, Brick));
			assets.Add("WALL_BRICK_DARK_DOOR_HORZ",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>((char) TCODSpecialCharacter.HorzLine,
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.Firebrick.ScaleValue(0.5f).TCODColor, ColorPresets.Black.TCODColor,
			                                                                                                                                TCODBackgroundFlag.Alpha)));
			assets.Add("WALL_BRICK_DARK_DOOR_VERT",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>((char) TCODSpecialCharacter.VertLine,
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.Firebrick.ScaleValue(0.5f).TCODColor, ColorPresets.Black.TCODColor,
			                                                                                                                                TCODBackgroundFlag.Alpha)));

			assets.Add("WINDOW_BRICK_DARK_HORZ",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>((char) TCODSpecialCharacter.HorzLine,
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.Firebrick.ScaleSaturation(0.5f).TCODColor, ColorPresets.Black.TCODColor,
			                                                                                                                                TCODBackgroundFlag.Alpha)));
			assets.Add("WINDOW_BRICK_DARK_VERT",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>((char) TCODSpecialCharacter.VertLine,
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.Firebrick.ScaleSaturation(0.5f).TCODColor, ColorPresets.Black.TCODColor,
			                                                                                                                                TCODBackgroundFlag.Alpha)));

			assets.Add("COUNTER_WOOD_RED",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('n', new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.Red.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));

			assets.Add("TOILET",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('p',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.FloralWhite.TCODColor, ColorPresets.Black.TCODColor,
			                                                                                                                                TCODBackgroundFlag.Alpha)));
			assets.Add("BATHROOMSINK",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('n',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.FloralWhite.TCODColor, ColorPresets.Black.TCODColor,
			                                                                                                                                TCODBackgroundFlag.Alpha)));
			assets.Add("SINK",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('n',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.FloralWhite.TCODColor, ColorPresets.Black.TCODColor,
			                                                                                                                                TCODBackgroundFlag.Alpha)));
			assets.Add("BATH",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('D',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.FloralWhite.TCODColor, ColorPresets.Black.TCODColor,
			                                                                                                                                TCODBackgroundFlag.Alpha)));
			assets.Add("SHOWER",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('H',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.FloralWhite.TCODColor, ColorPresets.Black.TCODColor,
			                                                                                                                                TCODBackgroundFlag.Alpha)));
			assets.Add("TREE_SMALL",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>((char) TCODSpecialCharacter.Spade,
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.Green.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("BED_WOODEN",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('B',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(new TCODColor(145, 104, 58), ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("SHELF_WOOD",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('n',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(new TCODColor(145, 104, 58), ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("SHELF_METAL",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('n',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.LightGray.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("TELEVISION",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('T', new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.Blue.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("FRIDGE",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('B',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.LightGray.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("DESK_WOODEN",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('m',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(new TCODColor(145, 104, 58), ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("CHAIR_WOODEN",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('b',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(new TCODColor(145, 104, 58), ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("CASH_REGISTER",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('c', new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.Red.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("SOFA",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('w',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.Green.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("OVEN",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('n',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.LightGray.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("STAIR_WOODEN_UP",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('<',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(new TCODColor(145, 104, 58), ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("STAIR_WOODEN_DOWN",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('>',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(new TCODColor(145, 104, 58), ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("DOOR_GARAGE",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('N', new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.Red.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("FENCE_WOODEN",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('#',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.White.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("LAMP_STANDARD",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('P',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.Beige.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("TABLE_WOODEN",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('m',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(new TCODColor(145, 104, 58), ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("PLANTPOT_FIXED",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('t',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.Green.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));

			assets.Add("DOOR_APART_1_HORZ",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>((char) TCODSpecialCharacter.HorzLine,
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.White.ScaleValue(0.5f).TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Set)));
			assets.Add("DOOR_APART_1_VERT",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>((char) TCODSpecialCharacter.VertLine,
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.White.ScaleValue(0.5f).TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Set)));
			assets.Add("WINDOW_HOUSE_VERT",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('|', new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.White.ScaleSaturation(0.5f).TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Set)));
			assets.Add("WINDOW_HOUSE_HORZ",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('-', new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.White.ScaleSaturation(0.5f).TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Set)));

			assets.Add("SAFE_SIMPLE",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('+',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.DarkGray.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));

			assets.Add("WALL_DRY", new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('#', new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.White.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Set)));

			#region Items

			assets.Add("ITEM",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('!',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.SteelBlue.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("WEAPON",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('[',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.SteelBlue.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));

			assets.Add("LARGE_KNIFE",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('[',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.SteelBlue.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("SMALL_KNIFE",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('[',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.SteelBlue.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("AXE",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('[',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.SteelBlue.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("HATCHET",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('[',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.SteelBlue.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("BRASS_KNUCKLES",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('[',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.SteelBlue.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));

			assets.Add("GUN",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('[',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.SteelBlue.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("GLOCK17",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('[',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.SteelBlue.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("GLOCK22",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('[',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.SteelBlue.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));

			assets.Add("BULLET",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('.',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.Brass.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("MAGAZINE_GLOCK17",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('.',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.Brass.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("BULLET_9x19MM",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('.',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.Brass.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("MAGAZINE_GLOCK22",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('.',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.Brass.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("BULLET_.40S&W",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('.',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.Brass.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));

			assets.Add("SHIRT",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>(']',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.YellowGreen.TCODColor, ColorPresets.Black.TCODColor,
			                                                                                                                                TCODBackgroundFlag.Alpha)));
			assets.Add("FOOTBALL_SHOULDER_PADS",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>(']',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.LightGray.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));

			assets.Add("SHOES",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>('.',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.SaddleBrown.TCODColor, ColorPresets.Black.TCODColor,
			                                                                                                                                TCODBackgroundFlag.Alpha)));

			assets.Add("PANTS",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>(']',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.Khaki.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));
			assets.Add("JEANS",
			           new Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>>(']',
			                                                                            new Tuple<TCODColor, TCODColor, TCODBackgroundFlag>(ColorPresets.DarkerBlue.TCODColor, ColorPresets.Black.TCODColor, TCODBackgroundFlag.Alpha)));

			#endregion
		}

		public Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>> this[string asset] {
			get { return assets[asset]; }
		}
	}
}