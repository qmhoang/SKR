using System;
using System.Collections.Generic;
using System.Linq;
using DEngine.Actor;
using DEngine.Core;
using SkrGame.Gameplay.Combat;
using SkrGame.Gameplay.Talent;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;

namespace SkrGame.Universe.Factories {
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
                                              RequiresTarget = TargetType.Directional,
                                              ActionOnTargetFunction =
                                                      delegate(Talent t, Actor self, Point point, dynamic[] args)
                                                          {
                                                              Actor target = self.Level.GetActorAtLocation(point);
                                                              MeleeComponent melee = null;
                                                              // if we have something in our right hand, probably want to use that
                                                              if (self.IsItemEquipped(BodyPartType.RightHand)) {
                                                                  var item = self.GetItemAtBodyPart(BodyPartType.RightHand);
                                                                  melee = (item.Is(typeof(MeleeComponent))
                                                                                   ? item.As<MeleeComponent>()
                                                                                   : null);
                                                              } else if (self.IsItemEquipped(BodyPartType.LeftHand)) {
                                                                  var item = self.GetItemAtBodyPart(BodyPartType.LeftHand);
                                                                  melee = (item.Is(typeof(MeleeComponent))
                                                                                   ? item.As<MeleeComponent>()
                                                                                   : null);
                                                              } 
                                                              
                                                              if (melee == null)
                                                                  melee = self.Characteristics.Punch;

                                                              if (self.Position.DistanceTo(target.Position) > (melee.Reach + t.Range + 1)) {
                                                                  self.World.InsertMessage("Too far to attack.");
                                                                  return ActionResult.Aborted;
                                                              }

                                                              MeleeCombat.AttackMeleeWithWeapon(self, target, melee, MeleeCombat.GetRandomBodyPart(target));
                                                              return ActionResult.Success;
                                                          }
                                          });
                case Skill.Range:
                    return new Talent(new TalentTemplate()
                                          {
                                                  Skill = identifier,
                                                  Name = "Shoot target.",
                                                  InitialRank = 1,
                                                  MaxRank = 1,
                                                  RequiresTarget = TargetType.Positional,
                                                  ActionOnTargetFunction =
                                                          delegate(Talent t, Actor self, Point p, dynamic[] args)
                                                              {
                                                                  Actor target = self.Level.GetActorAtLocation(p);

                                                                  GunComponent gun = null;

                                                                  if (self.IsItemEquipped(BodyPartType.RightHand)) {
                                                                      var item = self.GetItemAtBodyPart(BodyPartType.RightHand);
                                                                      gun = item.Is(typeof(GunComponent)) ? item.As<GunComponent>() : null;
                                                                  } else if (self.IsItemEquipped(BodyPartType.LeftHand)) {
                                                                      var item = self.GetItemAtBodyPart(BodyPartType.LeftHand);
                                                                      gun = item.Is(typeof(GunComponent)) ? item.As<GunComponent>() : null;
                                                                  }

                                                                  if (gun == null) {
                                                                      self.World.InsertMessage("No range weapon equipped.");
                                                                      return ActionResult.Aborted;
                                                                  }

                                                                  if (self.Position.DistanceTo(target.Position) > (gun.Range + t.Range + 1)) {
                                                                      self.World.InsertMessage("Too far to attack.");
                                                                      return ActionResult.Aborted;
                                                                  }

                                                                  if (gun.Shots <= 0) {
                                                                      self.World.InsertMessage("You squeeze the trigger only the hear the sound of nothing happening...");
                                                                      self.ActionPoints -= gun.WeaponSpeed;
                                                                      return ActionResult.Failed;
                                                                  }

                                                                  MeleeCombat.AttackRangeWithGun(self, target, gun, MeleeCombat.GetRandomBodyPart(target), true);
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
                                                                  RequiresTarget = TargetType.None,
                                                                  Args = new List<TalentTemplate.GenerateArgsListFunction>()
                                                                             {
                                                                                     delegate(Talent t, Actor self, Point p) // what gun are we reloading, return an empty list should cause optionsprompt to quit
                                                                                         {
                                                                                             var weapons = new List<GunComponent>();
                
                                                                                             if (self.IsItemEquipped(BodyPartType.LeftHand) &&
                                                                                                 self.GetItemAtBodyPart(BodyPartType.LeftHand).Is(typeof(GunComponent)))
                                                                                                 weapons.Add(self.GetItemAtBodyPart(BodyPartType.LeftHand).As<GunComponent>());
                
                                                                                             if (self.IsItemEquipped(BodyPartType.RightHand) &&
                                                                                                 self.GetItemAtBodyPart(BodyPartType.RightHand).Is(typeof(GunComponent)))
                                                                                                 weapons.Add(self.GetItemAtBodyPart(BodyPartType.RightHand).As<GunComponent>());
                
                                                                                             return weapons;
                                                                                         },
                                                                                         delegate(Talent t, Actor self, Point p)
                                                                                             {
                                                                                                 List<BulletComponent> mags = new List<BulletComponent>();
                                                                                                 foreach (Item i in self.Items)
                                                                                                     if (i.Is(typeof(BulletComponent)))
                                                                                                         mags.Add(i.As<BulletComponent>());
                                                                                                 return mags;

                                                                                             },
                                                                             },
                
                                                                  ArgsDesciptor = new List<TalentTemplate.ArgDesciptorFunction>()
                                                                                      {
                                                                                              (t, self, target, arg) => ((GunComponent) arg).Item.Name,                                                                                              
                
                                                                                      },
                                                                  ActionOnTargetFunction = delegate(Talent t, Actor self, Point p, dynamic[] args)
                                                                                               {
                                                                                                   if (!(args[0] is GunComponent)) {
                                                                                                       self.World.InsertMessage("Not a firearm.");
                                                                                                       return ActionResult.Aborted;
                                                                                                   }
                
                                                                                                   var gun = args[0] as GunComponent;
                
                                                                                                   if (!(args[1] is BulletComponent)) {
                                                                                                       self.World.InsertMessage("Not a magazine.");
                                                                                                       return ActionResult.Aborted;
                                                                                                   }
                
//                                                                                                   var mag = args[1] as MagazineComponent;
//                
//                                                                                                   if (!mag.FirearmId.Equals(gun.Item.RefId)) {
//                                                                                                       self.World.InsertMessage("Magazine doesn't work with this gun.");
//                                                                                                       return ActionResult.Aborted;
//                                                                                                   }
//                
//                                                                                                   if (gun.Magazine) {
//                                                                                                       //guns contains a magazine already eject it                                                                    
//                //                                                                                       self.AddItem(World.Instance.CreateItem());
//                                                                                                   }
//                                                                                                   self.RemoveItem(mag.Item);
                //                                                                                   gun.Magazine = mag.Item;
                
                                                                                                   //todo factor in speed for quick reload
                                                                                                   self.ActionPoints -= gun.ReloadSpeed;

                                                                                                   if (gun.ShotsRemaining > 0) {
                                                                                                       var droppedAmmo = World.Instance.CreateItem(gun.AmmoCaliber);
                                                                                                       droppedAmmo.Amount = gun.ShotsRemaining;

                                                                                                       self.Level.AddItem(droppedAmmo, self.Position);
                                                                                                   }
                                                                                                   
                
                                                                                                   self.World.InsertMessage(String.Format("{0} reloads {1}, dropping all excess bullets", self.Name, gun.Item.Name));
                
                                                                                                   return ActionResult.Success;
                                                                                               }
                                                          });
                #region Old SKR Attacks
                //                case Skill.TargetAttack:
//                    return new Talent(new TalentTemplate()
//                                          {
//                                                  Skill = identifier,
//                                                  Name = "Attack target specific body part.",
//                                                  InitialRank = 1,
//                                                  MaxRank = 1,
//                                                  RequiresTarget = TargetType.Directional,
//                                                  Args = new List<TalentTemplate.GenerateArgsListFunction>()
//                                                             {
//                                                                     delegate(Talent t, Actor self, Point p)
//                                                                         {
//                                                                             List<MeleeComponent> attacks = new List<MeleeComponent>
//                                                                                                                {
//                                                                                                                        self.Characteristics.Kick,
//                                                                                                                        self.Characteristics.Punch,
//                                                                                                                };
//
//                                                                             foreach (var item in from bp in self.BodyParts
//                                                                                                  where self.IsItemEquipped(bp.Type)
//                                                                                                  select self.GetItemAtBodyPart(bp.Type)) {
//                                                                                 if (item.Is(ItemAction.MeleeAttack))
//                                                                                     attacks.Add(item.As<MeleeComponent>(ItemAction.MeleeAttack));
//                                                                                 if (item.Is(ItemAction.MeleeAttack))
//                                                                                     attacks.Add(item.As<MeleeComponent>(ItemAction.MeleeAttack));
//                                                                             }
//
//                                                                             return attacks;
//                                                                         },
//                                                                     delegate(Talent t, Actor self, Point p)
//                                                                         {
//                                                                             if (!self.Level.DoesActorExistAtLocation(p))
//                                                                                 return null;
//
//                                                                             return self.Level.GetActorAtLocation(p).Characteristics.BodyPartsList;
//                                                                         },
//                                                             },
//
//                                                  ArgsDesciptor = new List<TalentTemplate.ArgDesciptorFunction>()
//                                                                      {
//                                                                              delegate(Talent t, Actor self, Point target, dynamic arg)
//                                                                                  {
//                                                                                      var weapon = (MeleeComponent) arg;
//                                                                                      return weapon.Item == null
//                                                                                                     ? weapon.ActionDescription
//                                                                                                     : String.Format("{0} with {1}", weapon.ActionDescription, weapon.Item.Name);
//                                                                                  },
//                                                                              (t, self, target, arg) => arg.ToString(),
//                                                                      },
//
//                                                  ActionOnTargetFunction =
//                                                          delegate(Talent t, Actor self, Point p, dynamic[] args)
//                                                              {
//                                                                  Actor target = self.Level.GetActorAtLocation(p);
//                                                                  if (!(args[0] is MeleeComponent)) {
//                                                                      self.World.InsertMessage("Not a melee weapon.");
//                                                                      return ActionResult.Aborted;
//                                                                  }
//
//                                                                  var melee = args[0] as MeleeComponent;
//
//                                                                  if (!(args[1] is BodyPart)) {
//                                                                      self.World.InsertMessage("Not a body part.");
//                                                                      return ActionResult.Aborted;
//                                                                  }
//
//                                                                  if (self.Position.DistanceTo(target.Position) > (melee.Reach + t.Range + 1)) {
//                                                                      // diagonal attacks are >1, so if weapons have range of 0, we can't use them
//                                                                      self.World.InsertMessage("Too far to attack.");
//                                                                      return ActionResult.Aborted;
//                                                                  }
//
//                                                                  MeleeCombat.AttackMeleeWithWeapon(self, target, melee, args[1], true);
//                                                                  return ActionResult.Success;
//                                                              }
//                                          });
//                case Skill.RangeTargetAttack:
//                    return new Talent(new TalentTemplate()
//                                          {
//                                                  Skill = identifier,
//                                                  Name = "Shoot target specific body part.",
//                                                  InitialRank = 1,
//                                                  MaxRank = 1,
//                                                  RequiresTarget = TargetType.Positional,
//                                                  Args = new List<TalentTemplate.GenerateArgsListFunction>()
//                                                             {
//                                                                     delegate(Talent t, Actor self, Point p)
//                                                                         {
//                                                                             return (from bp in self.BodyParts
//                                                                                     where self.IsItemEquipped(bp.Type)
//                                                                                     select self.GetItemAtBodyPart(bp.Type)
//                                                                                     into item
//                                                                                     where item.Is(ItemAction.Shoot)
//                                                                                     select item.As<FirearmComponent>(ItemAction.Shoot)).ToList();
//                                                                         },
//                                                                     delegate(Talent t, Actor self, Point p)
//                                                                         {
//                                                                             if (!self.Level.DoesActorExistAtLocation(p))
//                                                                                 return null;
//
//                                                                             return self.Level.GetActorAtLocation(p).Characteristics.BodyPartsList;
//                                                                         },
//                                                             },
//
//                                                  ArgsDesciptor = new List<TalentTemplate.ArgDesciptorFunction>()
//                                                                      {
//                                                                              delegate(Talent t, Actor self, Point p, dynamic arg)
//                                                                                  {
//                                                                                      var weapon = (FirearmComponent) arg;
//                                                                                      return weapon.Item == null
//                                                                                                     ? weapon.ActionDescription
//                                                                                                     : String.Format("{0} with {1}", weapon.ActionDescription, weapon.Item.Name);
//                                                                                  },
//                                                                              (t, self, target, arg) => arg.ToString()
//
//                                                                      },
//
//                                                  ActionOnTargetFunction =
//                                                          delegate(Talent t, Actor self, Point p, dynamic[] args)
//                                                              {
//                                                                  Actor target = self.Level.GetActorAtLocation(p);
//
//                                                                  if (!(args[0] is FirearmComponent)) {
//                                                                      self.World.InsertMessage("Not a firearm.");
//                                                                      return ActionResult.Aborted;
//                                                                  }
//
//                                                                  var gun = args[0] as FirearmComponent;
//
//                                                                  if (!(args[1] is BodyPart)) {
//                                                                      self.World.InsertMessage("Not a body part.");
//                                                                      return ActionResult.Aborted;
//                                                                  }
//
//                                                                  if (self.Position.DistanceTo(target.Position) > (gun.Range + t.Range + 1)) {
//                                                                      self.World.InsertMessage("Too far to attack.");
//                                                                      return ActionResult.Aborted;
//                                                                  }
//
//                                                                  if (gun.Magazine == false || gun.Shots <= 0) {
//                                                                      self.World.InsertMessage("You squeeze the trigger only the hear the sound of nothing happening...");
//                                                                      self.ActionPoints -= gun.WeaponSpeed;
//                                                                      return ActionResult.Failed;
//                                                                  }
//
//                                                                  MeleeCombat.AttackRangeWithGun(self, target, gun, args[1], true);
//                                                                  return ActionResult.Success;
//                                                              }
//                                          });
//                case Skill.Reload:
//                    return new Talent(new TalentTemplate
//                                          {
//                                                  Skill = identifier,
//                                                  Name = "Reload firearm",
//                                                  InitialRank = 1,
//                                                  MaxRank = 1,
//                                                  RequiresTarget = TargetType.None,
//                                                  Args = new List<TalentTemplate.GenerateArgsListFunction>()
//                                                             {
//                                                                     delegate(Talent t, Actor self, Point p) // what gun are we reloading, return an empty list should cause optionsprompt to quit
//                                                                         {
//                                                                             var weapons = new List<FirearmComponent>();
//
//                                                                             if (self.IsItemEquipped(BodyPartType.LeftHand) &&
//                                                                                 self.GetItemAtBodyPart(BodyPartType.LeftHand).Is(ItemAction.Shoot))
//                                                                                 weapons.Add(self.GetItemAtBodyPart(BodyPartType.LeftHand).As<FirearmComponent>(ItemAction.Shoot));
//
//                                                                             if (self.IsItemEquipped(BodyPartType.RightHand) &&
//                                                                                 self.GetItemAtBodyPart(BodyPartType.RightHand).Is(ItemAction.Shoot))
//                                                                                 weapons.Add(self.GetItemAtBodyPart(BodyPartType.RightHand).As<FirearmComponent>(ItemAction.Shoot));
//
//                                                                             return weapons;
//                                                                         },
//                                                                     delegate(Talent t, Actor self, Point p)
//                                                                         {
//                                                                             List<MagazineComponent> mags = new List<MagazineComponent>();
//                                                                             foreach (Item i in self.Items)
//                                                                                 if (i.Is(ItemAction.ReloadFirearm))
//                                                                                     mags.Add(i.As<MagazineComponent>(ItemAction.ReloadFirearm));
//                                                                             return mags;
//                                                                         },
//                                                             },
//
//                                                  ArgsDesciptor = new List<TalentTemplate.ArgDesciptorFunction>()
//                                                                      {
//                                                                              (t, self, target, arg) => ((FirearmComponent) arg).Item.Name,
//                                                                              (t, self, target, arg) => ((MagazineComponent) arg).Item.Name,
//
//                                                                      },
//                                                  ActionOnTargetFunction = delegate(Talent t, Actor self, Point p, dynamic[] args)
//                                                                               {
//                                                                                   if (!(args[0] is FirearmComponent)) {
//                                                                                       self.World.InsertMessage("Not a firearm.");
//                                                                                       return ActionResult.Aborted;
//                                                                                   }
//
//                                                                                   var gun = args[0] as FirearmComponent;
//
//                                                                                   if (!(args[1] is MagazineComponent)) {
//                                                                                       self.World.InsertMessage("Not a magazine.");
//                                                                                       return ActionResult.Aborted;
//                                                                                   }
//
//                                                                                   var mag = args[1] as MagazineComponent;
//
//                                                                                   if (!mag.FirearmId.Equals(gun.Item.RefId)) {
//                                                                                       self.World.InsertMessage("Magazine doesn't work with this gun.");
//                                                                                       return ActionResult.Aborted;
//                                                                                   }
//
//                                                                                   if (gun.Magazine) {
//                                                                                       //guns contains a magazine already eject it                                                                    
////                                                                                       self.AddItem(World.Instance.CreateItem());
//                                                                                   }
//                                                                                   self.RemoveItem(mag.Item);
////                                                                                   gun.Magazine = mag.Item;
//
//                                                                                   //todo revolvers and shotguns
//                                                                                   self.ActionPoints -= gun.ReloadSpeed;
//
//                                                                                   self.World.InsertMessage(String.Format("{0} reloads {1} with a {2}", self.Name, gun.Item.Name, mag.Item.Name));
//
//                                                                                   return ActionResult.Success;
//                                                                               }
                //                                          });
                #endregion

                case Skill.UseFeature:
                    return new Talent(new TalentTemplate()
                                          {
                                                  Skill = identifier,
                                                  Name = "Use feature",
                                                  InitialRank = 1,
                                                  MaxRank = 1,
                                                  RequiresTarget = TargetType.Directional,
                                                  Args = new List<TalentTemplate.GenerateArgsListFunction>()
                                                             {
                                                                     delegate(Talent t, Actor self, Point p) // what gun are we reloading, return an empty list should cause optionsprompt to quit
                                                                         {
                                                                             var feature = self.Level.GetFeatureAtLocation(p);
                                                                             return feature == null ? null : feature.ActiveUsages;
                                                                         },
                                                             },

                                                  ArgsDesciptor = new List<TalentTemplate.ArgDesciptorFunction>()
                                                                      {
                                                                              (t, self, target, arg) => arg,

                                                                      },
                                                  ActionOnTargetFunction = delegate(Talent t, Actor self, Point p, dynamic[] args)
                                                                               {
                                                                                   self.Level.GetFeatureAtLocation(p).Use(self, args[0]);
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
