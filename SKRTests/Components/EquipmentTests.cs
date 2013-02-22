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
			                              		new GameObject(0, 0, null),
			                              		new EquipmentComponent(new List<string>
			                              		                       {
			                              		                       		"slot1",
			                              		                       		"slot2"
			                              		                       })
			                              });
			equipment = entity.Get<EquipmentComponent>();

			slot1item0 = entityManager.Create(new List<Component>
			                                     {
			                                     		new GameObject(-1, -1, null),
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
			                                      		new GameObject(-1, -1, null),
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
			                                      		new GameObject(-1, -1, null),
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
		public void TestEquip() {
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
		public void TestGetter() {
			equipment.Equip("slot1", slot1item0);

			Assert.AreSame(equipment["slot1"], slot1item0);
			Assert.AreSame(equipment.GetEquippedItemAt("slot1"), slot1item0);
			Assert.Throws<ArgumentException>(() => equipment.GetEquippedItemAt("slot2"));	// valid slot, no item equip
		}

		[Test]
		public void TestUnequip() {
			Assert.IsTrue(equipment.ContainSlot("slot1"));
			Assert.IsFalse(equipment.IsSlotEquipped("slot1"));

			equipment.Equip("slot1", slot1item0);
			Assert.IsTrue(equipment.IsSlotEquipped("slot1"));

			equipment.Unequip("slot1");
			Assert.IsFalse(equipment.IsSlotEquipped("slot1"));
		}

		[Test]
		public void TestCopy() {
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