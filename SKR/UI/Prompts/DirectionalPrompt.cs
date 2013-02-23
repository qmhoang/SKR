using System;
using DEngine.Core;
using Ogui.UI;
using SKR.UI.Menus;
using libtcod;

namespace SKR.UI.Prompts {
	public class DirectionalPrompt : PromptWindow {
		private readonly Action<Point> actionPosition;
		private Point origin;
		private string message;

		/// <summary>
		/// The action is done on a local position case.  North = 0, -1.  West = -1, 0
		/// </summary>
		/// <param name="message"></param>
		/// <param name = "origin"></param>
		/// <param name="actionPosition"></param>
		public DirectionalPrompt(string message, Point origin, Action<Point> actionPosition, PromptWindowTemplate template)
				: base(template) {
			this.actionPosition = actionPosition;
			this.origin = origin;
			this.message = message;
		}

		protected override string Text {
			get { return string.Format("{0} Which direction? [789456123], Any other key = Abort.", message); }
		}

		protected override void OnKeyPressed(KeyboardData key) {
			base.OnKeyPressed(key);

			if (key.KeyCode == TCODKeyCode.KeypadEight || key.KeyCode == TCODKeyCode.Up)
				actionPosition(origin + Direction.North);

			if (key.KeyCode == TCODKeyCode.KeypadTwo || key.KeyCode == TCODKeyCode.Down)
				actionPosition(origin + Direction.South);

			if (key.KeyCode == TCODKeyCode.KeypadFour || key.KeyCode == TCODKeyCode.Left)
				actionPosition(origin + Direction.West);

			if (key.KeyCode == TCODKeyCode.KeypadSix || key.KeyCode == TCODKeyCode.Right)
				actionPosition(origin + Direction.East);

			if (key.KeyCode == TCODKeyCode.KeypadSeven)
				actionPosition(origin + Direction.Northwest);

			if (key.KeyCode == TCODKeyCode.KeypadNine)
				actionPosition(origin + Direction.Northeast);

			if (key.KeyCode == TCODKeyCode.KeypadOne)
				actionPosition(origin + Direction.Southwest);

			if (key.KeyCode == TCODKeyCode.KeypadThree)
				actionPosition(origin + Direction.Southeast);

			ExitWindow();
		}
	}
}