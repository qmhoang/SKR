using System;
using System.Linq;
using DEngine.Components;
using DEngine.Entities;
using Ogui.UI;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;
using Attribute = SkrGame.Universe.Entities.Actors.Attribute;

namespace SKR.UI.Gameplay {
	public class StatusPanel : Panel {
		private Entity player;
		private Calendar calendar;
		public StatusPanel(World world, PanelTemplate template)
				: base(template) {
			this.player = world.Player;
			calendar = world.Calendar;
		}

		private void PrintAttribute( int x, int y, Attribute attribute) {
			Canvas.PrintString(x, y, String.Format("{0}: {1}/{2}", attribute.Name.Substring(0, 3), attribute.Value, attribute.MaximumValue));			
		}

		protected override void Redraw() {
			base.Redraw();
			
			Canvas.PrintString(1, 1, player.Get<Identifier>().Name);
			PrintAttribute(1, 3, player.Get<DefendComponent>().Health);
			PrintAttribute(1, 4, player.Get<Person>().Stamina);

			PrintAttribute(1, 6, player.Get<Person>().Energy);
			PrintAttribute(1, 7, player.Get<Person>().Food);
			PrintAttribute(1, 8, player.Get<Person>().Water);
			PrintAttribute(1, 9, player.Get<Person>().Bladder);

			Canvas.PrintString(1, 12, calendar.Time.ToShortDateString());
			Canvas.PrintString(1, 13, calendar.Time.ToLongTimeString());
		}
	}
}