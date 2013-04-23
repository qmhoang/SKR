using System;
using System.Collections.Generic;
using System.Text;
using Ogui.Core;
using Ogui.UI;
using SKR.UI.Menus;
using libtcod;

namespace SKR.UI.Prompts {
	public class OptionsPrompt<T> : PromptWindow {
		private readonly Action<T> _actionCount;
		private List<T> _options;
		private Func<T, string> _toStringFunction;
		private string _message;
		private string _noOptions;

		public OptionsPrompt(string message, string noOptions, IEnumerable<T> options, Func<T, string> toStringFunc, Action<T> actionCount, PromptWindowTemplate template)
				: base(template) {
			this._actionCount = actionCount;
			this._options = new List<T>(options);
			this._toStringFunction = toStringFunc;
			this._message = message;
			this._noOptions = noOptions;			
		}

		protected override void OnSettingUp() {
			base.OnSettingUp();
			if (_options.Count <= 0) {
				GameLog.Aborted(_noOptions);
				ExitWindow();
			} else if (_options.Count == 1) {
				_actionCount(_options[0]);
				ExitWindow();
			}
		}

		protected override string Text {
			get {
				StringBuilder sb = new StringBuilder();
				char c = 'a';
				foreach (var option in _options) {
					sb.AppendFormat("{1}[{0}]", c, ColorPresets.Green.ForegroundCodeString);
					sb.Append(Color.StopColorCode);
					sb.Append(_toStringFunction(option));
					sb.Append(" ");
					c++;
				}

				return String.Format("{0} : {1}{2}[Esc]{3} to exit.", _message, sb, ColorPresets.Red.ForegroundCodeString, Color.StopColorCode);
			}
		}

		protected override void OnKeyPressed(KeyboardData keyData) {
			base.OnKeyPressed(keyData);

			if (Char.IsLetter(keyData.Character)) {
				int index = Char.ToLower(keyData.Character) - 'a';
				if (index < _options.Count && index >= 0) {
					_actionCount(_options[index]);
					ExitWindow();
				}
			} else if (keyData.KeyCode == TCODKeyCode.Escape)
				ExitWindow();
		}
	}
}