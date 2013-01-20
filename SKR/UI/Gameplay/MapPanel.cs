using System;
using System.Collections.Generic;
using System.Linq;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using DEngine.Extensions;
using Ogui.Core;
using Ogui.UI;
using SKR.Universe;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;
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
		private VisibilityMap playerVision;

		public MapPanel(EntityManager manager, AssetsManager assetsManager, PanelTemplate template)
				: base(template) {			
			ViewOffset = new Point(0, 0);
			assets = assetsManager;

			entities = manager.Get(typeof(Location), typeof(Sprite), typeof(VisibleComponent));

			player = World.Instance.Player;
			var location = player.Get<Location>();
			oldPos = location.Position;
			playerVision = new VisibilityMap(location.Level.Size);
			ComputePlayerFOV(location);
//			location.Level.CalculateFOV(location.Position, 10);

		}

		private void ComputePlayerFOV(Location location) {
			ShadowCastingFOV.ComputeRecursiveShadowcasting(playerVision, location.Level, location.Position.X, location.Position.Y, 10, true);
		}

		protected override void Update() {
			base.Update();

			if (oldPos != player.Get<Location>().Position) {
//				player.Get<Location>().Level.CalculateFOV(player.Get<Location>().Position, 10);
				ComputePlayerFOV(player.Get<Location>());
			}
		}

		protected override void Redraw() {
			base.Redraw();
			var level = player.Get<Location>().Level;

			ViewOffset = new Point(Math.Min(Math.Max(player.Get<Location>().X - Size.Width / 2, 0),
			                                level.Width - Size.Width),
			                       Math.Min(Math.Max(player.Get<Location>().Y - Size.Height / 2, 0),
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
							if (playerVision.IsVisible(localPosition)) {								
								Canvas.PrintChar(x, y, texture.Item1, texture.Item2);
							}
						} else {
							Canvas.PrintChar(localPosition, texture.Item1, texture.Item2);
						}
					}

				}
			}

			// draw entities
			foreach (var entity in entities.OrderBy(entity => entity.Get<Sprite>().ZOrder)) {
				Point localPosition = entity.Get<Location>().Position - ViewOffset;
				var texture = assets[entity.Get<Sprite>().Asset];

				if (IsPointWithinPanel(localPosition)) {

					if (!Program.SeeAll.Enabled) {
						if (playerVision.IsVisible(entity.Get<Location>().Position)) {
							if (entity.Get<VisibleComponent>().VisibilityIndex > 0)
								Canvas.PrintChar(localPosition, texture.Item1, texture.Item2);
						}
					} else {
						Canvas.PrintChar(localPosition, texture.Item1, texture.Item2);
					}
				}
			}

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