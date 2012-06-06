using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Actor.Components.Graphics;
using DEngine.Core;

namespace SKR.UI.Graphics {
    public class TCODImage : Image  {
        public char Ascii { get; private set; }
        public Pigment Color { get; private set; }
        public Point Position { get { return Owner.Position; } }

        public TCODImage(char ascii, Pigment color) {
            Ascii = ascii;
            Color = color;
        }
    }
}
