using SKR.Universe.Entities.Actors;

namespace SKR.Universe.Entities.Items {
    public enum DamageType {
        Cut,
        Pierce,
        Impale,
        Crush,
    }

    public class RangeComponent : ItemComponent {
        public Skill Skill { get; private set; }
        public int Accuracy { get; private set; }
        public int Damage { get; private set; }
        public DamageType DamageType { get; private set; }
        public double Penetration { get; private set; }
        public int Range { get; private set; }
        public int Shots { get; private set; }
        public int RoF { get; private set; }
        public int ReloadSpeed { get; private set; }
        public int WeaponSpeed { get; private set; }
        public int Reach { get; private set; }
        public int Strength { get; private set; }
        public int Parry { get; private set; }
    }

    public class MeleeComponent : ItemComponent {
        public Skill Skill { get; private set; }
        public int HitBonus { get; private set; }
        public int Damage { get; private set; }
        public DamageType DamageType { get; private set; }
        public double Penetration { get; private set; }
        public int WeaponSpeed { get; private set; }
        public int Reach { get; private set; }
        public int Strength { get; private set; }
        public int Parry { get; private set; }
        
        public MeleeComponent(Skill skill, int hitBonus, int damage, DamageType damageType, double penetration, int weaponSpeed, int reach, int strength, int parry, ItemAction action, string actionDesc, string actionDescPlural) {
            Skill = skill;
            HitBonus = hitBonus;
            Damage = damage;
            DamageType = damageType;
            Penetration = penetration;
            WeaponSpeed = weaponSpeed;
            Reach = reach;
            Strength = strength;
            Parry = parry;
            Action = action;
            ActionDescription = actionDesc;
            ActionDescriptionPlural = actionDescPlural;
        }

        public override string ToString() {
            return ActionDescription;
        }
    }
}
