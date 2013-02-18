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

		public void Update() {
			var playerActor = player.Get<ActorComponent>();
//			if (playerActor.Actor.RequiresInput)
//				return;
			while (playerActor.AP.Updateable && !playerActor.Actor.RequiresInput) {
				var action = player.Get<ActorComponent>().NextAction();

				var result = action.OnProcess();

				if (result == ActionResult.Failed || result == ActionResult.Success) {
					playerActor.AP.ActionPoints -= action.APCost;
				}
			}
			if (!playerActor.AP.Updateable) {
				playerActor.AP.Gain();
				foreach (var entity in entities) {
					if (entity == player)
						continue;

					var entityActor = entity.Get<ActorComponent>();
					entityActor.AP.Gain();
					while (entityActor.AP.Updateable && !entityActor.Actor.RequiresInput) {
						var action = entity.Get<ActorComponent>().NextAction();

						var result = action.OnProcess();

						if (result == ActionResult.Failed || result == ActionResult.Success) {
							entityActor.AP.ActionPoints -= action.APCost;
						}
					}
				}
			}
		}
	}
}
