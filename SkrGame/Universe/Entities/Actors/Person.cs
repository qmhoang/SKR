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

		public Attribute GetAttribute(string attrb) {
			return Attributes[attrb];
		}

		public Person() {
			Attributes = new StaticDictionary<string, Attribute>(
					new Dictionary<string, Attribute>
					{
							{"attribute_strength", new Attribute("Strength", World.MEAN * 2, World.MEAN)},
							{"attribute_agility", new Attribute("Agility", World.MEAN * 2, World.MEAN)},
							{"attribute_constitution", new Attribute("Constitution", World.MEAN * 2, World.MEAN)},

							{"attribute_intellect", new Attribute("Intellect", World.MEAN * 2, World.MEAN)},
							{"attribute_cunning", new Attribute("Cunning", World.MEAN * 2, World.MEAN)},
							{"attribute_resolve", new Attribute("Resolve", World.MEAN * 2, World.MEAN)},

							{"attribute_presence", new Attribute("Presence", World.MEAN * 2, World.MEAN)},
							{"attribute_grace", new Attribute("Grace", World.MEAN * 2, World.MEAN)},
							{"attribute_willpower", new Attribute("Willpower", World.MEAN * 2, World.MEAN)},
					});

			Stats = new StaticDictionary<string, Attribute>(
					new Dictionary<string, Attribute>
					{
							{"stat_stamina", new Attribute("Stamina", World.MEAN, World.MEAN)},
							{"stat_composure", new Attribute("Composure", World.MEAN, World.MEAN)},

							{"stat_energy", new Attribute("Energy", World.MEAN, World.MEAN)},
							{"stat_food", new Attribute("Food", World.MEAN, World.MEAN)},
							{"stat_water", new Attribute("Water", World.MEAN, World.MEAN)},
							{"stat_bladder", new Attribute("Bladder", World.MEAN, World.MEAN)},
							{"stat_cleanliness", new Attribute("Cleanliness", World.MEAN, World.MEAN)},

							// social?  - composure will replace for player
							// environment - probably not necessary
							// fun - composure replaces
					});

			Skills = new StaticDictionary<string, Skill>(
					new Dictionary<string, Skill>
					{
							{"skill_jumping", new Skill("Jumping", this, 100, 0, (user, t) => t.Owner.Attributes["attribute_strength"] + t.Rank)},

							{"skill_unarmed", new Skill("Unarmed", this, 100, 0, (user, t) => t.Owner.Attributes["attribute_agility"] + t.Rank)},
							{"skill_pistol", new Skill("Pistol", this, 100, 0, (user, t) => t.Owner.Attributes["attribute_agility"] + t.Rank)},
							{"skill_knife", new Skill("Knife", this, 100, 0, (user, t) => t.Owner.Attributes["attribute_agility"] + t.Rank)},
							{"skill_axe", new Skill("Axe", this, 100, 0, (user, t) => t.Owner.Attributes["attribute_agility"] + t.Rank)},

							{"skill_lockpicking", new Skill("Lockpicking", this, 100, 0, (user, t) => t.Owner.Attributes["attribute_intellect"] + t.Rank)},
					});
		}

		public override Component Copy() {
			//todo
			return new Person();
		}
	}
}