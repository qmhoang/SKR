using System;
using System.Diagnostics.Contracts;
using DEngine.Actions;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;

namespace SkrGame.Actions {
	public class ReloadAction : ActorAction {
		private Entity weapon;
		private Entity ammo;

		public ReloadAction(Entity entity, Entity weapon, Entity ammo) : base(entity) {
			Contract.Requires<ArgumentException>(weapon.Has<RangeComponent>());
			Contract.Requires<ArgumentException>(ammo.Has<AmmoComponent>());
			Contract.Requires<ArgumentException>(entity.Has<Location>());
			Contract.Requires<ArgumentException>(entity.Has<ActorComponent>());
			Contract.Requires<ArgumentException>(weapon.Get<RangeComponent>().AmmoType == ammo.Get<AmmoComponent>().Type);

			this.weapon = weapon;
			this.ammo = ammo;
		}

		public override int APCost {
			get { return weapon.Get<RangeComponent>().APToReload; }
		}

		public override ActionResult OnProcess() {
			var rangeWeapon = weapon.Get<RangeComponent>();
			var ammunition = ammo.Get<Item>();
			var world = Entity.Get<Location>().Level.World;

			// todo revolvers and single load weapons

			// first we unload all ammos currently in the gun to the group, semi-simulating dropping the magazine
			if (rangeWeapon.ShotsRemaining > 0) {
				var droppedAmmo = ammo.Copy();

				droppedAmmo.Get<Item>().Amount = rangeWeapon.ShotsRemaining;
				rangeWeapon.ShotsRemaining = 0;
				droppedAmmo.Get<VisibleComponent>().Reset();

				world.Log.Normal(String.Format("{0} reloads {1} with {2}, dropping all excess ammo.", Entity.Get<Identifier>().Name, weapon.Get<Identifier>().Name, ammo.Get<Identifier>().Name));
			} else {
				world.Log.Normal(String.Format("{0} reloads {1} with {2}.", Entity.Get<Identifier>().Name, weapon.Get<Identifier>().Name, ammo.Get<Identifier>().Name));
			}

			if (ammunition.StackType == StackType.Hard) {
				if (ammunition.Amount >= rangeWeapon.Shots) {
					ammunition.Amount -= rangeWeapon.Shots;
					rangeWeapon.ShotsRemaining = rangeWeapon.Shots;
				} else {
					ammunition.Amount -= weapon.Get<Item>().Amount;

					if (Entity.Has<ContainerComponent>()) {
						Entity.Get<ContainerComponent>().Remove(ammo);
					}

					world.EntityManager.Remove(ammo);
				}
			}
			return ActionResult.Success;
		}
	}
}