using System;
using DEngine.Core;
using DEngine.Entity;
using SkrGame.Gameplay.Combat;
using SkrGame.Universe.Entities.Actors;

namespace SkrGame.Universe.Entities.Items.Components {
    public class MeleeComponentTemplate {
        public int HitBonus { get; set; }
        public Rand Damage { get; set; }
        public Action<Actor, Actor> OnHit { get; set; }
        public double Penetration { get; set; }
        public int WeaponSpeed {
            get { return World.ActionPointsToSpeed(APToAttack); }
            set { APToAttack = World.SpeedToActionPoints(value); }
        }
        public int APToAttack { get; set; }
        public int Reach { get; set; }
        public int Parry { get; set; }
        public int Targetting { get; set; }

        public string ComponentId { get; set; }

        public string ActionDescription { get; set; }
        public string ActionDescriptionPlural { get; set; }

        public string Skill { get; set; }
        public int Strength { get; set; }

        public int APToReady { get; set; }
        public bool UnreadyAfterAttack { get; set; }

        public DamageType DamageType { get; set; }
    }

    public class MeleeComponent : EntityComponent {

        public string ActionDescription { get; private set; }
        public string ActionDescriptionPlural { get; private set; }

        public int HitBonus { get; protected set; }

        public DamageType DamageType { get; protected set; }
        public double Penetration { get; protected set; }

        public Action<Actor, Actor> OnHit { get; private set; }

        public int APToAttack { get; protected set; }

        public string Skill { get; private set; }

        public Rand Damage { get; protected set; }
        public int APToReady { get; protected set; }
        public int Strength { get; protected set; }

        public int Reach { get; protected set; }
        public int Parry { get; protected set; }
        public int Targetting { get; protected set; }

        public MeleeComponent(MeleeComponentTemplate template) {
            ActionDescription = template.ActionDescription;
            ActionDescriptionPlural = template.ActionDescriptionPlural;

            Skill = template.Skill;

            HitBonus = template.HitBonus;
            Damage = template.Damage;
            DamageType = template.DamageType;
            OnHit = template.OnHit;
            Penetration = template.Penetration;
            APToReady = template.APToReady;
            APToAttack = template.APToAttack;
            Reach = template.Reach;
            Parry = template.Parry;
            Targetting = template.Targetting;
            Strength = template.Strength;
        }

        public override string ToString() {
            return ActionDescription;
        }
    }
}
