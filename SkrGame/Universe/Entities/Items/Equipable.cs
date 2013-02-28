using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DEngine.Core;
using DEngine.Entities;

namespace SkrGame.Universe.Entities.Items {
	public class Equipable : Component {
		public StaticDictionary<string, IEnumerable<string>> SlotsOccupied { get; private set; }
		
		private Equipable(StaticDictionary<string, IEnumerable<string>> slotsOccupied) {
			SlotsOccupied = slotsOccupied;
		}

		public static Equipable SingleSlot(params string[] slots) {
			return new Equipable(new StaticDictionary<string, IEnumerable<string>>(slots.Select(s => new KeyValuePair<string, IEnumerable<string>>(s, new List<string> {s}))));
		}

		public static Equipable MultipleSlots(params string[] slots) {
			return new Equipable(new StaticDictionary<string, IEnumerable<string>>(slots.Select(s => new KeyValuePair<string, IEnumerable<string>>(s, new List<string>(slots)))));
		}

		public override Component Copy() {
			return new Equipable(new StaticDictionary<string, IEnumerable<string>>(SlotsOccupied));
		}
	}
}