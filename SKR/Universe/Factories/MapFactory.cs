﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Core;
using DEngine.Extensions;
using SKR.Universe.Location;
using libtcod;

namespace SKR.Universe.Factories {
    public abstract class MapFactory : Factory<string, Level> {
    }

    public class SourceMapFactory : MapFactory {        
        private List<Terrain> terrainDefinitions;
        private FeatureFactory featureFactory;

        public SourceMapFactory(FeatureFactory featureFactory) {                  
            terrainDefinitions = new List<Terrain>();
            this.featureFactory = featureFactory;

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
        }

        public override Level Construct(string identifier) {
            switch (identifier)
            {
                #region TestMap
                case "TestMap":
                    return FromString(
                                      new Dictionary<char, Tuple<string, string>>
                                          {
                                                  {'#', new Tuple<string, string>("StoneFloor", "BlockWall")},
                                                  {'.', new Tuple<string, string>("StoneFloor", null)},
                                                  {'/', new Tuple<string, string>("StoneFloor", "Door")},
                                                  {'+', new Tuple<string, string>("StoneFloor", null)},
                                                  {'^', new Tuple<string, string>("StoneFloor", null)},
                                          },
                                      "############################################################", 
                                      "#..........................................................#", 
                                      "#....../...................................................#", 
                                      "#..........................................................#", 
                                      "#......+...................................................#", 
                                      "#..........................................................#", 
                                      "#......^...................................................#", 
                                      "#..........................................................#", 
                                      "#..........................................................#", 
                                      "#..........................................................#", 
                                      "#..........................................................#", 
                                      "#..........................................................#", 
                                      "#..........................................................#", 
                                      "#..........................................................#", 
                                      "#..........................................................#", 
                                      "#..........................................................#", 
                                      "#..........................................................#", 
                                      "#..........................................................#", 
                                      "#..........................................................#", 
                                      "#..........................................................#", 
                                      "#..........................................................#", 
                                      "#..........................................................#", 
                                      "#..........................................................#", 
                                      "#..........................................................#", 
                                      "############################################################");
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
                                      "RRRRRRRRRRRRRRR",
                                      "rrrrrrrrrrrrrrr",
                                      "ppppppppppppppp",
                                      "p------p------p",
                                      "p-~~~~~p~~~~~-p",
                                      "p-~7o9ppp7o9~-p",
                                      "p703,18d83,109p",
                                      "p5,,,,5,5,,X,5p",
                                      "pOCTC,D,D,,,,Op",
                                      "p5v,,,5,5Ss,t5p",
                                      "p500003,100803p",
                                      "p5+==F,,,,<5ppp",
                                      "pO,,,,,,70029p~",
                                      "p5t,,,,,D..A5p~",
                                      "p10o00o09..AOp~",
                                      "pppppppp5WNv5p~",
                                      "~~~~~~~p10003p~",
                                      "~~~~~~~ppppppp~");
                #endregion
                case "TestMotel":
                    return FromString(new Dictionary<char, Tuple<string, string>>()
                                          {
                                                  {'p', new Tuple<string, string>("PAVEMENT", null)},
                                                  {'8', new Tuple<string, string>("WALL_BRICK_DARK", null)},
                                                  {'#', new Tuple<string, string>("WALL_DRY", null)},
                                                  {'=', new Tuple<string, string>("COUNTER_WOOD_RED", null)},
                                                  {',', new Tuple<string, string>("FLOOR_WOOD_TWO", null)},
                                                  {'.', new Tuple<string, string>("FLOOR_TILE_YORKSTONE", null)},
                                                  {';', new Tuple<string, string>("CARPET_GREY", null)},
                                                  {'c', new Tuple<string, string>("CONCRETE_FLOOR", null)},
                                                  {'x', new Tuple<string, string>("CONCRETE_CRACK_1", null)},
                                                  {'+', new Tuple<string, string>("CARPET_GREY", "SINK")},
                                                  {'D', new Tuple<string, string>("CARPET_GREY", "WALL_BRICK_DARK_DOOR")},
                                                  {'d', new Tuple<string, string>("WALL_DRY", "DOOR_APART_1")},
                                                  {'O', new Tuple<string, string>("WINDOW_HOUSE", "WALL_BRICK_DARK")},
                                                  {'W', new Tuple<string, string>("CARPET_GREY", "WC")},
                                                  {'~', new Tuple<string, string>("CARPET_GREY", "GRASS_ONE")},
                                                  {'N', new Tuple<string, string>("CARPET_GREY", "WHB")},
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
                                      "ppppppppppppppppppppppppppppppppppp",
                                      "p88888888888OO888OO888OO888OO88~~~p",
                                      "p8Bb;#HW#+#;;;E#E;;;#;;;E#E;;;8~~~p",
                                      "pO;;;#.N#.#uC;e#e;Cu#uC;e#e;Cu8~~Tp",
                                      "pO;C;#d##d#;;;;#;;;;#;;;;#;;;;8~~~p",
                                      "p8;u;;;;d,#;#######;#;#######;8~~~p",
                                      "p8#######,#;d.N#N.d;#;d.N#N.d;8T~~p",
                                      "p8Bb;#HW#,#;#WH#HW#;#;#WH#HW#;8~~~p",
                                      "pO;;;#.N#,#d#######d#d#######d8~~~p",
                                      "pO;C;#d##,,,,,,,,,,,,,,,,,,,,,O~T~p",
                                      "p8;u;;;;d,88OO888OO888OO888OO88~~~p",
                                      "p88888888D8~~~T~~~~~T~~~~~~~~T~~~~p",
                                      "pccccccccccccccxccccccccccccccccccp",
                                      "p88O8888O88cccccccccccccccccccccccp",
                                      "p8WN.H#,,,Dcc~~~~cccccccccxcccxcccp",
                                      "p8##d##,?=8cc~TT~ccccccxccccccccccp",
                                      "pOBb;X#,,+Occ~~~~ccccccccccccxccccp",
                                      "p8];;;d,,F8ccccccccccccxccccccccccp",
                                      "p88O8O88O88cccccccccccccccccccccccp",
                                      "ppppppppppppppppppppppppppppppppppp");
                    ;
            }
            return null;
        }

        private Level FromString(Dictionary<char, Tuple<string, string>> charIdentifiers, params string[] definition) {
            int width = definition[0].Length;
            int height = definition.Count();
            var map = new Level(new Size(width, height), "Grass");

            foreach (var terrain in terrainDefinitions) {
                map.AddTerrain(terrain);
            }

            for (int y = 0; y < height; y++) {
                var s = definition[y];
                
                if (s.Length != width)
                    throw new ArgumentException("Not all the rows are of the same width");

                for (int x = 0; x < s.Length; x++) {                    
                    map.SetTerrain(x, y, charIdentifiers[s[x]].Item1);
                    if (!String.IsNullOrEmpty(charIdentifiers[s[x]].Item2)) {
                        var feature = featureFactory.Construct(charIdentifiers[s[x]].Item2);
                        feature.Position = new Point(x, y);
                        map.AddFeature(feature);  
                    }
                  
                }
            }
            map.GenerateFov();
            return map;
        }
    }
}
