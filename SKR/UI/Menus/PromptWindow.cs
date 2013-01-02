using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using DEngine.Core;
using DEngine.Extensions;
using Ogui.Core;
using Ogui.UI;
using SKR.UI.Gameplay;
using SKR.Universe;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors.PC;
using libtcod;
using log4net;

namespace SKR.UI.Menus {
	public abstract class PromptWindow : Window {
		protected abstract string Text { get; }


		protected static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


		protected PromptWindow(WindowTemplate template)
			: base(template) {
			IsPopup = true;
		}

		protected override void Redraw() {
			base.Redraw();

			Canvas.Console.setBackgroundFlag(TCODBackgroundFlag.Alpha);
			Canvas.PrintString(0, 0, "* ", Pigments[PigmentType.Window].ReplaceForeground(ColorPresets.Blue));
			Canvas.Console.printRect(2, 0, Size.Width - 4, Size.Height - 2, Text);
		}
	}

	public class BooleanPrompt : PromptWindow {
		private readonly Action<bool> actionBoolean;
		private readonly bool defaultBooleanAction;
		private string message;

		public BooleanPrompt(string message, bool defaultBooleanAction, Action<bool> actionBoolean, WindowTemplate template)
			: base(template) {
			this.defaultBooleanAction = defaultBooleanAction;
			this.actionBoolean = actionBoolean;
			this.message = message;
		}

		protected override string Text {
			get { return string.Format("{0}{1}, [Space] for default. Any other key = Abort.", message, (defaultBooleanAction ? " [Y/n]" : " [y/N]")); }
		}

		protected override void OnKeyPressed(KeyboardData keyData) {
			base.OnKeyPressed(keyData);

			if (keyData.Character == 'y')
				actionBoolean(true);
			if (keyData.Character == 'n' || keyData.KeyCode == TCODKeyCode.Escape)
				actionBoolean(false);
			if (keyData.KeyCode == TCODKeyCode.Space)
				actionBoolean(defaultBooleanAction);

			ExitWindow();
		}
	}

	public class CountPrompt : PromptWindow {
		private readonly Action<int> actionCount;
		private readonly int maxValue;
		private readonly int minValue;
		private int value;
		private bool negative; // this is used to store negative when you have a 0 since there is no negative 0
		private bool firstUse;
		private string message;

		public CountPrompt(string message, Action<int> actionCount, int maxValue, int minValue, int initialValue, WindowTemplate template)
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
		public DirectionalPrompt(string message, Point origin, Action<Point> actionPosition, WindowTemplate template)
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
				actionPosition(origin + Point.North);

			if (key.KeyCode == TCODKeyCode.KeypadTwo || key.KeyCode == TCODKeyCode.Down)
				actionPosition(origin + Point.South);

			if (key.KeyCode == TCODKeyCode.KeypadFour || key.KeyCode == TCODKeyCode.Left)
				actionPosition(origin + Point.West);

			if (key.KeyCode == TCODKeyCode.KeypadSix || key.KeyCode == TCODKeyCode.Right)
				actionPosition(origin + Point.East);

			if (key.KeyCode == TCODKeyCode.KeypadSeven)
				actionPosition(origin + Point.Northwest);

			if (key.KeyCode == TCODKeyCode.KeypadNine)
				actionPosition(origin + Point.Northeast);

			if (key.KeyCode == TCODKeyCode.KeypadOne)
				actionPosition(origin + Point.Southwest);

			if (key.KeyCode == TCODKeyCode.KeypadThree)
				actionPosition(origin + Point.Southeast);

