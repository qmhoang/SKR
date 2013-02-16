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

//	public class ActorCondition {
//		private Dictionary<Condition, int> myStatus;
//		private Actor actor;
//
//		public ActorCondition(Actor actor) {
//			this.actor = actor;
//			myStatus = new Dictionary<Condition, int>();
//		}
//
//		/// <summary>
//		/// Gets status condition of person, return int value of condition, -1 if player doesn't have condition
//		/// </summary>
//		/// <param name="condition"></param>
//		/// <returns></returns>
//		public int GetConditionStatus(Condition condition) {
//			return myStatus.ContainsKey(condition) ? myStatus[condition] : -1;
//		}
//
//		public void SetConditionStatus(Condition condition, int status) {
//			if (myStatus.ContainsKey(condition))
//				myStatus[condition] = status;
//			else
//				myStatus.Add(condition, status);
//		}
//	}

	public class TagChangedEvent : EventArgs {
		public string TagId { get; private set; }
		public int ValueChanged { get; private set; }
		public int NewValue { get; private set; }

		public TagChangedEvent(string tagId, int valueChanged, int newValue) {
			TagId = tagId;
			ValueChanged = valueChanged;
			NewValue = newValue;
		}
	}

	public class Skill {
		public Person Owner { get; set; }
		/// <summary>
		/// raw rank represents this talent's skill, unmodified by anything
		/// </summary>
		public int RawRank { get; set; }
		/// <summary>
		/// RealRank will call the function supplied in the template to calculate this talent's modified rank, it can be anything
		/// from a simple adding another attribute to it or something much more complicated
		/// </summary>
		public int Rank { get { return calculateRealRank(Owner, this); } }
		public int MaxRank { get; private set; }

		private readonly Func<Person, Skill, int> calculateRealRank;

		public Skill(Person owner, int maxRank, int initialRank = 0, Func<Person, Skill, int> calcRealRank = null) {
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

		private readonly Dictionary<string, Skill> skills;
		

		public int Lift {
			get { return Strength * Strength * 18 * (int) Math.Pow(World.STANDARD_DEVIATION, -2.0); }
		}
		
		public int GetSkill(string skill) {
			return skills[skill].Rank;
		}

		public Person() {
			Strength = Agility = Constitution = Intellect = Cunning = Resolve = Presence = Grace = Willpower = World.MEAN;		
			skills = new Dictionary<string, Skill>
			         {
			         		{"skill_unarmed", new Skill(this, 100, 0, (user, t) => t.Owner.Agility)},
							{"skill_pistol", new Skill(this, 100, 0, (user, t) => t.Owner.Agility)},
							{"skill_knife", new Skill(this, 100, 0, (user, t) => t.Owner.Agility)},
							{"skill_axe", new Skill(this, 100, 0, (user, t) => t.Owner.Agility)},
			         };

		}


		public override Component Copy() {
			//todo
			return new Person();
		}


	}
}