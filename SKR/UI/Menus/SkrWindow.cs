using Ogui.UI;
using SkrGame.Universe;

namespace SKR.UI.Menus {
	public class SkrWindowTemplate : WindowTemplate {
		public World World { get; set; }
	}

	public abstract class SkrWindow : Window {
		protected World World { get; private set; }
		public SkrWindow(SkrWindowTemplate template) : base(template) {
			World = template.World;
		}
	}
}