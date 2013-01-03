using System;
using System.Collections.Generic;
using System.Linq;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entity;
using DEngine.Extensions;
using Ogui.Core;
using Ogui.UI;
using SKR.Universe;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors.PC;
using SkrGame.Universe.Locations;
using libtcod;

namespace SKR.UI.Gameplay {
	public class MapPanel : Panel {
		
		internal Point ViewOffset { get; private set; }
		private AssetsManager assets;

		private FilteredCollection entities;
		private Entity player;
		private Point oldPos;		


		public MapPanel(EntityManager manager, AssetsManager assetsManager, PanelTemplate template)
				: base(template) {			
			ViewOffset = new Point(0, 0);
			assets = assetsManager;

			entities = manager.Get(typeof(Location), typeof(Sprite));

			player = World.Instance.Player;
			oldPos = player.As<Location>().Position;
			player.As<Location>().Level.CalculateFOV(player.As<Location>().Position, 10);

		}

		protected override void Update() {
			base.Update();

			if (oldPos != player.As<Location>().Position) {
				player.As<Location>().Level.CalculateFOV(player.As<Location>().Position, 10);
			}
		}

		protected override void Redraw() {
			base.Redraw();
			var level = player.As<Location>().Level;

			ViewOffset = new Point(Math.Min(Math.Max(player.As<Location>().X - Size.Width / 2, 0),
			                                level.Width - Size.Width),
			                       Math.Min(Math.Max(player.As<Location>().Y - Size.Height / 2, 0),
			                                level.Height - Size.Height));

			//draw map
			for (int x = 0; x < Size.Width; x++) {
				for (int y = 0; y < Size.Height; y++) {
					Point localPosition = ViewOffset.Shift(x, y);
					if (!level.IsInBoundsOrBorder(localPosition))
						continue;

					var texture = assets[((Level)level).GetTerrain(localPosition).Asset];
					if (texture == null)
						continue;
					if (IsPointWithinPanel(localPosition)) {
						if (!Program.SeeAll.Enabled) {
							if (level.IsVisible(localPosition)) {								
								Canvas.PrintChar(x, y, texture.Item1, texture.Item2);
							}
						} else {
							Canvas.PrintChar(localPosition, texture.Item1, texture.Item2);
						}
					}

				}
			}

			// draw entities
			foreach (var entity in entities.OrderBy(entity => entity.As<Sprite>().Order)) {
				Point localPosition = entity.As<Location>().Position - ViewOffset;
				var texture = assets[entity.As<Sprite>().Asset];

				if (IsPointWithinPanel(localPosition)) {

					if (!Program.SeeAll.Enabled) {
						if (level.IsVisible(entity.As<Location>().Position)) {
							Canvas.PrintChar(localPosition, texture.Item1, texture.Item2);
						}
					} else {
						Canvas.PrintChar(localPosition, texture.Item1, texture.Item2);
					}
				}
			}



//			for (int x = 0; x < Size.Width; x++)
//				for (int y = 0; y < Size.Height; y++) {
//					Point localPosition = ViewOffset.Shift(x, y);
//
//					if (!player.Level.IsInBoundsOrBorder(localPosition))
//						continue;
//
//					var texture = assets[player.Level.GetTerrain(localPosition).Asset];
//
//					if (texture == null)
//						continue;
//
//					if (IsPointWithinPanel(x, y))
//						if (!Program.SeeAll.Enabled) {
//							if (player.HasLineOfSight(localPosition)) {
//								player.Level.Vision[localPosition.X, localPosition.Y] = true;
//								Canvas.PrintChar(x, y, texture.Item1, texture.Item2);
//							} else if (player.Level.Vision[localPosition.X, localPosition.Y])
//								Canvas.PrintChar(x, y, texture.Item1, new Pigment(texture.Item2.Foreground.ScaleValue(0.3f), texture.Item2.Background.ScaleValue(0.3f)));
//						} else {
//							Canvas.PrintChar(x, y, texture.Item1, texture.Item2);
//							player.Level.Vision[localPosition.X, localPosition.Y] = true;
//						}
//				}
//
//			foreach (var feature in player.Level.Features) {
//				Point localPosition = feature.Position - ViewOffset;
//
//				if (IsPointWithinPanel(localPosition)) {
//					var texture = assets[feature.Asset];
//
//					if (!Program.SeeAll.Enabled) {
//						if (player.HasLineOfSight(feature.Position))
//							Canvas.PrintChar(localPosition.X, localPosition.Y, texture.Item1, texture.Item2);
//						else if (player.Level.Vision[feature.Position.X, feature.Position.Y])
//							Canvas.PrintChar(localPosition.X, localPosition.Y, texture.Item1, new Pigment(texture.Item2.Foreground.ScaleValue(0.3f), texture.Item2.Background.ScaleValue(0.3f)));
//					} else
//						Canvas.PrintChar(localPosition.X, localPosition.Y, texture.Item1, texture.Item2);
//				}
//			}
//
//			foreach (var tuple in player.Level.Items) {
//				Point localPosition = tuple.Item1 - ViewOffset;
//
//				if (!Program.SeeAll.Enabled)
//					if (IsPointWithinPanel(localPosition)) {
//						var texture = assets[tuple.Item2.Asset];
//
//						if (texture == null)
//							continue;
//						DrawTexture(localPosition, tuple.Item1, texture);
//					}
//			}
//
//			foreach (var actor in player.Level.Actors) {
//				Point localPosition = actor.Position - ViewOffset;
//				if (IsPointWithinPanel(localPosition)) {
//					var texture = assets[actor.Asset];
//
//					if (texture == null)
//						continue;
//					DrawTexture(localPosition, actor.Position, texture);
//				}
//			}
//
//
//			{
//				var texture = assets[player.Asset];
//
//				if (texture != null)
//					Canvas.PrintChar(player.Position - ViewOffset, texture.Item1, texture.Item2);
//			}

		}

//		private void DrawTexture(Point localPosition, Point mapPosition, Tuple<char, Pigment> texture) {
//			if (!Program.SeeAll.Enabled) {
//				if (player.HasLineOfSight(mapPosition))
//					Canvas.PrintChar(localPosition.X, localPosition.Y, texture.Item1, texture.Item2);
//			} else if (IsPointWithinPanel(localPosition))
//				Canvas.PrintChar(localPosition.X, localPosition.Y, texture.Item1, texture.Item2);
//		}

		private bool IsPointWithinPanel(Point p) {
			return p.X < Size.Width && p.Y < Size.Height && p.X >= 0 && p.Y >= 0;
		}

		private bool IsPointWithinPanel(int x, int y) {
			return x < Size.Width && y < Size.Height && x >= 0 && y >= 0;
		}
	}
}