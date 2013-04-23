using System.Collections.Generic;
using DEngine.Actions;
using DEngine.Actor;
using DEngine.Core;
using DEngine.Level;
using SkrGame.Actions;
using SkrGame.Actions.Combat;
using SkrGame.Actions.Movement;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Locations;

namespace SkrGame.Universe.Entities.Controllers {
	public class NPC : Controller {
		private AStarPathFinder _pf;
		private Point _oldPos;
		private Level _level;
		
		public NPC() : this(new Queue<IAction>()) { }

		private NPC(Queue<IAction> actions) {
			Actions = actions;
		}

		public Queue<IAction> Actions { get; private set; }

		public override IAction NextAction() {
			return Actions.Count == 0 ? CalculateNextMove() : Actions.Dequeue();
		}

		
		private ActorAction CalculateNextMove() {
			var location = Holder.Entity.Get<GameObject>();

			if (_level != location.Level) {
				_level = location.Level;
				_oldPos = location.Location;
				_pf = new AStarPathFinder(_level, 1.41f);				
			}
			

			_oldPos = location.Location;
			
			var player = _level.World.Player;

			var target = player.Get<GameObject>().Location;

			var sight = Holder.Entity.Get<SightComponent>();
			sight.CalculateSight();
			if (sight.IsVisible(target)) {
				var distance = location.Location.DistanceTo(target);

				if (distance <= 1.5)
					return new MeleeAttackAction(Holder.Entity, player, Holder.Entity, player.Get<DefendComponent>().GetRandomPart());
				else {
					_pf.Compute(location.X, location.Y, target.X, target.Y);
					int nx = location.X, ny = location.Y;

					if (_pf.Walk(ref nx, ref ny, false)) {
						var newPosition = new Point(nx, ny);
						Direction dir = Direction.Towards(newPosition - location.Location);

						return new BumpAction(Holder.Entity, dir);
					}
				}
			}
			return new WaitAction(Holder.Entity);
		}

		public override void Cancel() {
			base.Cancel();

			Actions.Dequeue();
		}

		public override Controller Copy() {
			return new NPC(new Queue<IAction>(Actions));
		}

		public override void Disturb() {
			base.Disturb();

			Actions.Clear();
		}
	}
}