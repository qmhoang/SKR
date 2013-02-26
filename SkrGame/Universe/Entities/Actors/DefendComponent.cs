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
using Attribute = SkrGame.Universe.Entities.Stats.Attribute;

namespace SkrGame.Universe.Entities.Actors {
	/// <summary>
	/// Add this to an entity if the entity can be attacked, it contains its defense information
	/// </summary>
	public class DefendComponent : Component {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public class Appendage {
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

			public Appendage(string name, int maxHealth, int relsize, int targettingPenalty, DefendComponent owner = null) {
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

			public Appendage Copy(DefendComponent newOwner = null) {
				return new Appendage(Name, MaxHealth, RelativeSize, TargettingPenalty, newOwner);
			}
		}
		#region Health

		public Attribute Health { get; private set; }

		public bool Dead {
			get { return Health < 0; }
		}

		#endregion

		private List<Appendage> bodyParts;

		public IEnumerable<Appendage> BodyPartsList {
			get { return bodyParts; } 
		}

		public Appendage GetRandomPart() {
			return Rng.ItemWeighted(bodyParts, p => p.RelativeSize);
		}

		public int Dodge {
			get { return 50; }
		}

		private DefendComponent(int health, int maxHealth, List<Appendage> bodyParts) {
			Health = new Attribute("Health", "HP ", maxHealth, health);

			this.bodyParts = bodyParts;
		}

		public DefendComponent(int health, List<Appendage> bodyParts) {
			Contract.Requires<ArgumentException>(bodyParts.Count > 0);
			Health = new Attribute("Health", "HP ", health, health);
			this.bodyParts = bodyParts;

			foreach (var part in bodyParts) {
				part.Owner = this;
			}
		}

		public static DefendComponent CreateHuman(int health) {
			return new DefendComponent(health, new List<Appendage>()
			                               {
			                               		new Appendage("Torso", health, 125, 0),
			                               		new Appendage("Head", health, 15, -World.STANDARD_DEVIATION * 5 / 3),
			                               		new Appendage("Right Arm", health / 2, 25, -World.STANDARD_DEVIATION * 4 / 3),
			                               		new Appendage("Left Arm", health / 2, 25, -World.STANDARD_DEVIATION * 4 / 3),
			                               		new Appendage("Main Hand", health / 3, 5, -World.STANDARD_DEVIATION * 4 / 3),
			                               		new Appendage("Off Hand", health / 3, 5, -World.STANDARD_DEVIATION * 4 / 3),
			                               		new Appendage("Legs", health / 2, 75, -World.STANDARD_DEVIATION * 2 / 3),
			                               		new Appendage("Feet", health / 3, 10, -World.STANDARD_DEVIATION * 2 / 3),
			                               });
		}

		public static DefendComponent SinglePart(int health, string name) {
			return new DefendComponent(health, new List<Appendage>
			                                   {
			                                   		new Appendage(name, health, 1, 0)
			                                   });
		}

		[ContractInvariantMethod]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
		private void ObjectInvariant() {
			Contract.Invariant(Health != null);
			Contract.Invariant(bodyParts != null);
			Contract.Invariant(bodyParts.Count > 0);
		}

		public override Component Copy() {
			var defend = new DefendComponent(Health.Value, Health.MaximumValue, bodyParts.Select(part => part.Copy()).ToList());

			foreach (var part in defend.bodyParts) {
				part.Owner = defend;
			}

			return defend;
		}
	}
}