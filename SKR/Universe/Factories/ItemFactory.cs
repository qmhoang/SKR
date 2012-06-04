using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SKR.Universe.Entities.Actors;
using SKR.Universe.Entities.Items;

namespace SKR.Universe.Factories {
    public abstract class ItemFactory {
        public abstract Item CreateItem(string refId);
    }

    public sealed class SourceItemFactory : ItemFactory {
        public override Item CreateItem(string refId) {
            switch (refId) {
                case "largeknife":
                    return new Item("Large Knife", refId, ItemType.OneHandedWeapon, GuidFactory.Generate(), 10, 4000,
                        new MeleeComponent(Skill.Knife, 0, -1, DamageType.Cut, 1, 100, 1, 6, -1, ItemAction.MeleeAttackSwing, "slash", "slashes"),
                        new MeleeComponent(Skill.Knife, -2, 0, DamageType.Impale, 1, 100, 1, 6, -1, ItemAction.MeleeAttackThrust, "jab", "jabs"));
                case "axe":
                    return new Item("Axe", refId, ItemType.OneHandedWeapon, GuidFactory.Generate(), 40, 5000,
                        new MeleeComponent(Skill.Axe, 0, 2, DamageType.Cut, 1, 90, 1, 11, 0, ItemAction.MeleeAttackSwing, "hack", "hacks"));
                case "hatchet":
                    return new Item("Hatchet", refId, ItemType.OneHandedWeapon, GuidFactory.Generate(), 20, 4000,
                        new MeleeComponent(Skill.Axe, 0, 0, DamageType.Cut, 1, 92, 1, 8, 0, ItemAction.MeleeAttackSwing, "hack", "hacks"));
                case "brassknuckles":
                    return new Item("Brass Knuckles", refId, ItemType.OneHandedWeapon, GuidFactory.Generate(), 2, 1000,
                        new MeleeComponent(Skill.Brawling, 0, 0, DamageType.Crush, 1, 100, 1, 1, -1, ItemAction.MeleeAttackThrust, "punch", "punches"));

            }
            return null;
        }
    }
}
