using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Core;
using SKR.Universe;
using SKR.Universe.Entities.Actor;
using SKR.Universe.Entities.Items;
using Attribute = SKR.Universe.Entities.Actor.Attribute;

namespace SKR.Gameplay.Combat {
    public static class MeleeCombat {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static Dice dice = new Dice(3, 6);
        private static World world = World.Instance;

        public static void AttackMeleeWithWeapon(Person attacker, Person defender, MeleeComponent weapon, BodyPart bodyPart) {
            int atkDiff = attacker.Characteristics.GetSkill(weapon.Skill);
            int atkRoll = dice.Roll();

            Logger.InfoFormat("{0} (skill: {1} @ {2}) attacks {3} rolling {4} ({5} penalty for targetting {6})", attacker.Name, weapon.Skill, atkDiff, defender.Name, atkRoll, bodyPart.AttackPenalty, bodyPart.Name);

            if ((atkRoll - bodyPart.AttackPenalty) > atkDiff) {
                world.InsertMessage(String.Format("{0} {1} {2}'s {3}.... and misses.", attacker.Name, weapon.Action, defender.Name, bodyPart.Name));
                return;
            }

            // we hit
            int damage = DamageTableSwing(attacker.Characteristics.GetAttribute(Attribute.Strength)).Roll() + weapon.Damage;

            // todo get armor, etc

            switch (weapon.DamageType) {
                case DamageType.Cut:
                    damage = (int) Math.Ceiling(damage * 1.5);
                    break;
                case DamageType.Impale:
                    damage = (int) Math.Ceiling(damage * 2.0);
                    break;
                case DamageType.Pierce:
                case DamageType.Crush:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Logger.InfoFormat("{0} deals {1} {2} damage to {3}'s {4}", attacker.Name, damage, weapon.DamageType, defender.Name, bodyPart.Name);

            damage = Math.Min(damage, bodyPart.MaxHealth);

            Logger.InfoFormat("Damage reduced to {0} because of {1}'s max health", damage, bodyPart.Name);            
            defender.Hurt(bodyPart.Type, damage);

            world.InsertMessage(String.Format("{0} {1} {2}'s {3}.... and inflict {4} wounds.", attacker.Name, weapon.Action, defender.Name, bodyPart.Name, "todo-description"));

            // to do wounding
        }

        private static Dice DamageTableThrust(int strength) {
            return new Dice(1, 6, (int) Math.Floor((strength - 13)/2.0));
        }

        private static Dice DamageTableSwing(int strength) {
            return new Dice(1, 6, strength - 10);
        }
    }
}
