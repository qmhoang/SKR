using System.Collections.Generic;
using DEngine.Entities;
using NUnit.Framework;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Items;

namespace SKRTests.Actions {
	public class EquipItemHelper : ActionTests {
		protected Entity Slot1Item0;
		protected Entity Slot1Item1;
		protected Entity Slot2Item0;

		[SetUp]
		public void SetUp() {
			Slot1Item0 = EntityManager.Create(new List<Component>
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

			Slot1Item1 = EntityManager.Create(new List<Component>
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

			Slot2Item0 = EntityManager.Create(new List<Component>
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
	}
}