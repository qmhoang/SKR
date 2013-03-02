using System;
using System.Collections.Generic;
using System.Linq;
using DEngine.Entities;
using NUnit.Framework;
using SkrGame.Actions.Items;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;

namespace SKRTests.Actions.Items {
	public class EquipItemHelper : ItemTestsHelper {
		
		protected void Equip(string slot, Entity item) {
			var action = new EquipItemAction(Entity, item, slot, true);
			action.OnProcess();
		}

		protected void Unequip(string slot) {
			var action = new UnequipItemAction(Entity, slot);
			action.OnProcess();
		}

		[Test]
		public void TestEquip() {
			Assert.AreEqual(Entity.Get<EquipmentComponent>().EquippedItems.Count(), 0);
			Assert.IsFalse(Entity.Get<EquipmentComponent>().IsSlotEquipped("slot1"));
			Assert.IsFalse(Entity.Get<EquipmentComponent>().IsSlotEquipped("slot2"));

			Equip("slot1", Slot1Item0);

			Assert.AreEqual(Entity.Get<EquipmentComponent>().EquippedItems.Count(), 1);
			Assert.IsTrue(Entity.Get<EquipmentComponent>().IsSlotEquipped("slot1"));
			Assert.AreSame(Entity.Get<EquipmentComponent>()["slot1"], Slot1Item0);

			Assert.IsFalse(Entity.Get<EquipmentComponent>().IsSlotEquipped("slot2"));
		}

		[Test]
		public void TestUnequip() {
			Equip("slot1", Slot1Item0);

			Assert.AreEqual(Entity.Get<EquipmentComponent>().EquippedItems.Count(), 1);
			Assert.IsTrue(Entity.Get<EquipmentComponent>().IsSlotEquipped("slot1"));
			Assert.IsFalse(Entity.Get<EquipmentComponent>().IsSlotEquipped("slot2"));
			Assert.AreSame(Entity.Get<EquipmentComponent>()["slot1"], Slot1Item0);

			Unequip("slot1");

			Assert.AreEqual(Entity.Get<EquipmentComponent>().EquippedItems.Count(), 0);
			Assert.IsFalse(Entity.Get<EquipmentComponent>().IsSlotEquipped("slot1"));
			Assert.IsFalse(Entity.Get<EquipmentComponent>().IsSlotEquipped("slot2"));
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void TestUnequipEmpty() {
			Unequip("slot1");			
		}

		[Test]
		public void TestEquipSkilledItem() {
			Assert.IsFalse(Entity.Get<EquipmentComponent>().IsSlotEquipped("slot1"));
			Assert.AreEqual(Entity.Get<Creature>().Skills["skill_stealth"].Temporary, 0);

			Equip("slot1", Slot1Item2StealthBonus);

			Assert.IsTrue(Entity.Get<EquipmentComponent>().IsSlotEquipped("slot1"));
			Assert.AreEqual(Entity.Get<Creature>().Skills["skill_stealth"].Temporary, 1);

			Unequip("slot1");
			Assert.IsFalse(Entity.Get<EquipmentComponent>().IsSlotEquipped("slot1"));
			Assert.AreEqual(Entity.Get<Creature>().Skills["skill_stealth"].Temporary, 0);
		}
	}
}