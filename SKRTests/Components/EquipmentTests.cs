using System;
using System.Collections.Generic;
using DEngine.Entities;
using NUnit.Framework;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;

namespace SKRTests.Components {
	[TestFixture]
	public class EquipmentTests {
		private EntityManager entityManager;

		private Entity entity;
		private EquipmentComponent equipment;

		private Entity equipableItem;
		private Entity equipableItem2;

		[SetUp]
		public void SetUp() {
			entityManager = new EntityManager();

			entity = entityManager.Create(new List<Component>
			                              {
			                              		new EquipmentComponent(new List<string>
			                              		                       {
			                              		                       		"slot1",
			                              		                       		"slot2"
			                              		                       })
			                              });
			equipment = entity.Get<EquipmentComponent>();

			equipableItem = entityManager.Create(new List<Component>
			                                     {
			                                     		new Equipable(new Equipable.Template
			                                     		              {
			                                     		              		Slot = new List<string>
			                                     		              		       {
			                                     		              		       		"slot1"
			                                     		              		       }
			                                     		              })
			                                     });

			equipableItem2 = entityManager.Create(new List<Component>
			                                      {
			                                      		new Equipable(new Equipable.Template
			                                      		              {
			                                      		              		Slot = new List<string>
			                                      		              		       {
			                                      		              		       		"slot1"
			                                      		              		       }
			                                      		              })
			                                      });
		}

		[Test]
		public void TestEquip() {
			Assert.IsTrue(equipment.ContainSlot("slot1"));
			Assert.IsFalse(equipment.IsSlotEquipped("slot1"));

			equipment.Equip("slot1", equipableItem);

			Assert.IsTrue(equipment.IsSlotEquipped("slot1"));
			Assert.AreSame(equipment["slot1"], equipableItem);

			Assert.Throws<ArgumentException>(() => equipment.Equip("slot1", equipableItem2));
			Assert.Throws<ArgumentException>(() => equipment.Equip("slot2", equipableItem));

			Assert.Throws<ArgumentException>(delegate { equipment.GetEquippedItemAt("invalid"); });
			Assert.Throws<ArgumentNullException>(() => equipment.Equip("slot1", null));
		}

		[Test]
		public void TestUnequip() {
			Assert.IsTrue(equipment.ContainSlot("slot1"));
			Assert.IsFalse(equipment.IsSlotEquipped("slot1"));

			equipment.Equip("slot1", equipableItem);

			Entity removed;
			equipment.Unequip("slot1", out removed);

			Assert.AreSame(removed, equipableItem);
			Assert.IsFalse(equipment.IsSlotEquipped("slot1"));

			equipment.Unequip("slot2", out removed);

			Assert.IsNull(removed);
		}

//		[Test]
//		public void TestCopy() {
//			Assert.Ignore();
//		}
	}
}