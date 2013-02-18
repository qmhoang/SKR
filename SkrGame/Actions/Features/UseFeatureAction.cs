using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;

namespace SkrGame.Actions.Features {
	public class UseFeatureAction : FeatureAction {
		private int apLength;

		public UseFeatureAction(Entity entity, Entity feature, int apLength) : base(entity, feature) {
			this.apLength = apLength;
		}

		public override int APCost {
			get {
				var actionPointPerTurn = Entity.Get<ActorComponent>().AP.ActionPointPerTurn;
				return apLength < actionPointPerTurn ? apLength : actionPointPerTurn;
			}
		}

		public override ActionResult OnProcess() {
			var actionPointPerTurn = Entity.Get<ActorComponent>().AP.ActionPointPerTurn;

			World.Log.Normal(String.Format("{0} uses entity.  {1} left.", Identifier.GetNameOrId(Entity), apLength - actionPointPerTurn));

			if (apLength > actionPointPerTurn) {
				Entity.Get<ActorComponent>().Enqueue(new UseFeatureAction(Entity, Feature, apLength - actionPointPerTurn));
				return ActionResult.Success;
			}
			return ActionResult.Success;
		}
	}
}
