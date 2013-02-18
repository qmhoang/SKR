using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using SkrGame.Gameplay.Combat;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;
using log4net;

namespace SkrGame.Universe.Factories {
	public class ItemFactory {
		public static void Init(EntityFactory ef) {
			ef.Add("item",
			       new VisibleComponent(10),
			       new Sprite("ITEM", Sprite.ITEMS_LAYER),
			       new Identifier("Junk", "A piece of junk."),
			       new Item(
			       		new Item.Template
			       		{
//			       				Type = ItemType.Misc,
			       				Value = 0,
			       				Weight = 0,
			       				Size = 0,
			       				StackType = StackType.None,
			       		}));

			InitMelees(ef);
			InitPistols(ef);
			InitAmmos(ef);
			InitArmors(ef);
		}		

		private static void InitPistols(EntityFactory ef) {
			ef.Inherits("gun", "meleeweapon",
			            new Sprite("GUN", Sprite.ITEMS_LAYER),
			            new RangeComponent(
			            		new RangeComponent.Template
			            		{
//			            				ComponentId = "genericgun",
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
			            				AmmoType = "bullet",
			            		}));

			ef.Inherits("pistol1", "gun",
						new MeleeComponent(
								new MeleeComponent.Template
								{
									ComponentId = "pistolwhipmelee",
									ActionDescription = "pistol whip",
									ActionDescriptionPlural = "pistol whips",
									Skill = "skill_unarmed",
									HitBonus = -1,
									Damage = Rand.Constant(5 * (1 - 2)),
									DamageType = Combat.DamageTypes["crush"],
									Penetration = 1,
									WeaponSpeed = 85,
									APToReady = 100,
									Reach = 0,
									Strength = 8,
									Parry = -2
								}));

			ef.Inherits("pistol2", "gun",
						new MeleeComponent(
								new MeleeComponent.Template
								{
									ComponentId = "pistolwhipmelee",
									ActionDescription = "pistol whip",
									ActionDescriptionPlural = "pistol whips",
									Skill = "skill_unarmed",
									HitBonus = -1,
									Damage = Rand.Constant(5 * (2 - 2)),
									DamageType = Combat.DamageTypes["crush"],
									Penetration = 1,
									WeaponSpeed = 85,
									APToReady = 100,
									Reach = 0,
									Strength = 8,
									Parry = -2
								}));

			ef.Inherits("glock17", "pistol2",
			            //new Sprite("GLOCK17", Sprite.ITEMS_LAYER),
			            new Identifier("Glock 17"),
						new Item(new Item.Template
			                     {
//			                     		Type = ItemType.OneHandedWeapon,
			                     		Value = 60000,
			                     		Weight = 19,
			                     		Size = 2,
			                     		StackType = StackType.None,
			                     }),
			            new RangeComponent(
			            		new RangeComponent.Template
			            		{
//			            				ComponentId = "glock17shoot",
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
										APToReady = World.SecondsToActionPoints(1f),
										APToReload = World.SecondsToActionPoints(3),
			            				Recoil = 2,
			            				Reliability = 18,
			            				Strength = 8,
			            				AmmoType = "9x19mm",
			            		}));

			ef.Inherits("glock22", "pistol2",
			            //new Sprite("GLOCK22", Sprite.ITEMS_LAYER),
			            new Identifier("Glock 22"),
						new Item(new Item.Template
			                     {
//			                     		Type = ItemType.OneHandedWeapon,
			                     		Value = 40000,
			                     		Weight = 21,
			                     		Size = 2,
			                     		StackType = StackType.None,
			                     }),

			            new RangeComponent(
			            		new RangeComponent.Template
			            		{
//			            				ComponentId = "glock22shoot",
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
										APToReady = World.SecondsToActionPoints(1f),
			            				APToReload = World.SecondsToActionPoints(3),
			            				Recoil = 2,
			            				Reliability = 18,
			            				Strength = 8,
			            				AmmoType = ".40S&W"
			            		}));
			
		}

