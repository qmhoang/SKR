using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Actor.Components.Graphics;
using DEngine.Core;
using DEngine.Extensions;

namespace SKR.UI.Graphics {
    public class TCODGraphicsTransformer : IGraphicsTransformer<TCODImage> {
        private readonly Dictionary<string, TCODImage> images = new Dictionary<string, TCODImage>
                                                          {
                                                                  {"player", new TCODImage('@', new Pigment(ColorPresets.White, ColorPresets.Black)) },
                                                                  {"human", new TCODImage('@', new Pigment(ColorPresets.Gray, ColorPresets.Black)) },
                                                                  {"largeknife", new TCODImage('(', new Pigment(ColorPresets.Gray, ColorPresets.Black)) },
                                                                  {"axe", new TCODImage('(', new Pigment(ColorPresets.Gray, ColorPresets.Black)) },
                                                                  {"brassknuckles", new TCODImage('(', new Pigment(ColorPresets.Gray, ColorPresets.Black)) },
                                                          };

        public TCODImage Transform(Actor actor) {
            if (!images.ContainsKey(actor.RefId))
                throw new ArgumentException();
            return images[actor.RefId];
        }
    }
}
