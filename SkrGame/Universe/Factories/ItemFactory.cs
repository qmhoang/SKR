using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using DEngine.Random;
using SkrGame.Gameplay.Combat;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Entities.Items.Equipables;
using SkrGame.Universe.Entities.Items.Tools;
using log4net;

namespace SkrGame.Universe.Factories {
	public static class ItemFactory {
		public static void Init(EntityFactory ef) {
			ef.Add("base_item",
			       new VisibleComponent(10),
			       new Sprite("ITEM", Sprite.ItemsLayer),
			       new Identifier("Junk", "A piece of junk."),
			       new Item(0, 0, 0, 0, StackType.None));

			InitMelees(ef);
			InitPistols(ef);
			InitAmmos(ef);
			InitArmors(ef);

			ef.Inherits("paperclip", "base_item",
			            new Identifier("Paperclip", "A single paperclip."),
			            new Lockpick(-World.StandardDeviation * 3 / 2));
			ef.Inherits("lockpick", "base_item",
			            new Identifier("Lockpick", "A basic lockpick."),
			            new Lockpick(0));
		}

		private static void InitPistols(EntityFactory ef) {
			ef.Inherits("base_gun", "base_meleeweapon",
			            new Sprite("GUN", Sprite.ItemsLayer),
			            new RangeComponent(
			            		new RangeComponent.Template
			            		{
			            				ActionDescription = "shoot",
			            				ActionDescriptionPlural = "shoots",
			            				Skill = "skill_pistol",
			            				Accuracy = 2,
			            				Damage = Rand.Constant(1),
			            				DamageType = Combat.DamageTypes["pierce"],
			            				Penetration = 1,
			            				Shots = 1,
			            				Range = 100,
			            				RoF = 1,
			            				APToReady = World.SecondsToActionPoints(1f),
			            				APToReload = World.SecondsToActionPoints(1f),
			            				Recoil = 1,
			            				Reliability = 18,
			            				Strength = 8,
			            				AmmoCaliber = "base_bullet",
			            		}));

			var pistolTemplate = new MeleeComponent.Template
			               {
			               		ActionDescription = "pistol whip",
			               		ActionDescriptionPlural = "pistol whips",
			               		Skill = "skill_unarmed",
			               		HitBonus = -1,
			               		Damage = Rand.Constant(World.StandardIncrement * (1 - 2)),
			               		DamageType = Combat.DamageTypes["crush"],
			               		Penetration = 1,
			               		AttackSpeed = .86 * World.OneSecondInSpeed,
			               		APToReady = 100,
			               		Reach = 0,
			               		Strength = 8,
			               		Parry = -2
			               };
			ef.Inherits("base_pistol1", "base_gun",
			            new MeleeComponent(
			            		pistolTemplate));

			pistolTemplate.Damage = Rand.Constant(World.StandardIncrement * (2 - 2));
			pistolTemplate.AttackSpeed *= .95;

			ef.Inherits("base_pistol2", "base_gun",
			            new MeleeComponent(pistolTemplate));

			pistolTemplate.Damage = Rand.Constant(World.StandardIncrement * (3 - 2));
			pistolTemplate.AttackSpeed *= .95;

			ef.Inherits("base_pistol3", "base_gun",
			            new MeleeComponent(pistolTemplate));

			ef.Inherits("glock17", "base_pistol2",
			            //new Sprite("GLOCK17", Sprite.ITEMS_LAYER),
			            new Identifier("Glock 17"),
			            new Item(new Item.Template
			                     {
			                     		Value = 60000,
			                     		Weight = 19,
			                     		Size = 2,
			                     		StackType = StackType.None,
			                     }),
			            new RangeComponent(
			            		new RangeComponent.Template
			            		{
			            				ActionDescription = "shoot",
			            				ActionDescriptionPlural = "shoots",
			            				Skill = "skill_pistol",
			            				Accuracy = 2,
			            				Damage = Rand.Dice(2, World.StandardDeviation * 2) + Rand.Constant(2 * World.StandardIncrement),
			            				DamageType = Combat.DamageTypes["pierce"],
			            				Penetration = 1,
			            				Shots = 17,
			            				Range = 160,
			            				RoF = 3,
			            				APToReady = World.SecondsToActionPoints(1f),
			            				APToReload = World.SecondsToActionPoints(3),
			            				Recoil = 2,
			            				Reliability = 18,
			            				Strength = 8,
										SwapClips = true,
										OneInTheChamber = true,
			            				AmmoCaliber = "9x19mm",
			            		}));

			ef.Inherits("glock22", "base_pistol2",
			            //new Sprite("GLOCK22", Sprite.ITEMS_LAYER),
			            new Identifier("Glock 22"),
			            new Item(new Item.Template
			                     {
			                     		Value = 40000,
			                     		Weight = 21,
			                     		Size = 2,
			                     		StackType = StackType.None,
			                     }),
			            new RangeComponent(
			            		new RangeComponent.Template
			            		{
			            				ActionDescription = "shoot",
			            				ActionDescriptionPlural = "shoots",
			            				Skill = "skill_pistol",
			            				Accuracy = 2,
			            				Damage = Rand.Dice(2, World.StandardDeviation * 2) + Rand.Constant(10),
			            				DamageType = Combat.DamageTypes["pierce_large"],
			            				Penetration = 1,
			            				Shots = 15,
			            				Range = 160,
			            				RoF = 3,
			            				APToReady = World.SecondsToActionPoints(1f),
			            				APToReload = World.SecondsToActionPoints(3),
			            				Recoil = 2,
			            				Reliability = 18,
			            				Strength = 8,
										SwapClips = true,
										OneInTheChamber = true,
			            				AmmoCaliber = ".40S&W"
			            		}));

			ef.Inherits("model10", "base_pistol2",
			            //new Sprite("GLOCK22", Sprite.ITEMS_LAYER),
			            new Identifier("S&W Model 10"),
			            new Item(new Item.Template
			                     {
			                     		Value = 50000,
			                     		Weight = 20,
			                     		Size = 2,
			                     		StackType = StackType.None,
			                     }),
			            new RangeComponent(
			            		new RangeComponent.Template
			            		{
			            				ActionDescription = "shoot",
			            				ActionDescriptionPlural = "shoots",
			            				Skill = "skill_pistol",
			            				Accuracy = 2,
			            				Damage = Rand.Dice(2, World.StandardDeviation * 2),
			            				DamageType = Combat.DamageTypes["pierce_large"],
			            				Penetration = 1,
			            				Shots = 6,
			            				Range = 110,
			            				RoF = 3,
			            				APToReady = World.SecondsToActionPoints(1f),
			            				APToReload = World.SecondsToActionPoints(3),
			            				Recoil = 2,
			            				Reliability = 18,
			            				Strength = 9,
										SwapClips = false,
										OneInTheChamber = false,
			            				AmmoCaliber = ".38S"
			            		}));

			ef.Inherits("model27", "base_pistol2",
			            //new Sprite("MODEL27", Sprite.ITEMS_LAYER),
			            new Identifier("S&W Model 27"),
			            new Item(new Item.Template
			                     {
			                     		Value = 60000,
			                     		Weight = 30,
			                     		Size = 2,
			                     		StackType = StackType.None,
			                     }),
			            new RangeComponent(
			            		new RangeComponent.Template
			            		{
			            				ActionDescription = "shoot",
			            				ActionDescriptionPlural = "shoots",
			            				Skill = "skill_pistol",
			            				Accuracy = 2,
			            				Damage = Rand.Dice(3, World.StandardDeviation * 2),
			            				DamageType = Combat.DamageTypes["pierce_large"],
			            				Penetration = 1,
			            				Shots = 6,
			            				Range = 190,
			            				RoF = 3,
			            				APToReady = World.SecondsToActionPoints(1f),
			            				APToReload = World.SecondsToActionPoints(3),
			            				Recoil = 3,
			            				Reliability = 18,
			            				Strength = 10,
										SwapClips = false,
										OneInTheChamber = false,
			            				AmmoCaliber = ".357M"
			            		}));

			ef.Inherits("cpython", "base_pistol2",
			            //new Sprite("CPYTHON", Sprite.ITEMS_LAYER),
			            new Identifier("Colt Python"),
			            new Item(new Item.Template
			                     {
			                     		Value = 85000,
			                     		Weight = 29,
			                     		Size = 2,
			                     		StackType = StackType.None,
			                     }),
			            new RangeComponent(
			            		new RangeComponent.Template
			            		{
			            				ActionDescription = "shoot",
			            				ActionDescriptionPlural = "shoots",
			            				Skill = "skill_pistol",
			            				Accuracy = 2,
			            				Damage = Rand.Dice(3, World.StandardDeviation * 2),
			            				DamageType = Combat.DamageTypes["pierce_large"],
			            				Penetration = 1,
			            				Shots = 6,
			            				Range = 190,
			            				RoF = 3,
			            				APToReady = World.SecondsToActionPoints(1f),
			            				APToReload = World.SecondsToActionPoints(3),
			            				Recoil = 3,
			            				Reliability = 18,
			            				Strength = 10,
										SwapClips = false,
										OneInTheChamber = false,
			            				AmmoCaliber = ".357M"
			            		}));

		}

