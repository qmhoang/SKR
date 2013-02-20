using System.Collections.Generic;
using DEngine.Entities;

namespace SkrGame.Universe.Entities.Items {
	public class Equipable : Component {
		public class Template {
			public List<string> Slot { get; set; }
			public bool TwoHanded { get; set; }
	
		}
		public List<string> Slots { get; private set; }
		public bool TwoHanded { get; private set; }

		private Equipable() {}

		public Equipable(Template template) {
			Slots = template.Slot == null ? new List<string>() : new List<string>(template.Slot);
			TwoHanded = template.TwoHanded;
		}

		public override Component Copy() {
			return new Equipable()
			       {
			       		Slots = new List<string>(Slots),
			       		TwoHanded = TwoHanded
			       };
		}
	}
}