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
using SkrGame.Universe.Entities.Actors.PC;
using SkrGame.Universe.Locations;
using libtcod;

namespace SKR.UI.Gameplay {
	public class MapPanel : Panel {
		
		internal Point ViewOffset { get; private set; }
		private AssetsManager assets;

		private FilteredCollection entities;
		private Entity player;


		public MapPanel(EntityManager manager, AssetsManager assetsManager, PanelTemplate template)
				: base(template) {			
			ViewOffset = new Point(0, 0);
			assets = assetsManager;

			entities = manager.Get(typeof(Location), typeof(Sprite));

			player = manager.Get<PlayerMarker>().ToList()[0];
		}

		protected override void Redraw() {
			base.Redraw();
			var level = player.As<LevelComponent>().Level;
			//draw map
			for (int x = 0; x < Size.Width; x++) {
				for (int y = 0; y < Size.Height; y++) {
					if (!level.IsInBoundsOrBorder(x, y))
						continue;

					var texture = assets[level.GetTerrain(x, y).Asset];
					if (texture == null)
						continue;
					if (IsPointWithinPanel(x, y))
					{
						if (!Program.SeeAll.Enabled) {
							Canvas.PrintChar(x, y, texture.Item1, texture.Item2);
						} else {
							Canvas.PrintChar(x, y, texture.Item1, texture.Item2);
						}
					}
					
				}
			}			
			
			// draw entities
			foreach (var entity in entities.OrderBy(entity => entity.As<Sprite>().Order)) {
				if (IsPointWithinPanel(entity.As<Location>().Position))
					Canvas.PrintChar(entity.As<Location>().Position, assets[entity.As<Sprite>().Asset].Item1, assets[entity.As<Sprite>().Asset].Item2);
			}

//			ViewOffset = new Point(Math.Min(Math.Max(player.As<Position>().X - Size.Width / 2, 0),
//			                                player.Level.Width - Size.Width),
//								   Math.Min(Math.Max(player.As<Position>().Y - Size.Height / 2, 0),
//			                                player.Level.Height - Size.Height));

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