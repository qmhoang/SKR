using System;
using System.Collections.Generic;
using System.Reflection;
using DEngine.Actor;
using DEngine.Core;
using DEngine.Entity;
using SkrGame.Gameplay.Combat;
using SkrGame.Gameplay.Talent.Components;
using SkrGame.Universe.Entities.Items.Components;
using log4net;
using log4net.Repository.Hierarchy;

namespace SkrGame.Universe.Entities.Actors {
	/// <summary>
	/// Add this to an entity if the entity can be attacked, it contains its defense information
	/// </summary>
	public class DefendComponent : EntityComponent {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public class AttackablePart {
			public string Name { get; private set; }
			public DefendComponent Owner { get; private set; }
			public int Health { get; set; }
			public int MaxHealth { get; private set; }
			public int TargettingPenalty { get; private set; }

			public AttackablePart(string name, int maxHealth, int targettingPenalty, DefendComponent owner) {
				Name = name;
				Health = maxHealth;
				MaxHealth = maxHealth;
				TargettingPenalty = targettingPenalty;
				Owner = owner;
			}
		}

		#region Health

		private int health;
		public int Health {
			get { return health; }
			set { health = value; OnHealthChange(); }
		}

		private int maxHealth;
		public int MaxHealth {
			get { return maxHealth; }
			set { maxHealth = value; OnHealthChange(); }
		}

		public event EventHandler<EventArgs> HealthChanged;

		protected void OnHealthChange() {
			var handler = HealthChanged;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		public bool Dead {
			get { return Health < 0; }
		}

		public void OnDeath() {
		}

		#endregion

		private readonly List<AttackablePart> bodyParts;

		public IEnumerable<AttackablePart> BodyPartsList {
			get { return bodyParts; } 
		}

		public AttackablePart GetRandomPart() {
//			int roll = Rng.Roll(3, 6);
//
//			Logger.InfoFormat("Random body part roll: {0}", roll);
			//todo unfinished
//			if (roll <= 4)
//				return target.GetBodyPart(BodySlot.Head);
//			if (roll <= 5)
//				return target.GetBodyPart(BodySlot.OffHand);
//			if (roll <= 7)
//				return target.GetBodyPart(BodySlot.Leg); // left leg
//			if (roll <= 8)
//				return target.GetBodyPart(BodySlot.OffArm);
//			if (roll <= 11)
//				return target.GetBodyPart(BodySlot.Torso);
//			if (roll <= 12)
//				return target.GetBodyPart(BodySlot.MainArm);
//			if (roll <= 14)
//				return target.GetBodyPart(BodySlot.Leg); // right leg
//			if (roll <= 15)
//				return target.GetBodyPart(BodySlot.MainHand);
//			if (roll <= 16)
//				return target.GetBodyPart(BodySlot.Feet);
//			else
//				return target.GetBodyPart(BodySlot.Head);

			return bodyParts[Rng.Int(bodyParts.Count)];
		}

		public AttackablePart DefaultPart {
			get { return bodyParts[0]; }
		}

		public int Dodge {
			get { return 50; }
		}


		public DefendComponent() {		
			maxHealth = health = 50;
			bodyParts = new List<AttackablePart>()
			            {
			            		new AttackablePart("Torso", health, 0, this),
			            		new AttackablePart("Head", health, -World.STANDARD_DEVIATION * 5 / 3, this),
			            		new AttackablePart("Right Arm", health / 2, -World.STANDARD_DEVIATION * 4 / 3, this),
			            		new AttackablePart("Left Arm", health / 2, -World.STANDARD_DEVIATION * 4 / 3, this),
			            		new AttackablePart("Main Hand", health / 3, -World.STANDARD_DEVIATION * 4 / 3, this),
			            		new AttackablePart("Off Hand", health / 3, -World.STANDARD_DEVIATION * 4 / 3, this),
			            		new AttackablePart("Leg", health / 2, -World.STANDARD_DEVIATION * 2 / 3, this),
			            		new AttackablePart("Feet", health / 3, -World.STANDARD_DEVIATION * 2 / 3, this),
			            };


		}
	}
}