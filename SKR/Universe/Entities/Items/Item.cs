using System;
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
        Ammo
    }

    public class Item : IGuid, IRefId, IObject {
        public string Name { get; private set; }
        public string RefId { get; private set; }
        public long Guid { get; private set; }
        public int Weight { get; private set; }
        public int Value { get; private set; }

        public int NormalDurability { get; set; }
        public int CurrentDurability { get; set; }
        public int Hardness { get; set; }

        public ItemType Type { get; private set; }
        
        public MeleeComponent MeleeThrustWeapon { get; set; }
        public MeleeComponent MeleeSwingWeapon { get; set; }
        
        public bool IsSwing { get { return MeleeSwingWeapon != null; } }
        public bool IsThrust { get { return MeleeThrustWeapon != null; } }
        
        public Item(string name, string refId, ItemType type, long guid, int weight, int value, MeleeComponent meleeSwingWeapon, MeleeComponent meleeThrustWeapon) {
            Name = name;
            RefId = refId;
            Type = type;
            Guid = guid;
            Weight = weight;
            Value = value;
            MeleeThrustWeapon = meleeThrustWeapon;
            MeleeSwingWeapon = meleeSwingWeapon;
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