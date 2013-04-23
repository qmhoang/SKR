using System;
using System.Linq;
using DEngine.Core;
using DEngine.Level;
using Ogui.Core;
using Ogui.UI;
using SKR.UI.Gameplay;
using libtcod;

namespace SKR.UI.Prompts {
	public class TargetPrompt : PromptWindow {
		private readonly Action<Point> _actionPosition;
		private Point _selectedPosition;
		private Point _origin;
		private MapPanel _panel;
		private string _message;

		public TargetPrompt(string message, Point origin, Action<Point> actionPosition, MapPanel panel, PromptWindowTemplate template) : this(message, origin, origin, actionPosition, panel, template) { }

		public TargetPrompt(string message, Point origin, Point initialPoint, Action<Point> actionPosition, MapPanel panel, PromptWindowTemplate template)
				: base(template) {
			this._actionPosition = actionPosition;
			_selectedPosition = initialPoint;
			this._origin = origin;
			this._panel = panel;
			this._message = message;
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
			if ((!(sender is MapPanel)))
				return;
			var mapPanel = (MapPanel)sender;
			var pointList = Bresenham.GeneratePointsFromLine(_origin, _selectedPosition).ToList();

			bool path = true;

			for (int i = 0; i < pointList.Count; i++) {
				var point = pointList[i];
				if (path)
					path = mapPanel.World.CurrentLevel.IsWalkable(point);

				var adjusted = point - mapPanel.ViewOffset;
				if (i == pointList.Count - 1)
					mapPanel.Canvas.Console.setCharBackground(adjusted.X, adjusted.Y,
					                                          path ? ColorPresets.Green.TCODColor : ColorPresets.Red.TCODColor,
					                                          TCODSystem.getElapsedMilli() % 1000 > 500 ? TCODBackgroundFlag.Lighten : TCODBackgroundFlag.None);
				else
					mapPanel.Canvas.Console.setCharBackground(adjusted.X, adjusted.Y,
					                                          path ? ColorPresets.Green.TCODColor : ColorPresets.Red.TCODColor,
					                                          TCODBackgroundFlag.Lighten);
			}
		}

		protected override string Text {
			get { return string.Format("{0} Select position. [Enter] to select. [Esc] to exit.", _message); }
		}

		protected override void OnKeyPressed(KeyboardData key) {
			base.OnKeyPressed(key);

			if (key.KeyCode == TCODKeyCode.KeypadEight || key.KeyCode == TCODKeyCode.Up)
				_selectedPosition += Direction.North;

			if (key.KeyCode == TCODKeyCode.KeypadTwo || key.KeyCode == TCODKeyCode.Down)
				_selectedPosition += Direction.South;

			if (key.KeyCode == TCODKeyCode.KeypadFour || key.KeyCode == TCODKeyCode.Left)
				_selectedPosition += Direction.West;

			if (key.KeyCode == TCODKeyCode.KeypadSix || key.KeyCode == TCODKeyCode.Right)
				_selectedPosition += Direction.East;

			if (key.KeyCode == TCODKeyCode.KeypadSeven)
				_selectedPosition += Direction.Northwest;

			if (key.KeyCode == TCODKeyCode.KeypadNine)
				_selectedPosition += Direction.Northeast;

			if (key.KeyCode == TCODKeyCode.KeypadOne)
				_selectedPosition += Direction.Southwest;

			if (key.KeyCode == TCODKeyCode.KeypadThree)
				_selectedPosition += Direction.Southeast;

			if (key.KeyCode == TCODKeyCode.Escape)
				ExitWindow();

			if (key.KeyCode == TCODKeyCode.Enter || key.KeyCode == TCODKeyCode.KeypadEnter) {
				_actionPosition(_selectedPosition);
				ExitWindow();
			}
		}
	}
}