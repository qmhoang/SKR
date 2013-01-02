using System.Collections.Generic;
using DEngine.Core;
using SkrGame.Gameplay.Combat;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Entities.Items.Components;

namespace SkrGame.Universe.Factories {
	public abstract class ItemFactory : Factory<string, Item> {}

	// ReSharper disable RedundantArgumentName
	public sealed class SourceItemFactory : ItemFactory {
		private Dictionary<string, ItemTemplate> templates;

		public SourceItemFactory() {
			templates = new Dictionary<string, ItemTemplate>();

			Create("largeknife",
			       new ItemTemplate
			       {
			       		Name = "Large Knife",
			       		Asset = "LARGE_KNIFE",
			       		Type = ItemType.OneHandedWeapon,
			       		Value = 4000,
			       		Weight = 10,
			       		Size = 2,
			       		StackType = StackType.None,
			       		Slot = BodySlot.MainHand | BodySlot.OffHand,
			       		Components = new List<IItemComponentTemplate>
			       		             {
			       		             		new MeleeComponentTemplate
			       		             		{
			       		             				ComponentId = "largeknifeslash",
			       		             				ActionDescription = "slash",
			       		             				ActionDescriptionPlural = "slashes",
			       		             				Skill = "skill_knife",
			       		             				HitBonus = 0,
			       		             				Damage = Rand.Constant(-5),
			       		             				DamageType = Combat.DamageTypes["cut"],
			       		             				Penetration = 1,
			       		             				WeaponSpeed = 100,
			       		             				APToReady = 15,
			       		             				Reach = 1,
			       		             				Strength = 6,
			       		             				Parry = -1
			       		             		}
			       		             }
			       });


			Create("smallknife",
			       new ItemTemplate
			       {
			       		Name = "Small Knife",
			       		Asset = "SMALL_KNIFE",
			       		Type = ItemType.OneHandedWeapon,
			       		Value = 3000,
			       		Weight = 5,
			       		Size = 1,
			       		StackType = StackType.None,
			       		Slot = BodySlot.MainHand | BodySlot.OffHand,
			       		Components = new List<IItemComponentTemplate>
			       		             {
			       		             		new MeleeComponentTemplate
			       		             		{
			       		             				ComponentId = "smallknifethrust",
			       		             				ActionDescription = "jab",
			       		             				ActionDescriptionPlural = "jabs",
			       		             				Skill = "skill_knife",
			       		             				HitBonus = 0,
			       		             				Damage = Rand.Constant(0),
			       		             				DamageType = Combat.DamageTypes["impale"],
			       		             				Penetration = 1,
			       		             				WeaponSpeed = 110,
			       		             				APToReady = 5,
			       		             				Reach = 1,
			       		             				Strength = 6,
			       		             				Parry = -1
			       		             		}
			       		             }
			       });

			Create("axe",
			       new ItemTemplate
			       {
			       		Name = "Axe",
			       		Asset = "AXE",
			       		Type = ItemType.OneHandedWeapon,
			       		Value = 5000,
			       		Weight = 40,
			       		Size = 3,
			       		StackType = StackType.None,
			       		Slot = BodySlot.MainHand | BodySlot.OffHand,
			       		Components = new List<IItemComponentTemplate>
			       		             {
			       		             		new MeleeComponentTemplate
			       		             		{
			       		             				ComponentId = "axeswing",
			       		             				ActionDescription = "hack",
			       		             				ActionDescriptionPlural = "hacks",
			       		             				Skill = "skill_axe",
			       		             				HitBonus = 0,
			       		             				Damage = Rand.Constant(10),
			       		             				DamageType = Combat.DamageTypes["cut"],
			       		             				Penetration = 1,
			       		             				WeaponSpeed = 90,
			       		             				APToReady = 25,
			       		             				Reach = 1,
			       		             				Strength = 11,
			       		             				Parry = 0
			       		             		}
			       		             }
			       });

			Create("hatchet",
			       new ItemTemplate
			       {
			       		Name = "Hatchet",
			       		Asset = "HATCHET",
			       		Type = ItemType.OneHandedWeapon,
			       		Value = 4000,
			       		Weight = 20,
			       		Size = 2,
			       		StackType = StackType.None,
			       		Slot = BodySlot.MainHand | BodySlot.OffHand,
			       		Components = new List<IItemComponentTemplate>
			       		             {
			       		             		new MeleeComponentTemplate
			       		             		{
			       		             				ComponentId = "hatchetswing",
			       		             				ActionDescription = "hack",
			       		             				ActionDescriptionPlural = "hacks",
			       		             				Skill = "skill_axe",
			       		             				HitBonus = 0,
			       		             				Damage = Rand.Constant(0),
			       		             				DamageType = Combat.DamageTypes["cut"],
			       		             				Penetration = 1,
			       		             				WeaponSpeed = 92,
			       		             				Reach = 1,
			       		             				Strength = 8,
			       		             				Parry = 0
			       		             		}
			       		             }
			       });

			Create("brassknuckles",
			       new ItemTemplate
			       {
			       		Name = "Brass Knuckles",
			       		Asset = "BRASS_KNUCKLES",
			       		Type = ItemType.OneHandedWeapon,
			       		Value = 1000,
			       		Weight = 20,
			       		StackType = StackType.None,
			       		Slot = BodySlot.MainHand | BodySlot.OffHand,
			       		Components = new List<IItemComponentTemplate>
			       		             {
			       		             		new MeleeComponentTemplate
			       		             		{
			       		             				ComponentId = "brassknucklesswing",
			       		             				ActionDescription = "punch",
			       		             				ActionDescriptionPlural = "punches",
			       		             				Skill = "skill_unarmed",
			       		             				HitBonus = 0,
			       		             				Damage = Rand.Constant(0),
			       		             				DamageType = Combat.DamageTypes["crush"],
			       		             				Penetration = 1,
			       		             				WeaponSpeed = 100,
			       		             				Reach = 0,
			       		             				Strength = 1,
			       		             				Parry = -1
			       		             		}
			       		             }
			       });

			#region Old Firearms

			Create("glock17",
			       new ItemTemplate
			       {
			       		Name = "Glock 17",
			       		Asset = "GLOCK17",
			       		Type = ItemType.OneHandedWeapon,
			       		Value = 60000,
			       		Weight = 19,
						Size = 2,
			       		StackType = StackType.None,
						Slot = BodySlot.MainHand | BodySlot.OffHand,
			       		Components = new List<IItemComponentTemplate>
			       		             {
			       		             		new MeleeComponentTemplate
			       		             		{
			       		             				ComponentId = "glock17swing",
			       		             				ActionDescription = "pistol whip",
			       		             				ActionDescriptionPlural = "pistol whips",
			       		             				Skill = "skill_unarmed",
			       		             				HitBonus = -1,
			       		             				Damage = Rand.Constant(-5),
			       		             				DamageType = Combat.DamageTypes["crush"],
			       		             				Penetration = 1,
			       		             				WeaponSpeed = 85,
			       		             				Reach = 0,
			       		             				Strength = 8,
			       		             				Parry = -2
			       		             		},
			       		             		new RangeComponentTemplate
			       		             		{
			       		             				ComponentId = "glock17shoot",
			       		             				ActionDescription = "shoot",
			       		             				ActionDescriptionPlural = "shoots",
			       		             				Skill = "skill_pistol",
			       		             				Accuracy = 2,
			       		             				Damage = Rand.Dice(2, World.STANDARD_DEVIATION) + Rand.Constant(10),
			       		             				DamageType = Combat.DamageTypes["pierce"],
			       		             				Penetration = 1,
			       		             				Shots = 17,
			       		             				Range = 160,
			       		             				RoF = 3,
			       		             				ReloadSpeed = 3,
			       		             				Recoil = 2,
			       		             				Reliability = 18,
			       		             				Strength = 8,
			       		             				AmmoType = "9x19mm",
			       		             		}
			       		             }
			       });

			Create("glock22",
			       new ItemTemplate
			       {
			       		Name = "Glock 22",
			       		Asset = "GLOCK22",
			       		Type = ItemType.OneHandedWeapon,
			       		Value = 40000,
			       		Weight = 21,
			       		Size = 2,
			       		StackType = StackType.None,
						Slot = BodySlot.MainHand | BodySlot.OffHand,
			       		Components = new List<IItemComponentTemplate>
			       		             {

			       		             		new MeleeComponentTemplate
			       		             		{
			       		             				ComponentId = "glock22swing",
			       		             				ActionDescription = "pistol whip",
			       		             				ActionDescriptionPlural = "pistol whips",
			       		             				Skill = "skill_unarmed",
			       		             				HitBonus = -1,
			       		             				Damage = Rand.Constant(-5),
			       		             				DamageType = Combat.DamageTypes["crush"],
			       		             				Penetration = 1,
			       		             				WeaponSpeed = 85,
			       		             				Reach = 0,
			       		             				Strength = 8,
			       		             				Parry = -2
			       		             		},
			       		             		new RangeComponentTemplate()
			       		             		{
			       		             				ComponentId = "glock22shoot",
			       		             				ActionDescription = "shoot",
			       		             				ActionDescriptionPlural = "shoots",
			       		             				Skill = "skill_pistol",
			       		             				Accuracy = 2,
			       		             				Damage = Rand.Dice(2, World.STANDARD_DEVIATION) + Rand.Constant(10),
			       		             				DamageType = Combat.DamageTypes["pierce_large"],
			       		             				Penetration = 1,
			       		             				Shots = 15,
			       		             				Range = 160,
			       		             				RoF = 3,
			       		             				ReloadSpeed = 3,
			       		             				Recoil = 2,
			       		             				Reliability = 18,
			       		             				Strength = 8,
			       		             				AmmoType = ".40S&W"
			       		             		}
			       		             }
			       });

			Create("9x19mm",
			       new ItemTemplate
			       {
			       		Name = "9x19mm Parabellum",
			       		Asset = "BULLET_9x19MM",
			       		Type = ItemType.OneHandedWeapon,
			       		Value = 30,
			       		Weight = 0,
						Size = 0,
			       		StackType = StackType.Hard,
			       		Components = new List<IItemComponentTemplate>
			       		             {
			       		             		new AmmoComponentTemplate
			       		             		{
			       		             				ComponentId = "9x9mmbullet",
			       		             				ActionDescription = "load",
			       		             				ActionDescriptionPlural = "loads",
			       		             				Type = "9x19mm",
			       		             		}
			       		             }
			       });

			Create(".40S&W",
			       new ItemTemplate
			       {
			       		Name = ".40 Smith & Wesson",
			       		Asset = "BULLET_.40S&W",
			       		Type = ItemType.OneHandedWeapon,
			       		Value = 30,
			       		Weight = 0,
			       		Size = 0,
			       		StackType = StackType.Hard,
			       		Components = new List<IItemComponentTemplate>
			       		             {
			       		             		new AmmoComponentTemplate()
			       		             		{
			       		             				ComponentId = ".40S&Wbullet",
			       		             				ActionDescription = "load",
			       		             				ActionDescriptionPlural = "loads",
			       		             				Type = ".40S&W",
			       		             		}
			       		             }
			       });

			#endregion
		}

		private void Create(string id, ItemTemplate template) {
			template.RefId = id;
			templates.Add(id, template);
		}

		public override Item Construct(string refId) {
			return new Item(templates[refId]);
		}
	}
}

// ReSharper restore RedundantArgumentName