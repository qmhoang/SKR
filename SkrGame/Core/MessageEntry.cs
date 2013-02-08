using System.Linq;
using System.Text;
using NUnit.Framework;

namespace SkrGame.Core {
	public enum MessageType {
		Fail,
		Bad,
		Normal,
		Good,
		Special,
	}

	public class MessageEntry {
		public string Text { get; set; }
		public MessageType Type { get; set; }
		public int Count { get; set; }

		public MessageEntry(string text, MessageType type) {
			Text = text;
			Type = type;
			Count = 1;
		}
	}
}
