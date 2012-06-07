using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Core;
using DEngine.Utility;
using SKR.Universe.Location;

namespace SKR.Universe.Factories {
    public abstract class MapFactory : Factory<string, Level> {
    }

    public class SourceMapFactory : MapFactory {
        private TileFactory tileFactory;
        private UniqueIdFactory idFactory;

        public SourceMapFactory(TileFactory tileFactory, UniqueIdFactory idFactory) {
            this.tileFactory = tileFactory;
            this.idFactory = idFactory;
        }

        public override Level Construct(string identifier) {
            switch (identifier) {
                case "TestMap":
                    return FromString(idFactory.Construct(), 
                                      new Dictionary<char, TileEnum>
                                          {
                                                  {'#', TileEnum.Wall},
                                                  {'.', TileEnum.WoodFloor},
                                          },
                                      "############################################################", 
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

        private Level FromString(UniqueId id, Dictionary<char, TileEnum> charIdentifiers, params string[] definition) {
            int width = definition[0].Length;
            int height = definition.Count();
            var map = new Level(id, new Size(width, height), tileFactory.Construct(TileEnum.Unused));

            for (int y = 0; y < height; y++) {
                var s = definition[y];

                if (s.Length != width)
                    throw new ArgumentException("Not all the rows are of the same width");

                for (int x = 0; x < s.Length; x++) {
                    var key = s[x];
                    map.SetTile(x, y, tileFactory.Construct(charIdentifiers[key]));
                }
            }
            map.GenerateFov();
            return map;
        }
    }
}
