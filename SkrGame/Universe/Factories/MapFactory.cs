using System;
using System.Collections.Generic;
using System.Linq;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Locations;
using Level = SkrGame.Universe.Locations.Level;

namespace SkrGame.Universe.Factories {
//	public abstract class MapFactory : Factory<string, Level> {}

	public class MapFactory {
		private List<Terrain> terrainDefinitions;
		private EntityFactory ef;
		private EntityManager em;
		private World world;

		public MapFactory(World world) {
			terrainDefinitions = new List<Terrain>();
			this.world = world;
			this.ef = world.EntityFactory;
			this.em = world.EntityManager;

			terrainDefinitions.Add(new Terrain("Grass", "Grass", true, true, 1.0));
			terrainDefinitions.Add(new Terrain("StoneFloor", "StoneFloor", true, true, 1.0));
			terrainDefinitions.Add(new Terrain("ROAD", "ROAD", true, true, 1.0));
			terrainDefinitions.Add(new Terrain("ROAD_DIVIDER", "ROAD_DIVIDER", true, true, 1.0));
			terrainDefinitions.Add(new Terrain("PAVEMENT", "PAVEMENT", true, true, 1.0));
			terrainDefinitions.Add(new Terrain("FLOOR_WOOD_TWO", "FLOOR_WOOD_TWO", true, true, 1.0));
			terrainDefinitions.Add(new Terrain("GRASS_DARK", "GRASS_DARK", true, true, 1.0));
			terrainDefinitions.Add(new Terrain("CARPET_GREY", "CARPET_GREY", true, true, 1.0));
			terrainDefinitions.Add(new Terrain("FLOOR_TILE", "FLOOR_TILE", true, true, 1.0));
			terrainDefinitions.Add(new Terrain("FLOOR_TILE_PATTERN_TWO", "FLOOR_TILE_PATTERN_TWO", true, true, 1.0));

			terrainDefinitions.Add(new Terrain("GRASS_ONE", "GRASS_ONE", true, true, 1.0));
			terrainDefinitions.Add(new Terrain("FLOOR_TILE_YORKSTONE", "FLOOR_TILE_YORKSTONE", true, true, 1.0));
			terrainDefinitions.Add(new Terrain("CONCRETE_FLOOR", "CONCRETE_FLOOR", true, true, 1.0));
			terrainDefinitions.Add(new Terrain("CONCRETE_CRACK_1", "CONCRETE_CRACK_1", true, true, 1.0));
		}

