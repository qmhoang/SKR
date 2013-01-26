using System.Collections.Generic;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using NUnit.Framework;
using SkrGame.Systems;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;

namespace SKRTests.Systems {
	[TestFixture]
	public class ItemEquipmentSubsystemTests {
		private EntityManager entityManager;
		private ItemEquippingSubsystem subsystem;
		private Entity entity;

		private Entity slot1item0;
		private Entity slot1item1;
		private Entity slot2item0;

		[SetUp]
		public void Init() {
			entityManager = new EntityManager();
			subsystem = new ItemEquippingSubsystem(entityManager);

			entity = entityManager.Create(new List<Component>()
			                              {
			                              		new EquipmentComponent(new List<string>
			                              		                       {
			                              		                       		"slot1",
			                              		                       		"slot2"
			                              		                       }),
			                              		new Location(0, 0, null)
			                              });

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
			entity.Get<EquipmentComponent>().Equip("slot1", slot1item0);
			entity.Get<EquipmentComponent>().Equip("slot2", slot2item0);
		}

		[Test]
		public void TestMovement() {
			foreach (var e in entity.Get<EquipmentComponent>().EquippedItems) {
				Assert.AreEqual(e.Get<Location>().Position, entity.Get<Location>().Position);
			}

			var p1 = new Point(4, 8);
			entity.Get<Location>().Position = p1;

			Assert.AreEqual(p1, entity.Get<Location>().Position);
			foreach (var e in entity.Get<EquipmentComponent>().EquippedItems) {
				Assert.AreEqual(e.Get<Location>().Position, entity.Get<Location>().Position);
			}

			var p2 = new Point(9, -8);
			entity.Get<Location>().Position = p2;

			Assert.AreEqual(p2, entity.Get<Location>().Position);
			foreach (var e in entity.Get<EquipmentComponent>().EquippedItems) {
				Assert.AreEqual(e.Get<Location>().Position, entity.Get<Location>().Position);
			}
		}

		[Test]
		public void TestItemUnequippedMovement() {
			var loc = new Point(0, 3812);
			entity.Get<Location>().Position = loc;

			foreach (var e in entity.Get<EquipmentComponent>().EquippedItems) {
				Assert.AreEqual(e.Get<Location>().Position, entity.Get<Location>().Position);
			}


			entity.Get<EquipmentComponent>().Unequip("slot2");

			entity.Get<Location>().Position = new Point(51, -9);
			foreach (var e in entity.Get<EquipmentComponent>().EquippedItems) {
				Assert.AreEqual(e.Get<Location>().Position, entity.Get<Location>().Position);
			}

			Assert.AreEqual(slot2item0.Get<Location>().Position, loc);
		}
	}
}