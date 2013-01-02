using System;
using DEngine.Core;
using Ogui.Core;
using Ogui.UI;
using SkrGame.Core;

namespace SKR.UI.Gameplay {
	public class MessagePanel : Panel {
		private int currLine;
		private Canvas textCanvas;
		private bool alreadyDisposed;

		public MessagePanel(PanelTemplate template)
			: base(template) {
			currLine = 0;
			textCanvas = new Canvas(Size.Width - 2, Size.Height - 2);
		}

		public void HandleMessage(Message message) {
			int linesNeeded = ((message.Text.Length + 2) / textCanvas.Size.Width) + 1; // +2 message length for the * (and space).

			if ((linesNeeded + currLine) > textCanvas.Size.Height) {
				textCanvas.Scroll(0, textCanvas.Size.Height - (linesNeeded + currLine));
				currLine += textCanvas.Size.Height - (linesNeeded + currLine);
			}

			textCanvas.PrintString(0, currLine, "*", new Pigment(ColorPresets.Red, Pigments[PigmentType.ViewNormal].Background));

			switch (message.Type) {
				case MessageType.Normal:
					textCanvas.PrintString(0, currLine, "* ", Pigments[PigmentType.Window].ReplaceForeground(ColorPresets.Yellow));
					break;
				case MessageType.High:
					textCanvas.PrintString(0, currLine, "* ", Pigments[PigmentType.Window].ReplaceForeground(ColorPresets.Red));
					break;
				case MessageType.Low:
					textCanvas.PrintString(0, currLine, "* ", Pigments[PigmentType.Window].ReplaceForeground(ColorPresets.LightGreen));
					break;
				case MessageType.Special:
					textCanvas.PrintString(0, currLine, "* ", Pigments[PigmentType.Window].ReplaceForeground(ColorPresets.Blue));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			textCanvas.Console.printRect(2, currLine, Size.Width - 4, Size.Height - 2, message.Text);

			currLine += linesNeeded;
		}

		protected override void Redraw() {
			base.Redraw();

			Canvas.Blit(textCanvas, Point.One);
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