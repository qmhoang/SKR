using System;
using System.Collections.Generic;
using System.Linq;
using DEngine.Actor;
using DEngine.Core;

namespace SkrGame.Universe.Entities.Items {

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

    public abstract class ItemComponentTemplate {
        public string ComponentId { get; set; }
        public ItemAction Action { get; set; }

        public string ActionDescription { get; set; }
        public string ActionDescriptionPlural { get; set; }

        public Item Item { get; set; }
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

    public class Item {
        public string Name { get; private set; }

        public string Asset { get; private set; }
        public string RefId { get; private set; }
        public UniqueId UniqueId { get; private set; }
        public int Weight { get; private set; }
        public int Value { get; private set; }

        public int NormalDurability { get; set; }
        public int CurrentDurability { get; set; }
        public int Hardness { get; set; }

        public ItemType Type { get; private set; }

        private Dictionary<ItemAction, ItemComponent> components;

        public bool Is(ItemAction action) {
            return components.ContainsKey(action);
        }

        public T As<T>() where T : ItemComponent {
            return (T) components.Values.First(c => c is T);
        }

        public T As<T>(ItemAction action) where T : ItemComponent {
            if (Is(action))
                return (T)components[action];
            else
                throw new ArgumentException("This item has no component for this", "action");
        }  
   
        public Item(ItemTemplate template) {
            Name = template.Name;
            RefId = template.RefId;
            Asset = template.Asset;

            Type = template.Type;
            UniqueId = new UniqueId();
            Weight = template.Weight;
            Value = template.Value;
            components = new Dictionary<ItemAction, ItemComponent>();

            foreach (var comp in template.Components) {
                comp.Item = this;
                components.Add(comp.Action, comp);
            }
        }               
    }


    public class ItemTemplate {
        public string Name { get; set; }
        public string RefId { get; set; }
        public ItemType Type { get; set; }
        public int Weight { get; set; }
        public int Value { get; set; }
        public string Asset { get; set; }

        public IEnumerable<ItemComponent> Components { set; get; }        
    }
}