		private static void InitMelees(EntityFactory ef) {
			ef.Inherits("base_meleeweapon", "base_item",
			            new Sprite("WEAPON", Sprite.ItemsLayer),
						Equipable.SingleSlot("Main Hand", "Off Hand"),
			            new MeleeComponent(
			            		new MeleeComponent.Template
			            		{
			            				ActionDescription = "hit",
			            				ActionDescriptionPlural = "hits",
			            				Skill = "skill_unarmed",
			            				HitBonus = 0,
			            				Damage = Rand.Constant(-10),
			            				DamageType = Combat.DamageTypes["crush"],
			            				Penetration = 1,
										AttackSpeed = World.OneSecondInSpeed,
			            				APToReady = World.SecondsToActionPoints(1f),
			            				Reach = 1,
			            				Strength = 1,
			            				Parry = -3
			            		}));

			ef.Inherits("base_2hmelee", "base_meleeweapon",
			            Equipable.MultipleSlots("Main Hand", "Off Hand"));

			ef.Inherits("largeknife", "base_meleeweapon",
			            //new Sprite("LARGE_KNIFE", Sprite.ITEMS_LAYER),
			            new Identifier("Knife, Large", "A large knife."),
			            new Item(
			            		new Item.Template
			            		{
			            				Value = 4000,
			            				Weight = 10,
			            				Size = 2,
			            				StackType = StackType.None,

			            		}),
			            new MeleeComponent(
			            		new MeleeComponent.Template
			            		{
			            				ActionDescription = "slash",
			            				ActionDescriptionPlural = "slashes",
			            				Skill = "skill_knife",
			            				HitBonus = 0,
			            				Damage = Rand.Constant(-5),
			            				DamageType = Combat.DamageTypes["cut"],
			            				Penetration = 1,
										AttackSpeed = 1.1 * World.OneSecondInSpeed,
			            				APToReady = World.SecondsToActionPoints(1f),
			            				Reach = 1,
			            				Strength = 6,
			            				Parry = -1
			            		}));

			ef.Inherits("axe", "base_meleeweapon",
			            //new Sprite("AXE", Sprite.ITEMS_LAYER),
			            new Identifier("Axe", "An axe."),
			            new Item(
			            		new Item.Template
			            		{
			            				Value = 5000,
			            				Weight = 40,
			            				Size = 3,
			            				StackType = StackType.None,
			            		}),
			            new MeleeComponent(
			            		new MeleeComponent.Template
			            		{
			            				ActionDescription = "hack",
			            				ActionDescriptionPlural = "hacks",
			            				Skill = "skill_axe",
			            				HitBonus = 0,
			            				Damage = Rand.Constant(10),
			            				DamageType = Combat.DamageTypes["cut"],
			            				Penetration = 1,
										AttackSpeed = .9 * World.OneSecondInSpeed,
			            				APToReady = World.SecondsToActionPoints(1f),
			            				Reach = 1,
			            				Strength = 11,
			            				Parry = 0
			            		}));

			ef.Inherits("hatchet", "base_meleeweapon",
			            //new Sprite("HATCHET", Sprite.ITEMS_LAYER),
			            new Identifier("Hatchet", "A hatchet."),
			            new Item(
			            		new Item.Template
			            		{
			            				Value = 4000,
			            				Weight = 20,
			            				Size = 2,
			            				StackType = StackType.None,
			            		}),
			            new MeleeComponent(
			            		new MeleeComponent.Template
			            		{
			            				ActionDescription = "hack",
			            				ActionDescriptionPlural = "hacks",
			            				Skill = "skill_axe",
			            				HitBonus = 0,
			            				Damage = Rand.Constant(0),
			            				DamageType = Combat.DamageTypes["cut"],
			            				Penetration = 1,
										AttackSpeed = .92 * World.OneSecondInSpeed,
			            				APToReady = World.SecondsToActionPoints(1f),
			            				Reach = 1,
			            				Strength = 8,
			            				Parry = 0
			            		}));

			ef.Inherits("brassknuckles", "base_meleeweapon",
			            //new Sprite("BRASS_KNUCKLES", Sprite.ITEMS_LAYER),
			            new Identifier("Brass Knuckles"),
			            new Item(
			            		new Item.Template
			            		{
			            				Value = 1000,
			            				Weight = 20,
										Size = 1,
			            				StackType = StackType.None,
			            		}),
			            new MeleeComponent(
			            		new MeleeComponent.Template
			            		{
			            				ActionDescription = "punch",
			            				ActionDescriptionPlural = "punches",
			            				Skill = "skill_unarmed",
			            				HitBonus = 0,
			            				Damage = Rand.Constant(0),
			            				DamageType = Combat.DamageTypes["crush"],
			            				Penetration = 1,
										AttackSpeed = World.OneSecondInSpeed,
			            				APToReady = World.SecondsToActionPoints(1f),
			            				Reach = 0,
			            				Strength = 1,
			            				Parry = -1
			            		}));

			ef.Inherits("smallknife", "base_meleeweapon",
			            //new Sprite("SMALL_KNIFE", Sprite.ITEMS_LAYER),
			            new Identifier("Knife", "A knife."),
			            new Item(
			            		new Item.Template
			            		{
			            				Value = 3000,
			            				Weight = 5,
			            				Size = 1,
			            				StackType = StackType.None,
			            		}),
			            new MeleeComponent(
			            		new MeleeComponent.Template
			            		{
			            				ActionDescription = "jab",
			            				ActionDescriptionPlural = "jabs",
			            				Skill = "skill_knife",
			            				HitBonus = 0,
			            				Damage = Rand.Constant(0),
			            				DamageType = Combat.DamageTypes["impale"],
			            				Penetration = 1,
										AttackSpeed = 1.2 * World.OneSecondInSpeed,
			            				APToReady = World.SecondsToActionPoints(1f),
			            				Reach = 1,
			            				Strength = 6,
			            				Parry = -1
			            		}));

			ef.Inherits("club", "base_meleeweapon",
			            new Identifier("Club", "A good size club."),
			            new Item(new Item.Template
			                     {
			                     		Value = 3000,
			                     		Weight = 30,
			                     		Size = 3,
			                     		StackType = StackType.None,
			                     }),
			            new MeleeComponent(
			            		new MeleeComponent.Template
			            		{
			            				ActionDescription = "club",
			            				ActionDescriptionPlural = "clubs",
			            				Skill = "skill_axe",
			            				HitBonus = 0,
			            				Damage = Rand.Constant(7),
			            				DamageType = Combat.DamageTypes["crush"],
			            				Penetration = 1,
										AttackSpeed = .92 * World.OneSecondInSpeed,
			            				APToReady = World.SecondsToActionPoints(1f),
			            				Reach = 1,
			            				Strength = 10,
			            				Parry = 0
			            		}));

			ef.Inherits("baseball_bat", "base_2hmelee",
			            new Identifier("Baseball bat", "A baseball bat."),
			            new Item(new Item.Template
			                     {
			                     		Value = 4000,
			                     		Weight = 40,
			                     		Size = 4,
			                     		StackType = StackType.None,
			                     }),
			            new MeleeComponent(
			            		new MeleeComponent.Template
			            		{
			            				ActionDescription = "club",
			            				ActionDescriptionPlural = "clubs",
			            				Skill = "skill_2haxe",
			            				HitBonus = 0,
			            				Damage = Rand.Constant(12),
										DamageType = Combat.DamageTypes["crush"],
			            				Penetration = 1,
										AttackSpeed = .88 * World.OneSecondInSpeed,
			            				APToReady = World.SecondsToActionPoints(1f),
			            				Reach = 1,
			            				Strength = 10,
			            				Parry = 0
			            		}));
		}

