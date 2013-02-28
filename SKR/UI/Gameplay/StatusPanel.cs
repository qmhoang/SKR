using System;
using System.Linq;
using DEngine.Components;
using DEngine.Entities;
using Ogui.UI;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;
using libtcod;
using Attribute = SkrGame.Universe.Entities.Stats.Attribute;

namespace SKR.UI.Gameplay {
	public class StatusPanel : Panel {
		private Entity player;
		private Calendar calendar;
		public StatusPanel(World world, PanelTemplate template)
				: base(template) {
			this.player = world.Player;
			calendar = world.Calendar;
		}

		private void PrintAttribute(int x, int y, Attribute attribute) {
			Canvas.PrintString(x, y, String.Format("{0}: {1}/{2}", attribute.Abbreviation, attribute.Value, attribute.MaximumValue));			
		}

		protected override void Redraw() {
			base.Redraw();

			var person = player.Get<Person>();
			int i = 1;
			Canvas.PrintString(1, i++, player.Get<Identifier>().Name);
			i++;
			PrintAttribute(1, i++, player.Get<DefendComponent>().Health);
			PrintAttribute(1, i++, person.Stats["stat_stamina"]);
			PrintAttribute(1, i++, person.Stats["stat_composure"]);
			i++;
			PrintAttribute(1, i++, person.Stats["stat_energy"]);
			PrintAttribute(1, i++, person.Stats["stat_food"]);
			PrintAttribute(1, i++, person.Stats["stat_water"]);
			PrintAttribute(1, i++, person.Stats["stat_bladder"]);
			PrintAttribute(1, i++, person.Stats["stat_cleanliness"]);
			i++;
			i++;
			Canvas.PrintString(1, i++, person.Posture.ToString());			
			i++;
			i++;
			Canvas.PrintString(1, i++, calendar.DateTime.ToShortDateString());
			Canvas.PrintString(1, i++, calendar.DateTime.ToLongTimeString());
			i++;
			i++;
			i++;
			Canvas.PrintString(1, i++, String.Format("FPS: {0}", TCODSystem.getFps()));			
		}
	}
}