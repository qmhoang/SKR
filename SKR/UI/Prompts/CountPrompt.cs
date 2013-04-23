using System;
using Ogui.UI;
using SKR.UI.Menus;
using libtcod;

namespace SKR.UI.Prompts {
	public class CountPrompt : PromptWindow {
		private readonly Action<int> _actionCount;
		private readonly int _maxValue;
		private readonly int _minValue;
		private int _value;
		private bool _negative; // this is used to store negative when you have a 0 since there is no negative 0
		private bool _firstUse;
		private string _message;

		public CountPrompt(string message, Action<int> actionCount, int maxValue, int minValue, int initialValue, PromptWindowTemplate template)
				: base(template) {
			this._actionCount = actionCount;
			this._maxValue = maxValue;
			this._minValue = minValue;
			this._message = message;

			initialValue = initialValue > maxValue ? maxValue : initialValue;
			initialValue = initialValue < minValue ? minValue : initialValue;

			this._value = initialValue;

			_firstUse = false;

			_negative = initialValue < 0;
		}

		protected override string Text {
			get { return String.Format("{0}. How many ({1}-{2}): {3}, Press enter when done. Esc to exit.", _message, _minValue, _maxValue, _value); }
		}

		protected override void OnKeyPressed(KeyboardData key) {
			base.OnKeyPressed(key);

			if (_negative && _value > 0)
				_value *= -1;

			if (key.Character >= '0' && key.Character <= '9') {
				if (_firstUse) {
					// if this is the first key press, any key press will replace the amount
					_value = 0;
					_firstUse = false;
				}

				_value *= 10;

				_value += _negative ? -(key.Character - '0') : key.Character - '0';
			} else if (key.KeyCode == TCODKeyCode.Backspace)
				_value /= 10;
			else if (key.KeyCode == TCODKeyCode.KeypadSubtract || key.Character == '-') {
				_negative = !_negative;
				_value *= -1;
			}
			_value = Math.Min(_maxValue, _value);
			_value = Math.Max(_minValue, _value);

			if (key.KeyCode == TCODKeyCode.Enter || key.KeyCode == TCODKeyCode.KeypadEnter) {
				_actionCount(_value);
				ExitWindow();
			} else if (key.KeyCode == TCODKeyCode.Escape)
				ExitWindow();
		}
	}	
}