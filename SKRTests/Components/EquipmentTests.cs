using System;
using System.Collections.Generic;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using DEngine.Extensions;
using NUnit.Framework;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;

namespace SKRTests.Components {
	[TestFixture]
	public class EquipmentTests {
		private EntityManager entityManager;

		private Entity entity;
		private EquipmentComponent equipment;

		private Entity slot1item0;
		private Entity slot1item1;
		private Entity slot2item0;

		[SetUp]
		public void SetUp() {
			entityManager = new EntityManager();

			entity = entityManager.Create(new List<Component>
			                              {
			                              		new Location(0, 0, null),
			                              		new EquipmentComponent(new List<string>
			                              		                       {
			                              		                       		"slot1",
			                              		                       		"slot2"
			                              		                       })
			                              });
			equipment = entity.Get<EquipmentComponent>();

			slot1item0 = entityManager.Create(new List<Component>
			                                     {
			                                     		new Location(-1, -1, null),
			                                     		new Equipable(new Equipable.Template
			                                     		              {
			                                     		              		Slot = new List<string>
			                                     		              		       {
			                                     		              		       		"slot1"
			                                     		              		       }
			                                     		              })
			                                     });

			slot1item1 = entityManager.Create(new List<Component>
			                                      {
			                                      		new Location(-1, -1, null),
			                                      		new Equipable(new Equipable.Template
			                                      		              {
			                                      		              		Slot = new List<string>
			                                      		              		       {
			                                      		              		       		"slot1"
			                                      		              		       }
			                                      		              })
			                                      });

			slot2item0 = entityManager.Create(new List<Component>
			                                      {
			                                      		new Location(-1, -1, null),
			                                      		new Equipable(new Equipable.Template
			                                      		              {
			                                      		              		Slot = new List<string>
			                                      		              		       {
			                                      		              		       		"slot2"
			                                      		              		       }
			                                      		              })
			                                      });
		}

		[Test]
		public void Equip() {
			Assert.IsTrue(equipment.ContainSlot("slot1"));
			Assert.IsFalse(equipment.IsSlotEquipped("slot1"));

			equipment.Equip("slot1", slot1item0);

			Assert.IsTrue(equipment.IsSlotEquipped("slot1"));
			Assert.AreSame(equipment["slot1"], slot1item0);

			Assert.Throws<ArgumentException>(() => equipment.Equip("slot1", slot1item1));
			Assert.Throws<ArgumentException>(() => equipment.Equip("slot2", slot1item0));

			Assert.Throws<ArgumentException>(delegate { equipment.GetEquippedItemAt("invalid"); });
			Assert.Throws<ArgumentNullException>(() => equipment.Equip("slot1", null));
		}

		[Test]
		public void Getter() {
			equipment.Equip("slot1", slot1item0);

			Assert.AreSame(equipment["slot1"], slot1item0);
			Assert.AreSame(equipment.GetEquippedItemAt("slot1"), slot1item0);
			Assert.Throws<ArgumentException>(() => equipment.GetEquippedItemAt("slot2"));	// valid slot, no item equip
		}

		[Test]
		public void Unequip() {
			Assert.IsTrue(equipment.ContainSlot("slot1"));
			Assert.IsFalse(equipment.IsSlotEquipped("slot1"));

			equipment.Equip("slot1", slot1item0);
			Assert.IsTrue(equipment.IsSlotEquipped("slot1"));

			equipment.Unequip("slot1");
			Assert.IsFalse(equipment.IsSlotEquipped("slot1"));
		}

		[Test]
		public void Move() {
			equipment.Equip("slot1", slot1item0);
			equipment.Equip("slot2", slot2item0);

			equipment.EquippedItems.Each(e => Assert.AreEqual(e.Get<Location>().Point, entity.Get<Location>().Point));
			
			var p1 = new Point(4, 8);
			entity.Get<Location>().Point = p1;

			Assert.AreEqual(p1, entity.Get<Location>().Point);
			foreach (var e in entity.Get<EquipmentComponent>().EquippedItems) {
				Assert.AreEqual(e.Get<Location>().Point, entity.Get<Location>().Point);
			}

			var p2 = new Point(9, -8);
			entity.Get<Location>().Point = p2;

			Assert.AreEqual(p2, entity.Get<Location>().Point);
			foreach (var e in entity.Get<EquipmentComponent>().EquippedItems) {
				Assert.AreEqual(e.Get<Location>().Point, entity.Get<Location>().Point);
			}
		}


		[Test]
		public void UnequippedMove() {
			equipment.Equip("slot1", slot1item0);
			equipment.Equip("slot2", slot2item0);

			var loc = new Point(0, 3812);
			entity.Get<Location>().Point = loc;

			foreach (var e in entity.Get<EquipmentComponent>().EquippedItems) {
				Assert.AreEqual(e.Get<Location>().Point, entity.Get<Location>().Point);
			}


			entity.Get<EquipmentComponent>().Unequip("slot2");

			entity.Get<Location>().Point = new Point(51, -9);
			foreach (var e in entity.Get<EquipmentComponent>().EquippedItems) {
				Assert.AreEqual(e.Get<Location>().Point, entity.Get<Location>().Point);
			}

			Assert.AreEqual(slot2item0.Get<Location>().Point, loc);
		}

		[Test]
		public void Copy() {
			equipment.Equip("slot1", slot1item0);
			equipment.Equip("slot2", slot2item0);

			var ne = entity.Copy().Get<EquipmentComponent>();

			// items in conainer aren't in new container
			foreach (var e in equipment.EquippedItems) {
				CollectionAssert.DoesNotContain(ne.EquippedItems, e);
			}
			// vice versa
			foreach (var e in ne.EquippedItems) {
				CollectionAssert.DoesNotContain(equipment.EquippedItems, e);
			}
		}
	}
}