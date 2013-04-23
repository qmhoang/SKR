using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using DEngine.Random;
using SkrGame.Actions.Items;
using SkrGame.Effects;
using SkrGame.Gameplay.Combat;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Controllers;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Entities.Items.Equipables;

namespace SkrGame.Universe.Factories {
	public static class PersonFactory {
		public static void Init(EntityFactory ef) {
			ef.Add("person",
			       new Sprite("npc", Sprite.ActorLayer),
			       new Identifier("npc"),
			       new ActorComponent(new DoNothing(), new AP(World.OneSecondInSpeed / 2)),
			       new Creature(),
			       DefendComponent.CreateHuman(50),
			       new VisibleComponent(10),
			       new ContainerComponent(),
			       new EntityConditions(new HumanNeedsEffect(), new CreatureEncumbranceEffect()),
			       new EquipmentComponent(new List<string>
			                              {
												"Head",
			                              		"Torso",
			                              		"Arms",
			                              		"Main Hand",
			                              		"Off Hand",
			                              		"Hands",
			                              		"Legs",
			                              		"Feet"
			                              }),
			       new SightComponent(),
			       new MeleeComponent(new MeleeComponent.Template
			                          {
			                          		ActionDescription = "punch",
			                          		ActionDescriptionPlural = "punches",
			                          		Skill = "skill_unarmed",
			                          		HitBonus = 0,
			                          		Damage = Rand.Constant(-5),
			                          		DamageType = Combat.DamageTypes["crush"],
			                          		Penetration = 1,
			                          		AttackSpeed = World.OneSecondInSpeed,
			                          		APToReady = 1,
			                          		Reach = 0,
			                          		Strength = 1,
			                          		Parry = 0,
			                          }));

			ef.Inherits("npc", "person",
						new ActorComponent(new NPC(), new AP(World.OneSecondInSpeed / 2)));

			ef.Inherits("player", "person",
			            new Sprite("player", Sprite.PlayerLayer),
			            new Identifier("Player"),
						new ActorComponent(new Player(), new AP(World.OneSecondInSpeed / 2)));
		}
	}
}
