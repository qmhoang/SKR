using DEngine.Core;

namespace SkrGame.Universe.Entities.Items {
    public class BulletComponentTemplate : ItemComponentTemplate {
        public string Caliber { get; set; }
    }

    public class BulletComponent : ItemComponent {
        public string Caliber { get; private set; }        

        public BulletComponent(BulletComponentTemplate template)
            : base(template.ComponentId, template.Action, template.ActionDescription, template.ActionDescriptionPlural) {
            Caliber = template.Caliber;
        }
    } 
    
    public class AmountComponentTemplate : ItemComponentTemplate {
        public int Amount { get; set; }        
    }

    public class AmountComponent : ItemComponent {
        public int Amount { get; set; }

        public AmountComponent(AmountComponentTemplate template)
            : base(template.ComponentId, template.Action, template.ActionDescription, template.ActionDescriptionPlural) {
            Amount = template.Amount;
        }
    }
   
    // todo ammo cases

    public class GunComponentTemplate : WeaponComponentTemplate {
        public int Accuracy { get; set; }
        public Rand DamageRange { get; set; }
        public DamageType DamageType { get; set; }
        public double Penetration { get; set; }
        public int Range { get; set; }
        public int RoF { get; set; }        
        public int ReloadSpeed { get; set; }
        public int Recoil { get; set; }
        public int Bulk { get; set; }
        public int Reliability { get; set; }
        public string Caliber { get; set; }
        public int Shots { get; set; }
    }

    public class GunComponent : WeaponComponent {
        public int Accuracy { get; protected set; }
        public Rand DamageRange { get; protected set; }
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
        public string AmmoCaliber { get; protected set; }
        public int Shots { get; set; }
        public int ShotsRemaining { get; set; }

        public GunComponent(GunComponentTemplate template)
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
            AmmoCaliber = template.Caliber;
            ShotsRemaining = Shots = template.Shots;
        }
    }
}