			ExitWindow();
		}
	}

	public class LookWindow : PromptWindow {
		private Point selectedPosition;
		private MapPanel panel;
		private Func<Point, string> descriptor;

		public LookWindow(Point origin, Func<Point, string> descriptor, MapPanel panel, WindowTemplate template)
			: base(template) {
			this.panel = panel;
			this.descriptor = descriptor;
			selectedPosition = origin;
		}

		protected override string Text {
			get {
				var level = World.Instance.CurrentLevel;
				return String.Format("Use [789456123] to look around.\n{0}", descriptor(selectedPosition));				
			}
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

			var adjusted = selectedPosition - mapPanel.ViewOffset;

			mapPanel.Canvas.Console.setCharBackground(adjusted.X, adjusted.Y,
													  ColorPresets.Green.TCODColor,
													  TCODSystem.getElapsedMilli() % 1000 > 500 ? TCODBackgroundFlag.Lighten : TCODBackgroundFlag.None);
		}

		private void MovePosition(Point direction) {
			selectedPosition += direction;
		}

		protected override void OnKeyPressed(KeyboardData key) {
			base.OnKeyPressed(key);

			if (key.KeyCode == TCODKeyCode.KeypadEight || key.KeyCode == TCODKeyCode.Up)
				MovePosition(Point.North);

			if (key.KeyCode == TCODKeyCode.KeypadTwo || key.KeyCode == TCODKeyCode.Down)
				MovePosition(Point.South);

			if (key.KeyCode == TCODKeyCode.KeypadFour || key.KeyCode == TCODKeyCode.Left)
				MovePosition(Point.West);

			if (key.KeyCode == TCODKeyCode.KeypadSix || key.KeyCode == TCODKeyCode.Right)
				MovePosition(Point.East);

			if (key.KeyCode == TCODKeyCode.KeypadSeven)
				MovePosition(Point.Northwest);

			if (key.KeyCode == TCODKeyCode.KeypadNine)
				MovePosition(Point.Northeast);

			if (key.KeyCode == TCODKeyCode.KeypadOne)
				MovePosition(Point.Southwest);

			if (key.KeyCode == TCODKeyCode.KeypadThree)
				MovePosition(Point.Southeast);

			if (key.KeyCode == TCODKeyCode.Escape)
				ExitWindow();
		}
	}

	public class TargetPrompt : PromptWindow {
		private readonly Action<Point> actionPosition;
		private Point selectedPosition;
		private Point origin;
		private MapPanel panel;
		private string message;

		public TargetPrompt(string message, Point origin, Action<Point> actionPosition, MapPanel panel, WindowTemplate template)
			: base(template) {
			this.actionPosition = actionPosition;
			selectedPosition = origin;
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
			var pointList = Bresenham.GeneratePointsFromLine(origin, selectedPosition);

			bool path = true;

			for (int i = 0; i < pointList.Count; i++) {
				var point = pointList[i];
				if (path)
					path = World.Instance.CurrentLevel.IsWalkable(point);

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
				selectedPosition += Point.North;

			if (key.KeyCode == TCODKeyCode.KeypadTwo || key.KeyCode == TCODKeyCode.Down)
				selectedPosition += Point.South;

			if (key.KeyCode == TCODKeyCode.KeypadFour || key.KeyCode == TCODKeyCode.Left)
				selectedPosition += Point.West;

			if (key.KeyCode == TCODKeyCode.KeypadSix || key.KeyCode == TCODKeyCode.Right)
				selectedPosition += Point.East;

			if (key.KeyCode == TCODKeyCode.KeypadSeven)
				selectedPosition += Point.Northwest;

			if (key.KeyCode == TCODKeyCode.KeypadNine)
				selectedPosition += Point.Northeast;

			if (key.KeyCode == TCODKeyCode.KeypadOne)
				selectedPosition += Point.Southwest;

			if (key.KeyCode == TCODKeyCode.KeypadThree)
				selectedPosition += Point.Southeast;

			if (key.KeyCode == TCODKeyCode.Escape)
				ExitWindow();

			if (key.KeyCode == TCODKeyCode.Enter || key.KeyCode == TCODKeyCode.KeypadEnter) {
				actionPosition(selectedPosition);
				ExitWindow();
			}
		}
	}

	public class OptionsSelectionPrompt<T> : PromptWindow {
		private readonly Action<T> actionCount;
		private List<T> options;
		private Func<T, string> descriptorFunction;
		private string message;

		public OptionsSelectionPrompt(string message, List<T> options, Func<T, string> descriptor, Action<T> actionCount, WindowTemplate template)
			: base(template) {
			this.actionCount = actionCount;
			this.options = options;
			this.descriptorFunction = descriptor;
			this.message = message;
		}

		protected override void OnSettingUp() {
			base.OnSettingUp();
			if (options.Count <= 0) {
				World.Instance.AddMessage("No options to select from.");
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
