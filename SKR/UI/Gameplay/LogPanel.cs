using System;
using System.Linq;
using DEngine.Core;
using DEngine.Extensions;
using Ogui.Core;
using Ogui.UI;
using SkrGame.Universe;

namespace SKR.UI.Gameplay {
	public class LogPanelTemplate : PanelTemplate {
		public GameLog Log { get; set; }
	}

	public class LogPanel : Panel {
		private readonly int _maxNumOfDisplayLines;
		private readonly int _maxNumberOfCharacters;
		private VScrollBar _vScrollBar;
		private bool _alreadyDisposed;
		private GameLog _log;

		public LogPanel(LogPanelTemplate template) : base(template) {
			_log = template.Log;
			_maxNumOfDisplayLines = Size.Height - 2;
			_maxNumberOfCharacters = Size.Width - 3;
		}

		protected override void OnSettingUp() {
			base.OnSettingUp();
			_vScrollBar = new VScrollBar(new VScrollBarTemplate
			                            {
											Height = Size.Height - 2,
											MinimumValue = 0,
											StartingValue = _log.Entries.Count > _maxNumOfDisplayLines ? _log.Entries.Count - _maxNumOfDisplayLines : 0,
											MaximumValue = _log.Entries.Count,
											TopLeftPos = ScreenRect.TopRight.Shift(-2, 1),
											SpinDelay = 100,
											SpinSpeed = 50,
			                            });
			_log.Logged += OnNewEntry;
		}

		protected override void OnAdded() {
			base.OnAdded();
			ParentWindow.AddControl(_vScrollBar);
		}

		protected override void OnRemoved() {
			base.OnRemoved();
			ParentWindow.RemoveControl(_vScrollBar);
		}

		protected override void Redraw() {
			base.Redraw();

			int lineIndex = _vScrollBar.CurrentValue;
			int y = 0;

			while (y < _maxNumOfDisplayLines && lineIndex < _log.Entries.Count) {
				var entry = _log.Entries[lineIndex];

				Color fgColor;

				switch (entry.Type) {
					case MessageType.Aborted:
						fgColor = ColorPresets.Gray;
						break;
					case MessageType.Fail:
						fgColor = ColorPresets.LighterRed;
						break;
					case MessageType.Bad:
						fgColor = ColorPresets.Red;
						break;
					case MessageType.Normal:
						fgColor = ColorPresets.White;
						break;
					case MessageType.Good:
						fgColor = ColorPresets.Green;
						break;
					case MessageType.Special:
						fgColor = ColorPresets.Purple;
						break;
					default:
						fgColor = ColorPresets.White;
						break;
				}
				var lines = string.Format("* {0}", (entry.Count > 1 ? string.Format("{0}(x{1})", entry.Text, entry.Count) : entry.Text)).WordWrap(_maxNumberOfCharacters);

				for (int j = 0; j < lines.Length; j++) {
					WriteLine(j == 0 ? 2 : 4, y + 1, lines[j], fgColor);
					y++;
				}
				lineIndex++;
			}
		}

		private void OnNewEntry(Log<MessageType> sender, EventArgs e) {
			if (_log.Entries[_log.Entries.Count - 1].Count == 1)
				_vScrollBar.MaximumValue++;
			
			if (_log.Entries.Count - _vScrollBar.CurrentValue > _maxNumOfDisplayLines) {
				_vScrollBar.CurrentValue++;
			}

		}

		private void WriteLine(int x, int y, string message, Color fgColor) {
			Canvas.PrintString(x, y, message, Pigments[PigmentType.Window].ReplaceForeground(fgColor));			
		}

		protected override void Dispose(bool isDisposing) {
			base.Dispose(isDisposing);

			if (_alreadyDisposed)
				return;
			if (isDisposing)
				if (_vScrollBar != null)
					_vScrollBar.Dispose();
			_alreadyDisposed = true;
		}
	}
}