using SKR.Universe.Entities.Actors;

namespace SKR.Universe.Entities.Items {
    public enum DamageType {
        Cut,        
        Impale,
        Crush,
        Pierce,
        PierceLarge,
    }
    public class MeleeComponent : WeaponComponent {
        public int HitBonus { get; protected set; }
        private int damage;
        public override int Damage {
            get { return damage; }            
        }

        private DamageType damageType;
        public override DamageType DamageType { get { return damageType; } }

        private double penetration;
        public override double Penetration { get { return penetration; } }

        private int weaponSpeed;
        public override int WeaponSpeed { get { return weaponSpeed; } }

        public int Reach { get; protected set; }
        public int Strength { get; protected set; }
        public int Parry { get; protected set; }

        public MeleeComponent(string componentId, ItemAction action, string actionDescription, string actionDescriptionPlural, Skill skill, int hitBonus, int damage, DamageType damageType, double penetration, int weaponSpeed, int reach, int strength, int parry) : base(componentId, action, actionDescription, actionDescriptionPlural, skill) {            
            HitBonus = hitBonus;
            this.damage = damage;
            this.damageType = damageType;
            this.penetration = penetration;
            this.weaponSpeed = weaponSpeed;
            Reach = reach;
            Strength = strength;
            Parry = parry;
        }

        public override string ToString() {
            return ActionDescription;
        }
    }
}
