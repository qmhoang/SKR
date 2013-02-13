using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using DEngine.Extensions;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Locations;

namespace SkrGame.Systems {
	public class ActionPointSystem {
		private FilteredCollection entities;
		private Entity player;

		public ActionPointSystem(Entity player, EntityManager entityManager) {
			entities = entityManager.Get(typeof(ActorComponent));
			this.player = player;
		}

//		public void Update() {
//			entities.Each(e => e.Get<ActorComponent>().AP.Gain());
//
//			foreach (var entity in entities) {
//				if (entity.Get<ActorComponent>().AP.Updateable) {
//					var ar = entity.Get<ActorComponent>().NextAction();
//				}
//			}
//		}

		public void Update() {
			var playerAP = player.Get<ActorComponent>();
			if (!playerAP.Actor.RequiresInput) {
				foreach (var entity in entities) {
//					if (entity != player) {
						var entityAP = entity.Get<ActorComponent>();
						entityAP.AP.Gain();
						while (entityAP.AP.Updateable && !entityAP.Actor.RequiresInput) {
							var action = entity.Get<ActorComponent>().NextAction();

							var result = action.OnProcess();

							if (result == ActionResult.Failed || result == ActionResult.Success) {
								entityAP.AP.ActionPoints -= action.APCost;
							}
						}
//					}
				}

//				playerAP.AP.Gain();
//				player.Broadcast<IUpdateable>(c => c.Update());
			}
//				player.Broadcast("Update", new UpdateEvent(player, playerAP.ActionPointPerTurn));
		}
	}
}
