using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Utility;
using SKR.Gameplay.Combat;
using SKR.Gameplay.Talent;
using SKR.Universe.Entities.Actors;
using SKR.Universe.Entities.Items;
using Attribute = SKR.Universe.Entities.Actors.Attribute;

namespace SKR.Universe.Factories {
    public class SourceTalentFactory : Factory<Skill, Talent> {
        public override Talent Construct(Skill identifier) {
            switch (identifier) {
                case Skill.Attack:
                    return new Talent(new TalentTemplate()
                                          {
                                                  Skill = identifier,
                                                  Name = "Attack",
                                                  InitialRank = 1,
                                                  MaxRank = 1,
                                                  Radius = 0,
                                                  Range = 0,
                                                  Action = delegate(Talent t, Person self, Person target, dynamic o1, dynamic o2, dynamic o3, dynamic o4)
                                                    {
                                                        MeleeComponent melee;

                                                        // if we have something in our right hand, probably want to use that
                                                        if (self.IsItemEquipped(BodyPartType.RightHand)) {
                                                            var item = self.GetItemAtBodyPart(BodyPartType.RightHand);
                                                            melee = (MeleeComponent) (item.ContainsComponent(ItemAction.MeleeAttackSwing)
                                                                                              ? item.GetComponent(ItemAction.MeleeAttackSwing)
                                                                                              : item.GetComponent(ItemAction.MeleeAttackThrust));
                                                        } else if (self.IsItemEquipped(BodyPartType.LeftHand)) {
                                                            var item = self.GetItemAtBodyPart(BodyPartType.LeftHand);
                                                            melee = (MeleeComponent) (item.ContainsComponent(ItemAction.MeleeAttackSwing)
                                                                                              ? item.GetComponent(ItemAction.MeleeAttackSwing)
                                                                                              : item.GetComponent(ItemAction.MeleeAttackThrust));
                                                        } else
                                                            melee = self.Characteristics.Punch;

                                                        if (self.Position.DistanceTo(target.Position) > (t.Range + melee.Reach + 1)) {
                                                            self.World.InsertMessage("Too far to attack.");
                                                            return ActionResult.Aborted;
                                                        }

                                                        MeleeCombat.AttackMeleeWithWeapon(self, target, melee, MeleeCombat.GetRandomBodyPart(target));
                                                        return ActionResult.Success;
                                                    }
                                          });
                case Skill.TargetAttack:
                    return new Talent(new TalentTemplate()
                                          {
                                                  Skill = identifier,
                                                  Name = "Attack target specific body part.",
                                                  InitialRank = 1,
                                                  MaxRank = 1,
                                                  Radius = 0,
                                                  Range = 0,
                                                  Args0 = delegate(Talent t, Person self, Person target)
                                                                 {
                                                                     List<MeleeComponent> attacks = new List<MeleeComponent>
                                                                                                        {
                                                                                                                self.Characteristics.Kick,
                                                                                                                self.Characteristics.Punch,
                                                                                                        };

                                                                     foreach (var item in from bp in self.BodyParts
                                                                                          where self.IsItemEquipped(bp.Type)
                                                                                          select self.GetItemAtBodyPart(bp.Type)) {
                                                                         if (item.ContainsComponent(ItemAction.MeleeAttackSwing))
                                                                             attacks.Add(item.GetComponent(ItemAction.MeleeAttackSwing) as MeleeComponent);
                                                                         if (item.ContainsComponent(ItemAction.MeleeAttackThrust))
                                                                             attacks.Add(item.GetComponent(ItemAction.MeleeAttackThrust) as MeleeComponent);
                                                                     }

                                                                     return attacks;
                                                                 },

                                                  Arg0Desciptor = delegate (Talent t, Person self, Person target, dynamic arg)
                                                                      {
                                                                          var weapon = (MeleeComponent) arg;
                                                                          return weapon.Item == null
                                                                                         ? weapon.ActionDescription
                                                                                         : String.Format("{0} with {1}", weapon.ActionDescription, weapon.Item.Name);
                                                                      },
                                                  Args1 = (t, self, target) => target.Characteristics.BodyPartsList,

                                                  Action = delegate(Talent t, Person self, Person target, dynamic weapon, dynamic targetBodyPart, dynamic notused1, dynamic notused2)
                                                               {
                                                                   if (!(weapon is MeleeComponent)) {
                                                                       self.World.InsertMessage("Not a melee weapon.");
                                                                       return ActionResult.Aborted;
                                                                   }

                                                                   var melee = weapon as MeleeComponent;

                                                                   if (!(targetBodyPart is BodyPart)) {
                                                                       self.World.InsertMessage("Not a body part.");
                                                                       return ActionResult.Aborted;
                                                                   }

                                                                   if (self.Position.DistanceTo(target.Position) > (t.Range + melee.Reach + 1)) {
                                                                       self.World.InsertMessage("Too far to attack.");
                                                                       return ActionResult.Aborted;
                                                                   }

                                                                   MeleeCombat.AttackMeleeWithWeapon(self, target, melee, targetBodyPart, true);
                                                                   return ActionResult.Success;
                                                               }
                                          });
                           

                case Skill.Brawling:
                    return new Talent(new TalentTemplate()
                                          {
                                                  Skill = identifier, 
                                                  Name = "Brawling", 
                                                  InitialRank = 0, 
                                                  MaxRank = 10, 
                                                  Range = 0, 
                                                  Radius = 0,
                                                  CalculateRealRank = (t, self) => t.RawRank + self.GetTalent(Skill.Agility).RealRank
                                          });
                case Skill.Sword:
                    return new Talent(new TalentTemplate()
                                          {
                                                  Skill = identifier,
                                                  Name = "Sword",
                                                  InitialRank = 0,
                                                  MaxRank = 10,
                                                  Range = 0,
                                                  Radius = 0,
                                                  CalculateRealRank = (t, self) => t.RawRank + self.GetTalent(Skill.Agility).RealRank
                                          });
                case Skill.Knife:
                    return new Talent(new TalentTemplate()
                                          {
                                                  Skill = identifier,
                                                  Name = "Knife",
                                                  InitialRank = 0,
                                                  MaxRank = 10,
                                                  Range = 0,
                                                  Radius = 0,                                                  
                                                  CalculateRealRank = (t, self) => t.RawRank + self.GetTalent(Skill.Agility).RealRank
                                          });


                case Skill.Strength:
                    return new Talent(TalentTemplate.CreateAttribute(identifier));
                case Skill.Agility:
                    return new Talent(TalentTemplate.CreateAttribute(identifier));
                case Skill.Constitution:
                    return new Talent(TalentTemplate.CreateAttribute(identifier));
                case Skill.Intellect:
                    return new Talent(TalentTemplate.CreateAttribute(identifier));
                case Skill.Cunning:
                    return new Talent(TalentTemplate.CreateAttribute(identifier));
                case Skill.Resolve:
                    return new Talent(TalentTemplate.CreateAttribute(identifier));
                case Skill.Presence:
                    return new Talent(TalentTemplate.CreateAttribute(identifier));
                case Skill.Grace:
                    return new Talent(TalentTemplate.CreateAttribute(identifier));
                case Skill.Composure:
                    return new Talent(TalentTemplate.CreateAttribute(identifier));
                default:
                    throw new ArgumentOutOfRangeException("identifier");
            }
        }

    }
}
