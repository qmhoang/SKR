using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Entities;
using SkrGame.Effects;

namespace SkrGame.Universe.Entities {
	public sealed class ConditionHolder : Component {
		private readonly List<Effect> _effects;
		
		public ConditionHolder(params Effect[] effects) {
			this._effects = new List<Effect>();
			foreach (var c in effects) {
				Add(c);
			}
		}

		public IEnumerable<Effect> Effects {
			get { return _effects.ToList(); }
		}

		public void Add(Effect effect) {
			effect.Holder = this;
			_effects.Add(effect);
		}

		public bool Remove(Effect effect) {
			return _effects.Remove(effect);
		}

		public bool Contains(Effect effect) {
			return _effects.Contains(effect);
		}

		public override Component Copy() {
			return new ConditionHolder(_effects.Select(c => c.Copy()).ToArray());			
		}
	}
}
