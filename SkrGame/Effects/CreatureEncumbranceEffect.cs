using System.Linq;
using System.Reflection;
using DEngine.Components;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;
using log4net;

namespace SkrGame.Effects {
	public class CreatureEncumbranceEffect : Effect {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public override Effect Copy() {
			return new CreatureEncumbranceEffect();
		}

		protected override void OnTick(int apElapsed) {
			if (Holder.Entity.Has<Creature>()) {
				var person = Holder.Entity.Get<Creature>();
				int weight = 0;

				if (Holder.Entity.Has<ContainerComponent>()) {
					weight += Holder.Entity.Get<ContainerComponent>().Items.Sum(i => i.Get<Item>().Weight);
				}

				if (Holder.Entity.Has<EquipmentComponent>()) {
					weight += Holder.Entity.Get<EquipmentComponent>().EquippedItems.Sum(i => i.Get<Item>().Weight);
				}

				person.EncumbrancePenalty = (double) weight / person.Lift;
			} else {
				Logger.WarnFormat("{0} doesn't have person component.", Identifier.GetNameOrId(Holder.Entity));				
			}
		}
	}
}