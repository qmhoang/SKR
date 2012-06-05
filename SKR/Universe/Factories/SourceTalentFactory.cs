using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DEngine.Utility;
using SKR.Gameplay.Combat;
using SKR.Gameplay.Talent;
using SKR.Universe.Entities.Actors;
using SKR.Universe.Entities.Items;
using Attribute = SKR.Universe.Entities.Actors.Attribute;

namespace SKR.Universe.Factories {
    public class SourceTalentFactory : Factory<Skill, Person, Talent> {
        public override Talent Construct(Skill identifier, Person actor) {
            switch (identifier) {
                case Skill.Attack:
                    return new ActiveTalent(actor, identifier, "Attack body part", 1,
                                              delegate(Person self, Person target)
                                                  {
                                                      Talent t = self.GetTalent<ActiveTalent>(identifier);

//                                                      List<MeleeComponent> attacks = new List<MeleeComponent>
//                                                                                         {
//                                                                                                 self.Characteristics.Kick,
//                                                                                                 self.Characteristics.Punch,
//                                                                                         };
//
//                                                      foreach (var bodyPart in self.BodyParts.Where(bodyPart => self.IsItemEquipped(bodyPart.Type))) {
//                                                          var item = self.GetItemAtBodyPart(bodyPart.Type);
//                                                          if (item.ContainsAction(ItemAction.MeleeAttackSwing))
//                                                              attacks.Add(item.GetComponent(ItemAction.MeleeAttackSwing) as MeleeComponent);
//                                                          if (item.ContainsAction(ItemAction.MeleeAttackThrust))
//                                                              attacks.Add(item.GetComponent(ItemAction.MeleeAttackThrust) as MeleeComponent);
//                                                      }
//
//                                                      int max = 0;
//                                                      List<MeleeComponent> best = new List<MeleeComponent>();
//
//                                                      // get weapon with the best chance to hit
//                                                      foreach (var attack in attacks) {
//                                                          int i = GetRealRank(self, attack.Skill) + attack.HitBonus;
//                                                          if (i > max) {
//                                                              max = i;
//                                                              best.Add(attack);
//                                                          }
//                                                      } 

                                                      MeleeComponent melee;
 
                                                      // if we have something in our right hand, probably want to use that
                                                      if (self.IsItemEquipped(BodyPartType.RightHand)) {
                                                          var item = self.GetItemAtBodyPart(BodyPartType.RightHand);
                                                          melee = (MeleeComponent)(item.ContainsComponent(ItemAction.MeleeAttackSwing)
                                                                                            ? item.GetComponent(ItemAction.MeleeAttackSwing)
                                                                                            : item.GetComponent(ItemAction.MeleeAttackThrust));
                                                      } else if (self.IsItemEquipped(BodyPartType.LeftHand)) {
                                                          var item = self.GetItemAtBodyPart(BodyPartType.LeftHand);
                                                          melee = (MeleeComponent)(item.ContainsComponent(ItemAction.MeleeAttackSwing)
                                                                                            ? item.GetComponent(ItemAction.MeleeAttackSwing)
                                                                                            : item.GetComponent(ItemAction.MeleeAttackThrust));
                                                      } else
                                                          melee = self.Characteristics.Punch;

                                                      if (self.Position.DistanceTo(target.Position) > melee.Reach) {
                                                          self.World.InsertMessage("Too far to attack.");
                                                          return false;
                                                      }

                                                      MeleeCombat.AttackMeleeWithWeapon(self, target, melee, MeleeCombat.GetRandomBodyPart(target));
                                                      return true;
                                                  });
                case Skill.TargetAttack:
                    return new ActiveTalent<BodyPart, ItemComponent>(actor, identifier, "Attack body part", 1,
                                              delegate(Person self, Person target, BodyPart targetBodyPart, ItemComponent weapon)
                                                  {
                                                      Talent t = self.GetTalent<ActiveTalent<BodyPart, ItemComponent>>(identifier);
                                                      if (!(weapon is MeleeComponent)) {
                                                          self.World.InsertMessage("Not a melee weapon.");
                                                          return false;
                                                      }

                                                      var melee = weapon as MeleeComponent;

                                                      if (self.Position.DistanceTo(target.Position) > melee.Reach) {
                                                          self.World.InsertMessage("Too far to attack.");
                                                          return false;
                                                      }

                                                      MeleeCombat.AttackMeleeWithWeapon(self, target, melee, targetBodyPart, true);
                                                      return true;
                                                  }, delegate (Person self)
                                                         {
                                                             
                                                         });

                case Skill.Brawling:
                    return new DerivedTalent(actor, identifier, "Brawling", 0, self => self.GetTalent<AttributeTalent>(Skill.Agility).Rank);
                case Skill.Sword:
                    return new DerivedTalent(actor, identifier, "Sword", 10, self => self.GetTalent<AttributeTalent>(Skill.Agility).Rank);
                case Skill.Knife:
                    return new DerivedTalent(actor, identifier, "Knife", 10, self => self.GetTalent<AttributeTalent>(Skill.Sword).Rank);


                case Skill.Strength:
                    return new AttributeTalent(actor, identifier, identifier.ToString());
                case Skill.Agility:
                    return new AttributeTalent(actor, identifier, identifier.ToString(), 20);
                case Skill.Constitution:
                    return new AttributeTalent(actor, identifier, identifier.ToString(), 20);
                case Skill.Intellect:
                    return new AttributeTalent(actor, identifier, identifier.ToString(), 20);
                case Skill.Cunning:
                    return new AttributeTalent(actor, identifier, identifier.ToString(), 20);
                case Skill.Resolve:
                    return new AttributeTalent(actor, identifier, identifier.ToString(), 20);
                case Skill.Presence:
                    return new AttributeTalent(actor, identifier, identifier.ToString(), 20);
                case Skill.Grace:
                    return new AttributeTalent(actor, identifier, identifier.ToString(), 20);
                case Skill.Composure:
                    return new AttributeTalent(actor, identifier, identifier.ToString(), 20);
                default:
                    throw new ArgumentOutOfRangeException("identifier");
            }
        }

//        /// <summary>
//        /// Creates a passive skill that adds another skill's rank to its calculated rank.
//        /// </summary>
//        /// <param name="actor"></param>
//        /// <param name="skill"></param>
//        /// <param name="base"></param>        
//        /// <returns></returns>
//        private int CreateSkillWithBase(Person actor, Skill skill, Skill @base) {
//            Talent t = actor.GetTalent(skill);
//            Talent governing = actor.GetTalent(@base);
//            if (!(governing.Action is PassiveSkillAction))
//                throw new InvalidEnumArgumentException("attrb is not a passive skill");
//
//            int governingRank = (governing.Action as PassiveSkillAction).RealRank(actor);            
//            return t.Rank + governingRank;            
//        }

    }
}