		private static void InitAmmos(EntityFactory ef) {
			ef.Inherits("base_bullet", "base_item",
			            new Sprite("BULLET", Sprite.ItemsLayer),
			            new Identifier("Bullets"),
			            new Item(
			            		new Item.Template
			            		{
			            				Value = 30,
			            				Weight = 0,
			            				Size = 0,
			            				StackType = StackType.Hard
			            		}),
			            new AmmoComponent(
			            		new AmmoComponent.Template
			            		{
			            				ActionDescription = "load",
			            				ActionDescriptionPlural = "loads",
			            				Caliber = "base_bullet",
			            		}));

			ef.Inherits(".40S&W", "base_bullet",
			            //new Sprite("BULLET_.40S&W", Sprite.ITEMS_LAYER),
			            new Identifier(".40S&W", ".40 Smith & Wesson bullet"),
			            new Item(
			            		new Item.Template
			            		{
			            				Value = 30,
			            				Weight = 0,
			            				Size = 0,
			            				StackType = StackType.Hard
			            		}),
			            new AmmoComponent(
			            		new AmmoComponent.Template
			            		{
			            				ActionDescription = "load",
			            				ActionDescriptionPlural = "loads",
			            				Caliber = ".40S&W",
			            		}));

			ef.Inherits("9x9mm", "base_bullet",
			            //new Sprite("BULLET_9x19MM", Sprite.ITEMS_LAYER),
			            new Identifier("9x9mm", "9x19mm Parabellum bullet"),
			            new Item(
			            		new Item.Template
			            		{
			            				Value = 30,
			            				Weight = 0,
			            				Size = 0,
			            				StackType = StackType.Hard
			            		}),
			            new AmmoComponent(
			            		new AmmoComponent.Template
			            		{
			            				ActionDescription = "load",
			            				ActionDescriptionPlural = "loads",
			            				Caliber = "9x19mm",
			            		}));

			ef.Inherits(".357M", "base_bullet",
			            new Identifier(".357 Magnum", "base_bullet"),
			            new Item(
			            		new Item.Template
			            		{
			            				Value = 30,
			            				Weight = 0,
			            				Size = 0,
			            				StackType = StackType.Hard
			            		}),
			            new AmmoComponent(
			            		new AmmoComponent.Template
			            		{
			            				ActionDescription = "load",
			            				ActionDescriptionPlural = "loads",
			            				Caliber = ".357M",
			            		}));

			ef.Inherits(".38S", "base_bullet",
			            new Identifier(".357 Special", "base_bullet"),
			            new Item(
			            		new Item.Template
			            		{
			            				Value = 30,
			            				Weight = 0,
			            				Size = 0,
			            				StackType = StackType.Hard
			            		}),
			            new AmmoComponent(
			            		new AmmoComponent.Template
			            		{
			            				ActionDescription = "load",
			            				ActionDescriptionPlural = "loads",
			            				Caliber = ".38S",
			            		}));

			ef.Inherits(".44M", "base_bullet",
			            new Identifier(".44 Magnum", "base_bullet"),
			            new Item(
			            		new Item.Template
			            		{
			            				Value = 30,
			            				Weight = 0,
			            				Size = 0,
			            				StackType = StackType.Hard
			            		}),
			            new AmmoComponent(
			            		new AmmoComponent.Template
			            		{
			            				ActionDescription = "load",
			            				ActionDescriptionPlural = "loads",
			            				Caliber = ".44M",
			            		}));
		}

