using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using DEngine.Extensions;
using NUnit.Framework;
using SkrGame.Actions.Combat;
using SkrGame.Actions.Items;
using SkrGame.Universe;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;

namespace SKRTests.Actions.Combat {
	[TestFixture]
	class ReloadActionTests : SkrTests {
		protected Entity Pistol;	// swapmags = true, oneinthechamber = true
		protected Entity Revolver;	// swapmags = false, oneinthechamber = false
		protected Entity Shotgun;	// swapmags = false, oneinthechamber = true

		protected Entity PistolAmmo;
		protected Entity RevolverAmmo;
		protected Entity ShotgunAmmo;

		[SetUp]
		public void SetUp() {
			Pistol = EntityManager.Create(new List<Component>
			                              {
			                              		new GameObject(0, 0, Level),
			                              		Equipable.SingleSlot("slot1", "slot2"),
												new Item(
			                                  				new Item.Template
			                                  				{
			                                  						Value = 30,
			                                  						Weight = 0,
			                                  						Size = 0,
			                                  						StackType = StackType.None
			                                  				}),
			                              		new RangeComponent(
			                              				new RangeComponent.Template
			                              				{
			                              						ActionDescription = "shoot",
			                              						ActionDescriptionPlural = "shoots",
			                              						Skill = "skill_pistol",
			                              						Accuracy = 2,
			                              						Damage = Rand.Constant(1),
			                              						DamageType = SkrGame.Gameplay.Combat.Combat.DamageTypes["pierce"],
			                              						Penetration = 1,
			                              						Shots = 10,
			                              						Range = 100,
			                              						RoF = 1,
			                              						APToReady = World.SecondsToActionPoints(1f),
			                              						APToReload = World.SecondsToActionPoints(1f),
			                              						Recoil = 1,
			                              						Reliability = 18,
			                              						Strength = 8,
			                              						AmmoType = "pistol",
			                              						OneInTheChamber = true,
			                              						SwapClips = true,
			                              				})
			                              });

			Revolver = EntityManager.Create(new List<Component>
			                                {
			                                		new GameObject(0, 0, Level),
			                                		Equipable.SingleSlot("slot1", "slot2"),
													new Item(
			                                  				new Item.Template
			                                  				{
			                                  						Value = 30,
			                                  						Weight = 0,
			                                  						Size = 0,
			                                  						StackType = StackType.None
			                                  				}),
			                                		new RangeComponent(
			                                				new RangeComponent.Template
			                                				{
			                                						ActionDescription = "shoot",
			                                						ActionDescriptionPlural = "shoots",
			                                						Skill = "skill_pistol",
			                                						Accuracy = 2,
			                                						Damage = Rand.Constant(1),
			                                						DamageType = SkrGame.Gameplay.Combat.Combat.DamageTypes["pierce"],
			                                						Penetration = 1,
			                                						Shots = 10,
			                                						Range = 100,
			                                						RoF = 1,
			                                						APToReady = World.SecondsToActionPoints(1f),
			                                						APToReload = World.SecondsToActionPoints(1f),
			                                						Recoil = 1,
			                                						Reliability = 18,
			                                						Strength = 8,
			                                						AmmoType = "revolver",
			                                						OneInTheChamber = false,
			                                						SwapClips = false,
			                                				})
			                                });

			Shotgun = EntityManager.Create(new List<Component>
			                                {
			                                		new GameObject(0, 0, Level),
			                                		Equipable.SingleSlot("slot3"),
													new Item(
			                                  				new Item.Template
			                                  				{
			                                  						Value = 30,
			                                  						Weight = 0,
			                                  						Size = 0,
			                                  						StackType = StackType.None
			                                  				}),
			                                		new RangeComponent(
			                                				new RangeComponent.Template
			                                				{
			                                						ActionDescription = "shoot",
			                                						ActionDescriptionPlural = "shoots",
			                                						Skill = "skill_shotgun",
			                                						Accuracy = 2,
			                                						Damage = Rand.Constant(1),
			                                						DamageType = SkrGame.Gameplay.Combat.Combat.DamageTypes["pierce"],
			                                						Penetration = 1,
			                                						Shots = 10,
			                                						Range = 100,
			                                						RoF = 1,
			                                						APToReady = World.SecondsToActionPoints(1f),
			                                						APToReload = World.SecondsToActionPoints(1f),
			                                						Recoil = 1,
			                                						Reliability = 18,
			                                						Strength = 8,
			                                						AmmoType = "shotgun",
			                                						OneInTheChamber = true,
			                                						SwapClips = false,
			                                				})
			                                });

			PistolAmmo = EntityManager.Create(new List<Component>
			                                  {
			                                  		new GameObject(0, 0, Level),
			                                  		new ReferenceId("pistolammo"),
			                                  		new Item(
			                                  				new Item.Template
			                                  				{
			                                  						Value = 30,
			                                  						Weight = 0,
			                                  						Size = 0,
			                                  						StackType = StackType.Hard
			                                  				}),
			                                  		new AmmoComponent(
			                                  				new AmmoComponent.Template
			                                  				{
			                                  						ActionDescription = "load",
			                                  						ActionDescriptionPlural = "loads",
			                                  						Type = "pistol",
			                                  				})
			                                  });

			PistolAmmo.Get<Item>().Amount = 30;

			RevolverAmmo = EntityManager.Create(new List<Component>
			                                    {
			                                    		new GameObject(0, 0, Level),
			                                    		new ReferenceId("revolverammo"),
			                                    		new Item(
			                                    				new Item.Template
			                                    				{
			                                    						Value = 30,
			                                    						Weight = 0,
			                                    						Size = 0,
			                                    						StackType = StackType.Hard
			                                    				}),
			                                    		new AmmoComponent(
			                                    				new AmmoComponent.Template
			                                    				{
			                                    						ActionDescription = "load",
			                                    						ActionDescriptionPlural = "loads",
			                                    						Type = "revolver",
			                                    				})
			                                    });

			RevolverAmmo.Get<Item>().Amount = 30;

			ShotgunAmmo = EntityManager.Create(new List<Component>
			                                    {
			                                    		new GameObject(0, 0, Level),
			                                    		new ReferenceId("shotgunammo"),
			                                    		new Item(
			                                    				new Item.Template
			                                    				{
			                                    						Value = 30,
			                                    						Weight = 0,
			                                    						Size = 0,
			                                    						StackType = StackType.Hard
			                                    				}),
			                                    		new AmmoComponent(
			                                    				new AmmoComponent.Template
			                                    				{
			                                    						ActionDescription = "load",
			                                    						ActionDescriptionPlural = "loads",
			                                    						Type = "shotgun",
			                                    				})
			                                    });

			ShotgunAmmo.Get<Item>().Amount = 30;

			new GetItemAction(Entity, PistolAmmo, 30).OnProcess();
			new GetItemAction(Entity, RevolverAmmo, 30).OnProcess();
			new GetItemAction(Entity, ShotgunAmmo, 30).OnProcess();

			new EquipItemAction(Entity, Pistol, "slot1", true).OnProcess();
			new EquipItemAction(Entity, Revolver, "slot2", true).OnProcess();
			new EquipItemAction(Entity, Shotgun, "slot3", true).OnProcess();
		}

