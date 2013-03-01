using System;
using System.Diagnostics.Contracts;
using DEngine.Entities;

namespace SkrGame.Universe.Entities.Items {
	public sealed class AmmoComponent : Component {
		public class Template {
			public string Caliber { get; set; }

			public string ActionDescription { get; set; }
			public string ActionDescriptionPlural { get; set; }
		}

		public string Caliber { get; private set; }

		public string ActionDescription { get; private set; }
		public string ActionDescriptionPlural { get; private set; }

		private AmmoComponent(string type, string actionDescription, string actionDescriptionPlural) {
			Caliber = type;
			ActionDescription = actionDescription;
			ActionDescriptionPlural = actionDescriptionPlural;
		}

		public AmmoComponent(Template template) : this(template.Caliber, template.ActionDescription, template.ActionDescriptionPlural) {
			Contract.Requires<ArgumentNullException>(template != null, "template");
		}

		public override Component Copy() {
			return new AmmoComponent(Caliber, ActionDescription, ActionDescriptionPlural);
		}
	}
}