using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Entities;
using SkrGame.Conditions;

namespace SkrGame.Universe.Entities {
	public sealed class EntityConditions : Component {
		private List<Condition> conditions;
		
		public EntityConditions(params Condition[] conditions) {
			this.conditions = new List<Condition>();
			foreach (var c in conditions) {
				Add(c);
			}
		}

		public IEnumerable<Condition> Conditions {
			get { return conditions.ToList(); }
		}

		public void Add(Condition condition) {
			condition.Holder = this;
			conditions.Add(condition);
		}

		public bool Remove(Condition condition) {
			return conditions.Remove(condition);
		}

		public bool Contains(Condition condition) {
			return conditions.Contains(condition);
		}

		public override Component Copy() {
			return new EntityConditions(conditions.Select(c => c.Copy()).ToArray());			
		}
	}
}