		[Test]
		public void TestReloadMagazineGunEmpty() {
			Pistol.Get<RangeComponent>().ShotsRemaining = 0;

			Assert.IsTrue(Pistol.Get<RangeComponent>().SwapClips);
			Assert.IsTrue(Pistol.Get<RangeComponent>().OneInTheChamber);
			Assert.AreEqual(Pistol.Get<RangeComponent>().ShotsRemaining, 0);
			Assert.AreEqual(PistolAmmo.Get<Item>().Amount, 30);

			new ReloadAction(Entity, Pistol, PistolAmmo).OnProcess();

			Assert.AreEqual(Pistol.Get<RangeComponent>().ShotsRemaining, 10);
			Assert.AreEqual(PistolAmmo.Get<Item>().Amount, 20);

			// there should be no dropped ammo
			Assert.IsTrue(Level.GetEntitiesAt<AmmoComponent>(Entity.Get<GameObject>().Location).Where(e => !Entity.Get<ContainerComponent>().Contains(e)).IsEmpty());
		}

		[Test]
		public void TestReloadMagazineGunFull() {
			Assert.IsTrue(Pistol.Get<RangeComponent>().SwapClips);
			Assert.IsTrue(Pistol.Get<RangeComponent>().OneInTheChamber);
			Assert.AreEqual(Pistol.Get<RangeComponent>().ShotsRemaining, 11);
			Assert.AreEqual(PistolAmmo.Get<Item>().Amount, 30);

			new ReloadAction(Entity, Pistol, PistolAmmo).OnProcess();

			Assert.AreEqual(Pistol.Get<RangeComponent>().ShotsRemaining, 11);
			Assert.AreEqual(PistolAmmo.Get<Item>().Amount, 20);

			// check for dropped ammo
			Assert.IsFalse(Level.GetEntitiesAt<AmmoComponent>(Entity.Get<GameObject>().Location).Where(e => !Entity.Get<ContainerComponent>().Contains(e)).IsEmpty());

			var dropped = Level.GetEntitiesAt<AmmoComponent>(Entity.Get<GameObject>().Location).First(e => !Entity.Get<ContainerComponent>().Contains(e));
			Assert.AreEqual(dropped.Get<Item>().Amount, 10);
		}

