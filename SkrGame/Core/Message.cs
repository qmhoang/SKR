using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkrGame.Core {
    public enum MessageType {
        Low,
        Normal,
        High,
        Special,
    }

    public class Message {
        public string Text { get; set; }
        public MessageType Type { get; set; }

        public Message(string text, MessageType type) {
            Text = text;
            Type = type;
        }
    }
}
