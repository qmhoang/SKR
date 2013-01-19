using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using DEngine.Entities;
using NUnit.Framework;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;

namespace SkrGame.Tests.Components {
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
			                                     		new Item(new Item.Template
			                                     		         {
			                                     		         		Slot = new List<string>
			                                     		         		       {
			                                     		         		       		"slot1"
			                                     		         		       }
			                                     		         })
			                                     });

			equipableItem2 = entityManager.Create(new List<Component>
			                                      {
			                                      		new Item(new Item.Template
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

	[TestFixture]
	internal class ContainerTests {
		private EntityManager entityManager;

		private Entity entity;
		private ContainerComponent container;

		private Entity singleItem;

		[SetUp]
		public void Init() {
			entityManager = new EntityManager();

			entity = entityManager.Create(new List<Component>()
			                              {
			                              		new ContainerComponent()
			                              });
			container = entity.Get<ContainerComponent>();

			singleItem = entityManager.Create(new List<Component>
			                                  {
			                                  		new Item(new Item.Template
			                                  		         {})
			                                  });
		}

		[Test]
		public void TestAdd() {
			container.Add(singleItem);

			Assert.AreEqual(container.Count, 1);

			container.Add(entityManager.Create(new List<Component>
			                                   {
			                                   		new Item(new Item.Template()
			                                   		         {})
			                                   }));
			Assert.AreEqual(container.Count, 2);

			Assert.Throws<ArgumentNullException>(() => container.Add(null));

			// adding an item that exist already returns false
			Assert.IsFalse(container.Add(singleItem));
		}

		[Test]
		public void TestRemove() {
			container.Add(singleItem);
			Assert.AreEqual(container.Count, 1);

			Assert.IsTrue(container.Remove(singleItem));
			Assert.AreEqual(container.Count, 0);

			Assert.Throws<ArgumentNullException>(() => container.Remove(null));
			Assert.IsFalse(container.Remove(singleItem));
		}

		[Test]
		public void TestPredicate() {
			container.Add(singleItem);

			Assert.IsTrue(container.Contains(singleItem));

			Assert.IsTrue(container.Exist(i => i == singleItem));

			// testing negative
			var item1 = entityManager.Create(new List<Component>
			                                 {
			                                 		new Item(new Item.Template()
			                                 		         {})
			                                 });
			Assert.IsFalse(container.Contains(item1));
			Assert.IsFalse(container.Exist(i => i == item1));
		}

		[Test]
		public void TestGet() {
			container.Add(singleItem);
			Assert.IsNull(container.GetItem(i => i.Id.ToString() == ""));
			Assert.AreSame(container.GetItem(i => i.Id == singleItem.Id), singleItem);
		}

//		[Test]
//		public void TestCopy() {
//			Assert.Ignore();
//		}
	}
}