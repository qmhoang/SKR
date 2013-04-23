using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using DEngine.Random;
using SkrGame.Gameplay.Combat;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Features;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Entities.Items.Equipables;
using SkrGame.Universe.Entities.Useables;

namespace SkrGame.Universe.Factories {
	public static class TestEntityFactory {
		private class BaseItem : EntityFactory.Template {
			public BaseItem(string refId) {
				Add(new ReferenceId(refId));
				Add(new VisibleComponent(10));
				Add(new Sprite("base_item", Sprite.ItemsLayer));
				Add(new Identifier("Junk", "A piece of junk."));
				Add(new Item(0, 0, 0, 0, StackType.None));
			}
		}

		private class Paperclip : BaseItem {
			public Paperclip()
				: base("paperclip") {
				Add(new Identifier("Paperclip", "A single paperclip."));
				Add(new Entities.Items.Tools.Lockpick(-World.StandardDeviation * 3 / 2));
			}
		}

		private class Lockpick : BaseItem {
			public Lockpick()
				: base("lockpick") {
				Add(new Identifier("Lockpick", "A basic lockpick."));
				Add(new Entities.Items.Tools.Lockpick(0));
			}
		}

		private class BaseMeleeWeapon : BaseItem {
			public BaseMeleeWeapon()
				: base("base_meleeweapon") {
				Add(new Sprite("WEAPON", Sprite.ItemsLayer));
				Add(Equipable.SingleSlot("Main Hand", "Off Hand"));
				Add(new MeleeComponent(
							new MeleeComponent.Template
							{
								ActionDescription = "hit",
								ActionDescriptionPlural = "hits",
								Skill = "skill_unarmed",
								HitBonus = 0,
								Damage = Rand.Constant(-10),
								DamageType = Combat.DamageTypes["crush"],
								Penetration = 1,
								AttackSpeed = World.OneSecondInSpeed,
								APToReady = World.SecondsToActionPoints(1f),
								Reach = 1,
								Strength = 1,
								Parry = -3
							}));
			}
		}

		public static void Init(EntityFactory ef) {
			ef.Inherits("testuse", "feature",
			            new UseBroadcaster(),
			            new TestUseableComponent(),
			            new ApplianceComponent(new List<ApplianceComponent.Use>()
			                                   {
			                                   		ApplianceComponent.Use.UseAppliance("Use",
			                                   		                                    "stat_bladder",
			                                   		                                    new TimeSpan(0, 0, 1, 0),
			                                   		                                    new TimeSpan(0, 0, 0, 1, 150),
			                                   		                                    (e, app) => e.Get<GameObject>().Level.World.Log.NormalFormat("{0} finishes using the {1}.",
			                                   		                                                                                                         Identifier.GetNameOrId(e),
			                                   		                                                                                                         Identifier.GetNameOrId(app.Entity)),
			                                   		                                    (e, app) => e.Get<GameObject>().Level.World.Log.NormalFormat("{0} is unable to use the {1}.",
			                                   		                                                                                                         Identifier.GetNameOrId(e),
			                                   		                                                                                                         Identifier.GetNameOrId(app.Entity)))
			                                   }));

			var t = typeof(TestEntityFactory);
			var nested = t.GetNestedTypes();
			var f = t.IsClass;
		}
	}
}
