using System;
using System.Linq;
using DEngine.Components;
using DEngine.Entities;
using Ogui.UI;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;

namespace SKR.UI.Gameplay {
	public class StatusPanel : Panel {
		private Entity player;
		public StatusPanel(Entity player, PanelTemplate template)
				: base(template) {
			this.player = player;
		}

		protected override void Redraw() {
			base.Redraw();
			Canvas.PrintString(1, 1, player.Get<Identifier>().Name);
			Canvas.PrintString(1, 3, String.Format("H: {0}/{1}", player.Get<DefendComponent>().Health, player.Get<DefendComponent>().MaxHealth));
		}
	}
}