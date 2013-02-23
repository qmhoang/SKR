using System.Reflection;
using DEngine.Core;
using Ogui.Core;
using Ogui.UI;
using libtcod;
using log4net;

namespace SKR.UI.Prompts {
	public class PromptWindowTemplate : WindowTemplate {
		public Log Log { get; set; }
	}

	public abstract class PromptWindow : Window {
		protected abstract string Text { get; }
		
		protected static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		protected Log GameLog;
		
		protected PromptWindow(PromptWindowTemplate template)
			: base(template) {
			IsPopup = true;
			GameLog = template.Log;
		}

		protected override void Redraw() {
			base.Redraw();

			Canvas.Console.setBackgroundFlag(TCODBackgroundFlag.Alpha);
			Canvas.PrintString(0, 0, "* ", Pigments[PigmentType.Window].ReplaceForeground(ColorPresets.Blue));
			Canvas.Console.printRect(2, 0, Size.Width - 4, Size.Height - 2, Text);
		}
	}
}
