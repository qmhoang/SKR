using SKR.Universe.Entities.Actors;

namespace SKR.Universe.Entities.Items {
    public abstract class WeaponComponent : ItemComponent {
        public Skill Skill { get; protected set; }
        public abstract int Damage { get; }
        public abstract DamageType DamageType { get; }
        public abstract double Penetration { get; }
        public abstract int WeaponSpeed { get; }

        protected WeaponComponent(string componentId, ItemAction action, string actionDescription, string actionDescriptionPlural, Skill skill) : base(componentId, action, actionDescription, actionDescriptionPlural) {
            Skill = skill;
        }
    }
}