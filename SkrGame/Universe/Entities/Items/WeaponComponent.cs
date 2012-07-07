using DEngine.Core;
using SkrGame.Universe.Entities.Actors;

namespace SkrGame.Universe.Entities.Items {
    public enum DamageType {
        Cut,
        Impale,
        Crush,
        Pierce,
        PierceLarge,
    }

    public abstract class WeaponComponentTemplate : ItemComponentTemplate {
        public Skill Skill { get; set; }
        public int Strength { get; set; }
    }

    public abstract class WeaponComponent : ItemComponent {
        public Skill Skill { get; protected set; }
        public Rand Damage { get; protected set; }
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