using System.Collections.Generic;
using DEngine.Components;
using DEngine.Entities;
using NUnit.Framework;
using SkrGame.Actions;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;

namespace SKRTests.Actions {
	public class ItemTestsHelper : ActionTests {
		protected Entity Item;
		protected Entity StackedItem0;
		protected Entity StackedItem1;

		[SetUp]
		public void SetUp() {
			Item = EntityManager.Create(new List<Component>
			                            {
			                            		new Location(-1, -1, Level),
			                            		new VisibleComponent(10),
			                            		new Item(new Item.Template
			                            		         {})

			                            });

			StackedItem0 = EntityManager.Create(new List<Component>
			                                    {
			                                    		new Location(-1, -1, Level),
			                                    		new ReferenceId("item"),
			                                    		new VisibleComponent(10),
			                                    		new Item(new Item.Template
			                                    		         {
			                                    		         		StackType = StackType.Hard,
			                                    		         })

			                                    });
			StackedItem1 = EntityManager.Create(new List<Component>
			                                    {
			                                    		new Location(-1, -1, Level),
			                                    		new ReferenceId("item"),
			                                    		new VisibleComponent(10),
			                                    		new Item(new Item.Template
			                                    		         {
			                                    		         		StackType = StackType.Hard,
			                                    		         })

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