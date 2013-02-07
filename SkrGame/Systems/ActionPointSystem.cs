using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Core.ComponentMessages;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Actors.NPC.AI;
using SkrGame.Universe.Entities.Actors.PC;
using SkrGame.Universe.Locations;

namespace SkrGame.Systems {
	public class ActionPointSystem {
		private FilteredCollection entities;
		private Entity player;

		public ActionPointSystem(EntityManager entityManager) {
			entities = entityManager.Get(typeof(ActionPoint));
			player = World.Instance.Player;
		}

		public void Update() {
			var playerAP = player.Get<ActionPoint>();
			if (!playerAP.Updateable) {
				foreach (var entity in entities) {
					if (!entity.Has<Player>()) {
						var entityAP = entity.Get<ActionPoint>();						
						entityAP.ActionPoints += entityAP.ActionPointPerTurn;
						if (entityAP.Updateable) {
							entity.Broadcast<IUpdateable>(c => c.Update());
//							entity.Broadcast("Update", new UpdateEvent(entity, entityAP.ActionPoints - playerAP.ActionPoints));
						}
					}
				}

				playerAP.ActionPoints += playerAP.ActionPointPerTurn;
				player.Broadcast<IUpdateable>(c => c.Update());
//				player.Broadcast("Update", new UpdateEvent(player, playerAP.ActionPointPerTurn));
			}
		}
	}
}
