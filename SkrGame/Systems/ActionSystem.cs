using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actions;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using DEngine.Extensions;
using SkrGame.Universe;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Locations;

namespace SkrGame.Systems {
	public class ActionSystem {
		private FilteredCollection entities;
		private Entity player;
		private World world;

		public ActionSystem(World world) {
			entities = world.EntityManager.Get(typeof(ActorComponent));
			this.player = world.Player;
			this.world = world;
		}

		public void Update() {
			var playerActor = player.Get<ActorComponent>();

			while (playerActor.AP.Updateable && playerActor.Controller.Actions.Count != 0) {
				var action = player.Get<ActorComponent>().NextAction();

				var result = action.OnProcess();

				if (result == ActionResult.Failed || result == ActionResult.Success) {
					playerActor.AP.ActionPoints -= action.APCost;

					if (player.Has<EntityConditions>()) {
						foreach (var c in player.Get<EntityConditions>().Conditions) {
							c.Update(action.APCost);
						}
					}

					world.OnActionProcessed();
				}
			}
			if (!playerActor.AP.Updateable) {
				playerActor.AP.Gain();
				foreach (var entity in entities) {
					if (entity == player)
						continue;

					var entityActor = entity.Get<ActorComponent>();
					entityActor.AP.Gain();
					while (entityActor.AP.Updateable) {
						var action = entity.Get<ActorComponent>().NextAction();

						var result = action.OnProcess();

						if (result == ActionResult.Failed || result == ActionResult.Success) {
							entityActor.AP.ActionPoints -= action.APCost;

							if (entity.Has<EntityConditions>()) {
								foreach (var c in entity.Get<EntityConditions>().Conditions) {
									c.Update(action.APCost);
								}
							}

							world.OnActionProcessed();
						}
					}
				}
			}
		}
	}
}
