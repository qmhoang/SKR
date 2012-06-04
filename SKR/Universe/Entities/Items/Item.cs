using System;
using System.Collections.Generic;
using DEngine.Actor;
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
    }

    public abstract class ItemComponent {
        public string ActionDescription { get; protected set; }
        public string ActionDescriptionPlural { get; protected set; }
        public ItemAction Action { get; protected set; }     
        public Item ParentItem { get; internal set; }
    }

    public class Item : IUniqueId, IRefId, IObject {


        public string Name { get; private set; }
        public string RefId { get; private set; }
        public long UniqueId { get; private set; }
        public int Weight { get; private set; }
        public int Value { get; private set; }

        public int NormalDurability { get; set; }
        public int CurrentDurability { get; set; }
        public int Hardness { get; set; }

        public ItemType Type { get; private set; }

        private Dictionary<ItemAction, ItemComponent> components;

        public bool ContainsAction(ItemAction action) {
            return components.ContainsKey(action);
        }

        public ItemComponent GetComponent(ItemAction action) {
            if (ContainsAction(action))
                return components[action];
            else
                throw new ArgumentException("This item has no component for this", "action");
        }
        
        public Item(string name, string refId, ItemType type, long guid, int weight, int value, params ItemComponent[] comps) {
            Name = name;
            RefId = refId;
            Type = type;
            UniqueId = guid;
            Weight = weight;
            Value = value;
            components = new Dictionary<ItemAction, ItemComponent>();            

            foreach (var comp in comps) {
                comp.ParentItem = this;
                components.Add(comp.Action, comp);
            }
        }
        
        public Color Color {
            get {
                switch (Type) {                                      
                    case ItemType.Ammo:
                    case ItemType.TwoHandedWeapon:
                    case ItemType.OneHandedWeapon:
                        return ColorPresets.Grey;
                    case ItemType.Armor:
                    case ItemType.Shield:

                        return ColorPresets.LightBlue;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public char Ascii {
            get {
                switch (Type) {
                    case ItemType.OneHandedWeapon:
                    case ItemType.TwoHandedWeapon:
                        return '(';
                    case ItemType.Armor:
                    case ItemType.Shield:
                        return ']'; ;
                    case ItemType.Ammo:
                        return '/';
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public Point Position { get; set; }      
    }
}