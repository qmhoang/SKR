using System.Collections.Generic;
using DEngine.Core;
using SKR.Universe.Entities.Items;

namespace SKR.Universe.Entities.Actors {
    public class ActorCharacteristics {
        private Person Actor;
        private Dictionary<Attribute, ActorAttribute> Attributes;
        private Dictionary<BodyPartType, BodyPart> BodyParts;
        private Dictionary<Skill, SkillAttribute> Skills;

        public int Health { get; set; }
        public int MaxHealth { get; protected set; }

        public MeleeComponent Punch;
        public MeleeComponent Kick;

        public int BasicDodge {
            get { return Attributes[Attribute.Agility].Current + Attributes[Attribute.Cunning].Current; }
        }

        public int Lift {
            get { return Attributes[Attribute.Strength].Current * Attributes[Attribute.Strength].Current * 2; }
        }

        public ActorCharacteristics(Person actor) {
            Actor = actor;

            Attributes = new Dictionary<Attribute, ActorAttribute>
                             {
                                     {Attribute.Strength, new ActorAttribute(10)},
                                     {Attribute.Agility, new ActorAttribute(10)},
                                     {Attribute.Constitution, new ActorAttribute(10)},
                                     {Attribute.Intellect, new ActorAttribute(10)},
                                     {Attribute.Cunning, new ActorAttribute(10)},
                                     {Attribute.Resolve, new ActorAttribute(10)},
                                     {Attribute.Presence, new ActorAttribute(10)},
                                     {Attribute.Grace, new ActorAttribute(10)},
                                     {Attribute.Composure, new ActorAttribute(10)},
                             };

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

            Skills = new Dictionary<Skill, SkillAttribute>
                         {
                                 {
                                         Skill.Brawling,
                                         new SkillAttribute(0, Attributes[Attribute.Agility], "Everyone was kung-fu fighting",
                                                            new List<Pair<ActorAttribute, int>>
                                                                {
                                                                        new Pair<ActorAttribute, int>(Attributes[Attribute.Agility], 2),                                                                        
                                                                })
                                         },
                                 {
                                         Skill.Knife, 
                                         new SkillAttribute(0, Attributes[Attribute.Agility], "Is that a knife in your pocket or are you happy to see me?",
                                                            new List<Pair<ActorAttribute, int>>
                                                                {
                                                                        new Pair<ActorAttribute, int>(Attributes[Attribute.Agility], 4),                                                                        
                                                                })
                                         }
                         };

            
            Punch = new MeleeComponent(Skill.Brawling, 0, -1, DamageType.Crush, 1, 100, 1, 0, 0, ItemAction.MeleeAttackThrust, "punch", "punches");
            Kick = new MeleeComponent(Skill.Brawling, -2, 1, DamageType.Crush, 1, 100, 1, 0, -100, ItemAction.MeleeAttackThrust, "kick", "kicks");
        }


        public int GetAttribute(Attribute attrb) {
            return Attributes[attrb].Current;
        }

        public int GetSkill(Skill skill) {
            return Skills[skill].Current;
        }

        public BodyPart GetBodyPart(BodyPartType bp) {
            return BodyParts[bp];
        }

        public IEnumerable<BodyPart> BodyPartsList {
            get { return BodyParts.Values; }
        }
    }
}