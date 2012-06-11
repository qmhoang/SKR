using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Core;
using DEngine.Utility;
using SKR.Gameplay.Combat;
using SKR.Gameplay.Talent;
using SKR.Universe.Entities.Actors;
using SKR.Universe.Entities.Items;
using SKR.Universe.Location;

namespace SKR.Universe.Factories {
    public abstract class TalentFactory : Factory<Skill, Talent> {
        
    }
    public class SourceTalentFactory : TalentFactory {
        public override Talent Construct(Skill identifier) {
            switch (identifier) {
                case Skill.Attack:
                    return new Talent(new TalentTemplate()
                                          {
                                              Skill = identifier,
                                              Name = "Attack",
                                              InitialRank = 1,
                                              MaxRank = 1,
                                              RequiresTarget = true,
                                              ActionOnTargetFunction =
                                                      delegate(Talent t, Actor self, Point point, dynamic[] args)
                                                          {
                                                              Actor target = self.Level.GetActorAtLocation(point);
                                                              MeleeComponent melee;
                                                              // if we have something in our right hand, probably want to use that
                                                              if (self.IsItemEquipped(BodyPartType.RightHand)) {
                                                                  var item = self.GetItemAtBodyPart(BodyPartType.RightHand);
                                                                  melee = (item.ContainsComponent(ItemAction.MeleeAttackSwing)
                                                                                   ? item.GetComponent<MeleeComponent>(ItemAction.MeleeAttackSwing)
                                                                                   : item.GetComponent<MeleeComponent>(ItemAction.MeleeAttackThrust));
                                                              } else if (self.IsItemEquipped(BodyPartType.LeftHand)) {
                                                                  var item = self.GetItemAtBodyPart(BodyPartType.LeftHand);
                                                                  melee = (item.ContainsComponent(ItemAction.MeleeAttackSwing)
                                                                                   ? item.GetComponent<MeleeComponent>(ItemAction.MeleeAttackSwing)
                                                                                   : item.GetComponent<MeleeComponent>(ItemAction.MeleeAttackThrust));
                                                              } else
                                                                  melee = self.Characteristics.Punch;

                                                              if (self.Position.DistanceTo(target.Position) > (melee.Reach + t.Range + 1)) {
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
                                                  RequiresTarget = true,
                                                  Args = new List<TalentTemplate.GenerateArgsListFunction>()
                                                             {
                                                                     delegate(Talent t, Actor self, Point p)
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
                                                                                     attacks.Add(item.GetComponent<MeleeComponent>(ItemAction.MeleeAttackSwing));
                                                                                 if (item.ContainsComponent(ItemAction.MeleeAttackThrust))
                                                                                     attacks.Add(item.GetComponent<MeleeComponent>(ItemAction.MeleeAttackThrust));
                                                                             }

                                                                             return attacks;
                                                                         },
                                                                     delegate(Talent t, Actor self, Point p)
                                                                         {
                                                                             if (!self.Level.DoesActorExistAtLocation(p))
                                                                                 return null;

                                                                             return self.Level.GetActorAtLocation(p).Characteristics.BodyPartsList;
                                                                         },
                                                             },

                                                  ArgsDesciptor = new List<TalentTemplate.ArgDesciptorFunction>()
                                                                      {
                                                                              delegate(Talent t, Actor self, Point target, dynamic arg)
                                                                                  {
                                                                                      var weapon = (MeleeComponent) arg;
                                                                                      return weapon.Item == null
                                                                                                     ? weapon.ActionDescription
                                                                                                     : String.Format("{0} with {1}", weapon.ActionDescription, weapon.Item.Name);
                                                                                  },
                                                                              (t, self, target, arg) => arg.ToString(),
                                                                      },

                                                  ActionOnTargetFunction =
                                                          delegate(Talent t, Actor self, Point p, dynamic[] args)
                                                              {
                                                                  Actor target = self.Level.GetActorAtLocation(p);
                                                                  if (!(args[0] is MeleeComponent)) {
                                                                      self.World.InsertMessage("Not a melee weapon.");
                                                                      return ActionResult.Aborted;
                                                                  }

                                                                  var melee = args[0] as MeleeComponent;

                                                                  if (!(args[1] is BodyPart)) {
                                                                      self.World.InsertMessage("Not a body part.");
                                                                      return ActionResult.Aborted;
                                                                  }

                                                                  if (self.Position.DistanceTo(target.Position) > (melee.Reach + t.Range + 1)) {
                                                                      // diagonal attacks are >1, so if weapons have range of 0, we can't use them
                                                                      self.World.InsertMessage("Too far to attack.");
                                                                      return ActionResult.Aborted;
                                                                  }

                                                                  MeleeCombat.AttackMeleeWithWeapon(self, target, melee, args[1], true);
                                                                  return ActionResult.Success;
                                                              }
                                          });
                case Skill.RangeTargetAttack:
                    return new Talent(new TalentTemplate()
                                          {
                                                  Skill = identifier,
                                                  Name = "Shoot target specific body part.",
                                                  InitialRank = 1,
                                                  MaxRank = 1,
                                                  RequiresTarget = true,

                                                  Args = new List<TalentTemplate.GenerateArgsListFunction>()
                                                             {
                                                                     delegate(Talent t, Actor self, Point p)
                                                                         {
                                                                             return (from bp in self.BodyParts
                                                                                     where self.IsItemEquipped(bp.Type)
                                                                                     select self.GetItemAtBodyPart(bp.Type)
                                                                                     into item
                                                                                     where item.ContainsComponent(ItemAction.Shoot)
                                                                                     select item.GetComponent<FirearmComponent>(ItemAction.Shoot)).ToList();
                                                                         },
                                                                     delegate(Talent t, Actor self, Point p)
                                                                         {
                                                                             if (!self.Level.DoesActorExistAtLocation(p))
                                                                                 return null;

                                                                             return self.Level.GetActorAtLocation(p).Characteristics.BodyPartsList;
                                                                         },
                                                             },

                                                  ArgsDesciptor = new List<TalentTemplate.ArgDesciptorFunction>()
                                                                      {
                                                                              delegate(Talent t, Actor self, Point p, dynamic arg)
                                                                                  {
                                                                                      var weapon = (FirearmComponent) arg;
                                                                                      return weapon.Item == null
                                                                                                     ? weapon.ActionDescription
                                                                                                     : String.Format("{0} with {1}", weapon.ActionDescription, weapon.Item.Name);
                                                                                  },
                                                                              (t, self, target, arg) => arg.ToString()

                                                                      },

                                                  ActionOnTargetFunction =
                                                          delegate(Talent t, Actor self, Point p, dynamic[] args)
                                                              {
                                                                  Actor target = self.Level.GetActorAtLocation(p);

                                                                  if (!(args[0] is FirearmComponent)) {
                                                                      self.World.InsertMessage("Not a firearm.");
                                                                      return ActionResult.Aborted;
                                                                  }

                                                                  var gun = args[0] as FirearmComponent;

                                                                  if (!(args[1] is BodyPart)) {
                                                                      self.World.InsertMessage("Not a body part.");
                                                                      return ActionResult.Aborted;
                                                                  }

                                                                  if (self.Position.DistanceTo(target.Position) > (gun.Range + t.Range + 1)) {
                                                                      self.World.InsertMessage("Too far to attack.");
                                                                      return ActionResult.Aborted;
                                                                  }

                                                                  if (gun.Magazine == null || gun.Magazine.GetComponent<MagazineComponent>(ItemAction.ReloadFirearm).Shots <= 0) {
                                                                      self.World.InsertMessage("You squeeze the trigger only the hear the sound of nothing happening...");
                                                                      self.ActionPoints -= gun.WeaponSpeed;
                                                                      return ActionResult.Failed;
                                                                  }

                                                                  MeleeCombat.AttackRangeWithGun(self, target, gun, args[1], true);
                                                                  return ActionResult.Success;
                                                              }
                                          });
                case Skill.Reload:
                    return new Talent(new TalentTemplate
                                          {
                                                  Skill = identifier,
                                                  Name = "Reload firearm",
                                                  InitialRank = 1,
                                                  MaxRank = 1,
                                                  RequiresTarget = false,
                                                  Args = new List<TalentTemplate.GenerateArgsListFunction>()
                                                             {
                                                                     delegate(Talent t, Actor self, Point p) // what gun are we reloading, return an empty list should cause optionsprompt to quit
                                                                         {
                                                                             var weapons = new List<FirearmComponent>();

                                                                             if (self.IsItemEquipped(BodyPartType.LeftHand) &&
                                                                                 self.GetItemAtBodyPart(BodyPartType.LeftHand).ContainsComponent(ItemAction.Shoot))
                                                                                 weapons.Add(self.GetItemAtBodyPart(BodyPartType.LeftHand).GetComponent<FirearmComponent>(ItemAction.Shoot));

                                                                             if (self.IsItemEquipped(BodyPartType.RightHand) &&
                                                                                 self.GetItemAtBodyPart(BodyPartType.RightHand).ContainsComponent(ItemAction.Shoot))
                                                                                 weapons.Add(self.GetItemAtBodyPart(BodyPartType.RightHand).GetComponent<FirearmComponent>(ItemAction.Shoot));

                                                                             return weapons;
                                                                         },
                                                                     delegate(Talent t, Actor self, Point p)
                                                                         {
                                                                             List<MagazineComponent> mags = new List<MagazineComponent>();
                                                                             foreach (Item i in self.Items)
                                                                                 if (i.ContainsComponent(ItemAction.ReloadFirearm))
                                                                                     mags.Add(i.GetComponent<MagazineComponent>(ItemAction.ReloadFirearm));
                                                                             return mags;
                                                                         },
                                                             },

                                                  ArgsDesciptor = new List<TalentTemplate.ArgDesciptorFunction>()
                                                                      {
                                                                              (t, self, target, arg) => ((FirearmComponent) arg).Item.Name,
                                                                              (t, self, target, arg) => ((MagazineComponent) arg).Item.Name,

                                                                      },
                                                  ActionOnTargetFunction = delegate(Talent t, Actor self, Point p, dynamic[] args)
                                                                               {
                                                                                   if (!(args[0] is FirearmComponent)) {
                                                                                       self.World.InsertMessage("Not a firearm.");
                                                                                       return ActionResult.Aborted;
                                                                                   }

                                                                                   var gun = args[0] as FirearmComponent;

                                                                                   if (!(args[1] is MagazineComponent)) {
                                                                                       self.World.InsertMessage("Not a magazine.");
                                                                                       return ActionResult.Aborted;
                                                                                   }

                                                                                   var mag = args[1] as MagazineComponent;

                                                                                   if (!mag.FirearmId.Equals(gun.Item.RefId)) {
                                                                                       self.World.InsertMessage("Magazine doesn't work with this gun.");
                                                                                       return ActionResult.Aborted;
                                                                                   }

                                                                                   if (gun.Magazine != null) {
                                                                                       //guns contains a magazine already eject it                                                                    
                                                                                       self.AddItem(gun.Magazine);
                                                                                   }
                                                                                   self.RemoveItem(mag.Item);
                                                                                   gun.Magazine = mag.Item;
                                                                                   self.ActionPoints -= gun.ReloadSpeed;

                                                                                   return ActionResult.Success;
                                                                               }
                                          });
                case Skill.UseFeature:
                    return new Talent(new TalentTemplate()
                                          {
                                                  Skill = identifier,
                                                  Name = "Use feature",
                                                  InitialRank = 1,
                                                  MaxRank = 1,
                                                  RequiresTarget = true,
                                                  Args = new List<TalentTemplate.GenerateArgsListFunction>()
                                                             {
                                                                     delegate(Talent t, Actor self, Point p) // what gun are we reloading, return an empty list should cause optionsprompt to quit
                                                                         {
                                                                             return self.Level.Features.Where(feature => feature.Position == p).ToList();
                                                                         },
                                                             },

                                                  ArgsDesciptor = new List<TalentTemplate.ArgDesciptorFunction>()
                                                                      {
                                                                              (t, self, target, arg) => ((Feature) arg).RefId,                                                                              

                                                                      },
                                                  ActionOnTargetFunction = delegate(Talent t, Actor self, Point p, dynamic[] args)
                                                                               {
//                                                                                   ((Feature) args[0]);
                                                                                   return ActionResult.Success;
                                                                               }
                                          });
                    ;
                case Skill.Brawling:
                    return new Talent(new TalentTemplate()
                                          {
                                                  Skill = identifier, 
                                                  Name = "Brawling", 
                                                  InitialRank = 0, 
                                                  MaxRank = 10, 
                                                  CalculateRealRank = (t, self) => t.RawRank + self.GetTalent(Skill.Agility).RealRank
                                          });
                case Skill.Sword:
                    return new Talent(new TalentTemplate()
                                          {
                                                  Skill = identifier,
                                                  Name = "Sword",
                                                  InitialRank = 0,
                                                  MaxRank = 10,
                                                  CalculateRealRank = (t, self) => t.RawRank + self.GetTalent(Skill.Agility).RealRank
                                          });
                case Skill.Knife:
                    return new Talent(new TalentTemplate()
                                          {
                                                  Skill = identifier,
                                                  Name = "Knife",
                                                  InitialRank = 0,
                                                  MaxRank = 10,                                                
                                                  CalculateRealRank = (t, self) => t.RawRank + self.GetTalent(Skill.Agility).RealRank
                                          });
                case Skill.Pistol:
                    return new Talent(new TalentTemplate()
                                          {
                                                  Skill = identifier,
                                                  Name = "Pistol",
                                                  InitialRank = 0,
                                                  MaxRank = 10,
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
