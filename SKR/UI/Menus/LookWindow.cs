using System;
using DEngine.Core;
using Ogui.Core;
using Ogui.UI;
using SKR.UI.Gameplay;
using SKR.UI.Prompts;
using libtcod;

namespace SKR.UI.Menus {
	public class LookWindow : PromptWindow {
		protected Point SelectedPosition;
		private MapPanel _panel;
		private Func<Point, string> _toStringFunction;

		public LookWindow(Point origin, Func<Point, string> toStringFunction, MapPanel panel, PromptWindowTemplate template)
				: base(template) {
			this._panel = panel;
			this._toStringFunction = toStringFunction;
			SelectedPosition = origin;
		}

		protected override string Text {
			get {
				return String.Format("Use [789456123] to look around.\n{0}", _toStringFunction(SelectedPosition));				
			}
		}

		protected override void OnAdded() {
			base.OnSettingUp();

			_panel.Draw += PanelDraw;
		}

		protected override void OnRemoved() {
			base.OnRemoved();
			_panel.Draw -= PanelDraw;
		}

		private void PanelDraw(object sender, EventArgs e) {
			if (!(sender is MapPanel))
				return;

			var mapPanel = (MapPanel) sender;

			var adjusted = SelectedPosition - mapPanel.ViewOffset;

			mapPanel.Canvas.Console.setCharBackground(adjusted.X, adjusted.Y,
			                                          ColorPresets.Green.TCODColor,
			                                          TCODSystem.getElapsedMilli() % 1000 > 500 ? TCODBackgroundFlag.Lighten : TCODBackgroundFlag.None);
		}

		protected virtual void MovePosition(Direction direction) {
			SelectedPosition += direction;
		}		

		protected override void OnKeyPressed(KeyboardData key) {
			base.OnKeyPressed(key);

			if (key.KeyCode == TCODKeyCode.KeypadEight || key.KeyCode == TCODKeyCode.Up)
				MovePosition(Direction.North);

			if (key.KeyCode == TCODKeyCode.KeypadTwo || key.KeyCode == TCODKeyCode.Down)
				MovePosition(Direction.South);

			if (key.KeyCode == TCODKeyCode.KeypadFour || key.KeyCode == TCODKeyCode.Left)
				MovePosition(Direction.West);

			if (key.KeyCode == TCODKeyCode.KeypadSix || key.KeyCode == TCODKeyCode.Right)
				MovePosition(Direction.East);

			if (key.KeyCode == TCODKeyCode.KeypadSeven)
				MovePosition(Direction.Northwest);

			if (key.KeyCode == TCODKeyCode.KeypadNine)
				MovePosition(Direction.Northeast);

			if (key.KeyCode == TCODKeyCode.KeypadOne)
				MovePosition(Direction.Southwest);

			if (key.KeyCode == TCODKeyCode.KeypadThree)
				MovePosition(Direction.Southeast);

			if (key.KeyCode == TCODKeyCode.Escape)
				ExitWindow();
		}
	}
}