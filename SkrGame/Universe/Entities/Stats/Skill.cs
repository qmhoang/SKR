using System;
using SkrGame.Universe.Entities.Actors;

namespace SkrGame.Universe.Entities.Stats {
	public sealed class Skill : Stat {
		public Person Owner { get; set; }
		/// <summary>
		/// raw rank represents this talent's skill, unmodified by anything
		/// </summary>
		public int Rank { get; private set; }

		public int Temporary { get; set; }

		/// <summary>
		/// RealRank will call the function supplied in the template to calculate this talent's modified rank, it can be anything
		/// from a simple adding another attribute to it or something much more complicated
		/// </summary>
		public override int Value {
			get { return calculateRealRank(Owner, this) + Temporary; }
			set { Rank = value; }
		}

		public override int MaximumValue { get; set; }

		private readonly Func<Person, Skill, int> calculateRealRank;

		public Skill(string name, Person owner, int max, int initialRank = 0, Func<Person, Skill, int> calcRealRank = null) : base(name) {
			Owner = owner;
			Rank = initialRank;
			MaximumValue = max;
			Temporary = 0;

			if (calcRealRank == null)
				calculateRealRank = (self, t) => t.Rank;
			else {
				calculateRealRank = calcRealRank;				
			}
		}
	}
}