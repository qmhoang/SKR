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
		private readonly FilteredCollection _entities;
		private readonly Entity _player;
		private readonly World _world;

		public ActionSystem(World world) {
			_entities = world.EntityManager.Get(typeof(ActorComponent));
			_player = world.Player;
			_world = world;
		}

		public void Update() {
			var playerActor = _player.Get<ActorComponent>();

			while (playerActor.AP.Updateable && playerActor.Controller.HasActionsQueued) {
				var action = _player.Get<ActorComponent>().NextAction();

				var result = action.OnProcess();

				if (result == ActionResult.Failed || result == ActionResult.Success) {
					playerActor.AP.ActionPoints -= action.APCost;

					if (_player.Has<EntityConditions>()) {
						foreach (var c in _player.Get<EntityConditions>().Effects) {
							c.Update(action.APCost);
						}
					}

					_world.OnActionProcessed();
				}
			}
			if (!playerActor.AP.Updateable) {
				playerActor.AP.Gain();
				foreach (var entity in _entities) {
					if (entity == _player)
						continue;

					var entityActor = entity.Get<ActorComponent>();
					entityActor.AP.Gain();
					while (entityActor.AP.Updateable) {
						var action = entity.Get<ActorComponent>().NextAction();

						var result = action.OnProcess();

						if (result == ActionResult.Failed || result == ActionResult.Success) {
							entityActor.AP.ActionPoints -= action.APCost;

							if (entity.Has<EntityConditions>()) {
								foreach (var c in entity.Get<EntityConditions>().Effects) {
									c.Update(action.APCost);
								}
							}

							_world.OnActionProcessed();
						}
					}
				}
			}
		}
	}
}
