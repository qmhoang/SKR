using System;
using System.Collections.Generic;
using System.Linq;
using DEngine.Actor;
using DEngine.Core;
using SkrGame.Core;
using SkrGame.Gameplay.Combat;
using SkrGame.Gameplay.Talent;
using SkrGame.Gameplay.Talent.Components;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Entities.Items.Components;

namespace SkrGame.Universe.Factories {
	public abstract class TalentFactory : Factory<string, Talent> {}

	public class SourceTalentFactory : TalentFactory {
		private Dictionary<string, TalentTemplate> templates;
		private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public SourceTalentFactory() {
			templates = new Dictionary<string, TalentTemplate>();

			LoadAttributes();
			LoadSkills();
			LoadBasicTalents();
		}

		private void LoadAttributes() {
			CreateAttribute("attrb_strength", "Strength");
			CreateAttribute("attrb_agility", "Agility");
			CreateAttribute("attrb_constitution", "Constitution");
			CreateAttribute("attrb_intellect", "Intellect");
			CreateAttribute("attrb_cunning", "Cunning");
			CreateAttribute("attrb_resolve", "Resolve");
			CreateAttribute("attrb_presence", "Presence");
			CreateAttribute("attrb_grace", "Grace");
			CreateAttribute("attrb_composure", "Composure");
		}

