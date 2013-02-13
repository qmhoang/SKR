using System;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Systems;
using SkrGame.Universe;

namespace SkrGame.Core {
	public class LongAction : Component, IUpdateable {
		public int ActionLengthInAP { get; set; }
		public Action<Entity> ActionComplete { get; set; }

		public LongAction(int actionLengthInAP, Action<Entity> action) {
			ActionLengthInAP = actionLengthInAP;
			ActionComplete = action;
		}

//		public override void Receive(string message, EventArgs e) {
//			base.Receive(message, e);
//
//			if (message == "Update") {
//				var m = (UpdateEvent) e;
//				m.Entity.Get<ActionPoint>().ActionPoints -= m.Entity.Get<ActionPoint>().ActionPointPerTurn;
//				ActionLengthInAP -= m.Entity.Get<ActionPoint>().ActionPointPerTurn;
//
//				if (ActionLengthInAP <= 0) {
//					m.Entity.Remove<LongAction>();
//					ActionComplete(m.Entity);
//				}
//			}
//		}

		public override Component Copy() {
			return new LongAction(ActionLengthInAP, ActionComplete);
		}

		public void Update() {
			
		}
	}
}