﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
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

	public class Log {
		public event EventHandler<EventArgs> Logged;

		private void OnLogged(EventArgs e) {
			EventHandler<EventArgs> handler = Logged;
			if (handler != null)
				handler(this, e);
		}

		public ReadOnlyCollection<MessageEntry> Entries { get { return entries.AsReadOnly(); } }		

		private List<MessageEntry> entries;

		public Log() {
			entries = new List<MessageEntry>();
		}

		public void Write(MessageType type, string text) {
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(text), "string \"text\" cannot be null or empty");

			if (entries.Count > 0 && entries[entries.Count - 1].Text == text) {
				entries[entries.Count - 1].Count++;
			} else {
				entries.Add(new MessageEntry(text, type));
			}

			OnLogged(EventArgs.Empty);
		}

		public void Fail(string text) {
			Write(MessageType.Fail, text);
		}

		public void Bad(string text) {
			Write(MessageType.Bad, text);
		}

		public void Normal(string text) {
			Write(MessageType.Normal, text);
		}

		public void Good(string text) {
			Write(MessageType.Good, text);
		}

		public void Special(string text) {
			Write(MessageType.Special, text);
		}
	}
}
