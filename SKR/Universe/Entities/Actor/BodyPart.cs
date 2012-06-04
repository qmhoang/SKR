using System;
using System.Collections.Generic;
using SKR.Universe.Entities.Items;

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
        public Person Owner { get; private set; }
        public int Health { get; set; }
        public int MaxHealth { get; protected set; }
        public int AttackPenalty { get; protected set; }
        private List<ItemType> acceptableItems;
        
        public BodyPart(string name, BodyPartType type, Person owner, int health, int attackPenalty, params ItemType[] acceptableItemTypes) {
            Name = name;
            Type = type;
            Owner = owner;

            if (health <= 0)
                health = 1;
            Health = health;
            MaxHealth = health;

            AttackPenalty = attackPenalty;
            acceptableItems = new List<ItemType>(acceptableItemTypes);
        }

        public bool Crippled { get { return Health < 0; } }

        public bool CanUseItem(Item item) {
            return acceptableItems.Contains(item.Type);
        }
    }
}