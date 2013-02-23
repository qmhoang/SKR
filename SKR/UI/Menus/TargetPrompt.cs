using System;
using System.Linq;
using DEngine.Core;
using DEngine.Level;
using Ogui.Core;
using Ogui.UI;
using SKR.UI.Gameplay;
using libtcod;

namespace SKR.UI.Menus {
	public class TargetPrompt : PromptWindow {
		private readonly Action<Point> actionPosition;
		private Point selectedPosition;
		private Point origin;
		private MapPanel panel;
		private string message;

		public TargetPrompt(string message, Point origin, Action<Point> actionPosition, MapPanel panel, PromptWindowTemplate template) : this(message, origin, origin, actionPosition, panel, template) { }

		public TargetPrompt(string message, Point origin, Point initialPoint, Action<Point> actionPosition, MapPanel panel, PromptWindowTemplate template)
				: base(template) {
			this.actionPosition = actionPosition;
			selectedPosition = initialPoint;
			this.origin = origin;
			this.panel = panel;
			this.message = message;
		}

		protected override void OnAdded() {
			base.OnSettingUp();

			panel.Draw += PanelDraw;
		}

		protected override void OnRemoved() {
			base.OnRemoved();
			panel.Draw -= PanelDraw;
		}

		private void PanelDraw(object sender, EventArgs e) {
			if ((!(sender is MapPanel)))
				return;
			var mapPanel = (MapPanel)sender;
			var pointList = Bresenham.GeneratePointsFromLine(origin, selectedPosition).ToList();

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
			get { return string.Format("{0} Select position. [Enter] to select. [Esc] to exit.", message); }
		}

		protected override void OnKeyPressed(KeyboardData key) {
			base.OnKeyPressed(key);

			if (key.KeyCode == TCODKeyCode.KeypadEight || key.KeyCode == TCODKeyCode.Up)
				selectedPosition += Direction.North;

			if (key.KeyCode == TCODKeyCode.KeypadTwo || key.KeyCode == TCODKeyCode.Down)
				selectedPosition += Direction.South;

			if (key.KeyCode == TCODKeyCode.KeypadFour || key.KeyCode == TCODKeyCode.Left)
				selectedPosition += Direction.West;

			if (key.KeyCode == TCODKeyCode.KeypadSix || key.KeyCode == TCODKeyCode.Right)
				selectedPosition += Direction.East;

			if (key.KeyCode == TCODKeyCode.KeypadSeven)
				selectedPosition += Direction.Northwest;

			if (key.KeyCode == TCODKeyCode.KeypadNine)
				selectedPosition += Direction.Northeast;

			if (key.KeyCode == TCODKeyCode.KeypadOne)
				selectedPosition += Direction.Southwest;

			if (key.KeyCode == TCODKeyCode.KeypadThree)
				selectedPosition += Direction.Southeast;

			if (key.KeyCode == TCODKeyCode.Escape)
				ExitWindow();

			if (key.KeyCode == TCODKeyCode.Enter || key.KeyCode == TCODKeyCode.KeypadEnter) {
				actionPosition(selectedPosition);
				ExitWindow();
			}
		}
	}
}