using DEngine.Entities;
using SkrGame.Universe.Entities.Actors;

namespace SkrGame.Gameplay.Talent.Components {
	public class AttributeComponentTemplate : TalentComponentTemplate {
		public int InitialRank { get; set; }
		public int MaxRank { get; set; }

		public override TalentComponent Construct() {
			return new AttributeComponent(this);
		}
	}

	public class AttributeComponent : TalentComponent {
		public int Rank { get; set; }
		public int MaxRank { get; private set; }

		public AttributeComponent(AttributeComponentTemplate template) {
			Rank = template.InitialRank;
			MaxRank = template.MaxRank;
		}
	}

	public class SkillComponentTemplate : TalentComponentTemplate {
		public int InitialRank { get; set; }
		public int MaxRank { get; set; }

		/// <summary>
		/// RealRank will call the this function to calculate this talent's modified rank, it can be anything
		/// from a simple adding another attribute to it or something much more complicated
		/// <b>If left null, it will default to simply return the raw rank</b>
		/// </summary>
		public delegate int CalculateRealRankFunction(SkillComponent talent, Entity self);

		public CalculateRealRankFunction CalculateRealRank { get; set; }

		public override TalentComponent Construct() {
			return new SkillComponent(this);
		}
	}

	public class SkillComponent : TalentComponent {
		/// <summary>
		/// raw rank represents this talent's skill, unmodified by anything
		/// </summary>
		public int RawRank { get; set; }
		/// <summary>
		/// RealRank will call the function supplied in the template to calculate this talent's modified rank, it can be anything
		/// from a simple adding another attribute to it or something much more complicated
		/// </summary>
		public int Rank { get { return calculateRealRank(this, Talent.Owner); } }
		public int MaxRank { get; private set; }

		private readonly SkillComponentTemplate.CalculateRealRankFunction calculateRealRank;

		public SkillComponent(SkillComponentTemplate template) {
			RawRank = template.InitialRank;
			MaxRank = template.MaxRank;

			calculateRealRank = template.CalculateRealRank;

			if (template.CalculateRealRank == null)
				calculateRealRank = (t, self) => t.RawRank;

		}
	}
}
