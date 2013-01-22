using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using DEngine.Actor;
using DEngine.Core;
using DEngine.Entities;
using SkrGame.Gameplay.Combat;
using SkrGame.Gameplay.Talent.Components;
using SkrGame.Universe.Entities.Items.Components;
using log4net;
using log4net.Repository.Hierarchy;

namespace SkrGame.Universe.Entities.Actors {
	/// <summary>
	/// Add this to an entity if the entity can be attacked, it contains its defense information
	/// </summary>
	public class DefendComponent : Component {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public class AttackablePart {
			public string Name { get; private set; }
			public DefendComponent Owner { get; set; }
			
			public int Health { get; set; }
			public int MaxHealth { get; private set; }

			public int RelativeSize { get; private set; }
			public int TargettingPenalty { get; private set; }

			public AttackablePart(string name, int maxHealth, int relsize, int targettingPenalty, DefendComponent owner) {
				Contract.Requires<ArgumentException>(maxHealth > 0);
				Contract.Requires<ArgumentException>(relsize > 0);
				Name = name;
				Health = maxHealth;
				MaxHealth = maxHealth;
				RelativeSize = relsize;
				TargettingPenalty = targettingPenalty;
				Owner = owner;
			}

			public AttackablePart Copy(DefendComponent newOwner = null) {
				return new AttackablePart(Name, MaxHealth, RelativeSize, TargettingPenalty, newOwner);
			}
		}

		private int totalSize;

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

		private List<AttackablePart> bodyParts;

		public IEnumerable<AttackablePart> BodyPartsList {
			get { return bodyParts; } 
		}

		public AttackablePart GetRandomPart() {
			var r = Rng.Int(totalSize);
			int total = 0;
			foreach (var attackablePart in BodyPartsList) {
				if (r <= attackablePart.RelativeSize + total)
					return attackablePart;
				total += attackablePart.RelativeSize;
			}

			throw new Exception("how did we get here?");			
		}

		public int Dodge {
			get { return 50; }
		}


		public DefendComponent() {			
			maxHealth = health = 50;
			bodyParts = new List<AttackablePart>()
			            {
			            		new AttackablePart("Torso", health, 100, 0, this),
			            		new AttackablePart("Head", health, 15, -World.STANDARD_DEVIATION * 5 / 3, this),
			            		new AttackablePart("Right Arm", health / 2, 25, -World.STANDARD_DEVIATION * 4 / 3, this),
			            		new AttackablePart("Left Arm", health / 2, 25, -World.STANDARD_DEVIATION * 4 / 3, this),
			            		new AttackablePart("Main Hand", health / 3, 5, -World.STANDARD_DEVIATION * 4 / 3, this),
			            		new AttackablePart("Off Hand", health / 3, 5, -World.STANDARD_DEVIATION * 4 / 3, this),
			            		new AttackablePart("Leg", health / 2, 75, -World.STANDARD_DEVIATION * 2 / 3, this),
			            		new AttackablePart("Feet", health / 3, 10, -World.STANDARD_DEVIATION * 2 / 3, this),
			            };

			totalSize = bodyParts.Sum(bp => bp.RelativeSize);
		}

		public override Component Copy() {
			var defend = new DefendComponent()
			             {
			             		Health = Health,
			             		MaxHealth = MaxHealth,
								bodyParts = bodyParts.Select(part => part.Copy()).ToList()
			             };

			foreach (var part in defend.bodyParts) {
				part.Owner = defend;
			}

			return defend;
		}
	}
}