		[Test]
		public void TestReloadMagazineGunPartial() {
			Pistol.Get<RangeComponent>().ShotsRemaining = 3;

			Assert.IsTrue(Pistol.Get<RangeComponent>().SwapClips);
			Assert.IsTrue(Pistol.Get<RangeComponent>().OneInTheChamber);
			Assert.AreEqual(Pistol.Get<RangeComponent>().ShotsRemaining, 3);
			Assert.AreEqual(PistolAmmo.Get<Item>().Amount, 30);

			new ReloadAction(Entity, Pistol, PistolAmmo).OnProcess();

			Assert.AreEqual(Pistol.Get<RangeComponent>().ShotsRemaining, 11);
			Assert.AreEqual(PistolAmmo.Get<Item>().Amount, 20);

			// check for dropped ammo
			Assert.IsFalse(Level.GetEntitiesAt<AmmoComponent>(Entity.Get<GameObject>().Location).Where(e => !Entity.Get<ContainerComponent>().Contains(e)).IsEmpty());

			var dropped = Level.GetEntitiesAt<AmmoComponent>(Entity.Get<GameObject>().Location).First(e => !Entity.Get<ContainerComponent>().Contains(e));
			Assert.AreEqual(dropped.Get<Item>().Amount, 2);
		}

		[Test]
		public void TestReloadLimitedAmmo() {
			Pistol.Get<RangeComponent>().ShotsRemaining = 1;
			PistolAmmo.Get<Item>().Amount = 3;

			Assert.IsTrue(Pistol.Get<RangeComponent>().SwapClips);
			Assert.IsTrue(Pistol.Get<RangeComponent>().OneInTheChamber);
			Assert.AreEqual(Pistol.Get<RangeComponent>().ShotsRemaining, 1);
			Assert.AreEqual(PistolAmmo.Get<Item>().Amount, 3);

			new ReloadAction(Entity, Pistol, PistolAmmo).OnProcess();

			Assert.AreEqual(Pistol.Get<RangeComponent>().ShotsRemaining, 4);

			// we used all of our ammo
			Assert.IsFalse(Entity.Get<ContainerComponent>().Contains(PistolAmmo));
			Assert.IsFalse(EntityManager.Contains(PistolAmmo));	

			// no dropped ammo
			Assert.IsTrue(Level.GetEntitiesAt<AmmoComponent>(Entity.Get<GameObject>().Location).Where(e => !Entity.Get<ContainerComponent>().Contains(e)).IsEmpty());
		}

		[Test]
		public void TestReloadRevolverFull() {
			Assert.IsFalse(Revolver.Get<RangeComponent>().SwapClips);
			Assert.IsFalse(Revolver.Get<RangeComponent>().OneInTheChamber);
			Assert.AreEqual(Revolver.Get<RangeComponent>().ShotsRemaining, 10);
			Assert.AreEqual(RevolverAmmo.Get<Item>().Amount, 30);

			new ReloadAction(Entity, Revolver, RevolverAmmo).OnProcess();

			Assert.AreEqual(Revolver.Get<RangeComponent>().ShotsRemaining, 10);
			Assert.AreEqual(RevolverAmmo.Get<Item>().Amount, 30);
		}

		[Test]
		public void TestReloadRevolverEmpty() {
			Revolver.Get<RangeComponent>().ShotsRemaining = 0;

			Assert.IsFalse(Revolver.Get<RangeComponent>().SwapClips);
			Assert.IsFalse(Revolver.Get<RangeComponent>().OneInTheChamber);
			Assert.AreEqual(Revolver.Get<RangeComponent>().ShotsRemaining, 0);
			Assert.AreEqual(RevolverAmmo.Get<Item>().Amount, 30);

			new ReloadAction(Entity, Revolver, RevolverAmmo).OnProcess();

			while (Entity.Get<ActorComponent>().Controller.Actions.Count > 0) {
				Entity.Get<ActorComponent>().NextAction().OnProcess();
			}

			Assert.AreEqual(Revolver.Get<RangeComponent>().ShotsRemaining, 10);
			Assert.AreEqual(RevolverAmmo.Get<Item>().Amount, 20);
		}

