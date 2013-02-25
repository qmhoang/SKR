using System.Collections.Generic;
using DEngine.Components;
using DEngine.Entities;
using NUnit.Framework;
using SkrGame.Actions.Items;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;

namespace SKRTests {
	public class ItemTestsHelper : SkrTests {
		protected Entity Item0;
		protected Entity Item1;
		protected Entity Item2;

		protected Entity StackedItem0;
		protected Entity StackedItem1;

		protected Entity Slot1Item0;
		protected Entity Slot1Item1;
		protected Entity Slot1Item2StealthBonus;
		protected Entity Slot2Item0;
		protected Entity OccupiesSlotsItem;

		[SetUp]
		public void SetUp() {
			Item0 = Create("item").Add(new GameObject(-1, -1, null));
			Item1 = Create("item").Add(new GameObject(-1, -1, null));
			Item2 = Create("item").Add(new GameObject(-1, -1, null));

			StackedItem0 = Create("bullet").Add(new GameObject(-1, -1, null));
			StackedItem1 = Create("bullet").Add(new GameObject(-1, -1, null));

			Slot1Item0 = EntityManager.Create(new List<Component>
			                                  {
			                                  		new GameObject(-1, -1, null),
			                                  		new Item(new Item.Template
			                                  		         {
			                                  		         		StackType = StackType.None,
			                                  		         		Size = 1,
			                                  		         		Weight = 1,
			                                  		         		Value = 1
			                                  		         }),
			                                  		Equipable.SingleSlot("slot1")
			                                  });

			Slot1Item1 = EntityManager.Create(new List<Component>
			                                  {
			                                  		new GameObject(-1, -1, null),
			                                  		new Item(new Item.Template
			                                  		         {
			                                  		         		StackType = StackType.None,
			                                  		         		Size = 1,
			                                  		         		Weight = 1,
			                                  		         		Value = 1
			                                  		         }),
			                                  		Equipable.SingleSlot("slot1")
			                                  });

			Slot1Item2StealthBonus = EntityManager.Create(new List<Component>
			                                              {
			                                              		new GameObject(-1, -1, null),
			                                              		new Item(new Item.Template
			                                              		         {
			                                              		         		StackType = StackType.None,
			                                              		         		Size = 1,
			                                              		         		Weight = 1,
			                                              		         		Value = 1
			                                              		         }),
			                                              		Equipable.SingleSlot("slot1"),
			                                              		new EquippedBonus(new EquippedBonus.Template
			                                              		                  {
			                                              		                  		Bonuses = new Dictionary<string, int>
			                                              		                  		          {
			                                              		                  		          		{"skill_stealth", 1}
			                                              		                  		          }
			                                              		                  })
			                                              });

			Slot2Item0 = EntityManager.Create(new List<Component>
			                                  {
			                                  		new GameObject(-1, -1, null),
			                                  		new Item(new Item.Template
			                                  		         {
			                                  		         		StackType = StackType.None,
			                                  		         		Size = 1,
			                                  		         		Weight = 1,
			                                  		         		Value = 1
			                                  		         }),
			                                  		Equipable.SingleSlot("slot2")
			                                  });

			OccupiesSlotsItem = EntityManager.Create(new List<Component>
			                                         {
			                                         		new GameObject(-1, -1, null),
			                                         		new Item(new Item.Template
			                                         		         {
			                                         		         		StackType = StackType.None,
			                                         		         		Size = 1,
			                                         		         		Weight = 1,
			                                         		         		Value = 1
			                                         		         }),
			                                         		Equipable.MultipleSlots("slot1", "slot2"),
			                                         });
		}

		protected void PickUp(Entity item, int amount = 1) {
			var action = new GetItemAction(Entity, item, amount);
			action.OnProcess();
		}

		protected void Drop(Entity item, int amount = 1) {
			var action = new DropItemAction(Entity, item, amount);
			action.OnProcess();
		}
	}
}