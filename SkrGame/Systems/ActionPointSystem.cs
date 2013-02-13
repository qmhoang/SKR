using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Components;
using DEngine.Entities;
using DEngine.Extensions;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Actors.NPC.AI;
using SkrGame.Universe.Locations;

namespace SkrGame.Systems {
	public class ActionPointSystem {
		private FilteredCollection entities;
		private Entity player;

		public ActionPointSystem(Entity player, EntityManager entityManager) {
			entities = entityManager.Get(typeof(ActorComponent));
			this.player = player;
		}

		public void Update() {
			entities.Each(e => e.Get<ActorComponent>().AP.Gain());

			foreach (var entity in entities) {
				if (entity.Get<ActorComponent>().AP.Updateable) {
					var ar = entity.Get<ActorComponent>().NextAction();
				}
			}
		}

//		public void Update() {
//			var playerAP = player.Get<ActionPoint>();
//			if (!playerAP.Updateable) {
//				foreach (var entity in entities) {
//					if (!entity.Has<Player>()) {
//						var entityAP = entity.Get<ActionPoint>();						
//						entityAP.ActionPoints += entityAP.ActionPointPerTurn;
//						if (entityAP.Updateable) {
//							entity.Broadcast<IUpdateable>(c => c.Update());
////							entity.Broadcast("Update", new UpdateEvent(entity, entityAP.ActionPoints - playerAP.ActionPoints));
//						}
//					}
//				}
//
//				playerAP.ActionPoints += playerAP.ActionPointPerTurn;
//				player.Broadcast<IUpdateable>(c => c.Update());
////				player.Broadcast("Update", new UpdateEvent(player, playerAP.ActionPointPerTurn));
//			}
//		}
	}
}
