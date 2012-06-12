using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Core;
using DEngine.Extensions;
using DEngine.Utility;
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
        }

        public override Level Construct(string identifier) {
            switch (identifier) {
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
