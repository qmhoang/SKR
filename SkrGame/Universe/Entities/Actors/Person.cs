using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using SkrGame.Gameplay.Combat;
using SkrGame.Universe.Entities.Features;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Entities.Stats;
using SkrGame.Universe.Locations;
using log4net;
using Attribute = SkrGame.Universe.Entities.Stats.Attribute;


namespace SkrGame.Universe.Entities.Actors {
	public enum Posture {
		Run,
		Stand,
		Crouch,
		Prone		// facing up / facing down?
	}

	public sealed class Person : Component {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public StaticDictionary<string, Skill> Skills { get; private set; }
		public StaticDictionary<string, Attribute> Attributes { get; private set; }
		public StaticDictionary<string, Attribute> Stats { get; private set; }

		public Posture Posture { get; set; }
		
		public int Lift {
			get { return Attributes["attribute_strength"] * Attributes["attribute_strength"] * 18 * (int) Math.Pow(World.StandardDeviation, -2.0); }
		}

		private const int SkillInitialRank = 0;
		private const int SkillMaxRank = World.MEAN * 2;

		private const int AttributeInitialRank = World.MEAN;
		private const int AttributeMaxRank = World.MEAN * 2;

		private const int StatInitialRank = 100;
		private const int StatMaxRank = 150;

		public Person() {
			Posture = Posture.Stand;
			
			Attributes = new StaticDictionary<string, Attribute>(
					new Dictionary<string, Attribute>
					{
							{"attribute_strength",		new Attribute("Strength",		"STR", AttributeInitialRank, AttributeMaxRank)},
							{"attribute_agility",		new Attribute("Agility",		"AGI", AttributeInitialRank, AttributeMaxRank)},
							{"attribute_constitution",	new Attribute("Constitution",	"CON", AttributeInitialRank, AttributeMaxRank)},

							{"attribute_intellect",		new Attribute("Intellect",		"INT", AttributeInitialRank, AttributeMaxRank)},
							{"attribute_cunning",		new Attribute("Cunning",		"CUN", AttributeInitialRank, AttributeMaxRank)},
							{"attribute_resolve",		new Attribute("Resolve",		"RES", AttributeInitialRank, AttributeMaxRank)},

							{"attribute_presence",		new Attribute("Presence",		"PRE", AttributeInitialRank, AttributeMaxRank)},
							{"attribute_grace",			new Attribute("Grace",			"GRA", AttributeInitialRank, AttributeMaxRank)},
							{"attribute_willpower",		new Attribute("Willpower",		"WIL", AttributeInitialRank, AttributeMaxRank)},
					});

			Stats = new StaticDictionary<string, Attribute>(
					new Dictionary<string, Attribute>
					{
							{"stat_stamina",			new Attribute("Stamina",		"SP ", World.MEAN, World.MEAN)},
							{"stat_composure",			new Attribute("Composure",		"CP ", World.MEAN, World.MEAN)},

							{"stat_energy",				new Attribute("Energy",			"SLP", StatInitialRank, StatMaxRank)},
							{"stat_food",				new Attribute("Food",			"FOD", StatInitialRank, StatMaxRank)},
							{"stat_water",				new Attribute("Water",			"H2O", StatInitialRank, StatMaxRank)},
							{"stat_bladder",			new Attribute("Bladder",		"BLD", StatInitialRank, StatMaxRank)},
							{"stat_cleanliness",		new Attribute("Cleanliness",	"CLN", StatInitialRank, StatMaxRank)},

							// social?  - composure will replace for player
							// environment - probably not necessary
							// fun - composure replaces
					});

			Skills = new StaticDictionary<string, Skill>(
					new Dictionary<string, Skill>
					{
							// STRENGTH
							{"skill_jumping",			new Skill("Jumping",		this, SkillInitialRank, SkillMaxRank, (user, t) => t.Owner.Attributes["attribute_strength"] + t.Rank)},

							// AGILITY
							{"skill_stealth",			new Skill("Stealth",		this, SkillInitialRank, SkillMaxRank, (user, t) => t.Owner.Attributes["attribute_agility"] + t.Rank)},

							{"skill_unarmed",			new Skill("Unarmed",		this, SkillInitialRank, SkillMaxRank, (user, t) => t.Owner.Attributes["attribute_agility"] + t.Rank)},
							{"skill_knife",				new Skill("Knife",			this, SkillInitialRank, SkillMaxRank, (user, t) => t.Owner.Attributes["attribute_agility"] + t.Rank)},
							{"skill_axe",				new Skill("Axe",			this, SkillInitialRank, SkillMaxRank, (user, t) => t.Owner.Attributes["attribute_agility"] + t.Rank)},

							{"skill_2haxe",				new Skill("Two-Handed Axe", this, SkillInitialRank, SkillMaxRank, (user, t) => t.Owner.Attributes["attribute_agility"] + t.Rank)},

							{"skill_pistol",			new Skill("Pistol",			this, SkillInitialRank, SkillMaxRank, (user, t) => t.Owner.Attributes["attribute_agility"] + t.Rank)},
							
							// skill_shotgun
							// skill_rifle

							// INTELLECT
							{"skill_lockpicking",		new Skill("Lockpicking",	this, SkillInitialRank, SkillMaxRank, (user, t) => t.Owner.Attributes["attribute_intellect"] + t.Rank)},

							// PRESENCE
							// skill_intimidation
							
							// GRACE
							// skill_manipulation
					});
		}

		public override Component Copy() {
			//todo
			return new Person();
		}
	}
}