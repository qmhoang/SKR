using System;
using System.Diagnostics.Contracts;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;

namespace SkrGame.Actions.Movement {
	public class ChangePostureAction : LoggedAction {
		private Posture posture;
		private Posture current;

		public ChangePostureAction(Entity entity, Posture posture)
				: base(entity) {
			Contract.Requires<ArgumentNullException>(entity != null, "entity");
			Contract.Requires<ArgumentException>(entity.Has<Creature>());
			
			this.current = entity.Get<Creature>().Posture;
			this.posture = posture;
			apcost = World.OneSecondInAP / 2;
		}

		private int apcost;

		public override int APCost {
			get { return apcost; }	// todo fat people take more time?
		}

		public override ActionResult OnProcess() {
			// stand -> run and run -> stand
			if ((current == Posture.Stand && posture == Posture.Run) || 
				(current == Posture.Run && posture == Posture.Stand)) {

				apcost = World.OneSecondInAP / 10;
				SetPosture(posture);
				return ActionResult.Success;
			}
			// crouch -> run and run -> crouch
			if ((current == Posture.Crouch && posture == Posture.Run) || 
				(current == Posture.Run && posture == Posture.Crouch)) {

				SetPosture(posture);
				return ActionResult.Success;
			}
			// stand -> crouch and crouch -> stand
			if ((current == Posture.Crouch && posture == Posture.Stand) || 
				(current == Posture.Stand && posture == Posture.Crouch)) {

				SetPosture(posture);
				return ActionResult.Success;
			}
			// crouch -> prone and prone -> crouch
			if ((current == Posture.Crouch && posture == Posture.Prone) || 
				(current == Posture.Prone && posture == Posture.Crouch)) {

				SetPosture(posture);
				return ActionResult.Success;
			}
			// prone -> stand or prone -> run requires we crouch first
			if ((current == Posture.Prone && posture == Posture.Stand) || 
				(current == Posture.Prone && posture == Posture.Run)) {

				SetPosture(Posture.Crouch);
				Entity.Get<ActorComponent>().Enqueue(new ChangePostureAction(Entity, posture));
				return ActionResult.Success;
			}
			// stand -> prone and run -> prone
			if ((current == Posture.Stand && posture == Posture.Prone) || 
				(current == Posture.Run && posture == Posture.Prone)) {

				SetPosture(posture);
				return ActionResult.Success;
			}

			World.Log.Normal(String.Format("{0} is already doing that.", EntityName));
			return ActionResult.Aborted;
		}

		private void SetPosture(Posture p) {
			switch (p) {
				case Posture.Run:
					World.Log.Normal(String.Format("{0} starts to run.", EntityName));
					break;
				case Posture.Stand:
					World.Log.Normal(Entity.Get<Creature>().Posture == Posture.Run
					                 		? String.Format("{0} stops running.", EntityName)
					                 		: String.Format("{0} stands up.", EntityName));
					break;
				case Posture.Crouch:
					World.Log.Normal(Entity.Get<Creature>().Posture == Posture.Prone 
						? String.Format("{0} pushes up into a crouch.", EntityName) 
						: String.Format("{0} crouches down.", EntityName));
					break;
				case Posture.Prone:
					World.Log.Normal(String.Format("{0} goes prone.", EntityName));
					break;
				default:
					throw new ArgumentOutOfRangeException("p");
			}
			Entity.Get<Creature>().Posture = p;
		}
	}
}