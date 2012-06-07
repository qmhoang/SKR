using System;
using System.Collections.Generic;
using DEngine.Core;
using SKR.Universe.Entities.Items;

namespace SKR.Universe.Entities.Actors {
    public class ActorCharacteristics {
        private Actor Actor;        
        private Dictionary<BodyPartType, BodyPart> BodyParts;        

        public int Health { get; set; }
        public int MaxHealth { get; protected set; }

        public MeleeComponent Punch;
        public MeleeComponent Kick;

        public int BasicDodge {
            get { return Actor.GetTalent(Skill.Agility).RealRank + Actor.GetTalent(Skill.Cunning).RealRank; }
        }

        public int Lift {
            get { return (Actor.GetTalent(Skill.Strength).RealRank + 10) * (Actor.GetTalent(Skill.Strength).RealRank + 10) * 2; }
        }

        public ActorCharacteristics(Actor actor) {
            Actor = actor;

            BodyParts = new Dictionary<BodyPartType, BodyPart>
                            {
                                    {BodyPartType.Body, new BodyPart("Torso", BodyPartType.Body, Actor, Health, 0)},
                                    {BodyPartType.Head, new BodyPart("Head", BodyPartType.Head, Actor, Health, -5)},
                                    {BodyPartType.RightArm, new BodyPart("Right Arm", BodyPartType.RightArm, Actor, Health / 2, -2)},
                                    {BodyPartType.LeftArm, new BodyPart("Left Arm", BodyPartType.LeftArm, Actor, Health / 2, -2)},
                                    {
                                            BodyPartType.RightHand,
                                            new BodyPart("Right Hand", BodyPartType.RightHand, Actor, Health / 3, -4, ItemType.OneHandedWeapon, ItemType.TwoHandedWeapon, ItemType.Shield)
                                            },
                                    {
                                            BodyPartType.LeftHand, 
                                            new BodyPart("Left Hand", BodyPartType.LeftHand, Actor, Health / 3, -4, ItemType.OneHandedWeapon, ItemType.TwoHandedWeapon, ItemType.Shield)
                                            },
                                    {BodyPartType.Leg, new BodyPart("Leg", BodyPartType.Leg, Actor, Health / 2, -2)},
                                    {BodyPartType.Feet, new BodyPart("Feet", BodyPartType.Feet, Actor, Health / 3, -4)}
                            };


            
            Punch = new MeleeComponent("punch", ItemAction.MeleeAttackThrust, "punch", "punches", Skill.Brawling, 0, -1, DamageType.Crush, 1, 100, 1, 0, 0);
            Kick = new MeleeComponent("kick", ItemAction.MeleeAttackThrust, "kick", "kicks", Skill.Brawling, -2, 1, DamageType.Crush, 1, 100, 1, 0, -100);
        }


        public BodyPart GetBodyPart(BodyPartType bp) {
            return BodyParts[bp];
        }

        public IEnumerable<BodyPart> BodyPartsList {
            get { return BodyParts.Values; }
        }
    }
}