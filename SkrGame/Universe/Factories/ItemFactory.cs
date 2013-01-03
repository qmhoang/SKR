using System.Collections.Generic;
using DEngine.Core;
using DEngine.Entity;
using SkrGame.Gameplay.Combat;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Entities.Items.Components;

namespace SkrGame.Universe.Factories {
	public abstract class ItemFactory : Factory<string, Template> {}

	// ReSharper disable RedundantArgumentName
	public sealed class SourceItemFactory : ItemFactory {
		private Dictionary<string, Template> templates;

		public SourceItemFactory() {
			templates = new Dictionary<string, Template>();

			Create("punch",
			       new MeleeComponent(
			       		new MeleeComponentTemplate
			       		{
			       				ComponentId = "punch",
			       				ActionDescription = "punch",
			       				ActionDescriptionPlural = "punches",
			       				Skill = "skill_unarmed",
			       				HitBonus = 0,
			       				Damage = Rand.Constant(-5),
			       				DamageType = Combat.DamageTypes["crush"],
			       				Penetration = 1,
			       				WeaponSpeed = 100,
			       				Reach = 0,
			       				Strength = 0,
			       				Parry = 0
			       		}));

			Create("largeknife",
			       new Item(
			       		new ItemTemplate
			       		{
			       				Name = "Large Knife",
			       				Asset = "LARGE_KNIFE",
			       				Type = ItemType.OneHandedWeapon,
			       				Value = 4000,
			       				Weight = 10,
			       				Size = 2,
			       				StackType = StackType.None,
			       				Slot = new List<string>()
			       				       {
			       				       		"MainHand",
			       				       		"OffHand",
			       				       }
			       		}),
			       new MeleeComponent(
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
			       		})
					);


			Create("smallknife",
			       new Item(
			       		new ItemTemplate
			       		{
			       				Name = "Small Knife",
			       				Asset = "SMALL_KNIFE",
			       				Type = ItemType.OneHandedWeapon,
			       				Value = 3000,
			       				Weight = 5,
			       				Size = 1,
			       				StackType = StackType.None,
			       				Slot = new List<string>()
			       				       {
			       				       		"MainHand",
			       				       		"OffHand",
			       				       }
			       		}),
			       new MeleeComponent(
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
			       		})
					);

			Create("axe",
			       new Item(
			       		new ItemTemplate
			       		{
			       				Name = "Axe",
			       				Asset = "AXE",
			       				Type = ItemType.OneHandedWeapon,
			       				Value = 5000,
			       				Weight = 40,
			       				Size = 3,
			       				StackType = StackType.None,
			       				Slot = new List<string>()
			       				       {
			       				       		"MainHand",
			       				       		"OffHand",
			       				       }
			       		}),
			       new MeleeComponent(
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
			       		})
					);

			Create("hatchet",
			       new Item(
			       		new ItemTemplate
			       		{
			       				Name = "Hatchet",
			       				Asset = "HATCHET",
			       				Type = ItemType.OneHandedWeapon,
			       				Value = 4000,
			       				Weight = 20,
			       				Size = 2,
			       				StackType = StackType.None,
			       				Slot = new List<string>()
			       				       {
			       				       		"MainHand",
			       				       		"OffHand",
			       				       }
			       		}),
			       new MeleeComponent(
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
			       		})
					);

			Create("brassknuckles",
			       new Item(
			       		new ItemTemplate
			       		{
			       				Name = "Brass Knuckles",
			       				Asset = "BRASS_KNUCKLES",
			       				Type = ItemType.OneHandedWeapon,
			       				Value = 1000,
			       				Weight = 20,
			       				StackType = StackType.None,
			       				Slot = new List<string>()
			       				       {
			       				       		"MainHand",
			       				       		"OffHand",
			       				       }
			       		}),
			       new MeleeComponent(
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
			       		})
					);

			#region Firearms

			Create("glock17",
			       new Item(new ItemTemplate
			                {
			                		Name = "Glock 17",
			                		Asset = "GLOCK17",
			                		Type = ItemType.OneHandedWeapon,
			                		Value = 60000,
			                		Weight = 19,
			                		Size = 2,
			                		StackType = StackType.None,
			                		Slot = new List<string>()
			                		       {
			                		       		"MainHand",
			                		       		"OffHand",
			                		       }
			                }),
			       new MeleeComponent(
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
			       		}),
			       new RangeComponent(
			       		new RangeComponentTemplate
			       		{
			       				ComponentId = "glock17shoot",
			       				ActionDescription = "shoot",
			       				ActionDescriptionPlural = "shoots",
			       				Skill = "skill_pistol",
			       				Accuracy = 2,
			       				Damage = Rand.Dice(2, World.STANDARD_DEVIATION * 2) + Rand.Constant(10),
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
			       		})
					);

			Create("glock22",
			       new Item(new ItemTemplate
			                {
			                		Name = "Glock 22",
			                		Asset = "GLOCK22",
			                		Type = ItemType.OneHandedWeapon,
			                		Value = 40000,
			                		Weight = 21,
			                		Size = 2,
			                		StackType = StackType.None,
			                		Slot = new List<string>()
			                		       {
			                		       		"MainHand",
			                		       		"OffHand",
			                		       }
			                }),
			       new MeleeComponent(
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
			       		}),
			       new RangeComponent(
			       		new RangeComponentTemplate
			       		{
			       				ComponentId = "glock22shoot",
			       				ActionDescription = "shoot",
			       				ActionDescriptionPlural = "shoots",
			       				Skill = "skill_pistol",
			       				Accuracy = 2,
			       				Damage = Rand.Dice(2, World.STANDARD_DEVIATION * 2) + Rand.Constant(10),
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
			       		})
					);

			Create("9x19mm",
			       new Item(
			       		new ItemTemplate
			       		{
			       				Name = "9x19mm Parabellum",
			       				Asset = "BULLET_9x19MM",
			       				Type = ItemType.OneHandedWeapon,
			       				Value = 30,
			       				Weight = 0,
			       				Size = 0,
			       				StackType = StackType.Hard
			       		}),
			       new AmmoComponent(
			       		new AmmoComponentTemplate
			       		{
			       				ComponentId = "9x9mmbullet",
			       				ActionDescription = "load",
			       				ActionDescriptionPlural = "loads",
			       				Type = "9x19mm",
			       		}));

			Create(".40S&W",
			       new Item(
			       		new ItemTemplate
			       		{
			       				Name = ".40 Smith & Wesson",
			       				Asset = "BULLET_.40S&W",
			       				Type = ItemType.OneHandedWeapon,
			       				Value = 30,
			       				Weight = 0,
			       				Size = 0,
			       				StackType = StackType.Hard
			       		}),
			       new AmmoComponent(
			       		new AmmoComponentTemplate
			       		{
			       				ComponentId = ".40S&Wbullet",
			       				ActionDescription = "load",
			       				ActionDescriptionPlural = "loads",
			       				Type = ".40S&W",
			       		})
					);

			#endregion
		}

		private void Create(string id, params EntityComponent[] components) {
			templates.Add(id, new Template(components));
		}

		public override Template Construct(string refId) {
			return templates[refId];
		}
	}
}

// ReSharper restore RedundantArgumentName