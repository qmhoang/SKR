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
	public enum Condition {
		Encrumbrance, // 0 - none, 1 - light, 2 - medium, etc
	}

	public enum Posture {
		Run,
		Stand,
		Crouch,
		Prone		// facing up / facing down?
	}
	
	public class Person : Component {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public StaticDictionary<string, Skill> Skills { get; private set; }
		public StaticDictionary<string, Attribute> Attributes { get; private set; }
		public StaticDictionary<string, Attribute> Stats { get; private set; }

		public Posture Posture { get; set; }
		
		public int Lift {
			get { return Attributes["attribute_strength"] * Attributes["attribute_strength"] * 18 * (int) Math.Pow(World.STANDARD_DEVIATION, -2.0); }
		}

		public Skill GetSkill(string skill) {
			return Skills[skill];
		}

		private const int SKILL_INITIAL_RANK = 0;
		private const int SKILL_MAX_RANK = World.MEAN * 2;

		private const int ATTRIBUTE_INITIAL_RANK = World.MEAN;
		private const int ATTRIBUTE_MAX_RANK = World.MEAN * 2;

		private const int STAT_INITIAL_RANK = World.MEAN;
		private const int STAT_MAX_RANK = World.MEAN;

		public Person() {
			Posture = Posture.Stand;
			
			Attributes = new StaticDictionary<string, Attribute>(
					new Dictionary<string, Attribute>
					{
							{"attribute_strength",		new Attribute("Strength",		"STR", ATTRIBUTE_INITIAL_RANK, ATTRIBUTE_MAX_RANK)},
							{"attribute_agility",		new Attribute("Agility",		"AGI", ATTRIBUTE_INITIAL_RANK, ATTRIBUTE_MAX_RANK)},
							{"attribute_constitution",	new Attribute("Constitution",	"CON", ATTRIBUTE_INITIAL_RANK, ATTRIBUTE_MAX_RANK)},

							{"attribute_intellect",		new Attribute("Intellect",		"INT", ATTRIBUTE_INITIAL_RANK, ATTRIBUTE_MAX_RANK)},
							{"attribute_cunning",		new Attribute("Cunning",		"CUN", ATTRIBUTE_INITIAL_RANK, ATTRIBUTE_MAX_RANK)},
							{"attribute_resolve",		new Attribute("Resolve",		"RES", ATTRIBUTE_INITIAL_RANK, ATTRIBUTE_MAX_RANK)},

							{"attribute_presence",		new Attribute("Presence",		"PRE", ATTRIBUTE_INITIAL_RANK, ATTRIBUTE_MAX_RANK)},
							{"attribute_grace",			new Attribute("Grace",			"GRA", ATTRIBUTE_INITIAL_RANK, ATTRIBUTE_MAX_RANK)},
							{"attribute_willpower",		new Attribute("Willpower",		"WIL", ATTRIBUTE_INITIAL_RANK, ATTRIBUTE_MAX_RANK)},
					});

			Stats = new StaticDictionary<string, Attribute>(
					new Dictionary<string, Attribute>
					{
							{"stat_stamina",			new Attribute("Stamina",		"SP ", STAT_INITIAL_RANK, STAT_MAX_RANK)},
							{"stat_composure",			new Attribute("Composure",		"CP ", STAT_INITIAL_RANK, STAT_MAX_RANK)},

							{"stat_energy",				new Attribute("Energy",			"SLP", STAT_INITIAL_RANK, STAT_MAX_RANK)},
							{"stat_food",				new Attribute("Food",			"FOD", STAT_INITIAL_RANK, STAT_MAX_RANK)},
							{"stat_water",				new Attribute("Water",			"H2O", STAT_INITIAL_RANK, STAT_MAX_RANK)},
							{"stat_bladder",			new Attribute("Bladder",		"BLD", STAT_INITIAL_RANK, STAT_MAX_RANK)},
							{"stat_cleanliness",		new Attribute("Cleanliness",	"CLN", STAT_INITIAL_RANK, STAT_MAX_RANK)},

							// social?  - composure will replace for player
							// environment - probably not necessary
							// fun - composure replaces
					});

			Skills = new StaticDictionary<string, Skill>(
					new Dictionary<string, Skill>
					{
							// STRENGTH
							{"skill_jumping",			new Skill("Jumping",		this, SKILL_INITIAL_RANK, SKILL_MAX_RANK, (user, t) => t.Owner.Attributes["attribute_strength"] + t.Rank)},

							// AGILITY
							{"skill_stealth",			new Skill("Stealth",		this, SKILL_INITIAL_RANK, SKILL_MAX_RANK, (user, t) => t.Owner.Attributes["attribute_agility"] + t.Rank)},

							{"skill_unarmed",			new Skill("Unarmed",		this, SKILL_INITIAL_RANK, SKILL_MAX_RANK, (user, t) => t.Owner.Attributes["attribute_agility"] + t.Rank)},
							{"skill_pistol",			new Skill("Pistol",			this, SKILL_INITIAL_RANK, SKILL_MAX_RANK, (user, t) => t.Owner.Attributes["attribute_agility"] + t.Rank)},
							{"skill_knife",				new Skill("Knife",			this, SKILL_INITIAL_RANK, SKILL_MAX_RANK, (user, t) => t.Owner.Attributes["attribute_agility"] + t.Rank)},
							{"skill_axe",				new Skill("Axe",			this, SKILL_INITIAL_RANK, SKILL_MAX_RANK, (user, t) => t.Owner.Attributes["attribute_agility"] + t.Rank)},

							{"skill_2haxe",				new Skill("Two-Handed Axe", this, SKILL_INITIAL_RANK, SKILL_MAX_RANK, (user, t) => t.Owner.Attributes["attribute_agility"] + t.Rank)},

							// INTELLECT
							{"skill_lockpicking",		new Skill("Lockpicking",	this, SKILL_INITIAL_RANK, SKILL_MAX_RANK, (user, t) => t.Owner.Attributes["attribute_intellect"] + t.Rank)},

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