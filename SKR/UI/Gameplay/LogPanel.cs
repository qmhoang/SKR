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
		private int currLine;
		private Canvas textCanvas;
		private bool alreadyDisposed;
		private Log log;

		public LogPanel(LogPanelTemplate template) : base(template) {
			currLine = 0;
			log = template.Log;
		}

		protected override void OnSettingUp() {
			base.OnSettingUp();
			textCanvas = new Canvas(Size.Width - 2, Size.Height - 2);
			log.Logged += OnNewEntry;
		}

		protected override void Redraw() {
			base.Redraw();

			Canvas.Blit(textCanvas, Point.One);
		}

		private void OnNewEntry(Log sender, EventArgs e) {
			WriteEntry(log.Entries.Last());
		}

		private void WriteEntry(MessageEntry entry) {
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

			if (entry.Count > 1) {
				string text = string.Format("* {0}", (entry.Count > 1 ? string.Format("{0}(x{1})", entry.Text, entry.Count) : entry.Text));

				var lines = text.WordWrap(textCanvas.Size.Width - 2);

				currLine -= lines.Length;

				for (int i = 0; i < lines.Length; i++) {
					if (i == 0)
						textCanvas.PrintString(0, currLine, lines[i], Pigments[PigmentType.Window].ReplaceForeground(fgColor));
					else {
						textCanvas.PrintString(2, currLine, lines[i], Pigments[PigmentType.Window].ReplaceForeground(fgColor));
					}
					currLine++;
				}	
			} else {
				string text = string.Format("* {0}", entry.Text);

				var lines = text.WordWrap(textCanvas.Size.Width - 2);

				if (currLine + lines.Length > textCanvas.Size.Height) {
					textCanvas.Scroll(0, textCanvas.Size.Height - (lines.Length + currLine));
					currLine += textCanvas.Size.Height - (lines.Length + currLine);
				}

				for (int i = 0; i < lines.Length; i++) {
					if (i == 0)
						textCanvas.PrintString(0, currLine, lines[i], Pigments[PigmentType.Window].ReplaceForeground(fgColor));
					else {
						textCanvas.PrintString(2, currLine, lines[i], Pigments[PigmentType.Window].ReplaceForeground(fgColor));
					}
					currLine++;
				}	
			}
		}

		protected override void Dispose(bool isDisposing) {
			base.Dispose(isDisposing);

			if (alreadyDisposed)
				return;
			if (isDisposing)
				if (textCanvas != null)
					textCanvas.Dispose();
			alreadyDisposed = true;
		}
	}
}