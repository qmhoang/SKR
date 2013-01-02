using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Components;
using DEngine.Entity;
using SkrGame.Universe.Entities.Actors.PC;

namespace SkrGame.Systems {
	public class ActionPointSystem {
		private FilteredCollection updateables;
		private Entity player;

		public ActionPointSystem(EntityManager entityManager) {
			updateables = entityManager.Get(typeof(ActionPoint));
			player = entityManager.Get<PlayerMarker>().ToList()[0];
		}

		protected void Update() {
			if (!player.As<ActionPoint>().Updateable) {
				foreach (var entity in updateables) {
					if (!entity.Is<PlayerMarker>()) {
						var entityAP = entity.As<ActionPoint>();
						entityAP.ActionPoints += entityAP.Speed;
					}
				}

				player.As<ActionPoint>().ActionPoints += player.As<ActionPoint>().Speed;
			}

		}
	}
}
