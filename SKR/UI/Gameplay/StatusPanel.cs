using System;
using System.Linq;
using DEngine.Components;
using DEngine.Entities;
using Ogui.UI;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Actors.PC;

namespace SKR.UI.Gameplay {
	public class StatusPanel : Panel {
		private Entity player;
		public StatusPanel(EntityManager manager, PanelTemplate template)
				: base(template) {
			player = World.Instance.Player;
		}

		protected override void Redraw() {
			base.Redraw();
			Canvas.PrintString(1, 1, player.Get<Identifier>().Name);
			Canvas.PrintString(1, 3, String.Format("H: {0}/{1}", player.Get<DefendComponent>().Health, player.Get<DefendComponent>().MaxHealth));
		}
	}
}