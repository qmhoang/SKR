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
	public class EquipmentTests : ItemTestsHelper {
		protected EquipmentComponent Equipment { get { return Entity.Get<EquipmentComponent>(); } }
		[Test]
		public void TestEquip() {
			Assert.IsTrue(Equipment.ContainSlot("slot1"));
			Assert.IsFalse(Equipment.IsSlotEquipped("slot1"));

			Equipment.Equip("slot1", Slot1Item0);

			Assert.IsTrue(Equipment.IsSlotEquipped("slot1"));
			Assert.AreSame(Equipment["slot1"], Slot1Item0);

			Assert.Throws<ArgumentException>(() => Equipment.Equip("slot1", Slot1Item1));
			Assert.Throws<ArgumentException>(() => Equipment.Equip("slot2", Slot1Item0));

			Assert.Throws<ArgumentException>(delegate { Equipment.GetEquippedItemAt("invalid"); });
			Assert.Throws<ArgumentNullException>(() => Equipment.Equip("slot1", null));
		}

		[Test]
		public void TestGetter() {
			Equipment.Equip("slot1", Slot1Item0);

			Assert.AreSame(Equipment["slot1"], Slot1Item0);
			Assert.AreSame(Equipment.GetEquippedItemAt("slot1"), Slot1Item0);
			Assert.Throws<ArgumentException>(() => Equipment.GetEquippedItemAt("slot2"));	// valid slot, no item equip
		}

		[Test]
		public void TestUnequip() {
			Assert.IsTrue(Equipment.ContainSlot("slot1"));
			Assert.IsFalse(Equipment.IsSlotEquipped("slot1"));

			Equipment.Equip("slot1", Slot1Item0);
			Assert.IsTrue(Equipment.IsSlotEquipped("slot1"));

			Equipment.Unequip("slot1");
			Assert.IsFalse(Equipment.IsSlotEquipped("slot1"));
		}

		[Test]
		public void TestEquipItemOccupiesMultipleSlot() {
			Assert.IsTrue(Equipment.ContainSlot("slot1"));
			Assert.IsTrue(Equipment.ContainSlot("slot2"));

			Assert.IsFalse(Equipment.IsSlotEquipped("slot1"));
			Assert.IsFalse(Equipment.IsSlotEquipped("slot2"));

			Equipment.Equip("slot1", OccupiesSlotsItem);

			Assert.IsTrue(Equipment.IsSlotEquipped("slot1"));
			Assert.IsTrue(Equipment.IsSlotEquipped("slot2"));

			Assert.AreSame(Equipment["slot1"], OccupiesSlotsItem);
			Assert.AreSame(Equipment["slot2"], OccupiesSlotsItem);

			Equipment.Unequip("slot2");

			Assert.IsFalse(Equipment.IsSlotEquipped("slot1"));
			Assert.IsFalse(Equipment.IsSlotEquipped("slot2"));
		}

		[Test]
		public void TestCopy() {
			Equipment.Equip("slot1", Slot1Item0);
			Equipment.Equip("slot2", Slot2Item0);

			var ne = Entity.Copy().Get<EquipmentComponent>();

			// items in conainer aren't in new container
			foreach (var e in Equipment.EquippedItems) {
				CollectionAssert.DoesNotContain(ne.EquippedItems, e);
			}
			// vice versa
			foreach (var e in ne.EquippedItems) {
				CollectionAssert.DoesNotContain(Equipment.EquippedItems, e);
			}
		}
	}
}