		public Level Construct(string identifier) {
			switch (identifier) {
					#region TestMap

				case "TestMap":
					return FromString(
							new Dictionary<char, Tuple<string, string>>
							{
									{'#', new Tuple<string, string>("StoneFloor", "WALL_BRICK_DARK")},
									{'.', new Tuple<string, string>("StoneFloor", null)},
									{'/', new Tuple<string, string>("StoneFloor", "Door")},
									{'+', new Tuple<string, string>("StoneFloor", "Window")},
									{'^', new Tuple<string, string>("StoneFloor", "LockedDoor")},
									{'z', new Tuple<string, string>("StoneFloor", "testuse")},
							},
							"...................................................",
							"....../...z........................................",
							"...................................................",
							"......+............................................",
							"...................................................",
							"......^............................................",
							"...................................................",
							"...................................................",
							"...................................................");

					#endregion

					#region TestHouse

				case "TestHouse":
					return FromString(new Dictionary<char, Tuple<string, string>>()
					                  {
					                  		{'R', new Tuple<string, string>("ROAD_DIVIDER", null)},
					                  		{'r', new Tuple<string, string>("ROAD", null)},
					                  		{'p', new Tuple<string, string>("PAVEMENT", null)},
					                  		{'#', new Tuple<string, string>("FLOOR_WOOD_TWO", "WALL_BRICK_DARK")},
					                  		{'=', new Tuple<string, string>("FLOOR_WOOD_TWO", "COUNTER_WOOD_RED")},
					                  		{',', new Tuple<string, string>("FLOOR_WOOD_TWO", null)},
					                  		{'.', new Tuple<string, string>("FLOOR_TILE", null)},
					                  		{'+', new Tuple<string, string>("FLOOR_WOOD_TWO", "SINK")},
					                  		{'D', new Tuple<string, string>("FLOOR_WOOD_TWO", "WALL_BRICK_DARK_DOOR_VERT")},
					                  		{'d', new Tuple<string, string>("FLOOR_WOOD_TWO", "WALL_BRICK_DARK_DOOR_HORZ")},
					                  		{'O', new Tuple<string, string>("FLOOR_WOOD_TWO", "WINDOW_BRICK_DARK_VERT")},
					                  		{'o', new Tuple<string, string>("FLOOR_WOOD_TWO", "WINDOW_BRICK_DARK_HORZ")},
					                  		{'W', new Tuple<string, string>("FLOOR_TILE", "TOILET")},
					                  		{'~', new Tuple<string, string>("GRASS_DARK", null)},
					                  		{'N', new Tuple<string, string>("FLOOR_TILE", "BATHROOMSINK")},
					                  		{'H', new Tuple<string, string>("FLOOR_TILE", "SHOWER")},
					                  		{'C', new Tuple<string, string>("FLOOR_WOOD_TWO", "CHAIR_WOODEN")},
					                  		{'T', new Tuple<string, string>("FLOOR_WOOD_TWO", "TABLE_WOODEN")},
					                  		{'B', new Tuple<string, string>("FLOOR_WOOD_TWO", "BED_WOODEN")},
					                  		{'b', new Tuple<string, string>("FLOOR_WOOD_TWO", "BED_WOODEN")},
					                  		{'E', new Tuple<string, string>("FLOOR_WOOD_TWO", "BED_WOODEN")},
					                  		{'e', new Tuple<string, string>("FLOOR_WOOD_TWO", "BED_WOODEN")},
					                  		{'/', new Tuple<string, string>("FLOOR_WOOD_TWO", "SHELF_WOOD")},
					                  		{'m', new Tuple<string, string>("FLOOR_WOOD_TWO", "SHELF_METAL")},
					                  		{'x', new Tuple<string, string>("FLOOR_WOOD_TWO", "TELEVISION")},
					                  		{'X', new Tuple<string, string>("FLOOR_WOOD_TWO", "TELEVISION")},
					                  		{'F', new Tuple<string, string>("FLOOR_WOOD_TWO", "FRIDGE")},
					                  		{'S', new Tuple<string, string>("FLOOR_WOOD_TWO", "SOFA")},
					                  		{'s', new Tuple<string, string>("FLOOR_WOOD_TWO", "SOFA")},
					                  		{'<', new Tuple<string, string>("FLOOR_WOOD_TWO", "STAIR_WOODEN_UP")},
					                  		{'>', new Tuple<string, string>("FLOOR_WOOD_TWO", "STAIR_WOODEN_DOWN")},
					                  		{'A', new Tuple<string, string>("FLOOR_WOOD_TWO", "BATH")},
					                  		{'a', new Tuple<string, string>("FLOOR_WOOD_TWO", "BATH")},
					                  		{'G', new Tuple<string, string>("FLOOR_WOOD_TWO", "DOOR_GARAGE")},
					                  		{'u', new Tuple<string, string>("FLOOR_WOOD_TWO", "DESK_WOODEN")},
					                  		{'-', new Tuple<string, string>("GRASS_DARK", "FENCE_WOODEN")},
					                  		{'t', new Tuple<string, string>("FLOOR_WOOD_TWO", "LAMP_STANDARD")},
					                  		{'v', new Tuple<string, string>("FLOOR_WOOD_TWO", "PLANTPOT_FIXED")},
					                  		{'0', new Tuple<string, string>("FLOOR_WOOD_TWO", "WALL_BRICK_DARK_HORZ")},
					                  		{'1', new Tuple<string, string>("FLOOR_WOOD_TWO", "WALL_BRICK_DARK_SOUTHWEST")},
					                  		{'8', new Tuple<string, string>("FLOOR_WOOD_TWO", "WALL_BRICK_DARK_T_SOUTH")},
					                  		{'3', new Tuple<string, string>("FLOOR_WOOD_TWO", "WALL_BRICK_DARK_SOUTHEAST")},
					                  		{'6', new Tuple<string, string>("FLOOR_WOOD_TWO", "WALL_BRICK_DARK_T_WEST")},
					                  		{'5', new Tuple<string, string>("FLOOR_WOOD_TWO", "WALL_BRICK_DARK_VERT")},
					                  		{'4', new Tuple<string, string>("FLOOR_WOOD_TWO", "WALL_BRICK_DARK_T_EAST")},
					                  		{'7', new Tuple<string, string>("FLOOR_WOOD_TWO", "WALL_BRICK_DARK_NORTHWEST")},
					                  		{'2', new Tuple<string, string>("FLOOR_WOOD_TWO", "WALL_BRICK_DARK_T_NORTH")},
					                  		{'9', new Tuple<string, string>("FLOOR_WOOD_TWO", "WALL_BRICK_DARK_NORTHEAST")},
					                  },
					                  "rrrrrrrrrrrrrrrrrrrrrrrrrrrrr",
					                  "rrrrrrrrrrrrrrrrrrrrrrrrrrrrr",
					                  "RrRrRrRrRrRrRrRrRrRrRrRrRrRrR",
					                  "rrrrrrrrrrrrrrrrrrrrrrrrrrrrr",
					                  "rrrrrrrrrrrrrrrrrrrrrrrrrrrrr",
					                  "ppppppppppppppppppppppppppppp",
					                  "ppppppppppppppppppppppppppppp",
					                  "pp------------p------------pp",
					                  "pp-~~~~~~~~~~~p~~~~~~~~~~~-pp",
					                  "pp-~~~~~~~~~~~p~~~~~~~~~~~-pp",
					                  "pp--~~70oo09ppppp70oo09~~--pp",
					                  "pp70o03,,,,180d083,,,,10o09pp",
					                  "pp5,,,,,,,,,5,,,5,,,,,X,,,5pp",
					                  "ppO,,,,,,,,,5,,,5,,,,,,,,,Opp",
					                  "ppOCTC,,,,,,D,,,D,,,,,,,,,Opp",
					                  "pp5v,,,,,,,,5,,,5Ss,,,,,,t5pp",
					                  "pp50000000003,,,10000008003pp",
					                  "pp5+,,,,t,,,,,,,,,,,,,<5ppppp",
					                  "ppO=,,,,,,,,,,,,,,,,,,,5ppppp",
					                  "ppO=,,,,,,,,,,,,,,7000029pp~~",
					                  "pp5F,,,,,,,,,,,,,,D....A5pp~~",
					                  "pp1000o000o000o0009....AOpp~~",
					                  "pppppppppppppppppp5WNv..5pp~~",
					                  "~ppppppppppppppppp1000003pp~~",
					                  "~~~~~~~~~~~~~~~pppppppppppp~~",
					                  "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~",
									  "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~",
									  "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

					#endregion

				case "TestMotel":
					return FromString(new Dictionary<char, Tuple<string, string>>()
					                  {
					                  		{'p', new Tuple<string, string>("PAVEMENT", null)},
					                  		{'8', new Tuple<string, string>("FLOOR_WOOD_TWO", "WALL_BRICK_DARK")},
					                  		{'#', new Tuple<string, string>("FLOOR_WOOD_TWO", "WALL_DRY")},
					                  		{'=', new Tuple<string, string>("FLOOR_WOOD_TWO", "COUNTER_WOOD_RED")},
					                  		{',', new Tuple<string, string>("FLOOR_WOOD_TWO", null)},
					                  		{'.', new Tuple<string, string>("FLOOR_TILE_YORKSTONE", null)},
					                  		{';', new Tuple<string, string>("CARPET_GREY", null)},
					                  		{'c', new Tuple<string, string>("CONCRETE_FLOOR", null)},
					                  		{'x', new Tuple<string, string>("CONCRETE_CRACK_1", null)},
					                  		{'+', new Tuple<string, string>("CARPET_GREY", "SINK")},
					                  		{'Q', new Tuple<string, string>("CARPET_GREY", "WALL_BRICK_DARK_DOOR_HORZ")},
					                  		{'q', new Tuple<string, string>("CARPET_GREY", "WALL_BRICK_DARK_DOOR_VERT")},
					                  		{'D', new Tuple<string, string>("CARPET_GREY", "DOOR_APART_1_HORZ")},
					                  		{'d', new Tuple<string, string>("CARPET_GREY", "DOOR_APART_1_VERT")},
					                  		{'O', new Tuple<string, string>("FLOOR_WOOD_TWO", "WINDOW_HOUSE_HORZ")},
					                  		{'o', new Tuple<string, string>("FLOOR_WOOD_TWO", "WINDOW_HOUSE_VERT")},
					                  		{'W', new Tuple<string, string>("CARPET_GREY", "TOILET")},
					                  		{'~', new Tuple<string, string>("GRASS_ONE", null)},
					                  		{'N', new Tuple<string, string>("CARPET_GREY", "BATHROOMSINK")},
					                  		{'H', new Tuple<string, string>("CARPET_GREY", "SHOWER")},
					                  		{'C', new Tuple<string, string>("CARPET_GREY", "CHAIR_WOODEN")},
					                  		{'T', new Tuple<string, string>("GRASS_ONE", "TREE_SMALL")},
					                  		{'B', new Tuple<string, string>("CARPET_GREY", "BED_WOODEN")},
					                  		{'b', new Tuple<string, string>("CARPET_GREY", "BED_WOODEN")},
					                  		{'E', new Tuple<string, string>("CARPET_GREY", "BED_WOODEN")},
					                  		{'e', new Tuple<string, string>("CARPET_GREY", "BED_WOODEN")},
					                  		{'/', new Tuple<string, string>("CARPET_GREY", "SHELF_WOOD")},
					                  		{'%', new Tuple<string, string>("CARPET_GREY", "SHELF_METAL")},
					                  		{'X', new Tuple<string, string>("CARPET_GREY", "TELEVISION")},
					                  		{'F', new Tuple<string, string>("FLOOR_WOOD_TWO", "FRIDGE")},
					                  		{'u', new Tuple<string, string>("CARPET_GREY", "DESK_WOODEN")},
					                  		{'?', new Tuple<string, string>("CARPET_GREY", "CASH_REGISTER")},
					                  		{']', new Tuple<string, string>("CARPET_GREY", "SAFE_SIMPLE")},
					                  },
					                  "ppppppppppppppppppppppppppppppppppppppppppp",
					                  "p888888888888OO88888OO88888OO88888OO888~~~p",
					                  "p8Bb;#HW#+#;;;;;E#E;;;;;#;;;;;E#E;;;;;8~~~p",
					                  "p8;;;#.N#.#uC;;;e#e;;;Cu#uC;;;e#e;;;Cu8~~Tp",
					                  "po;;;#..#D#;;;;;;#;;;;;;#;;;;;;#;;;;;;8~~~p",
					                  "po;;;#D##,#;;#########;;#;;#########;;8~~~p",
					                  "p8;C;;;;#,#;;d..N#N..d;;#;;d..N#N..d;;8T~~p",
					                  "p8;u;;;;d,#;;#.WH#HW.#;;#;;#.WH#HW.#;;8~~~p",
					                  "p8#######,#D###########D#D###########D8~~~p",
					                  "p8Bb;#HW#,,,,,,,,,,,,,,,,,,,,,,,,,,,,,o~T~p",
									  "p8;;;#.N#,88OO888OO888OO888OO88OO88OO88~~~p",
					                  "po;;;#..#,8~~~T~~~~~T~~~~~~~~T~~~~~~~~T~~~p",
					                  "po;;;#D##,occccxccccccccccccccccccccccccccp",
					                  "p8;C;;;;#,occccccccccccccccccccccccccxccccp",
					                  "p8;u;;;;d,8cc~~~~cccccccccxcccccccccccccccp",
					                  "p88888888Q8cc~TT~ccccccxcccccccxccccccccccp",
					                  "pcccccccccccc~~~~cccccccccccccxccccccxccccp",
					                  "pcccccccccccc~~~~ccccccxccccccccccccccccccp",
					                  "p88O8888O88cc~TT~cccccccccccccccccccccccccp",
					                  "p8WN.H#,,,qcc~~~~cccxcccccccccccccccccccccp",
					                  "p8....#,?=8cccccccccccccccccccccccccccccccp",
					                  "p8##D##,,+8ccccccccccccccccccccccxccccccccp",
					                  "p8Bb;X#,,Focccccccccccccccccccccccccccccccp",
					                  "po;;;;#,,,8ccccccccccccxccccccccccccccccccp",
					                  "p8];;;d,,,8ccxccccccccccccccccccccccccxcccp",
					                  "p88O8O88O88ccccccccxccccccccccccccccccccccp",
					                  "ppppppppppppppppppppppppppppppppppppppppppp");
				default:
					throw new ArgumentException();
			}			
		}

		private Level FromString(Dictionary<char, Tuple<string, string>> charIdentifiers, params string[] definition) {
			int width = definition[0].Length;
			int height = definition.Count();

			var map = new Level(new Size(width, height), world, "Grass", terrainDefinitions);

			for (int y = 0; y < height; y++) {
				var s = definition[y];

				if (s.Length != width)
					throw new ArgumentException("Not all the rows are of the same width");

				for (int x = 0; x < s.Length; x++) {
					map.SetTerrain(x, y, charIdentifiers[s[x]].Item1);
					if (!String.IsNullOrEmpty(charIdentifiers[s[x]].Item2)) {
						var feature = ef.Get(charIdentifiers[s[x]].Item2);

						em.Create(feature).Add(new GameObject(x, y, map));
											
					}
				}
			}			
			return map;
		}	
	}
}