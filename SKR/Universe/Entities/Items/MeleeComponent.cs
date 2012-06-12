using DEngine.Actor;
using DEngine.Core;
using SKR.Universe.Entities.Actors;

namespace SKR.Universe.Entities.Items {
    public enum DamageType {
        Cut,        
        Impale,
        Crush,
        Pierce,
        PierceLarge,
    }
    public class MeleeComponentTemplate : WeaponComponentTemplate {
        public int HitBonus {get; set;} 
        public IRand Damage {get; set;} 
        public DamageType DamageType {get; set;} 
        public double Penetration {get; set;} 
        public int WeaponSpeed {get; set;} 
        public int Reach {get; set;} 
        public int Parry { get; set; }
    }

    public class MeleeComponent : WeaponComponent {
        public int HitBonus { get; protected set; }

        private DamageType damageType;
        public override DamageType DamageType { get { return damageType; } }

        private double penetration;
        public override double Penetration { get { return penetration; } }

        private int weaponSpeed;
        public override int WeaponSpeed { get { return weaponSpeed; } }

        public int Reach { get; protected set; }
        public int Strength { get; protected set; }
        public int Parry { get; protected set; }

        public MeleeComponent(MeleeComponentTemplate template) : base(template.ComponentId, template.Action, template.ActionDescription, template.ActionDescriptionPlural, template.Skill, template.Strength) {
            HitBonus = template.HitBonus;
            Damage = template.Damage;
            damageType = template.DamageType;
            penetration = template.Penetration;
            weaponSpeed = template.WeaponSpeed;
            Reach = template.Reach;            
            Parry = template.Parry;
        }

        public override string ToString() {
            return ActionDescription;
        }
    }
}
