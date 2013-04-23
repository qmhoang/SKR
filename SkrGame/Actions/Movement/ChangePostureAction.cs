using System;
using System.Diagnostics.Contracts;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;

namespace SkrGame.Actions.Movement {
	public class ChangePostureAction : LoggedAction {
		private Posture _posture;
		private Posture _current;

		public ChangePostureAction(Entity entity, Posture posture)
				: base(entity) {
			Contract.Requires<ArgumentNullException>(entity != null, "entity");
			Contract.Requires<ArgumentException>(entity.Has<Creature>());
			
			this._current = entity.Get<Creature>().Posture;
			this._posture = posture;
			_apcost = World.OneSecondInAP / 2;
		}

		private int _apcost;

		public override int APCost {
			get { return _apcost; }	// todo fat people take more time?
		}

		public override ActionResult OnProcess() {
			// stand -> run and run -> stand
			if ((_current == Posture.Stand && _posture == Posture.Run) || 
				(_current == Posture.Run && _posture == Posture.Stand)) {

				_apcost = World.OneSecondInAP / 10;
				SetPosture(_posture);
				return ActionResult.Success;
			}
			// crouch -> run and run -> crouch
			if ((_current == Posture.Crouch && _posture == Posture.Run) || 
				(_current == Posture.Run && _posture == Posture.Crouch)) {

				SetPosture(_posture);
				return ActionResult.Success;
			}
			// stand -> crouch and crouch -> stand
			if ((_current == Posture.Crouch && _posture == Posture.Stand) || 
				(_current == Posture.Stand && _posture == Posture.Crouch)) {

				SetPosture(_posture);
				return ActionResult.Success;
			}
			// crouch -> prone and prone -> crouch
			if ((_current == Posture.Crouch && _posture == Posture.Prone) || 
				(_current == Posture.Prone && _posture == Posture.Crouch)) {

				SetPosture(_posture);
				return ActionResult.Success;
			}
			// prone -> stand or prone -> run requires we crouch first
			if ((_current == Posture.Prone && _posture == Posture.Stand) || 
				(_current == Posture.Prone && _posture == Posture.Run)) {

				SetPosture(Posture.Crouch);
				Entity.Get<ActorComponent>().Enqueue(new ChangePostureAction(Entity, _posture));
				return ActionResult.Success;
			}
			// stand -> prone and run -> prone
			if ((_current == Posture.Stand && _posture == Posture.Prone) || 
				(_current == Posture.Run && _posture == Posture.Prone)) {

				SetPosture(_posture);
				return ActionResult.Success;
			}

			Log.NormalFormat("{0} is already doing that.", EntityName);
			return ActionResult.Aborted;
		}

		private void SetPosture(Posture p) {
			switch (p) {
				case Posture.Run:
					Log.Normal(Entity.Get<Creature>().Posture == Posture.Stand
					           		? String.Format("{0} starts to run.", EntityName)
					           		: String.Format("{0} starts running from a crouch.", EntityName));
					break;
				case Posture.Stand:
					Log.Normal(Entity.Get<Creature>().Posture == Posture.Run
					                 		? String.Format("{0} stops running.", EntityName)
					                 		: String.Format("{0} stands up.", EntityName));
					break;
				case Posture.Crouch:
					Log.Normal(Entity.Get<Creature>().Posture == Posture.Prone 
						? String.Format("{0} pushes up into a crouch.", EntityName) 
						: String.Format("{0} crouches down.", EntityName));
					break;
				case Posture.Prone:
					Log.NormalFormat("{0} goes prone.", EntityName);
					break;
				default:
					throw new ArgumentOutOfRangeException("p");
			}
			Entity.Get<Creature>().Posture = p;
		}
	}
}