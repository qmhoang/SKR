using DEngine.Actor;
using DEngine.Entities;

namespace SKR.Universe.Entities.Components {
    public class ActionPointComponent : EntityComponent {
        public static readonly ComponentType ComponentType = new ComponentType("actComponent");
        public override ComponentType Type {
            get { return ComponentType; }
        }

        public bool Updateable { get { return ActionPoints > 0; } }        
        public int ActionPoints { get; set; }
        public int Speed { get; set; }

        
        public override void Update() {
            if (!Updateable)
                ActionPoints += Speed;
        }

    }
}