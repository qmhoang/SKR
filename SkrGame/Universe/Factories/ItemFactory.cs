//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using DEngine.Components;
//using DEngine.Core;
//using SkrGame.Gameplay.Combat;
//using SkrGame.Universe.Entities.Actors;
//using SkrGame.Universe.Entities.Items;
//using SkrGame.Universe.Entities.Items.Components;
//using log4net;
//
//namespace SkrGame.Universe.Factories {
//	public class ItemTemplate : Template {
//		protected static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
//
//		protected ItemTemplate() {
//			Add(new VisibleComponent(10),
//			    new Sprite("GENERIC_ITEM", Sprite.ITEMS_LAYER),
//				new ItemRefId("item"),
//				new Identifier("Junk", "A piece of junk."),
//			    new Item(
//			    		new Item.Template
//			    		{
//			    				Type = ItemType.Misc,
//			    				Value = 0,
//			    				Weight = 0,
//			    				Size = 0,
//			    				StackType = StackType.None,
//			    				Slot = new List<string>()
//			    		}));
//		}
//
//		public static ItemTemplate CreateItem(string id) {
//			return new ItemTemplate();
//		}
//
//		public ItemTemplate AtLocation(int x, int y, Map map) {
//			Add(new Location(new Point(x, y), map));
//			return this;
//		}
//	}
//
//	public class MeleeWeaponTemplate : ItemTemplate {
//		protected MeleeWeaponTemplate() {
//			// add generic sprite (and melee) if weapon doesn't have one
//			Add(new Sprite("WEAPON", Sprite.ITEMS_LAYER),
//				new ItemRefId("meleeweapon"),
//				new MeleeComponent(
//								new MeleeComponent.Template
//								{
//									ComponentId = "genericmeleeweapon",
//									ActionDescription = "hit",
//									ActionDescriptionPlural = "hits",
//									Skill = "skill_unarmed",
//									HitBonus = 0,
//									Damage = Rand.Constant(-10),
//									DamageType = Combat.DamageTypes["crush"],
//									Penetration = 0,
//									WeaponSpeed = 100,
//									APToReady = 10,
//									Reach = 1,
//									Strength = 1,
//									Parry = -3
//								}));
//		}
//
//		public static MeleeWeaponTemplate CreateMelee(string id) {
//			var weapon = new MeleeWeaponTemplate();
//			switch (id) {
//				case "largeknife":
//					weapon.Add(
//						//						new Sprite("LARGE_KNIFE", Sprite.ITEMS_LAYER),
//							new ItemRefId("largeknife"),
//							new Identifier("Knife, Large", "A large knife."),
//							new Item(
//									new Item.Template
//									{
//										Type = ItemType.OneHandedWeapon,
//										Value = 4000,
//										Weight = 10,
//										Size = 2,
//										StackType = StackType.None,
//										Slot = new List<string>()
//											       {
//											       		"Main Hand",
//											       }
//									}),
//							new MeleeComponent(
//									new MeleeComponent.Template
//									{
//										ComponentId = "largeknifeslash",
//										ActionDescription = "slash",
//										ActionDescriptionPlural = "slashes",
//										Skill = "skill_knife",
//										HitBonus = 0,
//										Damage = Rand.Constant(-5),
//										DamageType = Combat.DamageTypes["cut"],
//										Penetration = 1,
//										WeaponSpeed = 100,
//										APToReady = 15,
//										Reach = 1,
//										Strength = 6,
//										Parry = -1
//									}));
//					return weapon;
//
//				case "smallknife":
//					weapon.Add(
//							//						new Sprite("SMALL_KNIFE", Sprite.ITEMS_LAYER),
//							new ItemRefId("smallknife"),
//							new Identifier("Knife", "A knife."),
//							new Item(
//									new Item.Template
//									{											
//											Type = ItemType.OneHandedWeapon,
//											Value = 3000,
//											Weight = 5,
//											Size = 1,
//											StackType = StackType.None,
//											Slot = new List<string>()
//											       {
//											       		"Main Hand",
//											       }
//									}),
//							new MeleeComponent(
//									new MeleeComponent.Template
//									{
//											ComponentId = "smallknifethrust",
//											ActionDescription = "jab",
//											ActionDescriptionPlural = "jabs",
//											Skill = "skill_knife",
//											HitBonus = 0,
//											Damage = Rand.Constant(0),
//											DamageType = Combat.DamageTypes["impale"],
//											Penetration = 1,
//											WeaponSpeed = 110,
//											APToReady = 5,
//											Reach = 1,
//											Strength = 6,
//											Parry = -1
//									}));
//					return weapon;
//				case "axe":
//					weapon.Add(
//							//						new Sprite("AXE", Sprite.ITEMS_LAYER),
//							new ItemRefId("axe"),
//							new Identifier("Axe", "An axe."),
//
//							new Item(
//									new Item.Template
//									{
//											Type = ItemType.OneHandedWeapon,
//											Value = 5000,
//											Weight = 40,
//											Size = 3,
//											StackType = StackType.None,
//											Slot = new List<string>()
//											       {
//											       		"Main Hand",
//											       }
//									}),
//							new MeleeComponent(
//									new MeleeComponent.Template
//									{
//											ComponentId = "axeswing",
//											ActionDescription = "hack",
//											ActionDescriptionPlural = "hacks",
//											Skill = "skill_axe",
//											HitBonus = 0,
//											Damage = Rand.Constant(10),
//											DamageType = Combat.DamageTypes["cut"],
//											Penetration = 1,
//											WeaponSpeed = 90,
//											APToReady = 25,
//											Reach = 1,
//											Strength = 11,
//											Parry = 0
//									}));
//					return weapon;
//
//				case "hatchet":
//					weapon.Add(
//							//						new Sprite("HATCHET", Sprite.ITEMS_LAYER),
//							new ItemRefId("hatchet"),
//							new Identifier("Hatchet", "A hatchet."),
//							new Item(
//									new Item.Template
//									{
//											Type = ItemType.OneHandedWeapon,
//											Value = 4000,
//											Weight = 20,
//											Size = 2,
//											StackType = StackType.None,
//											Slot = new List<string>()
//											       {
//											       		"Main Hand",
//											       }
//									}),
//							new MeleeComponent(
//									new MeleeComponent.Template
//									{
//											ComponentId = "hatchetswing",
//											ActionDescription = "hack",
//											ActionDescriptionPlural = "hacks",
//											Skill = "skill_axe",
//											HitBonus = 0,
//											Damage = Rand.Constant(0),
//											DamageType = Combat.DamageTypes["cut"],
//											Penetration = 1,
//											WeaponSpeed = 92,
//											Reach = 1,
//											Strength = 8,
//											Parry = 0
//									}));
//					return weapon;
//				case "brassknuckles":
//					weapon.Add(
//							//							new Sprite("BRASS_KNUCKLES", Sprite.ITEMS_LAYER),
//							new ItemRefId("brassknuckles"),
//							new Identifier("Brass Knuckles"),
//							new Item(
//									new Item.Template
//									{
//											Type = ItemType.OneHandedWeapon,
//											Value = 1000,
//											Weight = 20,
//											StackType = StackType.None,
//											Slot = new List<string>()
//											       {
//											       		"Main Hand",
//											       }
//									}),
//							new MeleeComponent(
//									new MeleeComponent.Template
//									{
//											ComponentId = "brassknucklesswing",
//											ActionDescription = "punch",
//											ActionDescriptionPlural = "punches",
//											Skill = "skill_unarmed",
//											HitBonus = 0,
//											Damage = Rand.Constant(0),
//											DamageType = Combat.DamageTypes["crush"],
//											Penetration = 1,
//											WeaponSpeed = 100,
//											Reach = 0,
//											Strength = 1,
//											Parry = -1
//									}));
//					return weapon;
//			}
//			Logger.WarnFormat("Melee weapon: {0} not found returning generic melee weapon.", id);
//			return weapon;
//		}
//	}
//
//	public class GunWeaponTemplate : MeleeWeaponTemplate {
//		protected GunWeaponTemplate() {
//			// add generic gun component
//			Add(new Sprite("GUN", Sprite.ITEMS_LAYER),
//			    new RangeComponent(
//			    		new RangeComponent.Template
//			    		{
//			    				ComponentId = "genericgun",
//			    				ActionDescription = "shoot",
//			    				ActionDescriptionPlural = "shoots",
//			    				Skill = "skill_pistol",
//			    				Accuracy = 2,
//			    				Damage = Rand.Constant(1),
//			    				DamageType = Combat.DamageTypes["pierce"],
//			    				Penetration = 1,
//			    				Shots = 1,
//			    				Range = 100,
//			    				RoF = 1,
//								APToReady = World.SecondsToActionPoints(1f),			    				
//			    				Recoil = 1,
//			    				Reliability = 18,
//			    				Strength = 8,
//			    				AmmoType = "unused",
//			    		}));
//		}
//
//		private static GunWeaponTemplate PistolWhipBySize(int pistolSize) {
//			var weapon = new GunWeaponTemplate
//			             {
//			             		new MeleeComponent(
//			             				new MeleeComponent.Template
//			             				{
//			             						ComponentId = "pistolwhipmelee",
//			             						ActionDescription = "pistol whip",
//			             						ActionDescriptionPlural = "pistol whips",
//			             						Skill = "skill_unarmed",
//			             						HitBonus = -1,
//			             						Damage = Rand.Constant(5 * (pistolSize - 2)),
//			             						DamageType = Combat.DamageTypes["crush"],
//			             						Penetration = 1,
//			             						WeaponSpeed = 85,
//			             						Reach = 0,
//			             						Strength = 8,
//			             						Parry = -2
//			             				})
//			             };
//
//			return weapon;
//		}
//		
//		public static GunWeaponTemplate CreateGun(string id) {
//			switch (id) {
//				case "glock17": {
//						var weapon = GunWeaponTemplate.PistolWhipBySize(2);
//
//						weapon.Add(
//								new Sprite("GLOCK17", Sprite.ITEMS_LAYER),
//								new Identifier("Glock 17"),
//								new ItemRefId("glock17"),
//								new Item(new Item.Template
//										 {
//											 Type = ItemType.OneHandedWeapon,
//											 Value = 60000,
//											 Weight = 19,
//											 Size = 2,
//											 StackType = StackType.None,
//											 Slot = new List<string>
//							         		       {
//							         		       		"Main Hand",
//							         		       }
//										 }),
//								new RangeComponent(
//										new RangeComponent.Template
//										{
//											ComponentId = "glock17shoot",
//											ActionDescription = "shoot",
//											ActionDescriptionPlural = "shoots",
//											Skill = "skill_pistol",
//											Accuracy = 2,
//											Damage = Rand.Dice(2, World.STANDARD_DEVIATION * 2) + Rand.Constant(10),
//											DamageType = Combat.DamageTypes["pierce"],
//											Penetration = 1,
//											Shots = 17,
//											Range = 160,
//											RoF = 3,
//											ReloadSpeed = 3,
//											Recoil = 2,
//											Reliability = 18,
//											Strength = 8,
//											AmmoType = "9x19mm",
//										}));
//						return weapon;
//					}
//				case "glock22":
//				{
//					var weapon = GunWeaponTemplate.PistolWhipBySize(2);
//
//					weapon.Add(
//							//							new Sprite("GLOCK22", Sprite.ITEMS_LAYER),
//							new Identifier("Glock 22"),
//							new ItemRefId("glock22"),
//							new Item(new Item.Template
//							         {
//							         		Type = ItemType.OneHandedWeapon,
//							         		Value = 40000,
//							         		Weight = 21,
//							         		Size = 2,
//							         		StackType = StackType.None,
//							         		Slot = new List<string>()
//							         		       {
//							         		       		"Main Hand",
//							         		       }
//							         }),
//
//							new RangeComponent(
//									new RangeComponent.Template
//									{
//											ComponentId = "glock22shoot",
//											ActionDescription = "shoot",
//											ActionDescriptionPlural = "shoots",
//											Skill = "skill_pistol",
//											Accuracy = 2,
//											Damage = Rand.Dice(2, World.STANDARD_DEVIATION * 2) + Rand.Constant(10),
//											DamageType = Combat.DamageTypes["pierce_large"],
//											Penetration = 1,
//											Shots = 15,
//											Range = 160,
//											RoF = 3,
//											ReloadSpeed = 3,
//											Recoil = 2,
//											Reliability = 18,
//											Strength = 8,
//											AmmoType = ".40S&W"
//									}));
//						return weapon;
//				}				
//			}
//			Logger.WarnFormat("Gun weapon: {0} not found returning generic gun.", id);
//			return new GunWeaponTemplate();
//		}
//	}
//
//	public class AmmoTemplate : ItemTemplate {
//		protected AmmoTemplate() {
//			Add(new Sprite("BULLET", Sprite.ITEMS_LAYER),
//			    new ItemRefId("bullet"),
//			    new Identifier("Bullets"),
//				new Item(
//			    		new Item.Template
//			    		{
//			    				Type = ItemType.Ammo,
//			    				Value = 30,
//			    				Weight = 0,
//			    				Size = 0,
//			    				StackType = StackType.Hard
//			    		}),
//			    new AmmoComponent(
//			    		new AmmoComponent.Template
//			    		{
//			    				ComponentId = "bullet",
//			    				ActionDescription = "load",
//			    				ActionDescriptionPlural = "loads",
//			    				Type = "bullet",
//			    		}));
//		}
//
//		public static AmmoTemplate CreateAmmo(string id) {
//			var ammo = new AmmoTemplate();
//			
//			switch (id) {
//				case "9x9mm":
//				{
//					ammo.Add(
//							//						new Sprite("BULLET_9x19MM", Sprite.ITEMS_LAYER),
//							new ItemRefId("9x9mm"),
//							new Identifier("9x9mm", "9x19mm Parabellum bullet"),
//							new Item(
//									new Item.Template
//									{
//											Type = ItemType.Ammo,
//											Value = 30,
//											Weight = 0,
//											Size = 0,
//											StackType = StackType.Hard
//									}),
//							new AmmoComponent(
//									new AmmoComponent.Template
//									{
//											ComponentId = "9x9mmbullet",
//											ActionDescription = "load",
//											ActionDescriptionPlural = "loads",
//											Type = "9x19mm",
//									}));
//					return ammo;
//				}
//				case ".40S&W":
//				{
//					ammo.Add(
//							//							new Sprite("BULLET_.40S&W", Sprite.ITEMS_LAYER),
//							new ItemRefId(".40S&W"),
//							new Identifier(".40S&W", ".40 Smith & Wesson bullet"),
//							new Item(
//									new Item.Template
//									{
//											Type = ItemType.Ammo,
//											Value = 30,
//											Weight = 0,
//											Size = 0,
//											StackType = StackType.Hard
//									}),
//							new AmmoComponent(
//									new AmmoComponent.Template
//									{
//											ComponentId = ".40S&Wbullet",
//											ActionDescription = "load",
//											ActionDescriptionPlural = "loads",
//											Type = ".40S&W",
//									}));
//					return ammo;
//				}
//			}
//			Logger.WarnFormat("Ammo: {0} not found returning ammo gun.", id);
//
//			return ammo;
//		}
//	}
//
//	public class ArmorTemplate : ItemTemplate {
//		protected ArmorTemplate() {
//			Add(new Sprite("ARMOR", Sprite.ITEMS_LAYER),
//			    new ItemRefId("armor"),
//			    new Identifier("Generic Armor"),
//
//			    new Item(new Item.Template
//			             {
//			             		Type = ItemType.Armor,
//			             		Value = 100,
//			             		Weight = 10,
//			             		Size = 11,
//			             		StackType = StackType.None,
//			             		Slot = new List<string>
//			             		       {
//			             		       		"Torso",
//			             		       }
//			             }),
//			    new ArmorComponent(new ArmorComponent.Template
//			                       {
//			                       		ComponentId = "armor",
//			                       		DonTime = 1,
//			                       		Defenses = new List<ArmorComponent.LocationProtected>
//			                       		           {
//			                       		           		new ArmorComponent.LocationProtected("Torso", 10, new Dictionary<DamageType, int>
//			                       		           		                                                  {
//			                       		           		                                                  		{Combat.DamageTypes["true"], 0},
//			                       		           		                                                  		{Combat.DamageTypes["cut"], 1},
//			                       		           		                                                  		{Combat.DamageTypes["crush"], 1},
//			                       		           		                                                  		{Combat.DamageTypes["impale"], 1},
//			                       		           		                                                  		{Combat.DamageTypes["pierce_small"], 1},
//			                       		           		                                                  		{Combat.DamageTypes["pierce"], 1},
//			                       		           		                                                  		{Combat.DamageTypes["pierce_large"], 1},
//			                       		           		                                                  		{Combat.DamageTypes["pierce_huge"], 1},
//			                       		           		                                                  		{Combat.DamageTypes["burn"], 1},
//			                       		           		                                                  })
//			                       		           }
//			                       })
//					);
//		}
//
//		public static ArmorTemplate CreateArmor(string id) {
//			var armor = new ArmorTemplate();
//
//			switch (id) {
//				case "footballpads":
//					armor.Add(new Sprite("FOOTBALL_SHOULDER_PADS", Sprite.ITEMS_LAYER),
//					          new ItemRefId("footballpads"),
//					          new Identifier("Football Shoulder Pads"),
//							  new Item(new Item.Template
//					                   {
//					                   		Type = ItemType.Armor,
//					                   		Value = 5000,
//					                   		Weight = 50,
//					                   		Size = 11,
//					                   		StackType = StackType.None,
//					                   		Slot = new List<string>
//					                   		       {
//					                   		       		"Torso",
//					                   		       }
//					                   }),
//					          new ArmorComponent(new ArmorComponent.Template
//					                             {
//					                             		ComponentId = "footballarmor",
//					                             		DonTime = 10,
//					                             		Defenses = new List<ArmorComponent.LocationProtected>
//					                             		           {
//					                             		           		new ArmorComponent.LocationProtected("Torso", 30, new Dictionary<DamageType, int>
//					                             		           		                                                  {
//					                             		           		                                                  		{Combat.DamageTypes["true"], 0},
//					                             		           		                                                  		{Combat.DamageTypes["cut"], 8},
//					                             		           		                                                  		{Combat.DamageTypes["crush"], 15},
//					                             		           		                                                  		{Combat.DamageTypes["impale"], 6},
//					                             		           		                                                  		{Combat.DamageTypes["pierce_small"], 4},
//					                             		           		                                                  		{Combat.DamageTypes["pierce"], 4},
//					                             		           		                                                  		{Combat.DamageTypes["pierce_large"], 4},
//					                             		           		                                                  		{Combat.DamageTypes["pierce_huge"], 4},
//					                             		           		                                                  		{Combat.DamageTypes["burn"], 5},
//					                             		           		                                                  })
//					                             		           }
//					                             })
//							);
//
//					return armor;
//			}
//
//			Logger.WarnFormat("Armor: {0} not found returning generic armor.", id);
//			return armor;
//		}
//	}
//}
//
//// ReSharper restore RedundantArgumentName