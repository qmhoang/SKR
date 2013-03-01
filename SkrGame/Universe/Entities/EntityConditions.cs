using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Entities;
using SkrGame.Effects;

namespace SkrGame.Universe.Entities {
	public sealed class EntityConditions : Component {
		private readonly List<Effect> effects;
		
		public EntityConditions(params Effect[] effects) {
			this.effects = new List<Effect>();
			foreach (var c in effects) {
				Add(c);
			}
		}

		public IEnumerable<Effect> Effects {
			get { return effects.ToList(); }
		}

		public void Add(Effect effect) {
			effect.Holder = this;
			effects.Add(effect);
		}

		public bool Remove(Effect effect) {
			return effects.Remove(effect);
		}

		public bool Contains(Effect effect) {
			return effects.Contains(effect);
		}

		public override Component Copy() {
			return new EntityConditions(effects.Select(c => c.Copy()).ToArray());			
		}
	}
}