		private static void InitArmors(EntityFactory ef) {
			#region Shoes
			ef.Inherits("shoes", "base_item",
			            new Sprite("SHOES", Sprite.ItemsLayer),
						new Identifier("Shoes", "A pair of plain shoes."),
			            new Item(new Item.Template
			                     {
			                     		Value = 3500,
			                     		Weight = 20,
			                     		Size = 2,
			                     		StackType = StackType.None,
			                     }),
						Equipable.SingleSlot("Feet"),
			            new ArmorComponent(
			            		new ArmorComponent.Template
			            		{
			            				DonTime = 1,
			            				Defenses = new List<ArmorComponent.Part>
			            				           {
			            				           		new ArmorComponent.Part("Feet",
			            				           		                        50,
			            				           		                        new Dictionary<DamageType, int>
			            				           		                        {
			            				           		                        		{Combat.DamageTypes["true"], 0},
			            				           		                        		{Combat.DamageTypes["cut"], 2},
			            				           		                        		{Combat.DamageTypes["crush"], 2},
			            				           		                        		{Combat.DamageTypes["impale"], 2},
			            				           		                        		{Combat.DamageTypes["pierce_small"], 2},
			            				           		                        		{Combat.DamageTypes["pierce"], 2},
			            				           		                        		{Combat.DamageTypes["pierce_large"], 2},
			            				           		                        		{Combat.DamageTypes["pierce_huge"], 2},
			            				           		                        		{Combat.DamageTypes["burn"], 2},
			            				           		                        })
			            				           }
			            		}));

			ef.Inherits("sneakers", "shoes",
			            new Identifier("Sneakers", "A pair of sneakers."),
			            new Item(new Item.Template
			                     {
			                     		Value = 4000,
			                     		Weight = 17,
			                     		Size = 2,
			                     		StackType = StackType.None,
			                     }),
			            new EquippedBonus(new EquippedBonus.Template
			                               {
			                               		Bonuses = new Dictionary<string, int>
			                               		          {
			                               		          		{"skill_stealth", 2}
			                               		          }
			                               }));

			ef.Inherits("flipflop", "shoes",
			            new Identifier("Flip-flops", "A pair of thong flip-flops."),
			            new Item(new Item.Template
			                     {
			                     		Value = 3000,
			                     		Weight = 12,
			                     		Size = 2,
			                     		StackType = StackType.None,
			                     }),
			            new EquippedBonus(new EquippedBonus.Template
			                              {
			                              		Bonuses = new Dictionary<string, int>
			                              		          {
//			                               		          		{"skill_run", -5}
			                              		          }
			                              }));

			ef.Inherits("heels", "shoes",
			            new Identifier("Heels", "A pair of women's high heel shoes."),
			            new Item(new Item.Template
			                     {
			                     		Value = 9000,
			                     		Weight = 13,
			                     		Size = 2,
			                     		StackType = StackType.None,
			                     }),
								 // reaction?  influence?  appearence?
			            new EquippedBonus(new EquippedBonus.Template
			                              {
			                              		Bonuses = new Dictionary<string, int>
			                              		          {
//			                               		          		{"skill_run", -10}
			                              		          }
			                              }));

			ef.Inherits("cleats", "sneakers",
			            new Identifier("Cleats", "A pair of atheltic shoes fitted with spiked cleats."),
			            new Item(new Item.Template
			                     {
			                     		Value = 5000,
			                     		Weight = 20,
			                     		Size = 2,
			                     		StackType = StackType.None,
			                     }),
			            new EquippedBonus(new EquippedBonus.Template
			                              {
			                              		Bonuses = new Dictionary<string, int>
			                              		          {
			                              		          		{"skill_stealth", -2}
			                              		          }
			                              }));

			ef.Inherits("boots", "shoes",
			            new Identifier("Boots", "A pair of good boots."),
						new Item(new Item.Template
			                     {
			                     		Value = 2700,
			                     		Weight = 25,
			                     		Size = 2,
			                     		StackType = StackType.None,
			                     }),
			            new ArmorComponent(
			            		new ArmorComponent.Template
			            		{
			            				DonTime = 1,
			            				Defenses = new List<ArmorComponent.Part>
			            				           {
			            				           		new ArmorComponent.Part("Feet",
			            				           		                        65,
			            				           		                        new Dictionary<DamageType, int>
			            				           		                        {
			            				           		                        		{Combat.DamageTypes["true"], 0},
			            				           		                        		{Combat.DamageTypes["cut"], 7},
			            				           		                        		{Combat.DamageTypes["crush"], 7},
			            				           		                        		{Combat.DamageTypes["impale"], 7},
			            				           		                        		{Combat.DamageTypes["pierce_small"], 5},
			            				           		                        		{Combat.DamageTypes["pierce"], 5},
			            				           		                        		{Combat.DamageTypes["pierce_large"], 6},
			            				           		                        		{Combat.DamageTypes["pierce_huge"], 6},
			            				           		                        		{Combat.DamageTypes["burn"], 7},
			            				           		                        })
			            				           }
			            		}));

			ef.Inherits("ruggedboots", "base_item",
						new Identifier("Rugged boots", "A pair of rugged work boots with a steel toe."),
						new Item(new Item.Template
						{
							Value = 8000,
							Weight = 30,
							Size = 2,
							StackType = StackType.None,
						}),
						new ArmorComponent(
								new ArmorComponent.Template
								{
									DonTime = 1,
									Defenses = new List<ArmorComponent.Part>
			            				           {
			            				           		new ArmorComponent.Part("Feet",
			            				           		                        70,
			            				           		                        new Dictionary<DamageType, int>
			            				           		                        {
			            				           		                        		{Combat.DamageTypes["true"], 0},
			            				           		                        		{Combat.DamageTypes["cut"], 9},
			            				           		                        		{Combat.DamageTypes["crush"], 9},
			            				           		                        		{Combat.DamageTypes["impale"], 9},
			            				           		                        		{Combat.DamageTypes["pierce_small"], 9},
			            				           		                        		{Combat.DamageTypes["pierce"], 9},
			            				           		                        		{Combat.DamageTypes["pierce_large"], 9},
			            				           		                        		{Combat.DamageTypes["pierce_huge"], 9},
			            				           		                        		{Combat.DamageTypes["burn"], 9},
			            				           		                        })
			            				           }
								}));

			#endregion

			#region Gloves
			ef.Inherits("gloves", "base_item",
						new Sprite("GLOVES", Sprite.ItemsLayer),
						new Identifier("Gloves", "A pair of normal gloves."),
						new Item(new Item.Template
						{
							Value = 3500,
							Weight = 5,
							Size = 1,
							StackType = StackType.None,
						}),
						Equipable.SingleSlot("Hands"),
						new ArmorComponent(
								new ArmorComponent.Template
								{
									DonTime = 1,
									Defenses = new List<ArmorComponent.Part>
			            				           {
			            				           		new ArmorComponent.Part("Hands",
			            				           		                        50,
			            				           		                        new Dictionary<DamageType, int>
			            				           		                        {
			            				           		                        		{Combat.DamageTypes["true"], 0},
			            				           		                        		{Combat.DamageTypes["cut"], 1},
			            				           		                        		{Combat.DamageTypes["crush"], 1},
			            				           		                        		{Combat.DamageTypes["impale"], 1},
			            				           		                        		{Combat.DamageTypes["pierce_small"], 1},
			            				           		                        		{Combat.DamageTypes["pierce"], 1},
			            				           		                        		{Combat.DamageTypes["pierce_large"], 1},
			            				           		                        		{Combat.DamageTypes["pierce_huge"], 1},
			            				           		                        		{Combat.DamageTypes["burn"], 1},
			            				           		                        })
			            				           }
								}));
			#endregion

			#region Pants

			ef.Inherits("pants", "base_item",
			            new Sprite("PANTS", Sprite.ItemsLayer),
			            new Identifier("Pants", "A pair of khaki pants."),
			            new Item(new Item.Template
			                     {
			                     		Value = 4000,
			                     		Weight = 15,
			                     		Size = 3,
			                     		StackType = StackType.None,
			                     }),
						Equipable.SingleSlot("Legs"),
			            new ArmorComponent(
			            		new ArmorComponent.Template
			            		{
			            				DonTime = 1,
			            				Defenses = new List<ArmorComponent.Part>
			            				           {
			            				           		new ArmorComponent.Part("Feet",
			            				           		                        50,
			            				           		                        new Dictionary<DamageType, int>
			            				           		                        {
			            				           		                        		{Combat.DamageTypes["true"], 0},
			            				           		                        		{Combat.DamageTypes["cut"], 1},
			            				           		                        		{Combat.DamageTypes["crush"], 1},
			            				           		                        		{Combat.DamageTypes["impale"], 1},
			            				           		                        		{Combat.DamageTypes["pierce_small"], 1},
			            				           		                        		{Combat.DamageTypes["pierce"], 1},
			            				           		                        		{Combat.DamageTypes["pierce_large"], 1},
			            				           		                        		{Combat.DamageTypes["pierce_huge"], 1},
			            				           		                        		{Combat.DamageTypes["burn"], 1},
			            				           		                        })
			            				           }
			            		}));

			ef.Inherits("skirt", "pants",
			            new Identifier("Skirt", "A short cotton skirt."),
			            new Item(new Item.Template
			                     {
			                     		Value = 5000,
			                     		Weight = 12,
			                     		Size = 3,
			                     		StackType = StackType.None,
			                     }));

			ef.Inherits("jeans", "pants",
						new Sprite("PANTS", Sprite.ItemsLayer),
			            new Identifier("Jeans", "A pair of blue jeans."),
			            new Item(new Item.Template
			                     {
			                     		Value = 5000,
			                     		Weight = 15,
			                     		Size = 3,
			                     		StackType = StackType.None,
			                     }));
			#endregion

			#region Shirts
			ef.Inherits("shirt", "base_item",
			            new Sprite("SHIRT", Sprite.ItemsLayer),
			            new Identifier("Shirt", "A hawaiian shirt."),
			            new Item(new Item.Template
			                     {
			                     		Value = 2000,
			                     		Weight = 10,
			                     		Size = 11,
			                     		StackType = StackType.None,

			                     }),
						Equipable.SingleSlot("Torso"),
			            new ArmorComponent(
			            		new ArmorComponent.Template
			            		{
			            				DonTime = 1,
			            				Defenses = new List<ArmorComponent.Part>
			            				           {
			            				           		new ArmorComponent.Part("Torso",
			            				           		                        20,
			            				           		                        new Dictionary<DamageType, int>
			            				           		                        {
			            				           		                        		{Combat.DamageTypes["true"], 0},
			            				           		                        		{Combat.DamageTypes["cut"], 1},
			            				           		                        		{Combat.DamageTypes["crush"], 1},
			            				           		                        		{Combat.DamageTypes["impale"], 1},
			            				           		                        		{Combat.DamageTypes["pierce_small"], 1},
			            				           		                        		{Combat.DamageTypes["pierce"], 1},
			            				           		                        		{Combat.DamageTypes["pierce_large"], 1},
			            				           		                        		{Combat.DamageTypes["pierce_huge"], 1},
			            				           		                        		{Combat.DamageTypes["burn"], 1},
			            				           		                        })
			            				           }
			            		}));

			ef.Inherits("footballpads", "shirt",
			            new Sprite("FOOTBALL_SHOULDER_PADS", Sprite.ItemsLayer),
			            new Identifier("Football Shoulder Pads", "Shoulder pads for football players.  Jersey not included."),
			            new Item(new Item.Template
			                     {
			                     		Value = 5000,
			                     		Weight = 50,
			                     		Size = 11,
			                     		StackType = StackType.None,
			                     }),
			            Equipable.MultipleSlots("Torso", "Arms"),
			            new ArmorComponent(
			            		new ArmorComponent.Template
			            		{
			            				DonTime = 10,
			            				Defenses = new List<ArmorComponent.Part>
			            				           {
			            				           		new ArmorComponent.Part("Torso",
			            				           		                        40,
			            				           		                        new Dictionary<DamageType, int>
			            				           		                        {
			            				           		                        		{Combat.DamageTypes["true"], 0},
			            				           		                        		{Combat.DamageTypes["cut"], 8},
			            				           		                        		{Combat.DamageTypes["crush"], 15},
			            				           		                        		{Combat.DamageTypes["impale"], 6},
			            				           		                        		{Combat.DamageTypes["pierce_small"], 4},
			            				           		                        		{Combat.DamageTypes["pierce"], 4},
			            				           		                        		{Combat.DamageTypes["pierce_large"], 4},
			            				           		                        		{Combat.DamageTypes["pierce_huge"], 4},
			            				           		                        		{Combat.DamageTypes["burn"], 5},
			            				           		                        }),
			            				           		new ArmorComponent.Part("Left Arm",
			            				           		                        30,
			            				           		                        new Dictionary<DamageType, int>
			            				           		                        {
			            				           		                        		{Combat.DamageTypes["true"], 0},
			            				           		                        		{Combat.DamageTypes["cut"], 5},
			            				           		                        		{Combat.DamageTypes["crush"], 8},
			            				           		                        		{Combat.DamageTypes["impale"], 5},
			            				           		                        		{Combat.DamageTypes["pierce_small"], 4},
			            				           		                        		{Combat.DamageTypes["pierce"], 4},
			            				           		                        		{Combat.DamageTypes["pierce_large"], 4},
			            				           		                        		{Combat.DamageTypes["pierce_huge"], 4},
			            				           		                        		{Combat.DamageTypes["burn"], 4},
			            				           		                        }),
			            				           		new ArmorComponent.Part("Right Arm",
			            				           		                        30,
			            				           		                        new Dictionary<DamageType, int>
			            				           		                        {
			            				           		                        		{Combat.DamageTypes["true"], 0},
			            				           		                        		{Combat.DamageTypes["cut"], 5},
			            				           		                        		{Combat.DamageTypes["crush"], 8},
			            				           		                        		{Combat.DamageTypes["impale"], 5},
			            				           		                        		{Combat.DamageTypes["pierce_small"], 4},
			            				           		                        		{Combat.DamageTypes["pierce"], 4},
			            				           		                        		{Combat.DamageTypes["pierce_large"], 4},
			            				           		                        		{Combat.DamageTypes["pierce_huge"], 4},
			            				           		                        		{Combat.DamageTypes["burn"], 4},
			            				           		                        }),
			            				           }
			            		}));
			#endregion
		}
	}
}

// ReSharper restore RedundantArgumentName