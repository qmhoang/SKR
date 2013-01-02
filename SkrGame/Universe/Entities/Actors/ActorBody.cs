using System.Collections.Generic;
using DEngine.Core;
using SkrGame.Gameplay.Combat;
using SkrGame.Gameplay.Talent.Components;
using SkrGame.Universe.Entities.Items.Components;

namespace SkrGame.Universe.Entities.Actors {
	public class ActorBody {

		private readonly Dictionary<BodySlot, BodyPart> bodyParts;

		public int Health { get; set; }
		public int MaxHealth { get; set; }

		public MeleeComponent Punch;
		public MeleeComponent Kick;

		public IEnumerable<BodyPart> BodyPartsList {
			get { return bodyParts.Values; }
		}

		public BodyPart GetBodyPart(BodySlot bp) {
			return bodyParts[bp];
		}

		private Actor owner;

		public ActorBody(Actor owner) {
			this.owner = owner;
			MaxHealth = Health = owner.Talents.GetTalent("attrb_constitution").As<AttributeComponent>().Rank;

			bodyParts = new Dictionary<BodySlot, BodyPart>
			            {
			            		{BodySlot.Torso, new BodyPart("Torso", BodySlot.Torso, Health, 0)},
			            		{BodySlot.Head, new BodyPart("Head", BodySlot.Head, Health, -World.STANDARD_DEVIATION * 5 / 3)},
			            		{BodySlot.MainArm, new BodyPart("Right Arm", BodySlot.MainArm, Health / 2, -World.STANDARD_DEVIATION * 4 / 3)},
			            		{BodySlot.OffArm, new BodyPart("Left Arm", BodySlot.OffArm, Health / 2, -World.STANDARD_DEVIATION * 4 / 3)},
			            		{BodySlot.MainHand, new BodyPart("Main Hand", BodySlot.MainHand, Health / 3, -World.STANDARD_DEVIATION * 4 / 3)},
			            		{BodySlot.OffHand,new BodyPart("Off Hand", BodySlot.OffHand, Health / 3, -World.STANDARD_DEVIATION * 4 / 3)},
			            		{BodySlot.Leg, new BodyPart("Leg", BodySlot.Leg, Health / 2, -2)},
			            		{BodySlot.Feet, new BodyPart("Feet", BodySlot.Feet, Health / 3, -4)}
			            };


			Punch = new MeleeComponent(null, new MeleeComponentTemplate
			                                 {
			                                 		ComponentId = "punch",
			                                 		ActionDescription = "punch",
			                                 		ActionDescriptionPlural = "punches",
			                                 		Skill = "skill_unarmed",
			                                 		HitBonus = 0,
			                                 		Damage = Rand.Constant(-5),
			                                 		DamageType = Combat.DamageTypes["crush"],
			                                 		Penetration = 1,
			                                 		WeaponSpeed = 100,
			                                 		Reach = 0,
			                                 		Strength = 0,
			                                 		Parry = 0
			                                 });


		}
	}
}