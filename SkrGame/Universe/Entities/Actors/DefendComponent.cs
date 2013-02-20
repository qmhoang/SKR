using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using DEngine.Actor;
using DEngine.Core;
using DEngine.Entities;
using SkrGame.Gameplay.Combat;
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

			[ContractInvariantMethod]
			[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
			private void ObjectInvariant() {
				Contract.Invariant(MaxHealth > 0);
				Contract.Invariant(RelativeSize > 0);
			}

			public AttackablePart(string name, int maxHealth, int relsize, int targettingPenalty, DefendComponent owner = null) {
				Contract.Requires<ArgumentException>(maxHealth > 0);
				Contract.Requires<ArgumentException>(relsize > 0);
				Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(name));
				Contract.Requires<ArgumentException>(targettingPenalty <= 0);

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

		public event EventHandler<DefendComponent, EventArgs> HealthChanged;

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
			return Rng.ItemWeighted(bodyParts, p => p.RelativeSize);
		}

		public int Dodge {
			get { return 50; }
		}

		private DefendComponent(int health, int maxHealth, List<AttackablePart> bodyParts) {
			this.health = health;
			this.maxHealth = maxHealth;
			this.bodyParts = bodyParts;
		}

		public DefendComponent(int health, List<AttackablePart> bodyParts) {
			Contract.Requires<ArgumentException>(bodyParts.Count > 0);
			this.health = this.maxHealth = health;
			this.bodyParts = bodyParts;

			foreach (var part in bodyParts) {
				part.Owner = this;
			}
		}

		public static DefendComponent CreateHuman(int health) {
			return new DefendComponent(health, new List<AttackablePart>()
			                               {
			                               		new AttackablePart("Torso", health, 125, 0),
			                               		new AttackablePart("Head", health, 15, -World.STANDARD_DEVIATION * 5 / 3),
			                               		new AttackablePart("Right Arm", health / 2, 25, -World.STANDARD_DEVIATION * 4 / 3),
			                               		new AttackablePart("Left Arm", health / 2, 25, -World.STANDARD_DEVIATION * 4 / 3),
			                               		new AttackablePart("Main Hand", health / 3, 5, -World.STANDARD_DEVIATION * 4 / 3),
			                               		new AttackablePart("Off Hand", health / 3, 5, -World.STANDARD_DEVIATION * 4 / 3),
			                               		new AttackablePart("Leg", health / 2, 75, -World.STANDARD_DEVIATION * 2 / 3),
			                               		new AttackablePart("Feet", health / 3, 10, -World.STANDARD_DEVIATION * 2 / 3),
			                               });
		}

		public static DefendComponent SinglePart(int health, string name) {
			return new DefendComponent(health, new List<AttackablePart>()
			                                   {
			                                   		new AttackablePart(name, health, 1, 0)
			                                   });
		}

		[ContractInvariantMethod]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
		private void ObjectInvariant() {
			Contract.Invariant(health <= maxHealth);
			Contract.Invariant(maxHealth > 0);			
		}

		public override Component Copy() {
			var defend = new DefendComponent(health, maxHealth, bodyParts.Select(part => part.Copy()).ToList());

			foreach (var part in defend.bodyParts) {
				part.Owner = defend;
			}

			return defend;
		}
	}
}