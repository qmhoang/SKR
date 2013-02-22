using System;
using System.Collections.Generic;
using System.Text;
using Ogui.Core;
using Ogui.UI;
using libtcod;

namespace SKR.UI.Menus {
	public class OptionsPrompt<T> : PromptWindow {
		private readonly Action<T> action;
		private List<T> options;
		private Func<T, string> descriptorFunction;
		private string message;
		private Action fail;

		public OptionsPrompt(string message, IEnumerable<T> options, Func<T, string> descriptor, Action<T> action, Action fail, PromptWindowTemplate template)
				: base(template) {
			this.action = action;
			this.options = new List<T>(options);
			this.descriptorFunction = descriptor;
			this.message = message;
			this.fail = fail;
		}

		protected override void OnSettingUp() {
			base.OnSettingUp();
			if (options.Count <= 0) {
				fail();
				ExitWindow();
			} else if (options.Count == 1) {
				action(options[0]);
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
					sb.Append(descriptorFunction(option));
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
					action(options[index]);
					ExitWindow();
				}
			} else if (keyData.KeyCode == TCODKeyCode.Escape)
				ExitWindow();
		}
	}
}