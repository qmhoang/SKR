using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using DEngine.Core;
using DEngine.Entities;
using DEngine.Extensions;

namespace SkrGame.Universe.Entities {
	public class PositionChangedEvent : EventArgs {
		public Point Previous { get; private set; }
		public Point Current { get; private set; }

		public PositionChangedEvent(Point prev, Point curr) {
			Previous = prev;
			Current = curr;
		}
	}
}
