using DEngine.Actions;
using DEngine.Components;
using DEngine.Core;
using SkrGame.Actions;
using SkrGame.Actions.Combat;
using SkrGame.Universe.Locations;

namespace SkrGame.Universe.Entities.Actors {
	public class DoNothingActor : AbstractActor {
		public override IAction NextAction() {
			return new WaitAction(Holder.Entity);
		}

		public override AbstractActor Copy() {
			return new DoNothingActor();
		}
	}
	public class NPC : AbstractActor {
		private AStarPathFinder pf;
		private VisionMap vision;
		private Point oldPos;
		private Level level;

		public NPC() { }

		private void ComputeFOV(GameObject position) {
			
			ShadowCastingFOV.ComputeRecursiveShadowcasting(vision, position.Level, position.Location.X, position.Location.Y, 10, true);
		}

		public override IAction NextAction() {
			if (Actions.Count == 0) {
				return CalculateNextMove();				
			} else {
				return Actions.Dequeue();
			}

		}

		private ActorAction CalculateNextMove() {
			var location = Holder.Entity.Get<GameObject>();

			if (level != location.Level) {
				level = location.Level;
				oldPos = location.Location;
				pf = new AStarPathFinder(level, 1.41f);
				vision = new VisionMap(level.Size);
				ComputeFOV(Holder.Entity.Get<GameObject>());
			}

			if (oldPos != location.Location) {
				oldPos = location.Location;
				ComputeFOV(Holder.Entity.Get<GameObject>());
			}

			var player = level.World.Player;

			var target = player.Get<GameObject>().Location;

			if (vision.IsVisible(target)) {
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

		public override AbstractActor Copy() {
			return new NPC();
		}

		public override void Disturb() {
			base.Disturb();

			Actions.Clear();
		}
	}
}