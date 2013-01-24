using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Actors.NPC.AI;
using SkrGame.Universe.Entities.Actors.PC;
using SkrGame.Universe.Locations;

namespace SkrGame.Systems {
	public class Updateable : Component {
		public Action<Entity> OnUpdate { get; private set; }

		public Updateable(Action<Entity> onUpdate) {
			OnUpdate = onUpdate;
		}

		public override Component Copy() {
			return new Updateable(OnUpdate == null ? null : (Action<Entity>)OnUpdate.Clone());
		}
	}
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
						if (entityAP.Updateable && entity.Has<NpcIntelligence>()) {
							entity.Get<Updateable>().OnUpdate(entity);
						}
					}
				}

				player.Get<ActionPoint>().ActionPoints += player.Get<ActionPoint>().Speed;
			}

		}
	}
}
