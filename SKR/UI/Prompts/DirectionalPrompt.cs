using System;
using DEngine.Core;
using Ogui.UI;
using SKR.UI.Menus;
using libtcod;

namespace SKR.UI.Prompts {
	public class DirectionalPrompt : PromptWindow {
		private readonly Action<Point> _actionPosition;
		private Point _origin;
		private string _message;

		/// <summary>
		/// The action is done on a local position case.  North = 0, -1.  West = -1, 0
		/// </summary>
		/// <param name="message"></param>
		/// <param name = "origin"></param>
		/// <param name="actionPosition"></param>
		public DirectionalPrompt(string message, Point origin, Action<Point> actionPosition, PromptWindowTemplate template)
				: base(template) {
			this._actionPosition = actionPosition;
			this._origin = origin;
			this._message = message;
		}

		protected override string Text {
			get { return string.Format("{0} Which direction? [789456123], Any other key = Abort.", _message); }
		}

		protected override void OnKeyPressed(KeyboardData key) {
			base.OnKeyPressed(key);

			if (key.KeyCode == TCODKeyCode.KeypadEight || key.KeyCode == TCODKeyCode.Up)
				_actionPosition(_origin + Direction.North);

			if (key.KeyCode == TCODKeyCode.KeypadTwo || key.KeyCode == TCODKeyCode.Down)
				_actionPosition(_origin + Direction.South);

			if (key.KeyCode == TCODKeyCode.KeypadFour || key.KeyCode == TCODKeyCode.Left)
				_actionPosition(_origin + Direction.West);

			if (key.KeyCode == TCODKeyCode.KeypadSix || key.KeyCode == TCODKeyCode.Right)
				_actionPosition(_origin + Direction.East);

			if (key.KeyCode == TCODKeyCode.KeypadSeven)
				_actionPosition(_origin + Direction.Northwest);

			if (key.KeyCode == TCODKeyCode.KeypadNine)
				_actionPosition(_origin + Direction.Northeast);

			if (key.KeyCode == TCODKeyCode.KeypadOne)
				_actionPosition(_origin + Direction.Southwest);

			if (key.KeyCode == TCODKeyCode.KeypadThree)
				_actionPosition(_origin + Direction.Southeast);

			ExitWindow();
		}
	}
}