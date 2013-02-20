using System;
using System.Collections.Generic;
using System.Text;
using Ogui.Core;
using Ogui.UI;
using libtcod;

namespace SKR.UI.Menus {
	public class OptionsPrompt<T> : PromptWindow {
		private readonly Action<T> actionCount;
		private List<T> options;
		private Func<T, string> descriptorFunction;
		private string message;

		public OptionsPrompt(string message, IEnumerable<T> options, Func<T, string> descriptor, Action<T> actionCount, PromptWindowTemplate template)
				: base(template) {
			this.actionCount = actionCount;
			this.options = new List<T>(options);
			this.descriptorFunction = descriptor;
			this.message = message;
		}

		protected override void OnSettingUp() {
			base.OnSettingUp();
			if (options.Count <= 0) {
				GameLog.Normal("No options to select from.");
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
					actionCount(options[index]);
					ExitWindow();
				}
			} else if (keyData.KeyCode == TCODKeyCode.Escape)
				ExitWindow();
		}
	}
}