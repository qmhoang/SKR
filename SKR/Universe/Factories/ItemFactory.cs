using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Core;
using DEngine.Utility;
using SKR.Universe.Entities.Actors;
using SKR.Universe.Entities.Items;

namespace SKR.Universe.Factories {
    public abstract class ItemFactory : Factory<string, Item> {
        
    }
    // ReSharper disable RedundantArgumentName
    public sealed class SourceItemFactory : ItemFactory {
        private UniqueIdFactory idFactory;

        public SourceItemFactory(UniqueIdFactory idFactory) {
            this.idFactory = idFactory;
        }
        public override Item Construct(string refId) {
            switch (refId) {
                case "largeknife":
                    return new Item("Large Knife", refId, ItemType.OneHandedWeapon, idFactory.Construct(), 10, 4000,
                                    new MeleeComponent(refId + "swing", ItemAction.MeleeAttackSwing, "slash", "slashes", Skill.Knife,
                                                       hitBonus: 0,
                                                       damage: -1,

                                                       damageType: DamageType.Cut,
                                                       penetration: 1,
                                                       weaponSpeed: 100,
                                                       reach: 1,
                                                       strength: 6,
                                                       parry: -1));
                case "smallknife":
                    return new Item("Small Knife", refId, ItemType.OneHandedWeapon, idFactory.Construct(), 5, 3000,
                                      new MeleeComponent(refId + "thrust", ItemAction.MeleeAttackThrust, "jab", "jabs", Skill.Knife,
                                                       hitBonus: 0, 
                                                       damage: 0, 
                                                       damageType: DamageType.Impale, 
                                                       penetration: 1, 
                                                       weaponSpeed: 100, 
                                                       reach: 1, 
                                                       strength: 6, 
                                                       parry: -1));
                case "axe":
                    return new Item("Axe", refId, ItemType.OneHandedWeapon, idFactory.Construct(), 40, 5000,
                                    new MeleeComponent(refId + "swing", ItemAction.MeleeAttackSwing, "hack", "hacks", Skill.Axe,
                                                       hitBonus: 0,
                                                       damage: 2,
                                                       damageType: DamageType.Cut,
                                                       penetration: 1,
                                                       weaponSpeed: 90,
                                                       reach: 1,
                                                       strength: 11,
                                                       parry: 0));
                case "hatchet":
                    return new Item("Hatchet", refId, ItemType.OneHandedWeapon, idFactory.Construct(), 20, 4000,
                                    new MeleeComponent(refId + "swing", ItemAction.MeleeAttackSwing, "hack", "hacks", Skill.Axe,
                                                       hitBonus: 0,
                                                       damage: 0,
                                                       damageType: DamageType.Cut,
                                                       penetration: 1,
                                                       weaponSpeed: 92,
                                                       reach: 1,
                                                       strength: 8,
                                                       parry: 0));
                case "brassknuckles":
                    return new Item("Brass Knuckles", refId, ItemType.OneHandedWeapon, idFactory.Construct(), 2, 1000,
                                    new MeleeComponent(refId + "swing", ItemAction.MeleeAttackThrust, "punch", "punches", Skill.Brawling,
                                                       hitBonus: 0,
                                                       damage: 0,
                                                       damageType: DamageType.Crush,
                                                       penetration: 1,
                                                       weaponSpeed: 100,
                                                       reach: 0,
                                                       strength: 1,
                                                       parry: -1));

                case "glock17":
                    return new Item("Glock 17", refId, ItemType.OneHandedWeapon, idFactory.Construct(), 19, 60000,
                                    new MeleeComponent(refId + "swing", ItemAction.MeleeAttackSwing, "pistol whip", "pistol whips", Skill.Brawling,
                                                       hitBonus: -1,
                                                       damage: -1,
                                                       damageType: DamageType.Crush,
                                                       penetration: 1,
                                                       weaponSpeed: 85,
                                                       reach: 0,
                                                       strength: 8,
                                                       parry: -2),
                                    new FirearmComponent(refId + "shoot", ItemAction.Shoot, "shoot", "shoots", Skill.Pistol,
                                                         accuracy: 2,
                                                         damageRange: new Dice(2, 2, 2),
                                                         damageType: DamageType.Pierce,
                                                         penetration: 1,
                                                         range: 160,
                                                         roF: 17,
                                                         reloadSpeed: 3,
                                                         recoil: 2,
                                                         bulk: 2,
                                                         reliability: 18,
                                                         strength: 8,
                                                         caliber: "9x19mm"));
                case "glock17magazine":
                    return new Item("Magazine, Glock 17", refId, ItemType.Ammo, idFactory.Construct(), 6, 3200,
                                    new MagazineComponent(refId + "magazine", ItemAction.ReloadFirearm, "reload", "reloads",
                                                          shots: 17,
                                                          caliber: "9x19mm",
                                                          firearmId: "glock17"));
                case "9x19mm":
                    return new Item("9x19mm Parabellum", refId, ItemType.Ammo, idFactory.Construct(), 0, 30,
                                    new BulletComponent(refId + "bullet", ItemAction.LoadMagazine, "load", "loads",
                                                        used: false,
                                                        caliber: "9x19mm"));
            }
            return null;
        }
    }
}
// ReSharper restore RedundantArgumentName