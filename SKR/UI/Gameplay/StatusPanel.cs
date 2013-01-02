using System;
using System.Linq;
using DEngine.Entity;
using Ogui.UI;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Actors.PC;

namespace SKR.UI.Gameplay {
	public class StatusPanel : Panel {
		private Entity player;
		public StatusPanel(EntityManager manager, PanelTemplate template)
				: base(template) {
			player = manager.Get<PlayerMarker>().ToList()[0];
		}

		protected override void Redraw() {
			base.Redraw();
			Canvas.PrintString(1, 1, player.As<Actor>().Name);
			Canvas.PrintString(1, 3, String.Format("H: {0}/{1}", player.As<Actor>().Health, player.As<Actor>().MaxHealth));
		}
	}
}