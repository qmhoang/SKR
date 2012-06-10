using DEngine.Actor;
using DEngine.Core;
using SKR.Universe.Entities.Actors;

namespace SKR.Universe.Entities.Items {
    public class BulletComponentTemplate : ItemComponentTemplate {
        public bool Used { get; set; }
        public string Caliber { get; set; }
    }

    public class BulletComponent : ItemComponent {
        public bool Used { get; private set; }
        public string Caliber { get; private set; }

        public BulletComponent(BulletComponentTemplate template)
            : base(template.ComponentId, template.Action, template.ActionDescription, template.ActionDescriptionPlural) {
            Used = template.Used;
            Caliber = template.Caliber;
        }
    }

    public class MagazineComponentTemplate : ItemComponentTemplate {
        public int Capacity { get; set; }
        public int Shots { get; set; }
        public string Caliber { get; set; }
        public RefId FirearmId { get; set; }
    }

    public class MagazineComponent : ItemComponent {
        public int Capacity { get; private set; }
        public int Shots { get; set; }
        public string Caliber { get; private set; }
        public RefId FirearmId { get; private set; }

        public MagazineComponent(MagazineComponentTemplate template)
            : base(template.ComponentId, template.Action, template.ActionDescription, template.ActionDescriptionPlural) {
            Shots = template.Shots;
            Capacity = template.Shots;
            Caliber = template.Caliber;
            FirearmId = new RefId(template.FirearmId);
        }
    }

    public class FirearmComponentTemplate : WeaponComponentTemplate {
        public int Accuracy { get; set; }
        public IRand DamageRange { get; set; }
        public DamageType DamageType { get; set; }
        public double Penetration { get; set; }
        public int Range { get; set; }
        public int RoF { get; set; }
        public int ReloadSpeed { get; set; }
        public int Recoil { get; set; }
        public int Bulk { get; set; }
        public int Reliability { get; set; }
        public string Caliber { get; set; }
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
        public string Caliber { get; protected set; }
        public Item Magazine { get; set; }

        public FirearmComponent(FirearmComponentTemplate template)
            : base(template.ComponentId, template.Action, template.ActionDescription, template.ActionDescriptionPlural, template.Skill, template.Strength) {
            Accuracy = template.Accuracy;
            DamageRange = template.DamageRange;
            this.damageType = template.DamageType;
            this.penetration = template.Penetration;
            Range = template.Range;
            RoF = template.RoF;
            ReloadSpeed = template.ReloadSpeed;
            Recoil = template.Recoil;
            Bulk = template.Bulk;
            Reliability = template.Reliability;
            Caliber = template.Caliber; 
        }
    }
}