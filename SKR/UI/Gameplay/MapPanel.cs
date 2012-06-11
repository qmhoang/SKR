using System;
using System.Collections.Generic;
using DEngine.Core;
using DEngine.Extensions;
using DEngine.UI;
using SKR.Universe.Entities.Actors.PC;
using SKR.Universe.Location;
using libtcod;

namespace SKR.UI.Gameplay {
    public class AssetsManager {
        private Dictionary<string, Tuple<char, Pigment>> assets;

        public AssetsManager() {
            assets = new Dictionary<string, Tuple<char, Pigment>>();

            assets.Add("player", new Tuple<char, Pigment>('@', new Pigment(ColorPresets.White, ColorPresets.Black)));
            assets.Add("npc", new Tuple<char, Pigment>('@', new Pigment(ColorPresets.GhostWhite, ColorPresets.Black)));
            assets.Add("Wall", new Tuple<char, Pigment>('#', new Pigment(ColorPresets.Gray, ColorPresets.Black)));
            assets.Add("Trap", new Tuple<char, Pigment>('^', new Pigment(ColorPresets.Gray, ColorPresets.Black)));
            assets.Add("ClosedDoor", new Tuple<char, Pigment>('+', new Pigment(ColorPresets.Gray, ColorPresets.Black)));
            assets.Add("OpenedDoor", new Tuple<char, Pigment>('/', new Pigment(ColorPresets.Gray, ColorPresets.Black)));
            assets.Add("Grass", new Tuple<char, Pigment>((char)TCODSpecialCharacter.Block1, new Pigment(ColorPresets.Green, ColorPresets.Brown)));
            assets.Add("StoneFloor", new Tuple<char, Pigment>((char)TCODSpecialCharacter.Block1, new Pigment(ColorPresets.SandyBrown, ColorPresets.Brown)));
        }

        public Tuple<char, Pigment> this[string asset] {
            get { return assets[asset]; }
        }
    }

    public class MapPanel : Panel {
        private readonly Player player;

        internal Point ViewOffset { get; private set; }
        private AssetsManager assets;
               

        public MapPanel(PanelTemplate template, Player player) : base(template) {
            this.player = player;
            ViewOffset = new Point(0, 0);    
            assets = new AssetsManager();
        }

        protected override void Redraw() {
            base.Redraw();

            ViewOffset = new Point(Math.Min(Math.Max(player.Position.X - Size.Width / 2, 0),
                                            player.Level.Width - Size.Width),
                                   Math.Min(Math.Max(player.Position.Y - Size.Height / 2, 0),
                                            player.Level.Height - Size.Height));

            for (int x = 0; x < Size.Width; x++)
                for (int y = 0; y < Size.Height; y++) {
                    Point localPosition = ViewOffset.Shift(x, y);
                    
                    if (!player.Level.IsInBoundsOrBorder(localPosition))
                        continue;
                    
                    var texture = assets[player.Level.GetTerrain(localPosition).Asset];

                    if (texture == null)
                        continue;
#if DEBUG
                    if (!Program.SeeAll) {
#endif
                        if (player.HasLineOfSight(localPosition)) {
                            player.Level.Vision[localPosition.X, localPosition.Y] = true;
                            Canvas.PrintChar(x, y, texture.Item1, texture.Item2);
                        } else if (player.Level.Vision[localPosition.X, localPosition.Y])
                            if (IsPointWithinPanel(new Point(x, y)))
                                Canvas.PrintChar(x, y, texture.Item1, new Pigment(texture.Item2.Foreground.ScaleValue(0.3f), texture.Item2.Background.ScaleValue(0.3f)));
#if DEBUG
                    } else {
                        Canvas.PrintChar(x, y, texture.Item1, texture.Item2);
                        player.Level.Vision[localPosition.X, localPosition.Y] = true;
                    }
#endif
                }

            foreach (var feature in player.Level.Features) {
                Point localPosition = feature.Position - ViewOffset;
#if DEBUG
                if (!Program.SeeAll) {
#endif
                    var texture = assets[feature.Asset];
                    if (player.HasLineOfSight(feature.Position))
                        Canvas.PrintChar(localPosition.X, localPosition.Y, texture.Item1, texture.Item2);
                    else if (player.Level.Vision[feature.Position.X, feature.Position.Y])
                        if (IsPointWithinPanel(localPosition))
                            Canvas.PrintChar(localPosition.X, localPosition.Y, texture.Item1, new Pigment(texture.Item2.Foreground.ScaleValue(0.3f), texture.Item2.Background.ScaleValue(0.3f)));
                }
            }

            foreach (var actor in player.Level.Actors) {
                Point localPosition = actor.Position - ViewOffset;
                var texture = assets[actor.Asset];

                if (texture == null)
                    continue;
                DrawIndividual(localPosition, actor.Position, texture);
            }



            // drawing player            
            {
                var texture = assets[player.Asset];

                if (texture != null) {
                    Canvas.PrintChar(player.Position - ViewOffset, texture.Item1, texture.Item2);
                    
                }
                    
            }
        }




        private void DrawIndividual(Point localPosition, Point mapPosition, Tuple<char, Pigment> texture) {
#if DEBUG
            if (!Program.SeeAll) {
#endif
                if (player.HasLineOfSight(mapPosition)) {
                    Canvas.PrintChar(localPosition.X, localPosition.Y, texture.Item1, texture.Item2);
                }
            }
#if DEBUG
            else if (IsPointWithinPanel(localPosition))
                Canvas.PrintChar(localPosition.X, localPosition.Y, texture.Item1, texture.Item2);
#endif
        }

        private bool IsPointWithinPanel(Point p) {
            return p.X < Size.Width && p.Y < Size.Height && p.X >= 0 && p.Y >= 0;
        }
    }
}