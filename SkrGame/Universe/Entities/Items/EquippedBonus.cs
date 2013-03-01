using System;
using System.Collections.Generic;
using DEngine.Core;
using DEngine.Entities;

namespace SkrGame.Universe.Entities.Items {
	public sealed class EquippedBonus : Component {
		public class Template {
			public Dictionary<string, int> Bonuses { get; set; }
		}

		public StaticDictionary<string, int> Bonuses { get; private set; }

		private EquippedBonus(StaticDictionary<string, int> bonuses) {
			Bonuses = bonuses;
		}

		public EquippedBonus(Template template) {
			Bonuses = new StaticDictionary<string, int>(template.Bonuses);
		}

		public override Component Copy() {
			return new EquippedBonus(new StaticDictionary<string, int>(Bonuses));
		}
	}
}