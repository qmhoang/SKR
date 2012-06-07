using DEngine.Core;
using SKR.Universe.Entities.Actors;

namespace SKR.Universe.Entities.Items {
    public class BulletComponent : ItemComponent {
        public bool Used { get; private set; }
        public string Caliber { get; private set; }

        public BulletComponent(string componentId, ItemAction action, string actionDescription, string actionDescriptionPlural, bool used, string caliber)
            : base(componentId, action, actionDescription, actionDescriptionPlural) {
            Used = used;
            Caliber = caliber;
        }
    }

    public class MagazineComponent : ItemComponent {
        public int Capacity { get; private set; }
        public int Shots { get; set; }
        public string Caliber { get; private set; }
        public string FirearmId { get; private set; }

        public MagazineComponent(string componentId, ItemAction action, string actionDescription, string actionDescriptionPlural, int shots, string caliber, string firearmId)
            : base(componentId, action, actionDescription, actionDescriptionPlural) {
            Capacity = Shots;
            Shots = shots;
            Caliber = caliber;
            FirearmId = firearmId;
        }
    }

    public class FirearmComponent : WeaponComponent {
        public int Accuracy { get; protected set; }
        public IRand DamageRange { get; protected set; }
        public override int Damage {
            get { return DamageRange.Roll(); }            
        }

        private DamageType damageType;
        public override DamageType DamageType { get { return damageType; } }

        private double penetration;
        public override double Penetration { get { return penetration; } }

        public int Range { get; protected set; }        
        public int RoF { get; protected set; }
        public int ReloadSpeed { get; protected set; }        
        public override int WeaponSpeed {
            get { return World.SecondsToSpeed(1.0 / RoF); }            
        }        
        public int Recoil { get; protected set; }
        public int Bulk { get; protected set; }
        public int Reliability { get; protected set; }
        public int Strength { get; protected set; }
        public string Caliber { get; protected set; }
        public Item Magazine { get; set; }

        public FirearmComponent(string componentId, ItemAction action, string actionDescription, string actionDescriptionPlural, Skill skill, int accuracy, IRand damageRange, DamageType damageType, double penetration, int range, int roF, int reloadSpeed, int recoil, int bulk, int reliability, int strength, string caliber) : 
                base(componentId, action, actionDescription, actionDescriptionPlural, skill) {
            Accuracy = accuracy;
            DamageRange = damageRange;
            this.damageType = damageType;
            this.penetration = penetration;
            Range = range;            
            RoF = roF;
            ReloadSpeed = reloadSpeed;            
            Recoil = recoil;
            Bulk = bulk;
            Reliability = reliability;
            Strength = strength;
            Caliber = caliber;            
        }
    }
}