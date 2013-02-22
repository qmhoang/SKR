using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actions;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using DEngine.Extensions;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Locations;

namespace SkrGame.Systems {
	public class ActionSystem {
		private FilteredCollection entities;
		private Entity player;

		public ActionSystem(Entity player, EntityManager entityManager) {
			entities = entityManager.Get(typeof(ActorComponent));
			this.player = player;
		}

//		public IEnumerable<IAction> Process() {
//			foreach (var entity in entities) {
//				var actor = entity.Get<ActorComponent>();
//
//				actor.AP.Gain();
//
//				while (actor.AP.Updateable) {
//					var action = entity.Get<ActorComponent>().NextAction();
//
//					while (action.RequiresPrompt != PromptRequired.None) {
//						yield return action;
//					}
//
//					var result = action.OnProcess();
//
//					if (result == ActionResult.Failed || result == ActionResult.Success) {
//						actor.AP.ActionPoints -= action.APCost;
//					}
//				}
//			}
//
//		}

		public IEnumerable<IAction> Process() {
			var playerActor = player.Get<ActorComponent>();
			while (true) {
				while (playerActor.AP.Updateable) {
					var action = player.Get<ActorComponent>().NextAction();

					if (action.RequiresPrompt != PromptRequired.None) {
						yield return action;
					}

					var result = action.OnProcess();

					if (result == ActionResult.Failed || result == ActionResult.Success) {
						playerActor.AP.ActionPoints -= action.APCost;
					}
				}
				while (!playerActor.AP.Updateable) {
					playerActor.AP.Gain();
					foreach (var entity in entities) {
						if (entity == player)
							continue;

						var entityActor = entity.Get<ActorComponent>();
						entityActor.AP.Gain();
						while (entityActor.AP.Updateable) {
							var action = entity.Get<ActorComponent>().NextAction();

							if (action.RequiresPrompt != PromptRequired.None) {
								yield return action;
							}

							var result = action.OnProcess();

							if (result == ActionResult.Failed || result == ActionResult.Success) {
								entityActor.AP.ActionPoints -= action.APCost;
							}
						}
					}
				}
			}
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
