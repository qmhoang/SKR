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

    public class Item {
        public string Name { get; private set; }

        // Asset string is a unique string that represents what each item looks like.  A GUI uses this to draw the item depending on each item
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

        /// <summary>
        /// Get Item's Component
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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