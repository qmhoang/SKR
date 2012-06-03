namespace SKR.Universe.Entities.Actor {
    public enum BodyPartType {
        Body,
        Head,
        RightArm,
        LeftArm,
        RightHand,
        LeftHand,
        Leg,
        Feet,
        Neck,
        Eyes,
        Groin,
    }

    public class BodyPart {
        public string Name { get; private set; }
        public BodyPartType Type { get; private set; }
        public int Health { get; set; }
        public int MaxHealth { get; protected set; }
        public int AttackPenalty { get; protected set; }

        public bool Crippled { get { return Health < 0; } }

        public BodyPart(string name, BodyPartType type, int health, int attackPenalty) {
            Name = name;
            Type = type;

            if (health <= 0)
                health = 1;

            Health = health;
            MaxHealth = health;
            AttackPenalty = attackPenalty;
        }
    }
}