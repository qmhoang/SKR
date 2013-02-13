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
using libtcod;
using log4net;

namespace SKR.UI.Menus {
	public class PromptWindowTemplate : WindowTemplate {
		public Log Log { get; set; }
	}

	public abstract class PromptWindow : Window {
		protected abstract string Text { get; }
		
		protected static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		protected Log GameLog;
		
		protected PromptWindow(PromptWindowTemplate template)
			: base(template) {
			IsPopup = true;
			GameLog = template.Log;
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

		public BooleanPrompt(string message, bool defaultBooleanAction, Action<bool> actionBoolean, PromptWindowTemplate template)
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

	public class LookWindow : PromptWindow {
		protected Point SelectedPosition;
		private MapPanel panel;
		private Func<Point, string> descriptor;

		public LookWindow(Point origin, Func<Point, string> descriptor, MapPanel panel, PromptWindowTemplate template)
			: base(template) {
			this.panel = panel;
			this.descriptor = descriptor;
			SelectedPosition = origin;
		}

		protected override string Text {
			get {
				return String.Format("Use [789456123] to look around.\n{0}", descriptor(SelectedPosition));				
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

	public class TargetPrompt : PromptWindow {
		private readonly Action<Point> actionPosition;
		private Point selectedPosition;
		private Point origin;
		private MapPanel panel;
		private string message;

		public TargetPrompt(string message, Point origin, Action<Point> actionPosition, MapPanel panel, PromptWindowTemplate template)
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

	public class OptionsSelectionPrompt<T> : PromptWindow {
		private readonly Action<T> actionCount;
		private List<T> options;
		private Func<T, string> descriptorFunction;
		private string message;

		public OptionsSelectionPrompt(string message, IEnumerable<T> options, Func<T, string> descriptor, Action<T> actionCount, PromptWindowTemplate template)
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
