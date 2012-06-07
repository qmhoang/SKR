using System;
using System.Collections.Generic;
using System.Linq;
using DEngine.Actor;
using DEngine.Actor.Components.Graphics;
using DEngine.Core;
using DEngine.Extensions;

namespace SKR.Universe.Entities.Items {

    public enum ItemType {
        // weapons
        OneHandedWeapon,
        TwoHandedWeapon,
        Armor,
        Shield,
        Ammo,
        BodyPart,
    }

    public enum ItemAction {
        MeleeAttackSwing,
        MeleeAttackThrust,
        Wear,
        ReloadFirearm,
        LoadMagazine,
        Shoot,
    }

    public abstract class ItemComponent {
        public string ComponentId { get; protected set; }
        public ItemAction Action { get; protected set; }

        public Item Item { get; set; }

        public string ActionDescription { get; protected set; }
        public string ActionDescriptionPlural { get; protected set; }

        protected ItemComponent(string componentId, ItemAction action, string actionDescription, string actionDescriptionPlural) {
            ComponentId = componentId;
            Action = action;
            ActionDescription = actionDescription;
            ActionDescriptionPlural = actionDescriptionPlural;
        }
    }

    public class Item : IObject {
        public string Name { get; private set; }
        public string RefId { get; private set; }
        public UniqueId UniqueId { get; private set; }
        public int Weight { get; private set; }
        public int Value { get; private set; }

        public int NormalDurability { get; set; }
        public int CurrentDurability { get; set; }
        public int Hardness { get; set; }

        public ItemType Type { get; private set; }

        private Dictionary<ItemAction, ItemComponent> components;

        public bool ContainsComponent(ItemAction action) {
            return components.ContainsKey(action);
        }

        public T GetComponent<T>() where T : ItemComponent {
            return (T) components.Values.First(c => c is T);
        }

        public T GetComponent<T>(ItemAction action) where T : ItemComponent {
            if (ContainsComponent(action))
                return (T)components[action];
            else
                throw new ArgumentException("This item has no component for this", "action");
        }
        
        public Item(string name, string refId, ItemType type, UniqueId uid, int weight, int value, params ItemComponent[] comps) {
            Name = name;
            RefId = refId;
            Type = type;
            UniqueId = uid;
            Weight = weight;
            Value = value;
            components = new Dictionary<ItemAction, ItemComponent>();            

            foreach (var comp in comps) {
                comp.Item = this;
                components.Add(comp.Action, comp);
            }
        }       
        
        public Point Position { get; set; }      
    }
}