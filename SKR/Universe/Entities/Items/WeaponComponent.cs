using DEngine.Actor;
using DEngine.Core;
using SKR.Universe.Entities.Actors;

namespace SKR.Universe.Entities.Items {
    public abstract class WeaponComponentTemplate : ItemComponentTemplate {
        public Skill Skill { get; set; }
        public int Strength { get; set; }
    }

    public abstract class WeaponComponent : ItemComponent {
        public Skill Skill { get; protected set; }
        public IRand Damage { get; protected set; }
        public abstract DamageType DamageType { get; }
        public abstract double Penetration { get; }
        public abstract int WeaponSpeed { get; }
        public int Strength { get; protected set; }

        protected WeaponComponent(string componentId, ItemAction action, string actionDescription, string actionDescriptionPlural, Skill skill, int strength)
            : base(componentId, action, actionDescription, actionDescriptionPlural) {
            Skill = skill;
            Strength = strength;
        }
    }
}