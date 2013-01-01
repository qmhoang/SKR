﻿using System;
using DEngine.Core;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;

namespace SkrGame.Gameplay.Combat {
    public static class PhysicalCombat {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      


        public static BodyPart GetRandomBodyPart(Actor target) {
            int roll = Rng.Roll(3, 6);            

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

        private static double ChanceOfSuccess(double difficulty) {
            return GaussianDistribution.CumulativeTo(difficulty + World.Mean, World.Mean, World.StandardDeviation);
        }
        public static void AttackRangeWithGun(Actor attacker, Actor defender, GunComponent weapon, BodyPart bodyPart, bool targettingPenalty = false) {
            double skillBonus = attacker.GetTalent(weapon.Skill).RealRank;            
            double difficulty = 0.0;

            if (targettingPenalty)
                difficulty += bodyPart.AttackPenalty;

            var pointsOnPath = Bresenham.GeneratePointsFromLine(attacker.Position, defender.Position);            

            foreach (var location in pointsOnPath) {
                if (!attacker.Level.IsWalkable(location))
                    break;

                if (!attacker.Level.DoesActorExistAtLocation(location))
                    continue;

                var targetAtLocation = attacker.Level.GetActorAtLocation(location);

                double rangePenalty = Math.Min(0, -2 * Math.Log(targetAtLocation.Position.DistanceTo(attacker.Position) * .4));

                rangePenalty -= location == defender.Position ? 0 : World.StandardDeviation;        // not being targetted gives a sigma (std dev) penalty
                difficulty += skillBonus + rangePenalty;

                var hit = Attack(attacker, targetAtLocation, weapon, 0, bodyPart, difficulty);
                if (hit)
                    break;

                // todo drop ammo casing
            }

        }

        public static bool Attack(Actor attacker, Actor defender, WeaponComponent weapon, int damageBonus, BodyPart bodyPart, double difficulty) {          
            double atkRoll = Rng.Double();
            double chanceToHit = ChanceOfSuccess(difficulty);

            Logger.InfoFormat("{0} attacks {1} (needs:{2:0.00}, rolled:{3:0.00}, difficulty: {4:0.0}", attacker.Name, defender.Name, chanceToHit, atkRoll, difficulty);

            if (atkRoll > chanceToHit) {
                attacker.World.InsertMessage(String.Format("{0} {1} {2}'s {3}.... and misses.", attacker.Name, weapon.ActionDescriptionPlural, defender.Name, bodyPart.Name));
                return false;                
            }

            double defRoll = Rng.Double();
            double chanceToDodge;
            Logger.InfoFormat("{0} attempts to dodge {1}'s attack (needs:{2:0.00}, rolled:{3:0.00}, difficulty: {4:0.0}", attacker.Name, defender.Name, chanceToHit, atkRoll, difficulty);
            if (defRoll < defender.Dodge) {
                attacker.World.InsertMessage(String.Format("{0} {1} {2}'s {3}.... and misses.", attacker.Name, weapon.ActionDescriptionPlural, defender.Name, bodyPart.Name));
                return false; 
            }

            int damage = damageBonus + weapon.Damage.Roll();

            switch (weapon.DamageType) {
                case DamageType.PierceSmall:
                    damage = (int)Math.Ceiling(damage * 0.5);
                    break;                    
                case DamageType.PierceLarge:
                case DamageType.Cut:
                    damage = (int)Math.Ceiling(damage * 1.5);
                    break;
                case DamageType.PierceHuge:
                case DamageType.Impale:
                    damage = (int)Math.Ceiling(damage * 2.0);
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

            attacker.World.InsertMessage(String.Format("{0} {1} {2}'s {3}.... and inflict {4} wounds.", attacker.Name, weapon.ActionDescriptionPlural, defender.Name, bodyPart.Name, "todo-description"));

            // todo get armor, defenses, parries, etc



            // todo wounding
            return true;
        }

        public static void AttackMeleeWithWeapon(Actor attacker, Actor defender, MeleeComponent weapon, BodyPart bodyPart, bool targettingPenalty = false) {            
            int hitBonus = attacker.GetTalent(weapon.Skill).RealRank;            
               
            var strength = attacker.GetTalent(Skill.Strength).RealRank;
            int damage = Rand.Dice(1, 6).Roll() + Rand.Constant(strength).Roll();

            Attack(attacker, defender, weapon, damage, bodyPart, (hitBonus + (targettingPenalty ? bodyPart.AttackPenalty : 0)));
        }

//        private static Rand DamageTableThrust(int strength) {
//            return Rand.Dice(1, 6) + Rand.Constant((int)Math.Floor((strength - 3) / 2.0));
////            Rand.Dice(1, 6) + Rand.Fixed((int) Math.Floor((strength - 3) / 2.0));
////            return new Dice(1, 6, (int) Math.Floor((strength - 3)/2.0));
//        }
//
//        private static Rand DamageTableSwing(int strength) {
//            return Rand.Dice(1, 6) + Rand.Constant(strength);
//        }
    }
}