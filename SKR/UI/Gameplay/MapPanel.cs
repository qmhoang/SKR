using System;
using DEngine.Core;
using DEngine.Extensions;
using DEngine.UI;
using SKR.Universe.Entities.Actor.PC;
using SKR.Universe.Location;

namespace SKR.UI.Gameplay {
    public class MapPanel : Panel {
        private readonly Player player;

        internal Point ViewOffset { get; private set; }

        private static Color backgroundColor = ColorPresets.Black;

        public MapPanel(PanelTemplate template, Player player) : base(template) {
            this.player = player;
            ViewOffset = new Point(0, 0);
        }

        protected override void Redraw() {
            base.Redraw();

            ViewOffset = new Point(Math.Min(Math.Max(player.Position.X - Size.Width / 2, 0),
                                            player.Level.Width - Size.Width),
                                   Math.Min(Math.Max(player.Position.Y - Size.Height / 2, 0),
                                            player.Level.Height - Size.Height));

            player.CalculateFov();

            for (int x = 0; x < Size.Width; x++)
                for (int y = 0; y < Size.Height; y++) {
                    Point realPosition = ViewOffset.Shift(x, y);

                    Cell c = Cell.GetCell(player.Level.GetTile(realPosition));
#if DEBUG
                    if (!Program.SeeAll) {
#endif
                        if (player.Spot(realPosition)) {
                            player.Level.Vision[realPosition.X, realPosition.Y] = true;
                            Canvas.PrintChar(x, y, c.Ascii, c.Pigment);
                        } else if (player.Level.Vision[realPosition.X, realPosition.Y])
                            if (IsPointWithinPanel(new Point(x, y)))
                                Canvas.PrintChar(x, y, c.Ascii, new Pigment(c.Pigment.Foreground.ScaleValue(0.3f), c.Pigment.Background.ScaleValue(0.3f)));
#if DEBUG
                    } else {
                        Canvas.PrintChar(x, y, c.Ascii, c.Pigment);
                        player.Level.Vision[realPosition.X, realPosition.Y] = true;
                    }
#endif
                }

            foreach (var actor in player.Level.Actors) {
                Point localPosition = actor.Position - ViewOffset;

                DrawIndividual(localPosition, actor.Position, actor.Ascii, new Pigment(actor.Color, backgroundColor));
            }

            // drawing player
            Canvas.PrintChar(player.Position - ViewOffset, player.Ascii, new Pigment(player.Color, backgroundColor));
        }




        private void DrawIndividual(Point localPosition, Point mapPosition, char ascii, Pigment directVision) {
#if DEBUG
            if (!Program.SeeAll) {
#endif
                if (player.Spot(mapPosition)) {
                    Canvas.PrintChar(localPosition.X, localPosition.Y, ascii, directVision);
                }
            }
#if DEBUG
            else if (IsPointWithinPanel(localPosition))
                Canvas.PrintChar(localPosition.X, localPosition.Y, ascii, directVision);
#endif
        }

        private bool IsPointWithinPanel(Point p) {
            return p.X < Size.Width && p.Y < Size.Height && p.X >= 0 && p.Y >= 0;
        }
    }
}