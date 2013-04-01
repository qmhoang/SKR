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
		private AStarPathFinder pf;
		private Point oldPos;
		private Level level;
		
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

			if (level != location.Level) {
				level = location.Level;
				oldPos = location.Location;
				pf = new AStarPathFinder(level, 1.41f);				
			}
			

			oldPos = location.Location;
			
			var player = level.World.Player;

			var target = player.Get<GameObject>().Location;

			var sight = Holder.Entity.Get<SightComponent>();
			sight.CalculateSight();
			if (sight.IsVisible(target)) {
				var distance = location.Location.DistanceTo(target);

				if (distance <= 1.5)
					return new MeleeAttackAction(Holder.Entity, player, Holder.Entity, player.Get<DefendComponent>().GetRandomPart());
				else {
					pf.Compute(location.X, location.Y, target.X, target.Y);
					int nx = location.X, ny = location.Y;

					if (pf.Walk(ref nx, ref ny, false)) {
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