		private static void InitMelees(EntityFactory ef) {
			ef.Inherits("meleeweapon", "item",
			            new Sprite("WEAPON", Sprite.ITEMS_LAYER),
						new Equipable(
							new Equipable.Template
							{
								TwoHanded = false,
								Slot = new List<string>()
			            				       {
			            				       		"Main Hand",
													"Off Hand"
			            				       }
							}),
						new MeleeComponent(
			            		new MeleeComponent.Template
			            		{
			            				ComponentId = "genericmeleeweapon",
			            				ActionDescription = "hit",
			            				ActionDescriptionPlural = "hits",
			            				Skill = "skill_unarmed",
			            				HitBonus = 0,
			            				Damage = Rand.Constant(-10),
			            				DamageType = Combat.DamageTypes["crush"],
			            				Penetration = 1,
			            				WeaponSpeed = 100,
										APToReady = World.SecondsToActionPoints(1f),
			            				Reach = 1,
			            				Strength = 1,
			            				Parry = -3
			            		}));

			ef.Inherits("largeknife", "meleeweapon",
			            //new Sprite("LARGE_KNIFE", Sprite.ITEMS_LAYER),
						new Identifier("Knife, Large", "A large knife."),
			            new Item(
			            		new Item.Template
			            		{
//			            				Type = ItemType.OneHandedWeapon,
			            				Value = 4000,
			            				Weight = 10,
			            				Size = 2,
			            				StackType = StackType.None,

			            		}),
			            new MeleeComponent(
			            		new MeleeComponent.Template
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
										APToReady = World.SecondsToActionPoints(1f),
			            				Reach = 1,
			            				Strength = 6,
			            				Parry = -1
			            		}));

			ef.Inherits("axe", "meleeweapon",
			            //new Sprite("AXE", Sprite.ITEMS_LAYER),
			            new Identifier("Axe", "An axe."),
						new Item(
			            		new Item.Template
			            		{
//			            				Type = ItemType.OneHandedWeapon,
			            				Value = 5000,
			            				Weight = 40,
			            				Size = 3,
			            				StackType = StackType.None,
			            		}),
			            new MeleeComponent(
			            		new MeleeComponent.Template
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
										APToReady = World.SecondsToActionPoints(1f),
			            				Reach = 1,
			            				Strength = 11,
			            				Parry = 0
			            		}));

