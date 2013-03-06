using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using DEngine.Extensions;
using Ogui.Core;
using Ogui.UI;
using SkrGame.Universe;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Locations;
using libtcod;
using log4net;

namespace SKR.UI.Gameplay {
	public class MapPanel : Panel {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		internal Point ViewOffset { get; private set; }
		private AssetsManager assets;

		private FilteredCollection entities;
		private Entity player;

		public World World { get; private set; }

		private bool alreadyDisposed;

		public MapPanel(World world, AssetsManager assetsManager, PanelTemplate template)
				: base(template) {	
			ViewOffset = new Point(0, 0);
			assets = assetsManager;

			World = world;
			
			entities = world.EntityManager.Get(typeof(GameObject), typeof(Sprite), typeof(VisibleComponent));
			player = world.Player;
		}

		private void PrintChar(Point screenPosition, Tuple<char, Tuple<TCODColor, TCODColor, TCODBackgroundFlag>> texture) {
			Canvas.Console.setChar(screenPosition.X, screenPosition.Y, texture.Item1);

			Canvas.Console.setCharBackground(screenPosition.X, screenPosition.Y, texture.Item2.Item2, texture.Item2.Item3);
			Canvas.Console.setCharForeground(screenPosition.X, screenPosition.Y, texture.Item2.Item1);
		}

		protected override void Redraw() {
			base.Redraw();
			Canvas.Clear();
			var level = player.Get<GameObject>().Level;

			ViewOffset = new Point(Math.Min(Math.Max(player.Get<GameObject>().X - Canvas.Size.Width / 2, 0),
											level.Width - Canvas.Size.Width),
								   Math.Min(Math.Max(player.Get<GameObject>().Y - Canvas.Size.Height / 2, 0),
											level.Height - Canvas.Size.Height));

			//draw map
			var sight = player.Get<SightComponent>();
			sight.CalculateSight();
			for (int x = 0; x < Canvas.Size.Width; x++) {
				for (int y = 0; y < Canvas.Size.Height; y++) {
					Point screenPosition = new Point(x, y);
					Point location = ViewOffset + screenPosition;

					if (!level.IsInBounds(location))
						continue;

					var texture = assets[level.GetTerrain(location).Asset];
					if (texture == null) {
						Logger.WarnFormat("Texture not found for asset: {0}!", level.GetTerrain(location).Asset);
						continue;
					}

					if (IsPointWithinPanel(screenPosition)) {
						if (!Program.SeeAll.Enabled) {
							if (sight.IsVisible(location)) {
								PrintChar(screenPosition, texture);
							}
						} else {
							PrintChar(screenPosition, texture);
						}
					}

				}
			}

			// draw entities
			foreach (var entity in entities.OrderBy(e => e.Get<Sprite>().ZOrder)) {
				Point screenPosition = entity.Get<GameObject>().Location - ViewOffset;

				var texture = assets[entity.Get<Sprite>().Asset];
				if (texture == null) {
					Logger.WarnFormat("Texture not found for asset: {0}!", entity.Get<Sprite>().Asset);
					continue;
				}

				if (IsPointWithinPanel(screenPosition)) {

					if (!Program.SeeAll.Enabled) {
						if (sight.IsVisible(entity.Get<GameObject>().Location)) {
							if (entity.Get<VisibleComponent>().VisibilityIndex > 0)
								PrintChar(screenPosition, texture);
						}
					} else {
						PrintChar(screenPosition, texture);
					}
				}
			}
		}

		private bool IsPointWithinPanel(Point p) {
			return p.X < Canvas.Size.Width && p.Y < Canvas.Size.Height && p.X >= 0 && p.Y >= 0;
		}

		private bool IsPointWithinPanel(int x, int y) {
			return x < Size.Width && y < Size.Height && x >= 0 && y >= 0;
		}

		protected override void Dispose(bool isDisposing) {
			base.Dispose(isDisposing);

			if (alreadyDisposed)
				return;
			if (isDisposing)
				if (Canvas != null)
					Canvas.Dispose();
			alreadyDisposed = true;
		}
	}
}