		private void LoadBasicTalents() {
			Create("action_attack",
			       new TalentTemplate
			       {
			       		Name = "Attack",
			       		Components =
			       				new List<TalentComponentTemplate>
			       				{
			       						new ActiveTalentTemplate
			       						{
			       								InitialRank = 1,
			       								MaxRank = 1,
			       								RequiresTarget = TargetType.Directional,
			       								ActionOnTargetFunction =
			       										delegate(ActiveTalentComponent t, Actor user, Point point, dynamic[] args)
			       										{
			       											Actor target = user.Level.GetActorAtLocation(point);
			       											MeleeComponent melee = null;
			       											// if we have something in our main hand, probably want to use that
			       											if (user.IsItemEquipped(BodySlot.MainHand)) {
			       												var item = user.GetItemAtBodyPart(BodySlot.MainHand);
			       												melee = (item.Is(typeof (MeleeComponent))
			       												         		? item.As<MeleeComponent>()
			       												         		: null);
			       											} else if (user.IsItemEquipped(BodySlot.OffHand)) {
			       												var item = user.GetItemAtBodyPart(BodySlot.OffHand);
			       												melee = (item.Is(typeof (MeleeComponent))
			       												         		? item.As<MeleeComponent>()
			       												         		: null);
			       											}

			       											if (melee == null)
			       												melee = user.Punch;

			       											if (user.Position.DistanceTo(target.Position) > (melee.Reach + t.Range + 1)) {
			       												World.Instance.AddMessage("Too far to attack.");
			       												return ActionResult.Aborted;
			       											}

			       											var hitBonusFromSkill = user.GetTalent(melee.Skill).As<SkillComponent>().Rank;
			       											var bodyPart = target.GetBodyPart(BodySlot.Torso);

			       											var result = Combat.Attack(user, target, hitBonusFromSkill - World.MEAN);

			       											if (result == CombatEventResult.Hit) {
			       												var damage =
			       														Math.Max(
			       																Combat.GetStrengthDamage(user.GetTalent("attrb_strength").As<AttributeComponent>().Rank).Roll() + melee.Damage.Roll(),
			       																1);
			       												int damageResistance, realDamage;

			       												target.Damage(damage, melee.DamageType, bodyPart, out damageResistance, out realDamage);

			       												World.Instance.AddMessage(String.Format("{0} {1} {2}'s {3}.... and inflict {4} wounds.",
			       												                                        user.Name, melee.ActionDescriptionPlural, target.Name, bodyPart.Name, "todo-description"));


			       												CombatEventArgs hit = new CombatEventArgs(user, target, bodyPart, CombatEventResult.Hit, damage,
			       												                                          damageResistance, realDamage);
			       												user.OnAttacking(hit);
			       												target.OnDefending(hit);
			       											} else if (result == CombatEventResult.Miss) {
			       												World.Instance.AddMessage(String.Format("{0} {1} {2}'s {3}.... and misses.",
			       												                                        user.Name, melee.ActionDescriptionPlural, target.Name, bodyPart.Name));

			       												CombatEventArgs combatEvent = new CombatEventArgs(user, target, bodyPart);
			       												user.OnAttacking(combatEvent);
			       												target.OnDefending(combatEvent);
			       											} else if (result == CombatEventResult.Dodge) {
			       												World.Instance.AddMessage(String.Format("{0} {1} {2}'s {3}.... and {4} dodges.",
			       												                                        user.Name, melee.ActionDescriptionPlural, target.Name, bodyPart.Name, target.Name));

			       												CombatEventArgs combatEvent = new CombatEventArgs(user, target, bodyPart, CombatEventResult.Dodge);
			       												user.OnAttacking(combatEvent);
			       												target.OnDefending(combatEvent);
			       											}

			       											user.ActionPoints -= melee.APToAttack;
			       											return ActionResult.Success;
			       										}
			       						}
			       				},
			       });
			Create("action_range",
			       new TalentTemplate
			       {
			       		Name = "Shoot target.",
			       		Components =
			       				new List<TalentComponentTemplate>
			       				{
			       						new ActiveTalentTemplate
			       						{
			       								InitialRank = 1,
			       								MaxRank = 1,
			       								RequiresTarget = TargetType.Positional,
			       								Args = new List<ActiveTalentArgTemplate>
			       								       {
			       								       		new ActiveTalentArgTemplate
			       								       		{
			       								       				ArgFunction = delegate(ActiveTalentComponent t, Actor user, Point p)
			       								       				              {
			       								       				              	List<dynamic> guns = new List<dynamic>();
			       								       				              	if (user.IsItemEquipped(BodySlot.MainHand)) {
			       								       				              		var item = user.GetItemAtBodyPart(BodySlot.MainHand);
			       								       				              		if (item.Is(typeof (RangeComponent)))
			       								       				              			guns.Add(item.As<RangeComponent>());
			       								       				              	} else if (user.IsItemEquipped(BodySlot.OffHand)) {
			       								       				              		var item = user.GetItemAtBodyPart(BodySlot.OffHand);
			       								       				              		if (item.Is(typeof (RangeComponent)))
			       								       				              			guns.Add(item.As<RangeComponent>());
			       								       				              	}

			       								       				              	// todo shoot both guns at the same time
			       								       				              	return guns;
			       								       				              },
			       								       				ArgDesciptor = delegate(ActiveTalentComponent t, Actor user, Point p, dynamic arg)
			       								       				               {
			       								       				               	var weapon = (RangeComponent) arg;
			       								       				               	return weapon.Item == null
			       								       				               	       		? weapon.ActionDescription
			       								       				               	       		: String.Format("{0} with {1}", weapon.ActionDescription, weapon.Item.Name);
			       								       				               }
			       								       		},
			       								       		new ActiveTalentArgTemplate
			       								       		{
			       								       				ArgFunction = delegate(ActiveTalentComponent t, Actor user, Point p)
			       								       				              {
			       								       				              	if (!user.Level.DoesActorExistAtLocation(p))
			       								       				              		return null;

			       								       				              	return user.Level.GetActorAtLocation(p).BodyPartsList;
			       								       				              },
			       								       				ArgDesciptor = (t, user, target, arg) => arg.ToString(),
			       								       				Required = false,
			       								       				PromptDescription = "Shoot at which body part?"
			       								       		}
			       								       },
			       								ActionOnTargetFunction =
			       										delegate(ActiveTalentComponent t, Actor user, Point targettedLocation, dynamic[] args)
			       										{
			       											if (!(args[0] is RangeComponent)) {
			       												World.Instance.AddMessage("Not a range weapon.");
			       												return ActionResult.Aborted;
			       											}

			       											var weapon = (RangeComponent) args[0];

			       											bool actorAtLocation = true;
			       											if (args[1] == null)
			       												actorAtLocation = false;
			       											else if (!(args[1] is BodyPart)) {
			       												World.Instance.AddMessage("Not a body part.");
			       												return ActionResult.Aborted;
			       											}

			       											if (user.Position.DistanceTo(targettedLocation) > (weapon.Range + t.Range + 1)) {
			       												World.Instance.AddMessage("Too far to attack.");
			       												return ActionResult.Aborted;
			       											}

			       											if (weapon.ShotsRemaining <= 0) {
			       												World.Instance.AddMessage(String.Format("{0} attempts to use the {1} only to realize the weapon is not loaded",
			       												                                        user.Name, weapon.Item.Name));
			       												user.ActionPoints -= weapon.APToAttack;
			       												return ActionResult.Failed;
			       											}

			       											weapon.ShotsRemaining--;
			       											user.ActionPoints -= weapon.APToAttack;

			       											var hitBonusFromSkill = user.GetTalent(weapon.Skill).As<SkillComponent>().Rank - World.MEAN;

			       											var targets = Combat.GetTargetsOnPath(user.Position, targettedLocation).ToList();

			       											for (int i = 0; i < targets.Count; i++) {
			       												var point = targets[i];
			       												var targetAtLocation = user.Level.GetActorAtLocation(point);

			       												BodyPart bodyPartTargetted;

			       												if (actorAtLocation)
			       													bodyPartTargetted = (BodyPart) args[1];
			       												else
			       													bodyPartTargetted = Combat.GetRandomBodyPart(targetAtLocation);


			       												double range = targetAtLocation.Position.DistanceTo(user.Position) * World.TILE_LENGTH;
			       												double rangePenalty = Math.Min(0,
			       												                               -World.STANDARD_DEVIATION * Combat.RANGE_PENALTY_STD_DEV_MULT * Math.Log(range) +
			       												                               World.STANDARD_DEVIATION * 2 / 3);
			       												Logger.InfoFormat("Target: {2}, range to target: {0}, penalty: {1}", range, rangePenalty, targetAtLocation.Name);

			       												// not being targetted gives a sigma (std dev) penalty
			       												rangePenalty -= targettedLocation == targetAtLocation.Position ? 0 : World.STANDARD_DEVIATION;

			       												double difficultyOfShot = hitBonusFromSkill + rangePenalty + i * Combat.RANGE_PENALTY_TILE_OCCUPIED;
			       												Logger.InfoFormat("Shot difficulty: {0}, targetting penalty: {1}, weapon bonus: {2}, is target: {3}",
			       												                  difficultyOfShot, bodyPartTargetted.TargettingPenalty, hitBonusFromSkill,
			       												                  targettedLocation == targetAtLocation.Position);


			       												var result = Combat.Attack(user, targetAtLocation, difficultyOfShot);

			       												if (result == CombatEventResult.Miss) {
			       													if (point == targettedLocation) // if this is where the actor targetted
			       														World.Instance.AddMessage(String.Format("{0} {1} {2}'s {3}.... and misses.",
			       														                                        user.Name, weapon.ActionDescriptionPlural, targetAtLocation.Name,
			       														                                        bodyPartTargetted.Name));
			       													CombatEventArgs combatEvent = new CombatEventArgs(user, targetAtLocation, bodyPartTargetted);
			       													user.OnAttacking(combatEvent);
			       													targetAtLocation.OnDefending(combatEvent);
			       												} else if (result == CombatEventResult.Dodge) {
			       													if (point == targettedLocation)
			       														World.Instance.AddMessage(String.Format("{0} {1} {2}'s {3}.... and {4} dodges.",
			       														                                        user.Name, weapon.ActionDescriptionPlural, targetAtLocation.Name,
			       														                                        bodyPartTargetted.Name, targetAtLocation.Name));
			       													else {
			       														// we didn't target the actor, but the actor still dodges
			       													}
			       													CombatEventArgs combatEvent = new CombatEventArgs(user, targetAtLocation, bodyPartTargetted, CombatEventResult.Dodge);
			       													user.OnAttacking(combatEvent);
			       													targetAtLocation.OnDefending(combatEvent);
			       												} else if (result == CombatEventResult.Hit) {
			       													var damage =
			       															Math.Max(
			       																	Combat.GetStrengthDamage(user.GetTalent("attrb_strength").As<AttributeComponent>().Rank).Roll() +
			       																	weapon.Damage.Roll(), 1);
			       													int damageResistance, realDamage;

			       													targetAtLocation.Damage(damage, weapon.DamageType, bodyPartTargetted, out damageResistance, out realDamage);

			       													World.Instance.AddMessage(String.Format("{0} {1} {2}'s {3}.... and inflict {4} wounds.",
			       													                                        user.Name, weapon.ActionDescriptionPlural, targetAtLocation.Name,
			       													                                        bodyPartTargetted.Name,
			       													                                        "todo-description"));


			       													CombatEventArgs combatEvent = new CombatEventArgs(user, targetAtLocation, bodyPartTargetted, CombatEventResult.Hit, damage,
			       													                                                  damageResistance, realDamage);
			       													user.OnAttacking(combatEvent);
			       													targetAtLocation.OnDefending(combatEvent);

			       													return ActionResult.Success;
			       												}

			       												// todo drop ammo casing
			       											}

			       											World.Instance.AddMessage(String.Format("{0} {1} and hits nothing", user.Name, weapon.ActionDescriptionPlural));
			       											return ActionResult.Failed;
			       										}
			       						}
			       				},
			       });
			Create("action_reload",
			       new TalentTemplate
			       {
			       		Name = "Reload weapon",
			       		Components =
			       				new List<TalentComponentTemplate>
			       				{
			       						new ActiveTalentTemplate
			       						{
			       								InitialRank = 1,
			       								MaxRank = 1,
			       								RequiresTarget = TargetType.None,
			       								Args = new List<ActiveTalentArgTemplate>
			       								       {
			       								       		new ActiveTalentArgTemplate
			       								       		{
			       								       				ArgFunction = delegate(ActiveTalentComponent t, Actor user, Point p)
			       								       							// what gun are we reloading, return an empty list should cause optionsprompt to quit
			       								       				              {
			       								       				              	var weapons = new List<RangeComponent>();

			       								       				              	if (user.IsItemEquipped(BodySlot.OffHand) &&
			       								       				              	    user.GetItemAtBodyPart(BodySlot.OffHand).Is(typeof (RangeComponent)))
			       								       				              		weapons.Add(user.GetItemAtBodyPart(BodySlot.OffHand).As<RangeComponent>());

			       								       				              	if (user.IsItemEquipped(BodySlot.MainHand) &&
			       								       				              	    user.GetItemAtBodyPart(BodySlot.MainHand).Is(typeof (RangeComponent)))
			       								       				              		weapons.Add(user.GetItemAtBodyPart(BodySlot.MainHand).As<RangeComponent>());

			       								       				              	return weapons;
			       								       				              },
			       								       				ArgDesciptor = (t, user, target, arg) => ((RangeComponent) arg).Item.Name,
			       								       		},
			       								       		new ActiveTalentArgTemplate
			       								       		{
			       								       				ArgFunction = delegate(ActiveTalentComponent t, Actor user, Point p)
			       								       				              {
			       								       				              	List<AmmoComponent> list = new List<AmmoComponent>();
			       								       				              	foreach (Item item in user.Items)
			       								       				              		if (item.Is(typeof (AmmoComponent))) {
			       								       				              			var ammo = item.As<AmmoComponent>();
			       								       				              			list.Add(item.As<AmmoComponent>());
			       								       				              		}
			       								       				              	return list;
			       								       				              },
			       								       				ArgDesciptor = (t, user, target, arg) => ((AmmoComponent) arg).Item.Name,
			       								       		}
			       								       },
			       								ActionOnTargetFunction =
			       										delegate(ActiveTalentComponent t, Actor user, Point p, dynamic[] args)
			       										{
			       											if (!(args[0] is RangeComponent)) {
			       												World.Instance.AddMessage("Not a range weapon.", MessageType.High);
			       												return ActionResult.Aborted;
			       											}

			       											var weapon = (RangeComponent) args[0];

			       											if (!(args[1] is AmmoComponent)) {
			       												World.Instance.AddMessage("Not ammo.", MessageType.High);
			       												return ActionResult.Aborted;
			       											}

			       											var ammo = (AmmoComponent) args[1];

			       											if (!ammo.Type.Equals(weapon.AmmoType)) {
			       												World.Instance.AddMessage("Wrong ammo type for this weapon.", MessageType.High);
			       												return ActionResult.Aborted;
			       											}

			       											// todo give arrow back when replacing new ammo
			       											if (weapon.ShotsRemaining >= weapon.Shots) {
			       												World.Instance.AddMessage("Weapon is already fully loaded.");
			       												return ActionResult.Aborted;
			       											}

															// todo revolvers and single load weapons


															// to semi-simulate dropping magazines, drop remaining bullets on the ground
															if (weapon.ShotsRemaining > 0) {
																var droppedAmmo = World.Instance.CreateItem(weapon.AmmoType);
																droppedAmmo.Amount = weapon.ShotsRemaining;
																weapon.ShotsRemaining = 0;
																user.Level.AddItem(droppedAmmo, user.Position);
																World.Instance.AddMessage(String.Format("{0} reloads {1} with {2}, dropping all excess ammo.", user.Name, weapon.Item.Name,
																										ammo.Item.Name));
															} else {
																World.Instance.AddMessage(String.Format("{0} reloads {1} with {2}.", user.Name, weapon.Item.Name,
																										ammo.Item.Name));
															}

			       											if (ammo.Item.StackType == StackType.Hard && ammo.Item.Amount >= weapon.Shots) {
			       												ammo.Item.Amount -= weapon.Shots;
			       												weapon.ShotsRemaining = weapon.Shots;
			       											} else if (ammo.Item.StackType == StackType.Hard && ammo.Item.Amount > 0) {
				       											weapon.ShotsRemaining = ammo.Item.Amount;
																user.RemoveItem(ammo.Item);
			       											}
															
			       											user.ActionPoints -= weapon.APToReload;

			       											return ActionResult.Success;
			       										}
			       						}
			       				},
			       });

			Create("action_activate",
			       new TalentTemplate
			       {
			       		Name = "Activate feature",
			       		Components =
			       				new List<TalentComponentTemplate>
			       				{
			       						new ActiveTalentTemplate
			       						{
			       								InitialRank = 1,
			       								MaxRank = 1,
			       								RequiresTarget = TargetType.Directional,
			       								Args = new List<ActiveTalentArgTemplate>
			       								       {
			       								       		new ActiveTalentArgTemplate
			       								       		{
			       								       				ArgFunction = delegate(ActiveTalentComponent t, Actor user, Point p)
			       								       				              {
			       								       				              	var feature = user.Level.GetFeatureAtLocation(p);
			       								       				              	return feature == null ? null : feature.ActiveUsages;
			       								       				              },
			       								       				ArgDesciptor = (t, user, target, arg) => arg,
			       								       		},
			       								       },
			       								ActionOnTargetFunction =
			       										delegate(ActiveTalentComponent t, Actor user, Point p, dynamic[] args)
			       										{
			       											user.Level.GetFeatureAtLocation(p).Use(user, args[0]);
			       											return ActionResult.Success;
			       										}
			       						}
			       				},
			       });
		}

