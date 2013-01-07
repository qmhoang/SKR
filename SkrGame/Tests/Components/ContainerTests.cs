using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using DEngine.Entity;
using NUnit.Framework;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;

namespace SkrGame.Tests.Components {
	[TestFixture]
	class ContainerTests {
		private EntityManager entityManager;
		private Entity entity;
		private ContainerComponent container;
		private Entity singleItem;
		private Entity equipableItem;

		[SetUp]
		public void Init() {
			entityManager = new EntityManager();

			entity = entityManager.Create(new Template()
			                              {
			                              		new ContainerComponent(new List<string>
			                              		                       {
			                              		                       		"slot1",
																			"slot2"
			                              		                       })
			                              });
			container = entity.Get<ContainerComponent>();

			singleItem = entityManager.Create(new Template
			                                {
			                                		new Item(new Item.Template
			                                		         {
			                                		         		Name = "item",																	
			                                		         })
			                                });
			equipableItem = entityManager.Create(new Template
			                                {
			                                		new Item(new Item.Template
			                                		         {
			                                		         		Name = "equipable",																																	
																	Slot = new List<string>
																	       {
																	       		"slot1"
																	       }
			                                		         })
			                                });
		}

		[Test]
		public void TestAdd() {
			container.Add(singleItem);

			Assert.AreEqual(container.Count, 1);

			container.Add(entityManager.Create(new Template
			                                {
			                                		new Item(new Item.Template()
			                                		         {
			                                		         		Name = "item1"
			                                		         })
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
			var item1 = entityManager.Create(new Template
			                     {
			                     		new Item(new Item.Template()
			                     		         {
			                     		         		Name = "item1"
			                     		         })
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
	}
}
