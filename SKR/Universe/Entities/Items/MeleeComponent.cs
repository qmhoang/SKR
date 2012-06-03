using SKR.Universe.Entities.Actor;

namespace SKR.Universe.Entities.Items {
    public enum DamageType {
        Cut,
        Pierce,
        Impale,
        Crush,
    }

    public class MeleeComponent {
        public Skill Skill { get; private set; }
        public int HitBonus { get; private set; }
        public int Damage { get; private set; }
        public DamageType DamageType { get; private set; }
        public double Penetration { get; private set; }
        public int WeaponSpeed { get; private set; }
        public int Reach { get; private set; }
        public int Strength { get; private set; }
        public int Parry { get; private set; }
        public string Action { get; private set; }

        public MeleeComponent(Skill skill, int hitBonus, int damage, DamageType damageType, double penetration, int weaponSpeed, int reach, int strength, int parry, string action) {
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
        }
    }
}
