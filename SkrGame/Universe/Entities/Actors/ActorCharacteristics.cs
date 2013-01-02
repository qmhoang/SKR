//using System.Collections.Generic;
//using DEngine.Core;
//using SkrGame.Universe.Entities.Items;
//using SkrGame.Universe.Entities.Items.Components;
//
//namespace SkrGame.Universe.Entities.Actors {
//	public class ActorCharacteristics {
//		private Actor Actor;
//		private Dictionary<BodySlot, BodyPart> BodyParts;
//
//		public int Health { get; set; }
//		public int MaxHealth { get; protected set; }
//
//		public MeleeComponent Punch;
//		public MeleeComponent Kick;
//
//		public int BasicDodge {
//			get { return Actor.GetTalent(Skill.Agility).RealRank + Actor.GetTalent(Skill.Cunning).RealRank; }
//		}
//
//		public int Lift {
//			get { return (Actor.GetTalent(Skill.Strength).RealRank + 10) * (Actor.GetTalent(Skill.Strength).RealRank + 10) * 2; }
//		}
//
//		public ActorCharacteristics(Actor actor) {
//			Actor = actor;
//
//			BodyParts = new Dictionary<BodySlot, BodyPart>
//							{
//									{BodySlot.Body, new BodyPart("Torso", BodySlot.Body, Actor, Health, 0)},
//									{BodySlot.Head, new BodyPart("Head", BodySlot.Head, Actor, Health, -5)},
//									{BodySlot.RightArm, new BodyPart("Right Arm", BodySlot.RightArm, Actor, Health / 2, -2)},
//									{BodySlot.LeftArm, new BodyPart("Left Arm", BodySlot.LeftArm, Actor, Health / 2, -2)},
//									{
//											BodySlot.RightHand,
//											new BodyPart("Right Hand", BodySlot.RightHand, Actor, Health / 3, -4, ItemType.OneHandedWeapon, ItemType.TwoHandedWeapon, ItemType.Shield)
//											},
//									{
//											BodySlot.LeftHand,
//											new BodyPart("Left Hand", BodySlot.LeftHand, Actor, Health / 3, -4, ItemType.OneHandedWeapon, ItemType.TwoHandedWeapon, ItemType.Shield)
//											},
//									{BodySlot.Leg, new BodyPart("Leg", BodySlot.Leg, Actor, Health / 2, -2)},
//									{BodySlot.Feet, new BodyPart("Feet", BodySlot.Feet, Actor, Health / 3, -4)}
//							};
//
//
//
//			Punch = new MeleeComponent(new MeleeComponentTemplate
//										   {
//												   ComponentId = "punch",
//												   ActionDescription = "punch",
//												   ActionDescriptionPlural = "punches",
//												   Skill = Skill.Brawling,
//												   HitBonus = 0,
//												   Damage = Rand.Constant(-1),
//												   DamageType = DamageType.Crush,
//												   Penetration = 1,
//												   WeaponSpeed = 100,
//												   Reach = 0,
//												   Strength = 0,
//												   Parry = 0
//										   });
////            Kick = new MeleeComponent(new MeleeComponentTemplate
////                                          {
////                                              ComponentId = "kick",
////                                              Action = ItemAction.MeleeAttackThrust,
////                                              ActionDescription = "kick",
////                                              ActionDescriptionPlural = "kicks",
////                                              Skill = Skill.Brawling,
////                                              HitBonus = -2,
////                                              Damage = Rand.Constant(1),
////                                              DamageType = DamageType.Crush,
////                                              Penetration = 1,
////                                              WeaponSpeed = 100,
////                                              Reach = 1,
////                                              Strength = 0,
////                                              Parry = -100
////                                          });
//
//			MaxHealth = Health = Actor.GetTalent(Skill.Constitution).RealRank + 10;
//		}
//
//
//		public BodyPart GetBodyPart(BodySlot bp) {
//			return BodyParts[bp];
//		}
//
//		public IEnumerable<BodyPart> BodyPartsList {
//			get { return BodyParts.Values; }
//		}
//	}
//}