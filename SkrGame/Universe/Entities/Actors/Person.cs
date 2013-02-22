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
		
		public abstract int Value { get; set; }
		public abstract int MaximumValue { get; set; }

		public static implicit operator int(Stat s) {
			return s.Value;
		}

		protected Stat(string name) {
			Name = name;			
		}
	}

	public sealed class Attribute : Stat {
		public override int Value { get; set; }
		public override int MaximumValue { get; set; }		

		public Attribute(string name, int maxRank, int rank = 0) : base(name) {
			Value = rank;
			MaximumValue = maxRank;
		}
	}

	public sealed class Skill : Stat {
		public Person Owner { get; set; }
		/// <summary>
		/// raw rank represents this talent's skill, unmodified by anything
		/// </summary>
		public int RawRank { get; private set; }
		/// <summary>
		/// RealRank will call the function supplied in the template to calculate this talent's modified rank, it can be anything
		/// from a simple adding another attribute to it or something much more complicated
		/// </summary>
		public override int Value {
			get { return calculateRealRank(Owner, this); }
			set { RawRank = value; }
		}

		public override int MaximumValue { get; set; }

		private readonly Func<Person, Skill, int> calculateRealRank;

		public Skill(string name, Person owner, int maxRank, int initialRank = 0, Func<Person, Skill, int> calcRealRank = null) : base(name) {
			Owner = owner;
			RawRank = initialRank;
			MaximumValue = maxRank;

			if (calcRealRank == null)
				calculateRealRank = (self, t) => t.RawRank;
			else {
				calculateRealRank = calcRealRank;				
			}
		}
	}


	public class Person : Component {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public Attribute Strength { get; private set; }
		public Attribute Agility { get; private set; }
		public Attribute Constitution { get; private set; }

		public Attribute Intellect { get; private set; }
		public Attribute Cunning { get; private set; }
		public Attribute Resolve { get; private set; }

		public Attribute Presence { get; private set; }
		public Attribute Grace { get; private set; }
		public Attribute Willpower { get; private set; }

		public Attribute Stamina { get; private set; }

		public Attribute Energy { get; private set; } // use with stamina
		public Attribute Food { get; private set; }
		public Attribute Water { get; private set; }
		public Attribute Bladder { get; private set; }
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
			Strength = new Attribute("Strength", World.MEAN, World.MEAN);
			Agility = new Attribute("Agility", World.MEAN, World.MEAN);
			Constitution = new Attribute("Constitution", World.MEAN, World.MEAN);
			Intellect = new Attribute("Intellect", World.MEAN, World.MEAN);
			Cunning = new Attribute("Cunning", World.MEAN, World.MEAN);
			Resolve = new Attribute("Resolve", World.MEAN, World.MEAN);
			Presence = new Attribute("Presence", World.MEAN, World.MEAN);
			Grace = new Attribute("Grace", World.MEAN, World.MEAN);
			Willpower = new Attribute("Willpower", World.MEAN, World.MEAN);

			Stamina = new Attribute("Stamina", World.MEAN, World.MEAN);

			Energy = new Attribute("Energy", World.MEAN, World.MEAN);
			Food = new Attribute("Food", World.MEAN, World.MEAN);
			Water = new Attribute("Water", World.MEAN, World.MEAN);
			Bladder = new Attribute("Bladder", World.MEAN, World.MEAN);


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