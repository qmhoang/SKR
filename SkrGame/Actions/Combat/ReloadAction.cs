using System;
using System.Diagnostics.Contracts;
using System.Linq;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using DEngine.Extensions;
using SkrGame.Actions.Features;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;

namespace SkrGame.Actions.Combat {
	public class ReloadAction : LoggedAction {
		private Entity weapon;
		private Entity ammo;

		public ReloadAction(Entity entity, Entity weapon, Entity ammo) : base(entity) {
			Contract.Requires<ArgumentException>(weapon.Has<RangeComponent>());
			Contract.Requires<ArgumentException>(ammo.Has<AmmoComponent>());
			Contract.Requires<ArgumentException>(entity.Has<GameObject>());
			Contract.Requires<ArgumentException>(entity.Has<ActorComponent>());
			Contract.Requires<ArgumentException>(weapon.Get<RangeComponent>().AmmoCaliber == ammo.Get<AmmoComponent>().Caliber);

			this.weapon = weapon;
			this.ammo = ammo;
		}

		public override int APCost {
			get { return weapon.Get<RangeComponent>().APToReload; }
		}

		public override ActionResult OnProcess() {
			var rangeWeapon = weapon.Get<RangeComponent>();
			var ammunition = ammo.Get<Item>();

			

			// first we unload all ammos currently in the gun to the group, semi-simulating dropping the magazine
			if (rangeWeapon.SwapClips && rangeWeapon.ShotsRemaining > (rangeWeapon.OneInTheChamber ? 1 : 0)) {
				var droppedAmmo = ammo.Copy();
				// todo cocking the gun takes more AP
				if (rangeWeapon.OneInTheChamber) {
					droppedAmmo.Get<Item>().Amount = rangeWeapon.ShotsRemaining - 1;
					rangeWeapon.ShotsRemaining = 1;
				} else {
					// probably will never be used except for bugs, what weapon do we swap clips and not have a closed-chamber?
					droppedAmmo.Get<Item>().Amount = rangeWeapon.ShotsRemaining;
					rangeWeapon.ShotsRemaining = 0;
				}

				if (droppedAmmo.Has<VisibleComponent>()) {
					droppedAmmo.Get<VisibleComponent>().Reset();
				}

				World.Log.Normal(String.Format("{0} removes the magazine, dropping all excess ammo.", EntityName));
			}

			// inserting ammo
			if (rangeWeapon.SwapClips) {
				if (ammunition.StackType == StackType.Hard) {
					if (ammunition.Amount >= rangeWeapon.Shots) {
						ammunition.Amount -= rangeWeapon.Shots;
						rangeWeapon.ShotsRemaining += rangeWeapon.Shots;
					} else {
						rangeWeapon.ShotsRemaining += ammunition.Amount;
						ammunition.Amount -= weapon.Get<Item>().Amount;

						if (Entity.Has<ContainerComponent>()) {
							Entity.Get<ContainerComponent>().Remove(ammo);
						}

						World.EntityManager.Remove(ammo);
					}
					World.Log.Normal(String.Format("{0} reloads {1} with {2}.", EntityName, Identifier.GetNameOrId(weapon), Identifier.GetNameOrId(ammo)));

				} else {
					rangeWeapon.ShotsRemaining++;

					if (Entity.Has<ContainerComponent>()) {
						Entity.Get<ContainerComponent>().Remove(ammo);
					}

					World.EntityManager.Remove(ammo);
				}
			} else {
				if (rangeWeapon.ShotsRemaining == (rangeWeapon.Shots + (rangeWeapon.OneInTheChamber ? 1 : 0)))
					return ActionResult.Aborted;

				// we are inserting individual bullets
				rangeWeapon.ShotsRemaining++;

				if (ammunition.StackType == StackType.Hard) {
					if (ammunition.Amount > 1) {
						ammunition.Amount--;						
					} else {
						if (Entity.Has<ContainerComponent>()) {
							Entity.Get<ContainerComponent>().Remove(ammo);
						}

						World.EntityManager.Remove(ammo);
					}
				} else {
					if (Entity.Has<ContainerComponent>()) {
						Entity.Get<ContainerComponent>().Remove(ammo);
					}

					World.EntityManager.Remove(ammo);
				}

				if (rangeWeapon.ShotsRemaining < rangeWeapon.Shots + (rangeWeapon.OneInTheChamber ? 1 : 0)) {
					var ammos = Entity.Get<ContainerComponent>().Items.FilteredBy<Item, AmmoComponent>().Where(e => e.Get<AmmoComponent>().Caliber == rangeWeapon.AmmoCaliber);
					var enumerator = ammos.GetEnumerator();
					while (enumerator.MoveNext()) {
						Entity.Get<ActorComponent>().Enqueue(new ReloadAction(Entity, weapon, enumerator.Current));
					}
				}
				
			}

			return ActionResult.Success;
		}
	}
}