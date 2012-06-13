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
            assets.Add("Grass", new Tuple<char, Pigment>((char) TCODSpecialCharacter.Block1, new Pigment(ColorPresets.Green, ColorPresets.Brown)));
            assets.Add("StoneFloor", new Tuple<char, Pigment>((char) TCODSpecialCharacter.Block1, new Pigment(ColorPresets.SandyBrown, ColorPresets.Brown)));


            assets.Add("PAVEMENT", new Tuple<char, Pigment>((char) TCODSpecialCharacter.Block2, new Pigment(ColorPresets.DarkerGrey, ColorPresets.Gray)));
            assets.Add("FLOOR_WOOD_TWO", new Tuple<char, Pigment>((char) TCODSpecialCharacter.Block1, new Pigment(new Color(145, 104, 58), new Color(166, 120, 66))));
            assets.Add("GRASS_DARK", new Tuple<char, Pigment>((char) TCODSpecialCharacter.Block3, new Pigment(ColorPresets.Green, ColorPresets.Brown)));
            assets.Add("CARPET_GREY", new Tuple<char, Pigment>((char) TCODSpecialCharacter.Block1, new Pigment(ColorPresets.LightSlateGray, ColorPresets.LightGray)));
            assets.Add("FLOOR_TILE", new Tuple<char, Pigment>((char)TCODSpecialCharacter.Block2, new Pigment(ColorPresets.White, ColorPresets.Black)));

            
            assets.Add("WALL_BRICK_DARK", new Tuple<char, Pigment>('#', new Pigment(new Color(146, 68, 35), ColorPresets.Black)));
            assets.Add("COUNTER_WOOD_RED", new Tuple<char, Pigment>('n', new Pigment(ColorPresets.Red, ColorPresets.Black)));
            assets.Add("WALL_BRICK_DARK_DOOR_CLOSE", new Tuple<char, Pigment>('+', new Pigment(new Color(146, 68, 35), ColorPresets.Black)));
            assets.Add("WALL_BRICK_DARK_DOOR_OPEN", new Tuple<char, Pigment>('/', new Pigment(new Color(146, 68, 35), ColorPresets.Black)));
            assets.Add("WINDOW_BRICK_DARK_OPEN", new Tuple<char, Pigment>((char)TCODSpecialCharacter.DoubleHorzLine, new Pigment(new Color(146, 68, 35), ColorPresets.Black)));
            assets.Add("WINDOW_BRICK_DARK_CLOSE", new Tuple<char, Pigment>((char)TCODSpecialCharacter.DoubleVertLine, new Pigment(new Color(146, 68, 35), ColorPresets.Black)));
            assets.Add("TOILET", new Tuple<char, Pigment>('p', new Pigment(ColorPresets.FloralWhite, ColorPresets.Black)));
            assets.Add("BATHROOMSINK", new Tuple<char, Pigment>('n', new Pigment(ColorPresets.FloralWhite, ColorPresets.Black)));
            assets.Add("SINK", new Tuple<char, Pigment>('n', new Pigment(ColorPresets.FloralWhite, ColorPresets.Black)));
            assets.Add("BATH", new Tuple<char, Pigment>('D', new Pigment(new Color(146, 68, 35), ColorPresets.Black)));
            assets.Add("SHOWER", new Tuple<char, Pigment>('H', new Pigment(ColorPresets.FloralWhite, ColorPresets.Black)));
            assets.Add("TREE_SMALL", new Tuple<char, Pigment>((char) TCODSpecialCharacter.Spade, new Pigment(ColorPresets.Green, ColorPresets.Black)));
            assets.Add("BED_WOODEN", new Tuple<char, Pigment>('B', new Pigment(new Color(145, 104, 58), ColorPresets.Black)));
            assets.Add("SHELF_WOOD", new Tuple<char, Pigment>('n', new Pigment(new Color(145, 104, 58), ColorPresets.Black)));
            assets.Add("SHELF_METAL", new Tuple<char, Pigment>('n', new Pigment(ColorPresets.LightGray, ColorPresets.Black)));
            assets.Add("TELEVISION", new Tuple<char, Pigment>('T', new Pigment(ColorPresets.Blue, ColorPresets.Black)));
            assets.Add("FRIDGE", new Tuple<char, Pigment>('B', new Pigment(ColorPresets.LightGray, ColorPresets.Black)));
            assets.Add("DESK_WOODEN", new Tuple<char, Pigment>('m', new Pigment(new Color(145, 104, 58), ColorPresets.Black)));
            assets.Add("CHAIR_WOODEN", new Tuple<char, Pigment>('b', new Pigment(new Color(145, 104, 58), ColorPresets.Black)));
            assets.Add("CASH_REGISTER", new Tuple<char, Pigment>('c', new Pigment(ColorPresets.Red, ColorPresets.Black)));
            assets.Add("SOFA", new Tuple<char, Pigment>('w', new Pigment(ColorPresets.Green, ColorPresets.Black)));
            assets.Add("OVEN", new Tuple<char, Pigment>('n', new Pigment(ColorPresets.LightGray, ColorPresets.Black)));
            assets.Add("STAIR_WOODEN_UP", new Tuple<char, Pigment>('<', new Pigment(new Color(145, 104, 58), ColorPresets.Black)));
            assets.Add("STAIR_WOODEN_DOWN", new Tuple<char, Pigment>('>', new Pigment(new Color(145, 104, 58), ColorPresets.Black)));
            assets.Add("DOOR_GARAGE", new Tuple<char, Pigment>('N', new Pigment(ColorPresets.Red, ColorPresets.Black)));
            assets.Add("FENCE_WOODEN", new Tuple<char, Pigment>('I', new Pigment(ColorPresets.White, ColorPresets.Black)));
            assets.Add("LAMP_STANDARD", new Tuple<char, Pigment>('P', new Pigment(ColorPresets.Beige, ColorPresets.Black)));
            assets.Add("TABLE_WOODEN", new Tuple<char, Pigment>('m', new Pigment(new Color(145, 104, 58), ColorPresets.Black)));
            assets.Add("PLANTPOT_FIXED", new Tuple<char, Pigment>('t', new Pigment(ColorPresets.Green, ColorPresets.Black)));
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