using System;
using Ogui.UI;
using SKR.UI.Menus;
using libtcod;

namespace SKR.UI.Prompts {
	public class BooleanPrompt : PromptWindow {
		private readonly Action<bool> _actionBoolean;
		private readonly bool _defaultBooleanAction;
		private string _message;

		public BooleanPrompt(string message, bool defaultBooleanAction, Action<bool> actionBoolean, PromptWindowTemplate template)
				: base(template) {
			this._defaultBooleanAction = defaultBooleanAction;
			this._actionBoolean = actionBoolean;
			this._message = message;
		}

		protected override string Text {
			get { return string.Format("{0}{1}, [Space] for default. Any other key = Abort.", _message, (_defaultBooleanAction ? " [Y/n]" : " [y/N]")); }
		}

		protected override void OnKeyPressed(KeyboardData keyData) {
			base.OnKeyPressed(keyData);

			if (keyData.Character == 'y')
				_actionBoolean(true);
			if (keyData.Character == 'n' || keyData.KeyCode == TCODKeyCode.Escape)
				_actionBoolean(false);
			if (keyData.KeyCode == TCODKeyCode.Space)
				_actionBoolean(_defaultBooleanAction);

			ExitWindow();
		}
	}
}