			ef.Inherits("hatchet", "meleeweapon",
			            //new Sprite("HATCHET", Sprite.ITEMS_LAYER),
			            new Identifier("Hatchet", "A hatchet."),
			            new Item(
			            		new Item.Template
			            		{
//			            				Type = ItemType.OneHandedWeapon,
			            				Value = 4000,
			            				Weight = 20,
			            				Size = 2,
			            				StackType = StackType.None,
			            		}),
			            new MeleeComponent(
			            		new MeleeComponent.Template
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
										APToReady = World.SecondsToActionPoints(1f),
			            				Reach = 1,
			            				Strength = 8,
			            				Parry = 0
			            		}));

			ef.Inherits("brassknuckles", "meleeweapon",
			            //new Sprite("BRASS_KNUCKLES", Sprite.ITEMS_LAYER),
			            new Identifier("Brass Knuckles"),
			            new Item(
			            		new Item.Template
			            		{
//			            				Type = ItemType.OneHandedWeapon,
			            				Value = 1000,
			            				Weight = 20,
			            				StackType = StackType.None,
			            		}),
			            new MeleeComponent(
			            		new MeleeComponent.Template
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
										APToReady = World.SecondsToActionPoints(1f),
			            				Reach = 0,
			            				Strength = 1,
			            				Parry = -1
			            		}));

			ef.Inherits("smallknife", "meleeweapon",
			            //new Sprite("SMALL_KNIFE", Sprite.ITEMS_LAYER),
			            new Identifier("Knife", "A knife."),
			            new Item(
			            		new Item.Template
			            		{
//			            				Type = ItemType.OneHandedWeapon,
			            				Value = 3000,
			            				Weight = 5,
			            				Size = 1,
			            				StackType = StackType.None,
			            		}),
			            new MeleeComponent(
			            		new MeleeComponent.Template
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
										APToReady = World.SecondsToActionPoints(1f),
			            				Reach = 1,
			            				Strength = 6,
			            				Parry = -1
			            		}));
		}

		private static void InitAmmos(EntityFactory ef) {
			ef.Inherits("bullet", "item",
			            new Sprite("BULLET", Sprite.ITEMS_LAYER),
						new Identifier("Bullets"),
			            new Item(
			            		new Item.Template
			            		{
//			            				Type = ItemType.Ammo,
			            				Value = 30,
			            				Weight = 0,
			            				Size = 0,
			            				StackType = StackType.Hard
			            		}),
			            new AmmoComponent(
			            		new AmmoComponent.Template
			            		{
			            				ComponentId = "bullet",
			            				ActionDescription = "load",
			            				ActionDescriptionPlural = "loads",
			            				Type = "bullet",
			            		}));

			ef.Inherits(".40S&W", "bullet",
			            //new Sprite("BULLET_.40S&W", Sprite.ITEMS_LAYER),
			            new Identifier(".40S&W", ".40 Smith & Wesson bullet"),
			            new Item(
			            		new Item.Template
			            		{
//			            				Type = ItemType.Ammo,
			            				Value = 30,
			            				Weight = 0,
			            				Size = 0,
			            				StackType = StackType.Hard
			            		}),
			            new AmmoComponent(
			            		new AmmoComponent.Template
			            		{
			            				ComponentId = ".40S&Wbullet",
			            				ActionDescription = "load",
			            				ActionDescriptionPlural = "loads",
			            				Type = ".40S&W",
			            		}));

			ef.Inherits("9x9mm", "bullet",
			            //new Sprite("BULLET_9x19MM", Sprite.ITEMS_LAYER),
			            new Identifier("9x9mm", "9x19mm Parabellum bullet"),
			            new Item(
			            		new Item.Template
			            		{
//			            				Type = ItemType.Ammo,
			            				Value = 30,
			            				Weight = 0,
			            				Size = 0,
			            				StackType = StackType.Hard
			            		}),
			            new AmmoComponent(
			            		new AmmoComponent.Template
			            		{
			            				ComponentId = "9x9mmbullet",
			            				ActionDescription = "load",
			            				ActionDescriptionPlural = "loads",
			            				Type = "9x19mm",
			            		}));
		}

		private static void InitArmors(EntityFactory ef) {
			ef.Inherits("armor", "item",
			            new Sprite("ARMOR", Sprite.ITEMS_LAYER),
						new Identifier("Generic Armor"),
			            new Item(new Item.Template
			                     {
//			                     		Type = ItemType.Armor,
			                     		Value = 100,
			                     		Weight = 10,
			                     		Size = 11,
			                     		StackType = StackType.None,

			                     }),
						new Equipable(new Equipable.Template
						              {
										  Slot = new List<string>
			                     		       {
			                     		       		"Torso",
			                     		       }
						              }),
			            new ArmorComponent(new ArmorComponent.Template
			                               {
//			                               		ComponentId = "armor",
			                               		DonTime = 1,
			                               		Defenses = new List<ArmorComponent.Part>
			                               		           {
			                               		           		new ArmorComponent.Part("Torso", 10, new Dictionary<DamageType, int>
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

			ef.Inherits("footballpads", "armor",
			            new Sprite("FOOTBALL_SHOULDER_PADS", Sprite.ITEMS_LAYER),
						new Identifier("Football Shoulder Pads"),
			            new Item(new Item.Template
			                     {
//			                     		Type = ItemType.Armor,
			                     		Value = 5000,
			                     		Weight = 50,
			                     		Size = 11,
			                     		StackType = StackType.None,
			                     }),
			            new ArmorComponent(new ArmorComponent.Template
			                               {
//			                               		ComponentId = "footballarmor",
			                               		DonTime = 10,
			                               		Defenses = new List<ArmorComponent.Part>
			                               		           {
			                               		           		new ArmorComponent.Part("Torso", 30, new Dictionary<DamageType, int>
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
			                               		           		                                                  })
			                               		           }
			                               }));
		}
	}
}

// ReSharper restore RedundantArgumentName