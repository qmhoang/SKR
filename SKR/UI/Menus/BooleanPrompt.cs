using System;
using Ogui.UI;
using libtcod;

namespace SKR.UI.Menus {
	public class BooleanPrompt : PromptWindow {
		private readonly Action<bool> actionBoolean;
		private readonly bool defaultBooleanAction;
		private string message;

		public BooleanPrompt(string message, bool defaultBooleanAction, Action<bool> actionBoolean, PromptWindowTemplate template)
				: base(template) {
			this.defaultBooleanAction = defaultBooleanAction;
			this.actionBoolean = actionBoolean;
			this.message = message;
		}

		protected override string Text {
			get { return string.Format("{0}{1}, [Space] for default. Any other key = Abort.", message, (defaultBooleanAction ? " [Y/n]" : " [y/N]")); }
		}

		protected override void OnKeyPressed(KeyboardData keyData) {
			base.OnKeyPressed(keyData);

			if (keyData.Character == 'y')
				actionBoolean(true);
			if (keyData.Character == 'n' || keyData.KeyCode == TCODKeyCode.Escape)
				actionBoolean(false);
			if (keyData.KeyCode == TCODKeyCode.Space)
				actionBoolean(defaultBooleanAction);

			ExitWindow();
		}
	}
}