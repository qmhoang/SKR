using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Core;
using SKR.Gameplay.Talent;
using SKR.Universe;
using SKR.Universe.Entities.Actors;
using SKR.Universe.Entities.Items;
using Attribute = SKR.Universe.Entities.Actors.Attribute;

namespace SKR.Gameplay.Combat {
    public static class MeleeCombat {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static Dice dice = new Dice(3, 6);
        private static World world = World.Instance;

        public static BodyPart GetRandomBodyPart(Person target) {
            int roll = dice.Roll();

            //todo unfinished
            if (roll <= 4)
                return target.GetBodyPart(BodyPartType.Head);
            if (roll <= 5)
                return target.GetBodyPart(BodyPartType.LeftHand);
            if (roll <= 7)
                return target.GetBodyPart(BodyPartType.Leg);            // left leg
            if (roll <= 8)
                return target.GetBodyPart(BodyPartType.LeftArm);
            if (roll <= 11)
                return target.GetBodyPart(BodyPartType.Body);
            if (roll <= 12)
                return target.GetBodyPart(BodyPartType.RightArm);
            if (roll <= 14)
                return target.GetBodyPart(BodyPartType.Leg);            // right leg
            if (roll <= 15)
                return target.GetBodyPart(BodyPartType.RightHand);
            if (roll <= 16)
                return target.GetBodyPart(BodyPartType.Feet);
            else 
                return target.GetBodyPart(BodyPartType.Head);
        }


        public static void AttackMeleeWithWeapon(Person attacker, Person defender, MeleeComponent weapon, BodyPart bodyPart, bool targettingPenalty = false) {            
            int atkDiff = attacker.GetTalent(weapon.Skill).RealRank;
            int atkRoll = dice.Roll();

            if (targettingPenalty)
                Logger.InfoFormat("{0} (skill: {1} @ {2}) attacks {3} rolling {4} ({5} penalty for targetting {6})", attacker.Name, weapon.Skill, atkDiff, defender.Name, atkRoll, bodyPart.AttackPenalty, bodyPart.Name);
            else
                Logger.InfoFormat("{0} (skill: {1} @ {2}) attacks {3} rolling {4}", attacker.Name, weapon.Skill, atkDiff, defender.Name, atkRoll);

            if ((atkRoll - (targettingPenalty ? bodyPart.AttackPenalty : 0)) > atkDiff) {
                world.InsertMessage(String.Format("{0} {1} {2}'s {3}.... and misses.", attacker.Name, weapon.ActionDescriptionPlural, defender.Name, bodyPart.Name));
                return;
            }

            // we hit           
//            var strength = attacker.Characteristics.GetAttribute(Attribute.Strength);
            var strength = attacker.GetTalent(Skill.Strength).RealRank;
            int damage = (weapon.Action == ItemAction.MeleeAttackSwing ? DamageTableSwing(strength).Roll() : DamageTableThrust(strength).Roll()) + weapon.Damage;

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

            world.InsertMessage(String.Format("{0} {1} {2}'s {3}.... and inflict {4} wounds.", attacker.Name, weapon.ActionDescriptionPlural, defender.Name, bodyPart.Name, "todo-description"));

            // todo wounding
        }

        private static Dice DamageTableThrust(int strength) {
            return new Dice(1, 6, (int) Math.Floor((strength - 13)/2.0));
        }

        private static Dice DamageTableSwing(int strength) {
            return new Dice(1, 6, strength - 10);
        }
    }
}
