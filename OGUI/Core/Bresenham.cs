using System.Collections.Generic;
using DEngine.Core;
using libtcod;

namespace OGUI.Core {
    public static class Bresenham {
        public static List<Point> GeneratePointsFromLine(Point origin, Point end) {
            var list = new List<Point>();
            TCODLine.init(origin.X, origin.Y, end.X, end.Y);

            int x = origin.X;
            int y = origin.Y;
            while (!TCODLine.step(ref x, ref y)) {
                list.Add(new Point(x, y));
            }

            return list;
        }
    }
}
