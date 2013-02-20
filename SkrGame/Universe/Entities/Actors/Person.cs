using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using SkrGame.Gameplay.Combat;
using SkrGame.Universe.Entities.Features;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Locations;
using log4net;


namespace SkrGame.Universe.Entities.Actors {
	public enum Condition {
		Encrumbrance, // 0 - none, 1 - light, 2 - medium, etc
	}

	public abstract class Stat {
		public string Name { get; set; }
		public Person Owner { get; set; }
		public abstract int Rank { get; set; }
		public abstract int MaxRank { get; set; }

		protected Stat(string name, Person owner) {
			Name = name;
			Owner = owner;
		}
	}

	public sealed class Attribute : Stat {
		public override int Rank { get; set; }
		public override int MaxRank { get; set; }

		public Attribute(string name, Person owner, int maxRank, int rank = 0) : base(name, owner) {
			Rank = rank;
			MaxRank = maxRank;
		}
	}

	public sealed class Skill : Stat {
		/// <summary>
		/// raw rank represents this talent's skill, unmodified by anything
		/// </summary>
		public int RawRank { get; private set; }
		/// <summary>
		/// RealRank will call the function supplied in the template to calculate this talent's modified rank, it can be anything
		/// from a simple adding another attribute to it or something much more complicated
		/// </summary>
		public override int Rank {
			get { return calculateRealRank(Owner, this); }
			set { RawRank = value; }
		}

		public override int MaxRank { get; set; }

		private readonly Func<Person, Skill, int> calculateRealRank;

		public Skill(string name, Person owner, int maxRank, int initialRank = 0, Func<Person, Skill, int> calcRealRank = null) : base(name, owner) {
			Owner = owner;
			RawRank = initialRank;
			MaxRank = maxRank;

			if (calcRealRank == null)
				calculateRealRank = (self, t) => t.RawRank;
			else {
				calculateRealRank = calcRealRank;				
			}
		}
	}


	public class Person : Component {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public int Strength { get; set; }
		public int Agility { get; set; }
		public int Constitution { get; set; }

		public int Intellect { get; set; }
		public int Cunning { get; set; }
		public int Resolve { get; set; }

		public int Presence { get; set; }
		public int Grace { get; set; }
		public int Willpower { get; set; }

		public int Energy { get; set; }
		public int Food { get; set; }
		public int Water { get; set; }
		public int Bladder { get; set; }
		// social?  - composure will replace for player
		// environment - probably not necessary
		// fun - composure replaces
		// cleanliness

		private readonly Dictionary<string, Skill> skills;
		
		public int Lift {
			get { return Strength * Strength * 18 * (int) Math.Pow(World.STANDARD_DEVIATION, -2.0); }
		}

		public Skill GetSkill(string skill) {
			return skills[skill];
		}

		public Person() {
			Strength = Agility = Constitution = Intellect = Cunning = Resolve = Presence = Grace = Willpower = World.MEAN;		
			skills = new Dictionary<string, Skill>
			         {
			         		{"skill_unarmed", new Skill("Unarmed", this, 100, 0, (user, t) => t.Owner.Agility + t.RawRank)},
							{"skill_pistol", new Skill("Pistol", this, 100, 0, (user, t) => t.Owner.Agility + t.RawRank)},
							{"skill_knife", new Skill("Knife", this, 100, 0, (user, t) => t.Owner.Agility + t.RawRank)},
							{"skill_axe", new Skill("Axe", this, 100, 0, (user, t) => t.Owner.Agility + t.RawRank)},
							{"skill_lockpicking", new Skill("Lockpicking", this, 100, 0, (user, t) => t.Owner.Intellect + t.RawRank)},
			         };

		}


		public override Component Copy() {
			//todo
			return new Person();
		}


	}
}