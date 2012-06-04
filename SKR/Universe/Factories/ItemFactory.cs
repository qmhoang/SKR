using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SKR.Universe.Entities.Actor;
using SKR.Universe.Entities.Items;

namespace SKR.Universe.Factories {
    public abstract class ItemFactory {
        public abstract Item CreateItem(string refId);
    }

    public sealed class SourceItemFactory : ItemFactory {
        public override Item CreateItem(string refId) {
            switch (refId) {
                case "knife":
                    return new Item("Large Knife", refId, ItemType.OneHandedWeapon, GuidFactory.Generate(), 10, 4000,
                        new MeleeComponent(Skill.Knife, 0, -1, DamageType.Cut, 1, 100, 1, 6, -1, ItemAction.MeleeAttackSwing, "swing"),
                        new MeleeComponent(Skill.Knife, -2, 0, DamageType.Impale, 1, 100, 1, 6, -1, ItemAction.MeleeAttackThrust, "thrust"));
            }
            return null;
        }
    }
}
