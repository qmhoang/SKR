using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using DEngine.Actor;
using DEngine.Core;
using DEngine.Entities;
using DEngine.Random;
using SkrGame.Gameplay.Combat;
using log4net;
using log4net.Repository.Hierarchy;
using Attribute = SkrGame.Universe.Entities.Stats.Attribute;

namespace SkrGame.Universe.Entities.Actors {
	/// <summary>
	/// Add this to an entity if the entity can be attacked, it contains its defense information
	/// </summary>
	public sealed class BodyComponent : Component {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public class Appendage {
			public string Name { get; private set; }
			public BodyComponent Owner { get; set; }
			
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

			public Appendage(string name, int maxHealth, int relsize, int targettingPenalty, BodyComponent owner = null) {
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

			public Appendage Copy(BodyComponent newOwner = null) {
				return new Appendage(Name, MaxHealth, RelativeSize, TargettingPenalty, newOwner);
			}
		}

		#region Health

		public Attribute Health { get; set; }

		public bool Dead {
			get { return Health < 0; }
		}

		#endregion

		private List<Appendage> _bodyParts;

		public IEnumerable<Appendage> BodyPartsList {
			get { return _bodyParts; } 
		}

		public Appendage GetRandomPart() {
			return Rng.ItemWeighted(_bodyParts, p => p.RelativeSize);
		}

		public int Dodge {
			get { return 50; }
		}

		private BodyComponent(int health, int maxHealth, List<Appendage> bodyParts) {
			Health = new Attribute("Health", "HP ", maxHealth, health);

			this._bodyParts = bodyParts;
		}

		public BodyComponent(int health, List<Appendage> bodyParts) {
			Contract.Requires<ArgumentException>(bodyParts.Count > 0);
			Health = new Attribute("Health", "HP ", health, health);
			this._bodyParts = bodyParts;

			foreach (var part in bodyParts) {
				part.Owner = this;
			}
		}

		public static BodyComponent CreateHuman(int health) {
			return new BodyComponent(health, new List<Appendage>()
			                               {
			                               		new Appendage("Torso", health, 125, 0),
			                               		new Appendage("Head", health, 15, -World.StandardDeviation * 5 / 3),
			                               		new Appendage("Right Arm", health / 2, 25, -World.StandardDeviation * 4 / 3),
			                               		new Appendage("Left Arm", health / 2, 25, -World.StandardDeviation * 4 / 3),
			                               		new Appendage("Main Hand", health / 3, 5, -World.StandardDeviation * 4 / 3),
			                               		new Appendage("Off Hand", health / 3, 5, -World.StandardDeviation * 4 / 3),
			                               		new Appendage("Legs", health / 2, 75, -World.StandardDeviation * 2 / 3),
			                               		new Appendage("Feet", health / 3, 10, -World.StandardDeviation * 2 / 3),
			                               });
		}

		public static BodyComponent SinglePart(int health, string name) {
			return new BodyComponent(health, new List<Appendage>
			                                   {
			                                   		new Appendage(name, health, 1, 0)
			                                   });
		}

		[ContractInvariantMethod]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
		private void ObjectInvariant() {
			Contract.Invariant(Health != null);
			Contract.Invariant(_bodyParts != null);
			Contract.Invariant(_bodyParts.Count > 0);
		}

		public override Component Copy() {
			var defend = new BodyComponent(Health.Value, Health.MaximumValue, _bodyParts.Select(part => part.Copy()).ToList());

			foreach (var part in defend._bodyParts) {
				part.Owner = defend;
			}

			return defend;
		}
	}
}