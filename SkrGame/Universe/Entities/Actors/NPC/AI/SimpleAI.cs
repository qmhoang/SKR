using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using SkrGame.Gameplay;
using SkrGame.Gameplay.Combat;
using SkrGame.Gameplay.Talent.Components;
using SkrGame.Universe.Entities.Actors.PC;
using log4net;

namespace SkrGame.Universe.Entities.Actors.NPC.AI {

	public class SimpleAI : NpcIntelligence.AI {
		private AStarPathFinder pf;
		private VisionMap vision;
		private Point oldPos;
		public Entity Entity { get; set; }

		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

//		public SimpleIntelligence() {
//			pf = new AStarPathFinder(monster.Level, 1.41f);			
//		}
//
//		public SimpleIntelligence(Npc monster)
//			: base(monster) {
//			pf = new AStarPathFinder(monster.Level, 1.41f);
//		}

		public SimpleAI(Entity entity) {
			this.Entity = entity;

			pf = new AStarPathFinder(entity.Get<Location>().Level, 1.41f);
			vision = new VisionMap(entity.Get<Location>().Level.Size);
			ComputeFOV(entity.Get<Location>());
			oldPos = entity.Get<Location>().Position;
		}

		private void ComputeFOV(Location location) {
			ShadowCastingFOV.ComputeRecursiveShadowcasting(vision, location.Level, location.Position.X, location.Position.Y, 10, true);
		}

		public override void Update() {
			var position = Entity.Get<Location>().Position;

			if (oldPos != position) {
				ComputeFOV(Entity.Get<Location>());
			}

			Entity player = World.Instance.Player;

			var target = player.Get<Location>().Position;
			if (vision.IsVisible(target)) {
				var distance = position.DistanceTo(target);

				if (distance <= 1.5) {
					Combat.MeleeAttack(Entity, Entity, player, player.Get<DefendComponent>().GetRandomPart());
				} else {
					pf.Compute(position.X, position.Y, target.X, target.Y);
					int nx = position.X, ny = position.Y;

					if (pf.Walk(ref nx, ref ny, false)) {
						var newPosition = new Point(nx, ny);
						Point dir = newPosition - position;

						var result = Movement.BumpDirection(Entity, dir);

						// if an entity prevents movement, we can't do anything
						if (!result) {
							return;
						}

						Movement.MoveEntity(Entity, newPosition);

					} else {
						//					Actor.Wait();
						Entity.Get<ActionPoint>().ActionPoints -= World.SpeedToActionPoints(World.DEFAULT_SPEED);
					}
				}				
			} else {
//				Actor.Wait();
				Entity.Get<ActionPoint>().ActionPoints -= World.SpeedToActionPoints(World.DEFAULT_SPEED);
			}
		}
	}

//	internal class BasicHumanIntelligence : SimpleIntelligence {
//
//
//		public BasicHumanIntelligence(Npc actor)
//			: base(actor) {
//
//		}
//
//		public override void Update() {
//
//		}
//	}
//
//	internal class FightOrFlightIntelligence : SimpleIntelligence {
//		private Actor target;
//
//
//		public FightOrFlightIntelligence(Npc monster)
//			: base(monster) {
//			target = World.Instance.Player;
//		}
//	}
}