		[Test]
		public void TestReloadRevolverPartial() {
			Revolver.Get<RangeComponent>().ShotsRemaining = 3;

			Assert.IsFalse(Revolver.Get<RangeComponent>().SwapClips);
			Assert.IsFalse(Revolver.Get<RangeComponent>().OneInTheChamber);
			Assert.AreEqual(Revolver.Get<RangeComponent>().ShotsRemaining, 3);
			Assert.AreEqual(RevolverAmmo.Get<Item>().Amount, 30);

			new ReloadAction(Entity, Revolver, RevolverAmmo).OnProcess();

			while (Entity.Get<ActorComponent>().Controller.Actions.Count > 0) {
				Entity.Get<ActorComponent>().NextAction().OnProcess();
			}

			Assert.AreEqual(Revolver.Get<RangeComponent>().ShotsRemaining, 10);
			Assert.AreEqual(RevolverAmmo.Get<Item>().Amount, 23);
		}

		[Test]
		public void TestReloadRevolverLimitedAmmo() {
			Revolver.Get<RangeComponent>().ShotsRemaining = 1;
			RevolverAmmo.Get<Item>().Amount = 3;
			
			Assert.IsFalse(Revolver.Get<RangeComponent>().SwapClips);
			Assert.IsFalse(Revolver.Get<RangeComponent>().OneInTheChamber);
			Assert.AreEqual(Revolver.Get<RangeComponent>().ShotsRemaining, 1);
			Assert.AreEqual(RevolverAmmo.Get<Item>().Amount, 3);

			new ReloadAction(Entity, Revolver, RevolverAmmo).OnProcess();

			while (Entity.Get<ActorComponent>().Controller.Actions.Count > 0) {
				Entity.Get<ActorComponent>().NextAction().OnProcess();
			}

			Assert.AreEqual(Revolver.Get<RangeComponent>().ShotsRemaining, 4);

			// we used all of our ammo
			Assert.IsFalse(Entity.Get<ContainerComponent>().Contains(RevolverAmmo));
			Assert.IsFalse(EntityManager.Contains(RevolverAmmo));
		}

		[Test]
		public void TestReloadShotgunEmpty() {
			Shotgun.Get<RangeComponent>().ShotsRemaining = 0;

			Assert.IsFalse(Shotgun.Get<RangeComponent>().SwapClips);
			Assert.IsTrue(Shotgun.Get<RangeComponent>().OneInTheChamber);
			Assert.AreEqual(Shotgun.Get<RangeComponent>().ShotsRemaining, 0);
			Assert.AreEqual(ShotgunAmmo.Get<Item>().Amount, 30);

			new ReloadAction(Entity, Shotgun, ShotgunAmmo).OnProcess();

			while (Entity.Get<ActorComponent>().Controller.Actions.Count > 0) {
				Entity.Get<ActorComponent>().NextAction().OnProcess();
			}

			Assert.AreEqual(Shotgun.Get<RangeComponent>().ShotsRemaining, 11);
			Assert.AreEqual(ShotgunAmmo.Get<Item>().Amount, 19);
		}

		[Test]
		public void TestReloadShotgunLimitedAmmo() {
			Shotgun.Get<RangeComponent>().ShotsRemaining = 1;
			ShotgunAmmo.Get<Item>().Amount = 3;

			Assert.IsFalse(Shotgun.Get<RangeComponent>().SwapClips);
			Assert.IsTrue(Shotgun.Get<RangeComponent>().OneInTheChamber);
			Assert.AreEqual(Shotgun.Get<RangeComponent>().ShotsRemaining, 1);
			Assert.AreEqual(ShotgunAmmo.Get<Item>().Amount, 3);

			new ReloadAction(Entity, Shotgun, ShotgunAmmo).OnProcess();

			while (Entity.Get<ActorComponent>().Controller.Actions.Count > 0) {
				Entity.Get<ActorComponent>().NextAction().OnProcess();
			}

			Assert.AreEqual(Shotgun.Get<RangeComponent>().ShotsRemaining, 4);

			// we used all of our ammo
			Assert.IsFalse(Entity.Get<ContainerComponent>().Contains(ShotgunAmmo));
			Assert.IsFalse(EntityManager.Contains(ShotgunAmmo));
		}

		[Test]
		public void TestReloadShotgunFull() {
			Assert.IsFalse(Shotgun.Get<RangeComponent>().SwapClips);
			Assert.IsTrue(Shotgun.Get<RangeComponent>().OneInTheChamber);
			Assert.AreEqual(Shotgun.Get<RangeComponent>().ShotsRemaining, 11);
			Assert.AreEqual(ShotgunAmmo.Get<Item>().Amount, 30);

			new ReloadAction(Entity, Shotgun, ShotgunAmmo).OnProcess();

			while (Entity.Get<ActorComponent>().Controller.Actions.Count > 0) {
				Entity.Get<ActorComponent>().NextAction().OnProcess();
			}

			Assert.AreEqual(Shotgun.Get<RangeComponent>().ShotsRemaining, 11);
			Assert.AreEqual(ShotgunAmmo.Get<Item>().Amount, 30);
		}


	}
}
