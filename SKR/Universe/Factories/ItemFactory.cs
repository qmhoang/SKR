using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Core;
using SKR.Universe.Entities.Actors;
using SKR.Universe.Entities.Items;

namespace SKR.Universe.Factories {
    public abstract class ItemFactory : Factory<string, Item> { }

    // ReSharper disable RedundantArgumentName
    public sealed class SourceItemFactory : ItemFactory {
        private Dictionary<string, ItemTemplate> templates;

        public SourceItemFactory() {
            templates = new Dictionary<string, ItemTemplate>();

            Create("largeknife",
                   new ItemTemplate
                       {
                               Name = "Large Knife",
                               Type = ItemType.OneHandedWeapon,
                               Value = 4000,
                               Weight = 10,
                               Components = new List<ItemComponent>
                                                {
                                                        new MeleeComponent(
                                                                new MeleeComponentTemplate
                                                                    {
                                                                            ComponentId = "largeknifeslash",
                                                                            Action = ItemAction.MeleeAttackSwing,
                                                                            ActionDescription = "slash",
                                                                            ActionDescriptionPlural = "slashes",
                                                                            Skill = Skill.Knife,
                                                                            HitBonus = 0,
                                                                            Damage = new Constant(-1),
                                                                            DamageType = DamageType.Cut,
                                                                            Penetration = 1,
                                                                            WeaponSpeed = 100,
                                                                            Reach = 1,
                                                                            Strength = 6,
                                                                            Parry = -1
                                                                    }
                                                                )
                                                }
                       });


            Create("smallknife",
                   new ItemTemplate
                       {
                               Name = "Small Knife",
                               Type = ItemType.OneHandedWeapon,
                               Value = 3000,
                               Weight = 5,
                               Components = new List<ItemComponent>
                                                {
                                                        new MeleeComponent(
                                                                new MeleeComponentTemplate
                                                                    {
                                                                            ComponentId = "smallknifethrust",
                                                                            Action = ItemAction.MeleeAttackThrust,
                                                                            ActionDescription = "jab",
                                                                            ActionDescriptionPlural = "jabs",
                                                                            Skill = Skill.Knife,
                                                                            HitBonus = 0,
                                                                            Damage = new Constant(0),
                                                                            DamageType = DamageType.Impale,
                                                                            Penetration = 1,
                                                                            WeaponSpeed = 100,
                                                                            Reach = 1,
                                                                            Strength = 6,
                                                                            Parry = -1
                                                                    })
                                                }
                       });

            Create("axe",
                   new ItemTemplate
                       {
                               Name = "Axe",
                               Type = ItemType.OneHandedWeapon,
                               Value = 5000,
                               Weight = 40,
                               Components = new List<ItemComponent>
                                                {
                                                        new MeleeComponent(
                                                                new MeleeComponentTemplate
                                                                    {
                                                                            ComponentId = "axeswing",
                                                                            Action = ItemAction.MeleeAttackSwing,
                                                                            ActionDescription = "hack",
                                                                            ActionDescriptionPlural = "hacks",
                                                                            Skill = Skill.Axe,
                                                                            HitBonus = 0,
                                                                            Damage = new Constant(2),
                                                                            DamageType = DamageType.Cut,
                                                                            Penetration = 1,
                                                                            WeaponSpeed = 90,
                                                                            Reach = 1,
                                                                            Strength = 11,
                                                                            Parry = 0
                                                                    })
                                                }
                       });
            Create("hatchet",
                   new ItemTemplate
                       {
                               Name = "Hatchet",
                               Type = ItemType.OneHandedWeapon,
                               Value = 4000,
                               Weight = 20,
                               Components = new List<ItemComponent>
                                                {
                                                        new MeleeComponent(
                                                                new MeleeComponentTemplate
                                                                    {
                                                                            ComponentId = "hatchetswing",
                                                                            Action = ItemAction.MeleeAttackSwing,
                                                                            ActionDescription = "hack",
                                                                            ActionDescriptionPlural = "hacks",
                                                                            Skill = Skill.Axe,
                                                                            HitBonus = 0,
                                                                            Damage = new Constant(0),
                                                                            DamageType = DamageType.Cut,
                                                                            Penetration = 1,
                                                                            WeaponSpeed = 92,
                                                                            Reach = 1,
                                                                            Strength = 8,
                                                                            Parry = 0
                                                                    })
                                                }
                       });

            Create("brassknuckles",
                   new ItemTemplate
                       {
                               Name = "Brass Knuckles",
                               Type = ItemType.OneHandedWeapon,
                               Value = 4000,
                               Weight = 20,
                               Components = new List<ItemComponent>
                                                {
                                                        new MeleeComponent(
                                                                new MeleeComponentTemplate
                                                                    {
                                                                            ComponentId = "brassknucklesswing",
                                                                            Action = ItemAction.MeleeAttackThrust,
                                                                            ActionDescription = "punch",
                                                                            ActionDescriptionPlural = "punches",
                                                                            Skill = Skill.Brawling,
                                                                            HitBonus = 0,
                                                                            Damage = new Constant(0),
                                                                            DamageType = DamageType.Crush,
                                                                            Penetration = 1,
                                                                            WeaponSpeed = 100,
                                                                            Reach = 0,
                                                                            Strength = 1,
                                                                            Parry = -1
                                                                    })
                                                }
                       });
            Create("glock17",
                   new ItemTemplate
                       {
                               Name = "Glock 17",
                               Type = ItemType.OneHandedWeapon,
                               Value = 60000,
                               Weight = 19,
                               Components = new List<ItemComponent>
                                                {
                                                        new MeleeComponent(
                                                                new MeleeComponentTemplate
                                                                    {
                                                                            ComponentId = "glock17swing",
                                                                            Action = ItemAction.MeleeAttackSwing,
                                                                            ActionDescription = "pistol whip",
                                                                            ActionDescriptionPlural = "pistol whips",
                                                                            Skill = Skill.Brawling,
                                                                            HitBonus = -1,
                                                                            Damage = new Constant(-1),
                                                                            DamageType = DamageType.Crush,
                                                                            Penetration = 1,
                                                                            WeaponSpeed = 85,
                                                                            Reach = 0,
                                                                            Strength = 8,
                                                                            Parry = -2
                                                                    }),
                                                        new FirearmComponent(
                                                                new FirearmComponentTemplate
                                                                    {
                                                                            ComponentId = "glock17shoot",
                                                                            Action = ItemAction.Shoot,
                                                                            ActionDescription = "shoot",
                                                                            ActionDescriptionPlural = "shoots",
                                                                            Skill = Skill.Pistol,
                                                                            Accuracy = 2,
                                                                            DamageRange = new Dice(2, 6, 2),
                                                                            DamageType = DamageType.Pierce,
                                                                            Penetration = 1,
                                                                            Range = 160,
                                                                            RoF = 17,
                                                                            ReloadSpeed = 3,
                                                                            Recoil = 2,
                                                                            Bulk = 2,
                                                                            Reliability = 18,
                                                                            Strength = 8,
                                                                            Caliber = "9x19mm"
                                                                    })
                                                }
                       });
            Create("glock17magazine",
                   new ItemTemplate
                       {
                               Name = "Magazine, Glock 17",
                               Type = ItemType.Ammo,
                               Value = 3200,
                               Weight = 6,
                               Components = new List<ItemComponent>
                                                {
                                                        new MagazineComponent(
                                                                new MagazineComponentTemplate
                                                                    {
                                                                            ComponentId = "glock17magazine",
                                                                            Action = ItemAction.ReloadFirearm,
                                                                            ActionDescription = "reload",
                                                                            ActionDescriptionPlural = "reloads",
                                                                            Shots = 17,
                                                                            Caliber = "9x19mm",
                                                                            FirearmId = "glock17",
                                                                    })
                                                }
                       });
            Create("9x19mm",
                   new ItemTemplate
                       {
                               Name = "9x19mm Parabellum",
                               Type = ItemType.OneHandedWeapon,
                               Value = 30,
                               Weight = 0,
                               Components = new List<ItemComponent>
                                                {
                                                        new BulletComponent(
                                                                new BulletComponentTemplate
                                                                    {
                                                                            ComponentId = "9x9mmbullet",
                                                                            Action = ItemAction.LoadMagazine,
                                                                            ActionDescription = "load",
                                                                            ActionDescriptionPlural = "loads",
                                                                            Used = false,
                                                                            Caliber = "9x19mm",
                                                                    })
                                                }
                       });
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