		private void CreateAttribute(string id, string name) {
			Create(id, new TalentTemplate()
			           {
			           		Name = name,
			           		Components = new List<TalentComponentTemplate>
			           		             {
			           		             		new AttributeComponentTemplate
			           		             		{
			           		             				InitialRank = 0,
			           		             				MaxRank = 100
			           		             		}
			           		             }
			           });
		}

		private void LoadSkills() {
			Create("skill_unarmed",
			       new TalentTemplate
			       {
			       		Name = "Brawling",
			       		Components = new List<TalentComponentTemplate>
			       		             {
			       		             		new SkillComponentTemplate
			       		             		{
			       		             				InitialRank = 0,
			       		             				MaxRank = 10,
			       		             				CalculateRealRank = (t, user) => t.RawRank + user.GetTalent("attrb_agility").As<AttributeComponent>().Rank
			       		             		}
			       		             }
			       });
			Create("skill_sword",
			       new TalentTemplate
			       {
			       		Name = "Sword",
			       		Components = new List<TalentComponentTemplate>
			       		             {
			       		             		new SkillComponentTemplate
			       		             		{
			       		             				InitialRank = 0,
			       		             				MaxRank = 10,
			       		             				CalculateRealRank = (t, user) => t.RawRank + user.GetTalent("attrb_agility").As<AttributeComponent>().Rank
			       		             		},
			       		             }
			       });
			Create("skill_knife",
			       new TalentTemplate
			       {
			       		Name = "Knife",
			       		Components = new List<TalentComponentTemplate>
			       		             {
			       		             		new SkillComponentTemplate
			       		             		{
			       		             				InitialRank = 0,
			       		             				MaxRank = 10,
			       		             				CalculateRealRank = (t, user) => t.RawRank + user.GetTalent("attrb_agility").As<AttributeComponent>().Rank
			       		             		}
			       		             }
			       });
			Create("skill_pistol",
			       new TalentTemplate
			       {
			       		Name = "Pistol",
			       		Components = new List<TalentComponentTemplate>
			       		             {
			       		             		new SkillComponentTemplate
			       		             		{
			       		             				InitialRank = 0,
			       		             				MaxRank = 10,
			       		             				CalculateRealRank = (t, user) => t.RawRank + user.GetTalent("attrb_agility").As<AttributeComponent>().Rank
			       		             		}
			       		             }
			       });
			Create("skill_bow",
			       new TalentTemplate
			       {
			       		Name = "Bow",
			       		Components = new List<TalentComponentTemplate>
			       		             {
			       		             		new SkillComponentTemplate
			       		             		{
			       		             				InitialRank = 0,
			       		             				MaxRank = 10,
			       		             				CalculateRealRank = (t, user) => t.RawRank + user.GetTalent("attrb_agility").As<AttributeComponent>().Rank
			       		             		}
			       		             }
			       });
		}

//		public override Talent Construct(Skill identifier) {
//			switch (identifier) {
//				case Skill.Attack:
//					return new Talent(new TalentTemplate()
//										  {
//												  Skill = identifier,
//												  Name = "Attack",
//												  InitialRank = 1,
//												  MaxRank = 1,
//												  RequiresTarget = TargetType.Directional,
//												  ActionOnTargetFunction =
//														  delegate(Talent t, Actor self, Point point, dynamic[] args)
//															  {
//																  Actor target = self.Level.GetActorAtLocation(point);
//																  MeleeComponent melee = null;
//																  // if we have something in our right hand, probably want to use that
//																  if (self.IsItemEquipped(BodySlot.MainHand)) {
//																	  var item = self.GetItemAtBodyPart(BodySlot.MainHand);
//																	  melee = (item.Is(typeof (MeleeComponent))
//																					   ? item.As<MeleeComponent>()
//																					   : null);
//																  } else if (self.IsItemEquipped(BodySlot.OffHand)) {
//																	  var item = self.GetItemAtBodyPart(BodySlot.OffHand);
//																	  melee = (item.Is(typeof (MeleeComponent))
//																					   ? item.As<MeleeComponent>()
//																					   : null);
//																  }
//
//																  if (melee == null)
//																	  melee = self.Characteristics.Punch;
//
//																  if (self.Position.DistanceTo(target.Position) > (melee.Reach + t.Range + 1)) {
//																	  self.World.AddMessage("Too far to attack.");
//																	  return ActionResult.Aborted;
//																  }
//
//																  Combat.AttackMeleeWithWeapon(self, target, melee, Combat.GetRandomBodyPart(target));
//																  return ActionResult.Success;
//															  }
//										  });
//				case Skill.Range:
//					return new Talent(new TalentTemplate()
//										  {
//												  Skill = identifier,
//												  Name = "Shoot target.",
//												  InitialRank = 1,
//												  MaxRank = 1,
//												  RequiresTarget = TargetType.Positional,
//												  Args = new List<TalentTemplate.GenerateArgsListFunction>()
//															 {
//																	 delegate(Talent t, Actor self, Point p)
//																		 {
//																			 List<dynamic> guns = new List<dynamic>();
//																			 if (self.IsItemEquipped(BodySlot.MainHand)) {
//																				 var item = self.GetItemAtBodyPart(BodySlot.MainHand);
//																				 if (item.Is(typeof (RangeComponent)))
//																					 guns.Add(item.As<RangeComponent>());
//																			 } else if (self.IsItemEquipped(BodySlot.OffHand)) {
//																				 var item = self.GetItemAtBodyPart(BodySlot.OffHand);
//																				 if (item.Is(typeof (RangeComponent)))
//																					 guns.Add(item.As<RangeComponent>());
//																			 }
//
//																			 // todo shoot both guns at the same time
//																			 return guns;
////                                                                    return (from bp in self.BodyParts
////                                                                            where self.IsItemEquipped(bp.Type)
////                                                                            select self.GetItemAtBodyPart(bp.Type)
////                                                                            into item
////                                                                            where item.Is(ItemAction.Shoot)
////                                                                            select item.As<FirearmComponent>(ItemAction.Shoot)).ToList();
//																		 },
//																	 delegate(Talent t, Actor self, Point p)
//																		 {
//																			 if (!self.Level.DoesActorExistAtLocation(p))
//																				 return null;
//
//																			 return self.Level.GetActorAtLocation(p).Characteristics.BodyPartsList;
//																		 },
//															 },
//												  ArgsDesciptor = new List<TalentTemplate.ArgDesciptorFunction>()
//																	  {
//																			  delegate(Talent t, Actor self, Point p, dynamic arg)
//																				  {
//																					  var weapon = (RangeComponent) arg;
//																					  return weapon.Item == null
//																									 ? weapon.ActionDescription
//																									 : String.Format("{0} with {1}", weapon.ActionDescription, weapon.Item.Name);
//																				  },
//																			  (t, self, target, arg) => arg.ToString()
//																	  },
//												  ActionOnTargetFunction =
//														  delegate(Talent t, Actor self, Point p, dynamic[] args)
//															  {
//																  Actor target = self.Level.GetActorAtLocation(p);
//
//																  if (!(args[0] is RangeComponent)) {
//																	  self.World.AddMessage("Not a firearm.");
//																	  return ActionResult.Aborted;
//																  }
//
//																  var gun = args[0] as RangeComponent;
//
//																  if (!(args[1] is BodyPart)) {
//																	  self.World.AddMessage("Not a body part.");
//																	  return ActionResult.Aborted;
//																  }
//
//																  if (self.Position.DistanceTo(target.Position) > (gun.Range + t.Range + 1)) {
//																	  self.World.AddMessage("Too far to attack.");
//																	  return ActionResult.Aborted;
//																  }
//
//																  if (gun.Shots <= 0) {
//																	  self.World.AddMessage("You squeeze the trigger only the hear the sound of nothing happening...");
//																	  self.ActionPoints -= gun.WeaponSpeed;
//																	  return ActionResult.Failed;
//																  }
//
//																  Combat.AttackRangeWithGun(self, target, gun, args[1], true);
//
//																  gun.ShotsRemaining--;
//
//																  return ActionResult.Success;
//															  }
//										  });
//				case Skill.Reload:
//					return new Talent(new TalentTemplate
//										  {
//												  Skill = identifier,
//												  Name = "Reload firearm",
//												  InitialRank = 1,
//												  MaxRank = 1,
//												  RequiresTarget = TargetType.None,
//												  Args = new List<TalentTemplate.GenerateArgsListFunction>()
//															 {
//																	 delegate(Talent t, Actor self, Point p) // what gun are we reloading, return an empty list should cause optionsprompt to quit
//																		 {
//																			 var weapons = new List<RangeComponent>();
//
//																			 if (self.IsItemEquipped(BodySlot.OffHand) &&
//																				 self.GetItemAtBodyPart(BodySlot.OffHand).Is(typeof (RangeComponent)))
//																				 weapons.Add(self.GetItemAtBodyPart(BodySlot.OffHand).As<RangeComponent>());
//
//																			 if (self.IsItemEquipped(BodySlot.MainHand) &&
//																				 self.GetItemAtBodyPart(BodySlot.MainHand).Is(typeof (RangeComponent)))
//																				 weapons.Add(self.GetItemAtBodyPart(BodySlot.MainHand).As<RangeComponent>());
//
//																			 return weapons;
//																		 },
//																	 delegate(Talent t, Actor self, Point p)
//																		 {
//																			 List<BulletComponent> mags = new List<BulletComponent>();
//																			 foreach (Item i in self.Items)
//																				 if (i.Is(typeof (BulletComponent)))
//																					 mags.Add(i.As<BulletComponent>());
//																			 return mags;
//																		 },
//															 },
//												  ArgsDesciptor = new List<TalentTemplate.ArgDesciptorFunction>()
//																	  {
//																			  (t, self, target, arg) => ((RangeComponent) arg).Item.Name,
//																	  },
//												  ActionOnTargetFunction = delegate(Talent t, Actor self, Point p, dynamic[] args)
//																			   {
//																				   if (!(args[0] is RangeComponent)) {
//																					   self.World.AddMessage("Not a firearm.");
//																					   return ActionResult.Aborted;
//																				   }
//
//																				   var gun = args[0] as RangeComponent;
//
//																				   if (!(args[1] is BulletComponent)) {
//																					   self.World.AddMessage("Not a magazine.");
//																					   return ActionResult.Aborted;
//																				   }
//
////                                                                                                   var mag = args[1] as MagazineComponent;
////                
////                                                                                                   if (!mag.FirearmId.Equals(gun.Item.RefId)) {
////                                                                                                       self.World.InsertMessage("Magazine doesn't work with this gun.");
////                                                                                                       return ActionResult.Aborted;
////                                                                                                   }
////                
////                                                                                                   if (gun.Magazine) {
////                                                                                                       //guns contains a magazine already eject it                                                                    
////                //                                                                                       self.AddItem(World.Instance.CreateItem());
////                                                                                                   }
////                                                                                                   self.RemoveItem(mag.Item);
//																				   //                                                                                   gun.Magazine = mag.Item;
//
//																				   //todo factor in speed for quick reload
//																				   self.ActionPoints -= gun.ReloadSpeed;
//
//																				   if (gun.ShotsRemaining > 0) {
//																					   var droppedAmmo = World.Instance.CreateItem(gun.AmmoCaliber);
//																					   droppedAmmo.Amount = gun.ShotsRemaining;
//																					   gun.ShotsRemaining = 0;
//																					   self.Level.AddItem(droppedAmmo, self.Position);
//																				   }
//
////                                                                                   self.Items.Where(item => item.Is(typeof(AmountComponent)))
//
//																				   self.World.AddMessage(String.Format("{0} reloads {1}, dropping all excess bullets", self.Name, gun.Item.Name));
//
//																				   return ActionResult.Success;
//																			   }
//										  });
//
//					#region Old SKR Attacks
//
//					//                case Skill.TargetAttack:
////                    return new Talent(new TalentTemplate()
////                                          {
////                                                  Skill = identifier,
////                                                  Name = "Attack target specific body part.",
////                                                  InitialRank = 1,
////                                                  MaxRank = 1,
////                                                  RequiresTarget = TargetType.Directional,
////                                                  Args = new List<TalentTemplate.GenerateArgsListFunction>()
////                                                             {
////                                                                     delegate(Talent t, Actor self, Point p)
////                                                                         {
////                                                                             List<MeleeComponent> attacks = new List<MeleeComponent>
////                                                                                                                {
////                                                                                                                        self.Characteristics.Kick,
////                                                                                                                        self.Characteristics.Punch,
////                                                                                                                };
////
////                                                                             foreach (var item in from bp in self.BodyParts
////                                                                                                  where self.IsItemEquipped(bp.Type)
////                                                                                                  select self.GetItemAtBodyPart(bp.Type)) {
////                                                                                 if (item.Is(ItemAction.MeleeAttack))
////                                                                                     attacks.Add(item.As<MeleeComponent>(ItemAction.MeleeAttack));
////                                                                                 if (item.Is(ItemAction.MeleeAttack))
////                                                                                     attacks.Add(item.As<MeleeComponent>(ItemAction.MeleeAttack));
////                                                                             }
////
////                                                                             return attacks;
////                                                                         },
////                                                                     delegate(Talent t, Actor self, Point p)
////                                                                         {
////                                                                             if (!self.Level.DoesActorExistAtLocation(p))
////                                                                                 return null;
////
////                                                                             return self.Level.GetActorAtLocation(p).Characteristics.BodyPartsList;
////                                                                         },
////                                                             },
////
////                                                  ArgsDesciptor = new List<TalentTemplate.ArgDesciptorFunction>()
////                                                                      {
////                                                                              delegate(Talent t, Actor self, Point target, dynamic arg)
////                                                                                  {
////                                                                                      var weapon = (MeleeComponent) arg;
////                                                                                      return weapon.Item == null
////                                                                                                     ? weapon.ActionDescription
////                                                                                                     : String.Format("{0} with {1}", weapon.ActionDescription, weapon.Item.Name);
////                                                                                  },
////                                                                              (t, self, target, arg) => arg.ToString(),
////                                                                      },
////
////                                                  ActionOnTargetFunction =
////                                                          delegate(Talent t, Actor self, Point p, dynamic[] args)
////                                                              {
////                                                                  Actor target = self.Level.GetActorAtLocation(p);
////                                                                  if (!(args[0] is MeleeComponent)) {
////                                                                      self.World.InsertMessage("Not a melee weapon.");
////                                                                      return ActionResult.Aborted;
////                                                                  }
////
////                                                                  var melee = args[0] as MeleeComponent;
////
////                                                                  if (!(args[1] is BodyPart)) {
////                                                                      self.World.InsertMessage("Not a body part.");
////                                                                      return ActionResult.Aborted;
////                                                                  }
////
////                                                                  if (self.Position.DistanceTo(target.Position) > (melee.Reach + t.Range + 1)) {
////                                                                      // diagonal attacks are >1, so if weapons have range of 0, we can't use them
////                                                                      self.World.InsertMessage("Too far to attack.");
////                                                                      return ActionResult.Aborted;
////                                                                  }
////
////                                                                  MeleeCombat.AttackMeleeWithWeapon(self, target, melee, args[1], true);
////                                                                  return ActionResult.Success;
////                                                              }
////                                          });
////                case Skill.RangeTargetAttack:
////                    return new Talent(new TalentTemplate()
////                                          {
////                                                  Skill = identifier,
////                                                  Name = "Shoot target specific body part.",
////                                                  InitialRank = 1,
////                                                  MaxRank = 1,
////                                                  RequiresTarget = TargetType.Positional,
////                                                  Args = new List<TalentTemplate.GenerateArgsListFunction>()
////                                                             {
////                                                                     delegate(Talent t, Actor self, Point p)
////                                                                         {
////                                                                             return (from bp in self.BodyParts
////                                                                                     where self.IsItemEquipped(bp.Type)
////                                                                                     select self.GetItemAtBodyPart(bp.Type)
////                                                                                     into item
////                                                                                     where item.Is(ItemAction.Shoot)
////                                                                                     select item.As<FirearmComponent>(ItemAction.Shoot)).ToList();
////                                                                         },
////                                                                     delegate(Talent t, Actor self, Point p)
////                                                                         {
////                                                                             if (!self.Level.DoesActorExistAtLocation(p))
////                                                                                 return null;
////
////                                                                             return self.Level.GetActorAtLocation(p).Characteristics.BodyPartsList;
////                                                                         },
////                                                             },
////
////                                                  ArgsDesciptor = new List<TalentTemplate.ArgDesciptorFunction>()
////                                                                      {
////                                                                              delegate(Talent t, Actor self, Point p, dynamic arg)
////                                                                                  {
////                                                                                      var weapon = (FirearmComponent) arg;
////                                                                                      return weapon.Item == null
////                                                                                                     ? weapon.ActionDescription
////                                                                                                     : String.Format("{0} with {1}", weapon.ActionDescription, weapon.Item.Name);
////                                                                                  },
////                                                                              (t, self, target, arg) => arg.ToString()
////
////                                                                      },
////
////                                                  ActionOnTargetFunction =
////                                                          delegate(Talent t, Actor self, Point p, dynamic[] args)
////                                                              {
////                                                                  Actor target = self.Level.GetActorAtLocation(p);
////
////                                                                  if (!(args[0] is FirearmComponent)) {
////                                                                      self.World.InsertMessage("Not a firearm.");
////                                                                      return ActionResult.Aborted;
////                                                                  }
////
////                                                                  var gun = args[0] as FirearmComponent;
////
////                                                                  if (!(args[1] is BodyPart)) {
////                                                                      self.World.InsertMessage("Not a body part.");
////                                                                      return ActionResult.Aborted;
////                                                                  }
////
////                                                                  if (self.Position.DistanceTo(target.Position) > (gun.Range + t.Range + 1)) {
////                                                                      self.World.InsertMessage("Too far to attack.");
////                                                                      return ActionResult.Aborted;
////                                                                  }
////
////                                                                  if (gun.Magazine == false || gun.Shots <= 0) {
////                                                                      self.World.InsertMessage("You squeeze the trigger only the hear the sound of nothing happening...");
////                                                                      self.ActionPoints -= gun.WeaponSpeed;
////                                                                      return ActionResult.Failed;
////                                                                  }
////
////                                                                  MeleeCombat.AttackRangeWithGun(self, target, gun, args[1], true);
////                                                                  return ActionResult.Success;
////                                                              }
////                                          });
////                case Skill.Reload:
////                    return new Talent(new TalentTemplate
////                                          {
////                                                  Skill = identifier,
////                                                  Name = "Reload firearm",
////                                                  InitialRank = 1,
////                                                  MaxRank = 1,
////                                                  RequiresTarget = TargetType.None,
////                                                  Args = new List<TalentTemplate.GenerateArgsListFunction>()
////                                                             {
////                                                                     delegate(Talent t, Actor self, Point p) // what gun are we reloading, return an empty list should cause optionsprompt to quit
////                                                                         {
////                                                                             var weapons = new List<FirearmComponent>();
////
////                                                                             if (self.IsItemEquipped(BodyPartType.LeftHand) &&
////                                                                                 self.GetItemAtBodyPart(BodyPartType.LeftHand).Is(ItemAction.Shoot))
////                                                                                 weapons.Add(self.GetItemAtBodyPart(BodyPartType.LeftHand).As<FirearmComponent>(ItemAction.Shoot));
////
////                                                                             if (self.IsItemEquipped(BodyPartType.RightHand) &&
////                                                                                 self.GetItemAtBodyPart(BodyPartType.RightHand).Is(ItemAction.Shoot))
////                                                                                 weapons.Add(self.GetItemAtBodyPart(BodyPartType.RightHand).As<FirearmComponent>(ItemAction.Shoot));
////
////                                                                             return weapons;
////                                                                         },
////                                                                     delegate(Talent t, Actor self, Point p)
////                                                                         {
////                                                                             List<MagazineComponent> mags = new List<MagazineComponent>();
////                                                                             foreach (Item i in self.Items)
////                                                                                 if (i.Is(ItemAction.ReloadFirearm))
////                                                                                     mags.Add(i.As<MagazineComponent>(ItemAction.ReloadFirearm));
////                                                                             return mags;
////                                                                         },
////                                                             },
////
////                                                  ArgsDesciptor = new List<TalentTemplate.ArgDesciptorFunction>()
////                                                                      {
////                                                                              (t, self, target, arg) => ((FirearmComponent) arg).Item.Name,
////                                                                              (t, self, target, arg) => ((MagazineComponent) arg).Item.Name,
////
////                                                                      },
////                                                  ActionOnTargetFunction = delegate(Talent t, Actor self, Point p, dynamic[] args)
////                                                                               {
////                                                                                   if (!(args[0] is FirearmComponent)) {
////                                                                                       self.World.InsertMessage("Not a firearm.");
////                                                                                       return ActionResult.Aborted;
////                                                                                   }
////
////                                                                                   var gun = args[0] as FirearmComponent;
////
////                                                                                   if (!(args[1] is MagazineComponent)) {
////                                                                                       self.World.InsertMessage("Not a magazine.");
////                                                                                       return ActionResult.Aborted;
////                                                                                   }
////
////                                                                                   var mag = args[1] as MagazineComponent;
////
////                                                                                   if (!mag.FirearmId.Equals(gun.Item.RefId)) {
////                                                                                       self.World.InsertMessage("Magazine doesn't work with this gun.");
////                                                                                       return ActionResult.Aborted;
////                                                                                   }
////
////                                                                                   if (gun.Magazine) {
////                                                                                       //guns contains a magazine already eject it                                                                    
//////                                                                                       self.AddItem(World.Instance.CreateItem());
////                                                                                   }
////                                                                                   self.RemoveItem(mag.Item);
//////                                                                                   gun.Magazine = mag.Item;
////
////                                                                                   //todo revolvers and shotguns
////                                                                                   self.ActionPoints -= gun.ReloadSpeed;
////
////                                                                                   self.World.InsertMessage(String.Format("{0} reloads {1} with a {2}", self.Name, gun.Item.Name, mag.Item.Name));
////
////                                                                                   return ActionResult.Success;
////                                                                               }
//					//                                          });
//
//					#endregion
//
//				case Skill.UseFeature:
//					return new Talent(new TalentTemplate()
//										  {
//												  Skill = identifier,
//												  Name = "Use feature",
//												  InitialRank = 1,
//												  MaxRank = 1,
//												  RequiresTarget = TargetType.Directional,
//												  Args = new List<TalentTemplate.GenerateArgsListFunction>()
//															 {
//																	 delegate(Talent t, Actor self, Point p) // what gun are we reloading, return an empty list should cause optionsprompt to quit
//																		 {
//																			 var feature = self.Level.GetFeatureAtLocation(p);
//																			 return feature == null ? null : feature.ActiveUsages;
//																		 },
//															 },
//												  ArgsDesciptor = new List<TalentTemplate.ArgDesciptorFunction>()
//																	  {
//																			  (t, self, target, arg) => arg,
//																	  },
//												  ActionOnTargetFunction = delegate(Talent t, Actor self, Point p, dynamic[] args)
//																			   {
//																				   self.Level.GetFeatureAtLocation(p).Use(self, args[0]);
//																				   return ActionResult.Success;
//																			   }
//										  });
//					;
//				case Skill.Brawling:
//					return new Talent(new TalentTemplate()
//										  {
//												  Skill = identifier,
//												  Name = "Brawling",
//												  InitialRank = 0,
//												  MaxRank = 10,
//												  CalculateRealRank = (t, self) => t.RawRank + self.GetTalent(Skill.Agility).RealRank
//										  });
//				case Skill.Sword:
//					return new Talent(new TalentTemplate()
//										  {
//												  Skill = identifier,
//												  Name = "Sword",
//												  InitialRank = 0,
//												  MaxRank = 10,
//												  CalculateRealRank = (t, self) => t.RawRank + self.GetTalent(Skill.Agility).RealRank
//										  });
//				case Skill.Knife:
//					return new Talent(new TalentTemplate()
//										  {
//												  Skill = identifier,
//												  Name = "Knife",
//												  InitialRank = 0,
//												  MaxRank = 10,
//												  CalculateRealRank = (t, self) => t.RawRank + self.GetTalent(Skill.Agility).RealRank
//										  });
//				case Skill.Pistol:
//					return new Talent(new TalentTemplate()
//										  {
//												  Skill = identifier,
//												  Name = "Pistol",
//												  InitialRank = 0,
//												  MaxRank = 10,
//												  CalculateRealRank = (t, self) => t.RawRank + self.GetTalent(Skill.Agility).RealRank
//										  });
//
//					return new Talent(new TalentTemplate()
//										  {
//												  Skill = identifier,
//												  Name = "Better Pistol",
//												  InitialRank = 1,
//												  MaxRank = 1,
//												  
//										  });
//
//				case Skill.Strength:
//					return new Talent(TalentTemplate.CreateAttribute(identifier));
//				case Skill.Agility:
//					return new Talent(TalentTemplate.CreateAttribute(identifier));
//				case Skill.Constitution:
//					return new Talent(TalentTemplate.CreateAttribute(identifier));
//				case Skill.Intellect:
//					return new Talent(TalentTemplate.CreateAttribute(identifier));
//				case Skill.Cunning:
//					return new Talent(TalentTemplate.CreateAttribute(identifier));
//				case Skill.Resolve:
//					return new Talent(TalentTemplate.CreateAttribute(identifier));
//				case Skill.Presence:
//					return new Talent(TalentTemplate.CreateAttribute(identifier));
//				case Skill.Grace:
//					return new Talent(TalentTemplate.CreateAttribute(identifier));
//				case Skill.Composure:
//					return new Talent(TalentTemplate.CreateAttribute(identifier));
//				default:
//					throw new ArgumentOutOfRangeException("identifier");
//			}
//		}

		private void Create(string id, TalentTemplate template) {
			template.RefId = id;
			templates.Add(id, template);
		}


		public override Talent Construct(string identifier) {
			return new Talent(templates[identifier]);
		}
	}
}