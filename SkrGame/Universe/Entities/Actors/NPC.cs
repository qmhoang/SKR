using DEngine.Actions;
using DEngine.Components;
using DEngine.Core;
using SkrGame.Actions;
using SkrGame.Universe.Locations;

namespace SkrGame.Universe.Entities.Actors {
	public class NPC : AbstractActor {
		private AStarPathFinder pf;
		private VisionMap vision;
		private Point oldPos;
		private Level level;

		public NPC() { }

		private void ComputeFOV(Location position) {
			
			ShadowCastingFOV.ComputeRecursiveShadowcasting(vision, position.Level, position.Point.X, position.Point.Y, 10, true);
		}

		public override ActorAction NextAction() {
			if (Actions.Count == 0) {
				return CalculateNextMove();				
			} else {
				return Actions.Dequeue();
			}

		}

		private ActorAction CalculateNextMove() {
			var location = Holder.Entity.Get<Location>();

			if (level != location.Level) {
				level = location.Level;
				oldPos = location.Point;
				pf = new AStarPathFinder(level, 1.41f);
				vision = new VisionMap(level.Size);
				ComputeFOV(Holder.Entity.Get<Location>());
			}

			if (oldPos != location.Point) {
				oldPos = location.Point;
				ComputeFOV(Holder.Entity.Get<Location>());
			}

			var player = level.World.Player;

			var target = player.Get<Location>().Point;

			if (vision.IsVisible(target)) {
				var distance = location.Point.DistanceTo(target);

				if (distance <= 1.5)
					return new MeleeAttackAction(Holder.Entity, player, Holder.Entity, player.Get<DefendComponent>().GetRandomPart());
				else {
					pf.Compute(location.X, location.Y, target.X, target.Y);
					int nx = location.X, ny = location.Y;

					if (pf.Walk(ref nx, ref ny, false)) {
						var newPosition = new Point(nx, ny);
						Direction dir = Direction.Towards(newPosition - location.Point);

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

		public override AbstractActor Copy() {
			return new NPC();
		}

		public override void Disturb() {
			base.Disturb();

			Actions.Clear();
		}
	}
}