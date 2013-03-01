using System;
using System.Linq;
using DEngine.Core;
using DEngine.Extensions;
using Ogui.Core;
using Ogui.UI;

namespace SKR.UI.Gameplay {
	public class LogPanelTemplate : PanelTemplate {
		public Log Log { get; set; }
	}

	public class LogPanel : Panel {
		private readonly int maxNumOfDisplayLines;
		private readonly int maxNumberOfCharacters;
		private VScrollBar vScrollBar;
		private bool alreadyDisposed;
		private Log log;

		public LogPanel(LogPanelTemplate template) : base(template) {
			log = template.Log;
			maxNumOfDisplayLines = Size.Height - 2;
			maxNumberOfCharacters = Size.Width - 3;
		}

		protected override void OnSettingUp() {
			base.OnSettingUp();
			vScrollBar = new VScrollBar(new VScrollBarTemplate
			                            {
											Height = Size.Height - 2,
											MinimumValue = 0,
											StartingValue = log.Entries.Count > maxNumOfDisplayLines ? log.Entries.Count - maxNumOfDisplayLines : 0,
											MaximumValue = log.Entries.Count,
											TopLeftPos = ScreenRect.TopRight.Shift(-2, 1),
											SpinDelay = 100,
											SpinSpeed = 50,
			                            });
			log.Logged += OnNewEntry;
		}

		protected override void OnAdded() {
			base.OnAdded();
			ParentWindow.AddControl(vScrollBar);
		}

		protected override void OnRemoved() {
			base.OnRemoved();
			ParentWindow.RemoveControl(vScrollBar);
		}

		protected override void Redraw() {
			base.Redraw();

			int lineIndex = vScrollBar.CurrentValue;
			int y = 0;

			while (y < maxNumOfDisplayLines && lineIndex < log.Entries.Count) {
				var entry = log.Entries[lineIndex];

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
				var lines = string.Format("* {0}", (entry.Count > 1 ? string.Format("{0}(x{1})", entry.Text, entry.Count) : entry.Text)).WordWrap(maxNumberOfCharacters);

				for (int j = 0; j < lines.Length; j++) {
					WriteLine(j == 0 ? 2 : 4, y + 1, lines[j], fgColor);
					y++;
				}
				lineIndex++;
			}
		}

		private void OnNewEntry(Log sender, EventArgs e) {
			if (log.Entries[log.Entries.Count - 1].Count == 1)
				vScrollBar.MaximumValue++;
			
			if (log.Entries.Count - vScrollBar.CurrentValue > maxNumOfDisplayLines) {
				vScrollBar.CurrentValue++;
			}

		}

		private void WriteLine(int x, int y, string message, Color fgColor) {
			Canvas.PrintString(x, y, message, Pigments[PigmentType.Window].ReplaceForeground(fgColor));			
		}

		protected override void Dispose(bool isDisposing) {
			base.Dispose(isDisposing);

			if (alreadyDisposed)
				return;
			if (isDisposing)
				if (vScrollBar != null)
					vScrollBar.Dispose();
			alreadyDisposed = true;
		}
	}
}