using System;
using System.Diagnostics.Contracts;
using DEngine.Entities;

namespace SkrGame.Universe.Entities.Items {
	public class AmmoComponent : Component {
		public class Template {
			public string Type { get; set; }

			public string ActionDescription { get; set; }
			public string ActionDescriptionPlural { get; set; }
		}

		public string Type { get; private set; }

		public string ActionDescription { get; private set; }
		public string ActionDescriptionPlural { get; private set; }

		private AmmoComponent() { }

		public AmmoComponent(Template template) {
			Contract.Requires<ArgumentNullException>(template != null, "template");
			ActionDescription = template.ActionDescription;
			ActionDescriptionPlural = template.ActionDescriptionPlural;

			Type = template.Type;
		}

		public override Component Copy() {
			return new AmmoComponent
			       {
			       		ActionDescription = ActionDescription,
			       		ActionDescriptionPlural = ActionDescriptionPlural,

			       		Type = Type,
			       };
		}
	}
}