//using System;
//using System.Collections.Generic;
//using System.Linq;
//using DEngine.Actor;
//using DEngine.Components;
//using DEngine.Core;
//using DEngine.Entity;
//using SkrGame.Core;
//using SkrGame.Gameplay.Combat;
//using SkrGame.Gameplay.Talent;
//using SkrGame.Gameplay.Talent.Components;
//using SkrGame.Universe.Entities.Actors;
//using SkrGame.Universe.Entities.Items;
//using SkrGame.Universe.Entities.Items.Components;
//using SkrGame.Universe.Locations;
//
//namespace SkrGame.Universe.Factories {
//	public abstract class TalentFactory : Factory<string, Talent> {}
//
//	public class SourceTalentFactory : TalentFactory {
//		private Dictionary<string, TalentTemplate> templates;
//		private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
//
//		public SourceTalentFactory() {
//			templates = new Dictionary<string, TalentTemplate>();
//
//			LoadAttributes();
//			LoadSkills();
//			LoadBasicTalents();
//		}
//
//		private void LoadAttributes() {
//			CreateAttribute("attrb_strength", "Strength");
//			CreateAttribute("attrb_agility", "Agility");
//			CreateAttribute("attrb_constitution", "Constitution");
//			CreateAttribute("attrb_intellect", "Intellect");
//			CreateAttribute("attrb_cunning", "Cunning");
//			CreateAttribute("attrb_resolve", "Resolve");
//			CreateAttribute("attrb_presence", "Presence");
//			CreateAttribute("attrb_grace", "Grace");
//			CreateAttribute("attrb_composure", "Composure");
//		}
//
//		private static ActionResult MeleeAttack(ActiveTalentComponent t, Entity user, Point targettedPosition, dynamic[] args) {
//			if (!user.Is<Actor>())
//				throw new ArgumentException("non actor entities cannot attack", "user");
//			if (!user.Is<ActionPoint>())
//				throw new ArgumentException("entity cannot act", "user");
//
//			var attacker = user.As<Actor>();
//			var actorLocation = user.As<Location>();
//
//			MeleeComponent weapon = (MeleeComponent) args[0] ?? attacker.DefaultAttack;
//
//			if (actorLocation.Position.DistanceTo(targettedPosition) > (weapon.Reach + t.Range + 1)) {
//				World.Instance.AddMessage("Too far to attack.");
//				return ActionResult.Aborted;
//			}
//
//			if (!(args[1] is BodyPart)) {
//				World.Instance.AddMessage("Not targetting a body part.");
//				return ActionResult.Aborted;
//			}
//
//			var hitBonusFromSkill = attacker.GetTalent(weapon.Skill).As<SkillComponent>().Rank;
//
//			var possibleTargetsAtLocation = PossibleTargetsAtLocation(targettedPosition, actorLocation);
//
//			BodyPart bodyPartTargetted = (BodyPart) args[1];
//
//			// todo fix hack
//			var defender = possibleTargetsAtLocation.First().As<Actor>();
//			var result = Combat.Attack(attacker, defender, hitBonusFromSkill - World.MEAN);
//
//			if (result == CombatEventResult.Hit) {
//				var damage =
//						Math.Max(Combat.GetStrengthDamage(attacker.GetTalent("attrb_strength").As<AttributeComponent>().Rank).Roll() + weapon.Damage.Roll(),
//						         1);
//				int damageResistance, realDamage;
//
//				Combat.Damage(damage, weapon.DamageType, bodyPartTargetted, out damageResistance, out realDamage);
//
//				World.Instance.AddMessage(String.Format("{0} {1} {2}'s {3}.... and inflict {4} wounds.",
//				                                        attacker.Name, weapon.ActionDescriptionPlural, defender.Name, bodyPartTargetted.Name, "todo-description"));
//
//
//				Combat.ProcessCombat(new CombatEventArgs(attacker, defender, bodyPartTargetted, CombatEventResult.Hit, damage,
//				                                         damageResistance, realDamage));
//			} else if (result == CombatEventResult.Miss) {
//				World.Instance.AddMessage(String.Format("{0} {1} {2}'s {3}.... and misses.",
//				                                        attacker.Name, weapon.ActionDescriptionPlural, defender.Name, bodyPartTargetted.Name));
//
//				Combat.ProcessCombat(new CombatEventArgs(attacker, defender, bodyPartTargetted));
//			} else if (result == CombatEventResult.Dodge) {
//				World.Instance.AddMessage(String.Format("{0} {1} {2}'s {3}.... and {2} dodges.",
//				                                        attacker.Name, weapon.ActionDescriptionPlural, defender.Name, bodyPartTargetted.Name));
//
//				Combat.ProcessCombat(new CombatEventArgs(attacker, defender, bodyPartTargetted, CombatEventResult.Dodge));
//			}
//
//			user.As<ActionPoint>().ActionPoints -= weapon.APToAttack;
//			return ActionResult.Success;
//		}
//
//		private static IEnumerable<Entity> PossibleTargetsAtLocation(Point targettedPosition, Location actorLocation) {
//			return ((Level) actorLocation.Level).EntityManager.Where(e => FindActorsAtLocation(e, targettedPosition));
//		}
//
//		private static ActionResult AttackRange(ActiveTalentComponent t, Entity user, Point targettedPosition, dynamic[] args) {
//			if (!user.Is<Actor>())
//				throw new ArgumentException("non actor entities cannot attack", "user");
//			if (!user.Is<ActionPoint>())
//				throw new ArgumentException("entity cannot act", "user");
//
//			if (!(args[0] is RangeComponent)) {
//				World.Instance.AddMessage("Not a range weapon.");
//				return ActionResult.Aborted;
//			}
//
//			var attacker = user.As<Actor>();
//			var attackerLocation = user.As<Location>();
//
//			var weapon = (RangeComponent) args[0];
//
//			bool attackingSpecificActor = true;
//			// if there's no bodyPart given, we're attacking a location
//			if (args[1] == null)
//				attackingSpecificActor = false;
//			else if (!(args[1] is BodyPart)) {
//				World.Instance.AddMessage("Not a body part.");
//				return ActionResult.Aborted;
//			}
//
//			if (attackerLocation.DistanceTo(targettedPosition) > (weapon.Range + t.Range + 1)) {
//				World.Instance.AddMessage("Too far to attack.");
//				return ActionResult.Aborted;
//			}
//
//			if (weapon.ShotsRemaining <= 0) {
//				World.Instance.AddMessage(String.Format("{0} attempts to use the {1} only to realize the weapon is not loaded",
//				                                        attacker.Name, weapon.Item.Name));
//				user.As<ActionPoint>().ActionPoints -= weapon.APToAttack;
//				return ActionResult.Failed;
//			}
//
//			weapon.ShotsRemaining--;
//			user.As<ActionPoint>().ActionPoints -= weapon.APToAttack;
//
//			var hitBonusFromSkill = attacker.GetTalent(weapon.Skill).As<SkillComponent>().Rank - World.MEAN;
//
//			var locationsOnPath = Combat.GetTargetsOnPath(attackerLocation.Position, targettedPosition).ToList();
//
//
//			for (int i = 0; i < locationsOnPath.Count; i++) {
//				var point = locationsOnPath[i];
//				var possibleTargetsAtLocation = PossibleTargetsAtLocation(point, attackerLocation);
//
//				// todo fix hack
//				var defenderEntity = possibleTargetsAtLocation.First();
//				var defender = defenderEntity.As<Actor>();
//				var defenderLocation = defenderEntity.As<Location>();
//
//				BodyPart bodyPartTargetted;
//
//				if (attackingSpecificActor)
//					bodyPartTargetted = (BodyPart) args[1];
//				else
//					bodyPartTargetted = Combat.GetRandomBodyPart(defenderEntity.As<DefendComponent>());
//
//
//				double range = defenderLocation.DistanceTo(attackerLocation) * World.TILE_LENGTH;
//				double rangePenalty = Math.Min(0,
//				                               -World.STANDARD_DEVIATION * Combat.RANGE_PENALTY_STD_DEV_MULT * Math.Log(range) + World.STANDARD_DEVIATION * 2 / 3);
//				Logger.InfoFormat("Target: {2}, range to target: {0}, penalty: {1}", range, rangePenalty, defender.Name);
//
//				// not being targetted gives a sigma (std dev) penalty
//				rangePenalty -= targettedPosition == defenderLocation.Position ? 0 : World.STANDARD_DEVIATION;
//
//				double difficultyOfShot = hitBonusFromSkill + rangePenalty + i * Combat.RANGE_PENALTY_TILE_OCCUPIED;
//				Logger.InfoFormat("Shot difficulty: {0}, targetting penalty: {1}, weapon bonus: {2}, is target: {3}",
//				                  difficultyOfShot, bodyPartTargetted.TargettingPenalty, hitBonusFromSkill,
//				                  targettedPosition == defenderLocation.Position);
//
//				var result = Combat.Attack(attacker, defender, hitBonusFromSkill - World.MEAN);
//
//
//				if (result == CombatEventResult.Hit) {
//					var damage =
//							Math.Max(Combat.GetStrengthDamage(attacker.GetTalent("attrb_strength").As<AttributeComponent>().Rank).Roll() + weapon.Damage.Roll(),
//							         1);
//					int damageResistance, realDamage;
//
//					Combat.Damage(damage, weapon.DamageType, bodyPartTargetted, out damageResistance, out realDamage);
//
//					World.Instance.AddMessage(String.Format("{0} {1} {2}'s {3}.... and inflict {4} wounds.",
//					                                        attacker.Name, weapon.ActionDescriptionPlural, defender.Name, bodyPartTargetted.Name, "todo-description"));
//
//
//					Combat.ProcessCombat(new CombatEventArgs(attacker, defender, bodyPartTargetted, CombatEventResult.Hit, damage,
//					                                         damageResistance, realDamage));
//				} else if (result == CombatEventResult.Miss) {
//					if (point == targettedPosition) // if this is where the actor targetted
//						World.Instance.AddMessage(String.Format("{0} {1} {2}'s {3}.... and misses.",
//						                                        attacker.Name, weapon.ActionDescriptionPlural, defender.Name, bodyPartTargetted.Name));
//
//					Combat.ProcessCombat(new CombatEventArgs(attacker, defender, bodyPartTargetted));
//				} else if (result == CombatEventResult.Dodge) {
//					if (point == targettedPosition) // if this is where the actor targetted
//						World.Instance.AddMessage(String.Format("{0} {1} {2}'s {3}.... and {2} dodges.",
//						                                        attacker.Name, weapon.ActionDescriptionPlural, defender.Name, bodyPartTargetted.Name));
//
//					Combat.ProcessCombat(new CombatEventArgs(attacker, defender, bodyPartTargetted, CombatEventResult.Dodge));
//				}
//
//				return ActionResult.Success;
//			}
//
//			// todo drop ammo casing
//
//			World.Instance.AddMessage(String.Format("{0} {1} and hits nothing", attacker.Name, weapon.ActionDescriptionPlural));
//			return ActionResult.Failed;
//		}
//
//		private static bool FindActorsAtLocation(Entity e, Point targetLocation) {
//			return e.Is<Actor>() && e.Is<Location>() && e.As<Location>().Position == targetLocation;
//		}
//
//		private void LoadBasicTalents() {
//			Create("action_attack",
//			       new TalentTemplate
//			       {
//			       		Name = "Attack",
//			       		Components =
//			       				new List<TalentComponentTemplate>
//			       				{
//			       						new ActiveTalentTemplate
//			       						{
//			       								InitialRank = 1,
//			       								MaxRank = 1,
//			       								RequiresTarget = TargetType.Directional,
//			       								ActionOnTargetFunction = MeleeAttack
//			       						}
//			       				},
//			       });
//			Create("action_range",
//			       new TalentTemplate
//			       {
//			       		Name = "Shoot target.",
//			       		Components =
//			       				new List<TalentComponentTemplate>
//			       				{
//			       						new ActiveTalentTemplate
//			       						{
//			       								InitialRank = 1,
//			       								MaxRank = 1,
//			       								RequiresTarget = TargetType.Positional,
//			       								Args = new List<ActiveTalentArgTemplate>
//			       								       {
//			       								       		new ActiveTalentArgTemplate
//			       								       		{
//			       								       				ArgFunction =
//			       								       						delegate(ActiveTalentComponent t, Entity user, Point p)
//			       								       						{
//			       								       							if (!user.Is<DefendComponent>())
//			       								       								throw new ArgumentException("user eneity doesn't have a body to select weapon", "user");
//
//			       								       							List<dynamic> guns = new List<dynamic>();
//			       								       							if (user.As<DefendComponent>().GetBodyPart(BodySlot.MainHand).Equipped) {
//			       								       								var item = user.As<DefendComponent>().GetBodyPart(BodySlot.MainHand).Item;
//			       								       								if (item.Is(typeof (RangeComponent)))
//			       								       									guns.Add(item.As<RangeComponent>());
//			       								       							} else if (user.As<DefendComponent>().GetBodyPart(BodySlot.OffHand).Equipped) {
//			       								       								var item = user.As<DefendComponent>().GetBodyPart(BodySlot.OffHand).Item;
//			       								       								if (item.Is(typeof (RangeComponent)))
//			       								       									guns.Add(item.As<RangeComponent>());
//			       								       							}
//
//			       								       							// todo shoot both guns at the same time
//			       								       							return guns;
//			       								       						},
//			       								       				ArgDesciptor =
//			       								       						delegate(ActiveTalentComponent t, Entity user, Point p, dynamic arg)
//			       								       						{																				
//			       								       							var weapon = (RangeComponent) arg;
//			       								       							return weapon.Item == null
//			       								       							       		? weapon.ActionDescription
//			       								       							       		: String.Format("{0} with {1}", weapon.ActionDescription, weapon.Item.Name);
//			       								       						}
//			       								       		},
//			       								       		new ActiveTalentArgTemplate
//			       								       		{
//			       								       				ArgFunction = delegate(ActiveTalentComponent t, Entity user, Point p)
//			       								       				              {
//			       								       				              	if (PossibleTargetsAtLocation(p, user.As<Location>()).Where(e => e.Is<DefendComponent>()).Count() == 0)
//			       								       				              		return null;
//
//			       								       				              	return PossibleTargetsAtLocation(p, user.As<Location>()).Where(e => e.Is<DefendComponent>()).First().As<DefendComponent>().BodyPartsList;
//			       								       				              },
//			       								       				ArgDesciptor = (t, user, target, arg) => arg.ToString(),
//			       								       				Required = false,
//			       								       				PromptDescription = "Shoot at which body part?"
//			       								       		}
//			       								       },
//			       								ActionOnTargetFunction = AttackRange
//			       						}
//			       				},
//			       });
//			Create("action_reload",
//			       new TalentTemplate
//			       {
//			       		Name = "Reload weapon",
//			       		Components =
//			       				new List<TalentComponentTemplate>
//			       				{
//			       						new ActiveTalentTemplate
//			       						{
//			       								InitialRank = 1,
//			       								MaxRank = 1,
//			       								RequiresTarget = TargetType.None,
//			       								Args = new List<ActiveTalentArgTemplate>
//			       								       {
//			       								       		new ActiveTalentArgTemplate
//			       								       		{
//			       								       				ArgFunction = delegate(ActiveTalentComponent t, Entity user, Point p)
//			       								       								// what gun are we reloading, return an empty list should cause optionsprompt to quit
//			       								       				              {
//			       								       				              	if (!user.Is<DefendComponent>())
//			       								       				              		throw new ArgumentException("user eneity doesn't have a body to select weapon", "user");
//
//			       								       				              	List<dynamic> guns = new List<dynamic>();
//			       								       				              	if (user.As<DefendComponent>().GetBodyPart(BodySlot.MainHand).Equipped) {
//			       								       				              		var item = user.As<DefendComponent>().GetBodyPart(BodySlot.MainHand).Item;
//			       								       				              		if (item.Is(typeof (RangeComponent)))
//			       								       				              			guns.Add(item.As<RangeComponent>());
//			       								       				              	} else if (user.As<DefendComponent>().GetBodyPart(BodySlot.OffHand).Equipped) {
//			       								       				              		var item = user.As<DefendComponent>().GetBodyPart(BodySlot.OffHand).Item;
//			       								       				              		if (item.Is(typeof (RangeComponent)))
//			       								       				              			guns.Add(item.As<RangeComponent>());
//			       								       				              	}
//
//			       								       				              	// todo shoot both guns at the same time
//			       								       				              	return guns;
//			       								       				              },
//			       								       				ArgDesciptor = (t, user, target, arg) => ((RangeComponent) arg).Item.Name,
//			       								       		},
//			       								       		new ActiveTalentArgTemplate
//			       								       		{
//																
////			       								       				ArgFunction = delegate(ActiveTalentComponent t, Entity user, Point p)
////			       								       				              {
////			       								       				              	List<AmmoComponent> list = new List<AmmoComponent>();
////			       								       				              	foreach (Item item in user.As<Actor>().Items)
////			       								       				              		if (item.Is(typeof (AmmoComponent))) {
////			       								       				              			var ammo = item.As<AmmoComponent>();
////			       								       				              			list.Add(item.As<AmmoComponent>());
////			       								       				              		}
////			       								       				              	return list;
////			       								       				              },
//			       								       				ArgDesciptor = (t, user, target, arg) => ((AmmoComponent) arg).Item.Name,
//			       								       		}
//			       								       },
////			       								ActionOnTargetFunction =
////			       										delegate(ActiveTalentComponent t, Entity user, Point p, dynamic[] args)
////			       										{
////			       											if (!(args[0] is RangeComponent)) {
////			       												World.Instance.AddMessage("Not a range weapon.", MessageType.High);
////			       												return ActionResult.Aborted;
////			       											}
////
////			       											var weapon = (RangeComponent) args[0];
////
////			       											if (!(args[1] is AmmoComponent)) {
////			       												World.Instance.AddMessage("Not ammo.", MessageType.High);
////			       												return ActionResult.Aborted;
////			       											}
////
////			       											var ammo = (AmmoComponent) args[1];
////
////			       											if (!ammo.Type.Equals(weapon.AmmoType)) {
////			       												World.Instance.AddMessage("Wrong ammo type for this weapon.", MessageType.High);
////			       												return ActionResult.Aborted;
////			       											}
////
////			       											// todo give arrow back when replacing new ammo
////			       											if (weapon.ShotsRemaining >= weapon.Shots) {
////			       												World.Instance.AddMessage("Weapon is already fully loaded.");
////			       												return ActionResult.Aborted;
////			       											}
////
////			       											// todo revolvers and single load weapons
////
////
////			       											// to semi-simulate dropping magazines, drop remaining bullets on the ground
////			       											if (weapon.ShotsRemaining > 0) {
////			       												var droppedAmmo = World.Instance.CreateItem(weapon.AmmoType);
////			       												droppedAmmo.Amount = weapon.ShotsRemaining;
////			       												weapon.ShotsRemaining = 0;
////			       												user.Level.AddItem(droppedAmmo, user.Position);
////			       												World.Instance.AddMessage(String.Format("{0} reloads {1} with {2}, dropping all excess ammo.", user.Name, weapon.Item.Name,
////			       												                                        ammo.Item.Name));
////			       											} else
////			       												World.Instance.AddMessage(String.Format("{0} reloads {1} with {2}.", user.Name, weapon.Item.Name,
////			       												                                        ammo.Item.Name));
////
////			       											if (ammo.Item.StackType == StackType.Hard && ammo.Item.Amount >= weapon.Shots) {
////			       												ammo.Item.Amount -= weapon.Shots;
////			       												weapon.ShotsRemaining = weapon.Shots;
////			       											} else if (ammo.Item.StackType == StackType.Hard && ammo.Item.Amount > 0) {
////			       												weapon.ShotsRemaining = ammo.Item.Amount;
////			       												user.RemoveItem(ammo.Item);
////			       											}
////
////			       											user.ActionPoints -= weapon.APToReload;
////
////			       											return ActionResult.Success;
////			       										}
//			       						}
//			       				},
//			       });
//
////			Create("action_activate",
////			       new TalentTemplate
////			       {
////			       		Name = "Activate feature",
////			       		Components =
////			       				new List<TalentComponentTemplate>
////			       				{
////			       						new ActiveTalentTemplate
////			       						{
////			       								InitialRank = 1,
////			       								MaxRank = 1,
////			       								RequiresTarget = TargetType.Directional,
////			       								Args = new List<ActiveTalentArgTemplate>
////			       								       {
////			       								       		new ActiveTalentArgTemplate
////			       								       		{
////			       								       				ArgFunction = delegate(ActiveTalentComponent t, Actor user, Point p)
////			       								       				              {
////			       								       				              	var feature = user.Level.GetFeatureAtLocation(p);
////			       								       				              	return feature == null ? null : feature.ActiveUsages;
////			       								       				              },
////			       								       				ArgDesciptor = (t, user, target, arg) => arg,
////			       								       		},
////			       								       },
////			       								ActionOnTargetFunction =
////			       										delegate(ActiveTalentComponent t, Actor user, Point p, dynamic[] args)
////			       										{
////			       											user.Level.GetFeatureAtLocation(p).Use(user, args[0]);
////			       											return ActionResult.Success;
////			       										}
////			       						}
////			       				},
////			       });
//		}
//
//		private void CreateAttribute(string id, string name) {
//			Create(id, new TalentTemplate()
//			           {
//			           		Name = name,
//			           		Components = new List<TalentComponentTemplate>
//			           		             {
//			           		             		new AttributeComponentTemplate
//			           		             		{
//			           		             				InitialRank = 0,
//			           		             				MaxRank = 100
//			           		             		}
//			           		             }
//			           });
//		}
//
//		private void LoadSkills() {
//			Create("skill_unarmed",
//			       new TalentTemplate
//			       {
//			       		Name = "Brawling",
//			       		Components = new List<TalentComponentTemplate>
//			       		             {
//			       		             		new SkillComponentTemplate
//			       		             		{
//			       		             				InitialRank = 0,
//			       		             				MaxRank = 10,
//			       		             				CalculateRealRank = (t, user) => t.RawRank + user.GetTalent("attrb_agility").As<AttributeComponent>().Rank
//			       		             		}
//			       		             }
//			       });
//			Create("skill_sword",
//			       new TalentTemplate
//			       {
//			       		Name = "Sword",
//			       		Components = new List<TalentComponentTemplate>
//			       		             {
//			       		             		new SkillComponentTemplate
//			       		             		{
//			       		             				InitialRank = 0,
//			       		             				MaxRank = 10,
//			       		             				CalculateRealRank = (t, user) => t.RawRank + user.GetTalent("attrb_agility").As<AttributeComponent>().Rank
//			       		             		},
//			       		             }
//			       });
//			Create("skill_knife",
//			       new TalentTemplate
//			       {
//			       		Name = "Knife",
//			       		Components = new List<TalentComponentTemplate>
//			       		             {
//			       		             		new SkillComponentTemplate
//			       		             		{
//			       		             				InitialRank = 0,
//			       		             				MaxRank = 10,
//			       		             				CalculateRealRank = (t, user) => t.RawRank + user.GetTalent("attrb_agility").As<AttributeComponent>().Rank
//			       		             		}
//			       		             }
//			       });
//			Create("skill_pistol",
//			       new TalentTemplate
//			       {
//			       		Name = "Pistol",
//			       		Components = new List<TalentComponentTemplate>
//			       		             {
//			       		             		new SkillComponentTemplate
//			       		             		{
//			       		             				InitialRank = 0,
//			       		             				MaxRank = 10,
//			       		             				CalculateRealRank = (t, user) => t.RawRank + user.GetTalent("attrb_agility").As<AttributeComponent>().Rank
//			       		             		}
//			       		             }
//			       });
//			Create("skill_bow",
//			       new TalentTemplate
//			       {
//			       		Name = "Bow",
//			       		Components = new List<TalentComponentTemplate>
//			       		             {
//			       		             		new SkillComponentTemplate
//			       		             		{
//			       		             				InitialRank = 0,
//			       		             				MaxRank = 10,
//			       		             				CalculateRealRank = (t, user) => t.RawRank + user.GetTalent("attrb_agility").As<AttributeComponent>().Rank
//			       		             		}
//			       		             }
//			       });
//		}
//
//		private void Create(string id, TalentTemplate template) {
//			template.RefId = id;
//			templates.Add(id, template);
//		}
//
//
//		public override Talent Construct(string identifier) {
//			return new Talent(templates[identifier]);
//		}
//	}
//}