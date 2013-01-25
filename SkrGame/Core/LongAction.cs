using System;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Core.ComponentMessages;
using SkrGame.Systems;
using SkrGame.Universe;

namespace SkrGame.Core {
	public class LongAction : Component {
		public int APLength { get; set; }
		public Action<Entity> ActionComplete { get; set; }

		public LongAction(int apLength, Action<Entity> action) {
			APLength = apLength;
			ActionComplete = action;
		}

		public override void Receive(IComponentMessage data) {
			base.Receive(data);

			if (data is UpdateMessage) {
				var m = (UpdateMessage) data;
				m.Entity.Get<ActionPoint>().ActionPoints -= m.Entity.Get<ActionPoint>().Speed;
				APLength -= m.Entity.Get<ActionPoint>().Speed;

				if (APLength <= 0) {
					m.Entity.Remove<LongAction>();
					ActionComplete(m.Entity);
				}
			}
		}

		public override Component Copy() {
			return new LongAction(APLength, ActionComplete);
		}
	}
}