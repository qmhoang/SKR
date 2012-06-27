using DEngine.Actor;
using DEngine.Core;
using DEngine.Entities;

namespace SKR.Universe.Entities.Components {
    public abstract class VisionComponent : EntityComponent {
        private static readonly ComponentType type = new ComponentType("visionComponent");
        public override ComponentType Type {
            get { return type; }
        }

        public bool HasLineOfSight(DEngine.Actor.Entity target) {
            return HasLineOfSight(target.Position);
        }
        public abstract int SightRadius { get; }
        public abstract bool HasLineOfSight(Point position);
        public abstract bool CanSpot(DEngine.Actor.Entity target);

        public override void Update() { }
    }
}
