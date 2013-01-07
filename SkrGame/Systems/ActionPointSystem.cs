using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Components;
using DEngine.Entity;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;
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
			if (!player.Get<ActionPoint>().Updateable) {
				foreach (var entity in entities) {
					if (!entity.Has<Player>()) {
						var entityAP = entity.Get<ActionPoint>();
						entityAP.ActionPoints += entityAP.Speed;
					}
				}

				player.Get<ActionPoint>().ActionPoints += player.Get<ActionPoint>().Speed;
			}

		}
	}
}
