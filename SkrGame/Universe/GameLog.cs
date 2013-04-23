using System;
using DEngine.Core;

namespace SkrGame.Universe {
	public enum MessageType {
		Aborted,
		Fail,
		Bad,
		Normal,
		Good,
		Special,
	}

	public class GameLog : Log<MessageType> {
		public GameLog() {}

		public void Aborted(string text) {
			Write(MessageType.Aborted, text);
		}

		public void AbortedFormat(string text, params object[] args) {
			Write(MessageType.Aborted, String.Format(text, args));
		}

		public void Fail(string text) {
			Write(MessageType.Fail, text);
		}

		public void FailFormat(string text, params object[] args) {
			Write(MessageType.Fail, String.Format(text, args));
		}

		public void Bad(string text) {
			Write(MessageType.Bad, text);
		}

		public void BadFormat(string text, params object[] args) {
			Write(MessageType.Bad, String.Format(text, args));
		}

		public void Normal(string text) {
			Write(MessageType.Normal, text);
		}

		public void NormalFormat(string text, params object[] args) {
			Write(MessageType.Normal, String.Format(text, args));
		}

		public void Good(string text) {
			Write(MessageType.Good, text);
		}

		public void GoodFormat(string text, params object[] args) {
			Write(MessageType.Good, String.Format(text, args));
		}

		public void Special(string text) {
			Write(MessageType.Special, text);
		}

		public void SpecialFormat(string text, params object[] args) {
			Write(MessageType.Special, String.Format(text, args));
		}
	}
}