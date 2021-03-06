﻿using System;
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

	public sealed class Creature : Component {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public StaticDictionary<string, Skill> Skills { get; private set; }
		public StaticDictionary<string, Attribute> Attributes { get; private set; }
		public StaticDictionary<string, Attribute> Stats { get; private set; }

		public Posture Posture { get; set; }
		
		public double Lift {
			get { return Attributes["attribute_strength"] * Attributes["attribute_strength"] * 18 * Math.Pow(World.StandardDeviation, -2.0) + 1; }
		}

		public double EncumbrancePenalty { get; set; }

		private const int SkillInitialRank = 0;
		private const int SkillMaxRank = World.Mean * 2;

		private const int AttributeInitialRank = World.Mean;
		private const int AttributeMaxRank = World.Mean * 2;

		private const int StatInitialRank = 100;
		private const int StatMaxRank = 150;

		public Creature() {
			EncumbrancePenalty = 0.0f;

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
							{"stat_health",				new Attribute("Health",			"HP ", World.Mean, World.Mean)},
							{"stat_stamina",			new Attribute("Stamina",		"SP ", World.Mean, World.Mean)},
							{"stat_composure",			new Attribute("Composure",		"CP ", World.Mean, World.Mean)},

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

							// CONSTITUTION
							// skill_running

							// INTELLECT
							{"skill_lockpicking",		new Skill("Lockpicking",	this, SkillInitialRank, SkillMaxRank, (user, t) => t.Owner.Attributes["attribute_intellect"] + t.Rank)},

							// CUNNING

							// RESOLVE

							// PRESENCE
							// skill_intimidation
							
							// GRACE
							// skill_manipulation

							// WILLPOWER
					});


		}

		public override Component Copy() {
			//todo
			return new Creature()
			       {
//			       		Attributes = new StaticDictionary<string, Attribute>()
			       };
		}
	}
}