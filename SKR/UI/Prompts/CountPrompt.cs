using System;
using Ogui.UI;
using SKR.UI.Menus;
using libtcod;

namespace SKR.UI.Prompts {
	public class CountPrompt : PromptWindow {
		private readonly Action<int> actionCount;
		private readonly int maxValue;
		private readonly int minValue;
		private int value;
		private bool negative; // this is used to store negative when you have a 0 since there is no negative 0
		private bool firstUse;
		private string message;

		public CountPrompt(string message, Action<int> actionCount, int maxValue, int minValue, int initialValue, PromptWindowTemplate template)
				: base(template) {
			this.actionCount = actionCount;
			this.maxValue = maxValue;
			this.minValue = minValue;
			this.message = message;

			initialValue = initialValue > maxValue ? maxValue : initialValue;
			initialValue = initialValue < minValue ? minValue : initialValue;

			this.value = initialValue;

			firstUse = false;

			negative = initialValue < 0;
		}

		protected override string Text {
			get { return String.Format("{0}. How many ({1}-{2}): {3}, Press enter when done. Esc to exit.", message, minValue, maxValue, value); }
		}

		protected override void OnKeyPressed(KeyboardData key) {
			base.OnKeyPressed(key);

			if (negative && value > 0)
				value *= -1;

			if (key.Character >= '0' && key.Character <= '9') {
				if (firstUse) {
					// if this is the first key press, any key press will replace the amount
					value = 0;
					firstUse = false;
				}

				value *= 10;

				value += negative ? -(key.Character - '0') : key.Character - '0';
			} else if (key.KeyCode == TCODKeyCode.Backspace)
				value /= 10;
			else if (key.KeyCode == TCODKeyCode.KeypadSubtract || key.Character == '-') {
				negative = !negative;
				value *= -1;
			}
			value = Math.Min(maxValue, value);
			value = Math.Max(minValue, value);

			if (key.KeyCode == TCODKeyCode.Enter || key.KeyCode == TCODKeyCode.KeypadEnter) {
				actionCount(value);
				ExitWindow();
			} else if (key.KeyCode == TCODKeyCode.Escape)
				ExitWindow();
		}
	}	
}