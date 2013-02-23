using System;
using System.Collections.Generic;
using System.Text;
using Ogui.Core;
using Ogui.UI;
using SKR.UI.Menus;
using libtcod;

namespace SKR.UI.Prompts {
	public class OptionsPrompt<T> : PromptWindow {
		private readonly Action<T> actionCount;
		private List<T> options;
		private Func<T, string> toStringFunction;
		private string message;
		private string fail;

		public OptionsPrompt(string message, string fail, IEnumerable<T> options, Func<T, string> toStringFunc, Action<T> actionCount, PromptWindowTemplate template)
				: base(template) {
			this.actionCount = actionCount;
			this.options = new List<T>(options);
			this.toStringFunction = toStringFunc;
			this.message = message;
			this.fail = fail;			
		}

		protected override void OnSettingUp() {
			base.OnSettingUp();
			if (options.Count <= 0) {
				GameLog.Fail(fail);
				ExitWindow();
			} else if (options.Count == 1) {
				actionCount(options[0]);
				ExitWindow();
			}
		}

		protected override string Text {
			get {
				StringBuilder sb = new StringBuilder();
				char c = 'a';
				foreach (var option in options) {
					sb.AppendFormat("{1}[{0}]", c, ColorPresets.Green.ForegroundCodeString);
					sb.Append(Color.StopColorCode);
					sb.Append(toStringFunction(option));
					sb.Append(" ");
					c++;
				}

				return String.Format("{0} : {1}{2}[Esc]{3} to exit.", message, sb, ColorPresets.Red.ForegroundCodeString, Color.StopColorCode);
			}
		}

		protected override void OnKeyPressed(KeyboardData keyData) {
			base.OnKeyPressed(keyData);

			if (Char.IsLetter(keyData.Character)) {
				int index = Char.ToLower(keyData.Character) - 'a';
				if (index < options.Count && index >= 0) {
					actionCount(options[index]);
					ExitWindow();
				}
			} else if (keyData.KeyCode == TCODKeyCode.Escape)
				ExitWindow();
		}
	}
}