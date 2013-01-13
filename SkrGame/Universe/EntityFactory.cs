using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entity;
using SkrGame.Gameplay.Combat;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Entities.Items.Components;
using SkrGame.Universe.Factories;
using SkrGame.Universe.Locations;
using log4net;

namespace SkrGame.Universe {
	public class EntityFactory {
		private Dictionary<string, Template> compiledTemplates;
		private Dictionary<string, Tuple<string, Template>> tempTemplates;
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public EntityFactory() {
			compiledTemplates = new Dictionary<string, Template>();
			tempTemplates = new Dictionary<string, Tuple<string, Template>>();

			LoadDefinitions();
			Compile();
		}

		public void LoadDefinitions() {
			LoadFeatures();
			LoadItems();
		}

		public void Compile() {
			Queue<string> queues = new Queue<string>();
			foreach (var t in tempTemplates) {
				queues.Enqueue(t.Key);
			}

			while (queues.Count > 0) {
				string id = queues.Dequeue();
				var inherit = tempTemplates[id].Item1;

				Logger.DebugFormat("Dequequeing {0}{1}.", id, String.IsNullOrEmpty(inherit) ? "" : string.Format(" which inherits from {0}", inherit));
				if (id == "glock22")
					Console.WriteLine();
				if (String.IsNullOrEmpty(inherit)) {
					Logger.DebugFormat("No inheritance, compiling {0}", id);
					var cs = tempTemplates[id].Item2.Select(c => c.Copy());					
					compiledTemplates.Add(id, new Template(cs.ToArray()));
				} else if (compiledTemplates.ContainsKey(inherit)) {
					Logger.DebugFormat("Inherited class found, compiling {0}", id);
					var template = compiledTemplates[inherit];
					template.Add(tempTemplates[id].Item2.Select(c => c.Copy()).ToArray());

					compiledTemplates.Add(id, template);
				} else {
					Logger.DebugFormat("No inherited class found, requequeing {0}", id);
					queues.Enqueue(id);
				}
			}

			tempTemplates.Clear();
		}

		public Template Get(string id) {
			return compiledTemplates[id];
		}

		private ActionResult ActionDoor(Entity user, Entity door, UseableFeature.UseAction action, string assetOpened, string assetClosed) {
			if (action.Description == "Open door") {
				if (door.Has<Blocker>())
					door.Get<Blocker>().Transparent = door.Get<Blocker>().Walkable = true;
				if (door.Has<Sprite>())
					door.Get<Sprite>().Asset = assetOpened;
				action.Description = "Close door";
				user.Get<ActionPoint>().ActionPoints -= DOOR_USAGE_AP_COST;

				World.Instance.AddMessage(String.Format("{0} opens the door.", Identifier.GetNameOrId(user)));
				return ActionResult.Success;
			} else if (action.Description == "Close door") {
				// todo add a size component and check if some entity blocks the door from closing
				if (door.Has<Blocker>())
					door.Get<Blocker>().Transparent = door.Get<Blocker>().Walkable = false;
				if (door.Has<Sprite>())
					door.Get<Sprite>().Asset = assetClosed;

				action.Description = "Open door";
				user.Get<ActionPoint>().ActionPoints -= DOOR_USAGE_AP_COST;

				World.Instance.AddMessage(String.Format("{0} closes the door.", Identifier.GetNameOrId(user)));
				return ActionResult.Success;
			}

			World.Instance.AddMessage("There's something blocking the way.");
			return ActionResult.Aborted;
		}

		private ActionResult ActionWindow(Entity user, Entity window, UseableFeature.UseAction action, string assetOpened, string assetClosed) {
			if (action.Description == "Open window") {
				if (window.Has<Blocker>())
					window.Get<Blocker>().Transparent = true;
				if (window.Has<Sprite>())
					window.Get<Sprite>().Asset = assetOpened;
				action.Description = "Close window";
				user.Get<ActionPoint>().ActionPoints -= WINDOW_USAGE_AP_COST;

				World.Instance.AddMessage(String.Format("{0} opens the window.", Identifier.GetNameOrId(user)));
				return ActionResult.Success;
			} else if (action.Description == "Close window") {
				if (window.Has<Blocker>())
					window.Get<Blocker>().Transparent = false;
				if (window.Has<Sprite>())
					window.Get<Sprite>().Asset = assetClosed;

				action.Description = "Open window";
				user.Get<ActionPoint>().ActionPoints -= WINDOW_USAGE_AP_COST;

				World.Instance.AddMessage(String.Format("{0} closes the window.", Identifier.GetNameOrId(user)));
				return ActionResult.Success;
			}
			return ActionResult.Success;
		}

		private const int DOOR_USAGE_AP_COST = World.DEFAULT_SPEED;
		private const int WINDOW_USAGE_AP_COST = World.DEFAULT_SPEED;

		private void LoadFeatures() {
			tempTemplates.Add("feature",
			                  new Tuple<string, Template>("",
			                                              new Template(new VisibleComponent(10),
			                                                           new Sprite("FEATURE", Sprite.FEATURES_LAYER),
			                                                           new Identifier("Feature"),
			                                                           new Blocker(false, false))));

			tempTemplates.Add("nonblockingfeature",
				  new Tuple<string, Template>("feature",
											  new Template(new Blocker())));

			#region Doors and Windows

			tempTemplates.Add("Door",
			                  new Tuple<string, Template>("feature",
			                                              new Template()
			                                              {

			                                              		new Sprite("ClosedDoor", Sprite.FEATURES_LAYER),

			                                              		new Identifier("Wood door"),
			                                              		new UseableFeature(new List<UseableFeature.UseAction>()
			                                              		                   {
			                                              		                   		new UseableFeature.UseAction("Open door", (d, user, action) => ActionDoor(user, d, action, "OpenedDoor", "ClosedDoor"))
			                                              		                   })
			                                              }));
			tempTemplates.Add("WALL_BRICK_DARK_DOOR_HORZ",
			                  new Tuple<string, Template>("feature",
			                                              new Template()
			                                              {

			                                              		new Sprite("WALL_BRICK_DARK_DOOR_HORZ", Sprite.FEATURES_LAYER),

			                                              		new UseableFeature(new List<UseableFeature.UseAction>()
			                                              		                   {
			                                              		                   		new UseableFeature.UseAction("Open door",
			                                              		                   		                             (d, user, action) =>
			                                              		                   		                             ActionDoor(user, d, action, "WALL_BRICK_DARK_DOOR_VERT",
			                                              		                   		                                        "WALL_BRICK_DARK_DOOR_HORZ"))
			                                              		                   })
			                                              }));

			tempTemplates.Add("WALL_BRICK_DARK_DOOR_VERT",
			                  new Tuple<string, Template>("feature",
			                                              new Template
			                                              {

			                                              		new Sprite("WALL_BRICK_DARK_DOOR_VERT", Sprite.FEATURES_LAYER),

			                                              		new UseableFeature(new List<UseableFeature.UseAction>()
			                                              		                   {
			                                              		                   		new UseableFeature.UseAction("Open door",
			                                              		                   		                             (d, user, action) =>
			                                              		                   		                             ActionDoor(user, d, action, "WALL_BRICK_DARK_DOOR_HORZ",
			                                              		                   		                                        "WALL_BRICK_DARK_DOOR_VERT"))
			                                              		                   })
			                                              }));
			tempTemplates.Add("WINDOW_BRICK_DARK_VERT",
			                  new Tuple<string, Template>("feature",
			                                              new Template
			                                              {

			                                              		new Sprite("WINDOW_BRICK_DARK_VERT", Sprite.FEATURES_LAYER),

			                                              		new UseableFeature(new List<UseableFeature.UseAction>()
			                                              		                   {
			                                              		                   		new UseableFeature.UseAction("Open window",
			                                              		                   		                             (w, user, action) =>
			                                              		                   		                             ActionWindow(user, w, action, "WINDOW_BRICK_DARK_HORZ",
			                                              		                   		                                          "WINDOW_BRICK_DARK_VERT"))
			                                              		                   })
			                                              }));
			tempTemplates.Add("WINDOW_BRICK_DARK_HORZ",
			                  new Tuple<string, Template>("feature",
			                                              new Template
			                                              {

			                                              		new Sprite("WINDOW_BRICK_DARK_HORZ", Sprite.FEATURES_LAYER),

			                                              		new UseableFeature(new List<UseableFeature.UseAction>()
			                                              		                   {
			                                              		                   		new UseableFeature.UseAction("Open window",
			                                              		                   		                             (w, user, action) =>
			                                              		                   		                             ActionWindow(user, w, action, "WINDOW_BRICK_DARK_VERT",
			                                              		                   		                                          "WINDOW_BRICK_DARK_HORZ"))
			                                              		                   })
			                                              }));
			tempTemplates.Add("WINDOW_HOUSE_VERT",
			                  new Tuple<string, Template>("feature",
			                                              new Template
			                                              {

			                                              		new Sprite("WINDOW_HOUSE_VERT", Sprite.FEATURES_LAYER),

			                                              		new UseableFeature(new List<UseableFeature.UseAction>()
			                                              		                   {
			                                              		                   		new UseableFeature.UseAction("Open window",
			                                              		                   		                             (w, user, action) =>
			                                              		                   		                             ActionWindow(user, w, action, "WINDOW_HOUSE_HORZ", "WINDOW_HOUSE_VERT"))
			                                              		                   })
			                                              }));
			tempTemplates.Add("WINDOW_HOUSE_HORZ",
			                  new Tuple<string, Template>("feature",
			                                              new Template
			                                              {

			                                              		new Sprite("WINDOW_HOUSE_HORZ", Sprite.FEATURES_LAYER),

			                                              		new UseableFeature(new List<UseableFeature.UseAction>()
			                                              		                   {
			                                              		                   		new UseableFeature.UseAction("Open window",
			                                              		                   		                             (w, user, action) =>
			                                              		                   		                             ActionWindow(user, w, action, "WINDOW_HOUSE_VERT", "WINDOW_HOUSE_HORZ"))
			                                              		                   })
			                                              }));
			tempTemplates.Add("DOOR_APART_1_VERT",
			                  new Tuple<string, Template>("feature",
			                                              new Template
			                                              {

			                                              		new Sprite("DOOR_APART_1_VERT", Sprite.FEATURES_LAYER),

			                                              		new UseableFeature(new List<UseableFeature.UseAction>()
			                                              		                   {
			                                              		                   		new UseableFeature.UseAction("Open window",
			                                              		                   		                             (w, user, action) =>
			                                              		                   		                             ActionWindow(user, w, action, "DOOR_APART_1_HORZ", "DOOR_APART_1_VERT"))
			                                              		                   })
			                                              }));
			tempTemplates.Add("DOOR_APART_1_HORZ",
			                  new Tuple<string, Template>("feature",
			                                              new Template
			                                              {

			                                              		new Sprite("DOOR_APART_1_HORZ", Sprite.FEATURES_LAYER),

			                                              		new UseableFeature(new List<UseableFeature.UseAction>()
			                                              		                   {
			                                              		                   		new UseableFeature.UseAction("Open window",
			                                              		                   		                             (w, user, action) =>
			                                              		                   		                             ActionWindow(user, w, action, "DOOR_APART_1_VERT", "DOOR_APART_1_HORZ"))
			                                              		                   })
			                                              }));

			#endregion

			#region House Features

			tempTemplates.Add("COUNTER_WOOD_RED",
			                  new Tuple<string, Template>("feature",
			                                              new Template {new Sprite("COUNTER_WOOD_RED", Sprite.FEATURES_LAYER), new Blocker(false, false)}));
			tempTemplates.Add("SINK",
			                  new Tuple<string, Template>("feature",
			                                              new Template
			                                              {

			                                              		new Sprite("SINK", Sprite.FEATURES_LAYER),
			                                              		new UseableFeature(new List<UseableFeature.UseAction>()
			                                              		                   {
			                                              		                   		new UseableFeature.UseAction("Wash hands",
			                                              		                   		                             (entity, user, action) =>
			                                              		                   		                             {
			                                              		                   		                             	World.Instance.AddMessage(String.Format("{0} uses the sink.", Identifier.GetNameOrId(user)));
			                                              		                   		                             	return ActionResult.Success;
			                                              		                   		                             })
			                                              		                   })
			                                              }));
			tempTemplates.Add("TOILET",
							  new Tuple<string, Template>("nonblockingfeature",
			                                              new Template
			                                              {

			                                              		new Sprite("TOILET", Sprite.FEATURES_LAYER),
			                                              		new PassiveFeature(delegate(PassiveFeature f, Actor actor, double distance)
			                                              		                   {
			                                              		                   	if (Math.Abs(distance - 0) < Double.Epsilon)
			                                              		                   		actor.World.AddMessage(String.Format("{0} stands on top of the toilet.  Ew.",
			                                              		                   		                                     actor.Name));
			                                              		                   })
			                                              }));
			tempTemplates.Add("BATHROOMSINK",
			                  new Tuple<string, Template>("feature",
			                                              new Template {new Sprite("BATHROOMSINK", Sprite.FEATURES_LAYER), new Blocker(false, false)}));


			tempTemplates.Add("BATH",
							  new Tuple<string, Template>("nonblockingfeature",
			                                              new Template
			                                              {

			                                              		new Sprite("BATH", Sprite.FEATURES_LAYER),
			                                              		new PassiveFeature(delegate(PassiveFeature f, Actor actor, double distance)
			                                              		                   {
			                                              		                   	if (Math.Abs(distance - 0) < Double.Epsilon)
			                                              		                   		actor.World.AddMessage(String.Format("{0} steps into the bathtub.", actor.Name));
			                                              		                   }
			                                              				)
			                                              }));
			tempTemplates.Add("SHOWER",
							  new Tuple<string, Template>("nonblockingfeature",
			                                              new Template {new Sprite("SHOWER", Sprite.FEATURES_LAYER)}));
			tempTemplates.Add("CHAIR_WOODEN",
							  new Tuple<string, Template>("nonblockingfeature",
			                                              new Template {new Sprite("CHAIR_WOODEN", Sprite.FEATURES_LAYER)}));
			tempTemplates.Add("TREE_SMALL",
			                  new Tuple<string, Template>("feature",
			                                              new Template {new Sprite("TREE_SMALL", Sprite.FEATURES_LAYER), new Blocker(false, false)}));
			tempTemplates.Add("BED_WOODEN",
							  new Tuple<string, Template>("nonblockingfeature",
			                                              new Template
			                                              {

			                                              		new Sprite("BED_WOODEN", Sprite.FEATURES_LAYER),
			                                              		new PassiveFeature(delegate(PassiveFeature f, Actor actor, double distance)
			                                              		                   {
			                                              		                   	if (Math.Abs(distance - 0) < Double.Epsilon)
			                                              		                   		actor.World.AddMessage(String.Format("{0} jumps on the bed.", actor.Name));
			                                              		                   })
			                                              }));
			tempTemplates.Add("SHELF_WOOD",
			                  new Tuple<string, Template>("feature",
			                                              new Template {new Sprite("SHELF_WOOD", Sprite.FEATURES_LAYER), new Blocker(false, false)}));
			tempTemplates.Add("SHELF_METAL",
			                  new Tuple<string, Template>("feature",
			                                              new Template {new Sprite("SHELF_METAL", Sprite.FEATURES_LAYER), new Blocker(false, false)}));
			tempTemplates.Add("TELEVISION",
			                  new Tuple<string, Template>("feature",
			                                              new Template
			                                              {

			                                              		new Sprite("TELEVISION", Sprite.FEATURES_LAYER),

			                                              		new PassiveFeature(delegate(PassiveFeature f, Actor actor, double distance)
			                                              		                   {
			                                              		                   	if (distance < 5)
			                                              		                   		actor.World.AddMessage(String.Format("{0} hears the sound of television.",
			                                              		                   		                                     actor.Name));
			                                              		                   })
			                                              }));
			tempTemplates.Add("FRIDGE",
			                  new Tuple<string, Template>("feature",
			                                              new Template {new Sprite("FRIDGE", Sprite.FEATURES_LAYER), new Blocker(false, false)}));
			tempTemplates.Add("DESK_WOODEN",
			                  new Tuple<string, Template>("feature",
			                                              new Template {new Sprite("DESK_WOODEN", Sprite.FEATURES_LAYER), new Blocker(false, false)}));
			tempTemplates.Add("CASH_REGISTER",
			                  new Tuple<string, Template>("feature",
			                                              new Template {new Sprite("CASH_REGISTER", Sprite.FEATURES_LAYER), new Blocker(false, false)}));
			tempTemplates.Add("SOFA",
			                  new Tuple<string, Template>("nonblockingfeature",
			                                              new Template
			                                              {

			                                              		new Sprite("SOFA", Sprite.FEATURES_LAYER),
			                                              		new PassiveFeature(delegate(PassiveFeature f, Actor actor, double distance)
			                                              		                   {
			                                              		                   	if (Math.Abs(distance - 0) < Double.Epsilon)
			                                              		                   		actor.World.AddMessage(String.Format("{0} jumps on the sofa.  Whee!!", actor.Name));
			                                              		                   })
			                                              }));
			tempTemplates.Add("OVEN",
			                  new Tuple<string, Template>("feature",
			                                              new Template {new Sprite("OVEN", Sprite.FEATURES_LAYER), new Blocker(false, false)}));

			tempTemplates.Add("DOOR_GARAGE",
			                  new Tuple<string, Template>("feature",
			                                              new Template {new Sprite("DOOR_GARAGE", Sprite.FEATURES_LAYER), new Blocker(false, false)}));
			tempTemplates.Add("FENCE_WOODEN",
			                  new Tuple<string, Template>("feature",
			                                              new Template {new Sprite("FENCE_WOODEN", Sprite.FEATURES_LAYER), new Blocker(false, true)}));
			tempTemplates.Add("LAMP_STANDARD",
							  new Tuple<string, Template>("nonblockingfeature",
			                                              new Template {new Sprite("LAMP_STANDARD", Sprite.FEATURES_LAYER)}));
			tempTemplates.Add("TABLE_WOODEN",
							  new Tuple<string, Template>("nonblockingfeature",
			                                              new Template {new Sprite("TABLE_WOODEN", Sprite.FEATURES_LAYER)}));
			tempTemplates.Add("SAFE_SIMPLE",
							  new Tuple<string, Template>("nonblockingfeature",
			                                              new Template {new Sprite("SAFE_SIMPLE", Sprite.FEATURES_LAYER)}));

			#endregion

			#region Walls

			tempTemplates.Add("WALL_BRICK_DARK",
			                  new Tuple<string, Template>("feature",
			                                              new Template {new Sprite("WALL_BRICK_DARK", Sprite.FEATURES_LAYER), new Blocker(false, false)}));
			tempTemplates.Add("WALL_BRICK_DARK_VERT",
			                  new Tuple<string, Template>("feature",
			                                              new Template {new Sprite("WALL_BRICK_DARK_VERT", Sprite.FEATURES_LAYER), new Blocker(false, false)}));
			tempTemplates.Add("WALL_BRICK_DARK_HORZ",
			                  new Tuple<string, Template>("feature",
			                                              new Template {new Sprite("WALL_BRICK_DARK_HORZ", Sprite.FEATURES_LAYER), new Blocker(false, false)}));
			tempTemplates.Add("WALL_BRICK_DARK_T_NORTH",
			                  new Tuple<string, Template>("feature",
			                                              new Template {new Sprite("WALL_BRICK_DARK_T_NORTH", Sprite.FEATURES_LAYER), new Blocker(false, false)}));
			tempTemplates.Add("WALL_BRICK_DARK_T_SOUTH",
			                  new Tuple<string, Template>("feature",
			                                              new Template {new Sprite("WALL_BRICK_DARK_T_SOUTH", Sprite.FEATURES_LAYER), new Blocker(false, false)}));
			tempTemplates.Add("WALL_BRICK_DARK_T_EAST",
			                  new Tuple<string, Template>("feature",
			                                              new Template {new Sprite("WALL_BRICK_DARK_T_EAST", Sprite.FEATURES_LAYER), new Blocker(false, false)}));
			tempTemplates.Add("WALL_BRICK_DARK_T_WEST",
			                  new Tuple<string, Template>("feature",
			                                              new Template {new Sprite("WALL_BRICK_DARK_T_WEST", Sprite.FEATURES_LAYER), new Blocker(false, false)}));
			tempTemplates.Add("WALL_BRICK_DARK_NORTHEAST",
			                  new Tuple<string, Template>("feature",
			                                              new Template {new Sprite("WALL_BRICK_DARK_NORTHEAST", Sprite.FEATURES_LAYER), new Blocker(false, false)}));
			tempTemplates.Add("WALL_BRICK_DARK_NORTHWEST",
			                  new Tuple<string, Template>("feature",
			                                              new Template {new Sprite("WALL_BRICK_DARK_NORTHWEST", Sprite.FEATURES_LAYER), new Blocker(false, false)}));
			tempTemplates.Add("WALL_BRICK_DARK_SOUTHWEST",
			                  new Tuple<string, Template>("feature",
			                                              new Template {new Sprite("WALL_BRICK_DARK_SOUTHWEST", Sprite.FEATURES_LAYER), new Blocker(false, false)}));
			tempTemplates.Add("WALL_BRICK_DARK_SOUTHEAST",
			                  new Tuple<string, Template>("feature",
			                                              new Template {new Sprite("WALL_BRICK_DARK_SOUTHEAST", Sprite.FEATURES_LAYER), new Blocker(false, false)}));
			tempTemplates.Add("WALL_DRY",
			                  new Tuple<string, Template>("feature",
			                                              new Template {new Sprite("WALL_DRY", Sprite.FEATURES_LAYER), new Blocker(false, false)}));

			#endregion

			#region Stairs

			tempTemplates.Add("STAIR_WOODEN_UP",
			                  new Tuple<string, Template>("feature",
			                                              new Template {new Sprite("STAIR_WOODEN_UP", Sprite.FEATURES_LAYER), new Blocker(true, true)}));
			tempTemplates.Add("STAIR_WOODEN_DOWN",
			                  new Tuple<string, Template>("feature",
			                                              new Template {new Sprite("STAIR_WOODEN_DOWN", Sprite.FEATURES_LAYER), new Blocker(true, true)}));

			#endregion

			#region Misc Decorations

			tempTemplates.Add("PLANTPOT_FIXED",
			                  new Tuple<string, Template>("feature",
			                                              new Template {new Sprite("PLANTPOT_FIXED", Sprite.FEATURES_LAYER), new Blocker(true, true)}));

			#endregion
		}

		private void LoadItems() {
			tempTemplates.Add("item",
			                  new Tuple<string, Template>("",
			                                              new Template(new VisibleComponent(10),
			                                                           new Sprite("GENERIC_ITEM", Sprite.ITEMS_LAYER),
			                                                           new ItemRefId("item"),
			                                                           new Identifier("Junk", "A piece of junk."),
			                                                           new Item(
			                                                           		new Item.Template
			                                                           		{
			                                                           				Type = ItemType.Misc,
			                                                           				Value = 0,
			                                                           				Weight = 0,
			                                                           				Size = 0,
			                                                           				StackType = StackType.None,
			                                                           				Slot = new List<string>()
			                                                           		}))));

			tempTemplates.Add("meleeweapon",
			                  new Tuple<string, Template>("item",
			                                              new Template(new Sprite("WEAPON", Sprite.ITEMS_LAYER),
			                                                           new ItemRefId("meleeweapon"),
			                                                           new MeleeComponent(
			                                                           		new MeleeComponent.Template
			                                                           		{
			                                                           				ComponentId = "genericmeleeweapon",
			                                                           				ActionDescription = "hit",
			                                                           				ActionDescriptionPlural = "hits",
			                                                           				Skill = "skill_unarmed",
			                                                           				HitBonus = 0,
			                                                           				Damage = Rand.Constant(-10),
			                                                           				DamageType = Combat.DamageTypes["crush"],
			                                                           				Penetration = 0,
			                                                           				WeaponSpeed = 100,
			                                                           				APToReady = 10,
			                                                           				Reach = 1,
			                                                           				Strength = 1,
			                                                           				Parry = -3
			                                                           		}))));

			tempTemplates.Add("gun",
			                  new Tuple<string, Template>("meleeweapon",
			                                              new Template(new Sprite("GUN", Sprite.ITEMS_LAYER),
			                                                           new RangeComponent(
			                                                           		new RangeComponent.Template
			                                                           		{
			                                                           				ComponentId = "genericgun",
			                                                           				ActionDescription = "shoot",
			                                                           				ActionDescriptionPlural = "shoots",
			                                                           				Skill = "skill_pistol",
			                                                           				Accuracy = 2,
			                                                           				Damage = Rand.Constant(1),
			                                                           				DamageType = Combat.DamageTypes["pierce"],
			                                                           				Penetration = 1,
			                                                           				Shots = 1,
			                                                           				Range = 100,
			                                                           				RoF = 1,
			                                                           				APToReady = World.SecondsToActionPoints(1f),
			                                                           				Recoil = 1,
			                                                           				Reliability = 18,
			                                                           				Strength = 8,
			                                                           				AmmoType = "unused",
			                                                           		}))));

			tempTemplates.Add("pistol1",
			                  new Tuple<string, Template>("meleeweapon",
			                                              new Template(new MeleeComponent(
			                                                           		new MeleeComponent.Template
			                                                           		{
			                                                           				ComponentId = "pistolwhipmelee",
			                                                           				ActionDescription = "pistol whip",
			                                                           				ActionDescriptionPlural = "pistol whips",
			                                                           				Skill = "skill_unarmed",
			                                                           				HitBonus = -1,
			                                                           				Damage = Rand.Constant(5 * (1 - 2)),
			                                                           				DamageType = Combat.DamageTypes["crush"],
			                                                           				Penetration = 1,
			                                                           				WeaponSpeed = 85,
			                                                           				Reach = 0,
			                                                           				Strength = 8,
			                                                           				Parry = -2
			                                                           		}))));

			tempTemplates.Add("pistol2",
			                  new Tuple<string, Template>("meleeweapon",
			                                              new Template(new MeleeComponent(
			                                                           		new MeleeComponent.Template
			                                                           		{
			                                                           				ComponentId = "pistolwhipmelee",
			                                                           				ActionDescription = "pistol whip",
			                                                           				ActionDescriptionPlural = "pistol whips",
			                                                           				Skill = "skill_unarmed",
			                                                           				HitBonus = -1,
			                                                           				Damage = Rand.Constant(5 * (2 - 2)),
			                                                           				DamageType = Combat.DamageTypes["crush"],
			                                                           				Penetration = 1,
			                                                           				WeaponSpeed = 85,
			                                                           				Reach = 0,
			                                                           				Strength = 8,
			                                                           				Parry = -2
			                                                           		}))));

			tempTemplates.Add("glock17",
			                  new Tuple<string, Template>("pistol2",
			                                              new Template( //new Sprite("GLOCK17", Sprite.ITEMS_LAYER),
			                                              		new Identifier("Glock 17"),
																new ItemRefId("glock17"),
			                                              		new Item(new Item.Template
			                                              		         {
			                                              		         		Type = ItemType.OneHandedWeapon,
			                                              		         		Value = 60000,
			                                              		         		Weight = 19,
			                                              		         		Size = 2,
			                                              		         		StackType = StackType.None,
			                                              		         		Slot = new List<string>
			                                              		         		       {
			                                              		         		       		"Main Hand",
			                                              		         		       }
			                                              		         }),
			                                              		new RangeComponent(
			                                              				new RangeComponent.Template
			                                              				{
			                                              						ComponentId = "glock17shoot",
			                                              						ActionDescription = "shoot",
			                                              						ActionDescriptionPlural = "shoots",
			                                              						Skill = "skill_pistol",
			                                              						Accuracy = 2,
			                                              						Damage = Rand.Dice(2, World.STANDARD_DEVIATION * 2) + Rand.Constant(10),
			                                              						DamageType = Combat.DamageTypes["pierce"],
			                                              						Penetration = 1,
			                                              						Shots = 17,
			                                              						Range = 160,
			                                              						RoF = 3,
			                                              						ReloadSpeed = 3,
			                                              						Recoil = 2,
			                                              						Reliability = 18,
			                                              						Strength = 8,
			                                              						AmmoType = "9x19mm",
			                                              				}))));

			tempTemplates.Add("glock22",
			                  new Tuple<string, Template>("pistol2",
			                                              new Template( //new Sprite("GLOCK22", Sprite.ITEMS_LAYER),
			                                              		new Identifier("Glock 22"),
																new ItemRefId("glock22"),
			                                              		new Item(new Item.Template
			                                              		         {
			                                              		         		Type = ItemType.OneHandedWeapon,
			                                              		         		Value = 40000,
			                                              		         		Weight = 21,
			                                              		         		Size = 2,
			                                              		         		StackType = StackType.None,
			                                              		         		Slot = new List<string>()
			                                              		         		       {
			                                              		         		       		"Main Hand",
			                                              		         		       }
			                                              		         }),

			                                              		new RangeComponent(
			                                              				new RangeComponent.Template
			                                              				{
			                                              						ComponentId = "glock22shoot",
			                                              						ActionDescription = "shoot",
			                                              						ActionDescriptionPlural = "shoots",
			                                              						Skill = "skill_pistol",
			                                              						Accuracy = 2,
			                                              						Damage = Rand.Dice(2, World.STANDARD_DEVIATION * 2) + Rand.Constant(10),
			                                              						DamageType = Combat.DamageTypes["pierce_large"],
			                                              						Penetration = 1,
			                                              						Shots = 15,
			                                              						Range = 160,
			                                              						RoF = 3,
			                                              						ReloadSpeed = 3,
			                                              						Recoil = 2,
			                                              						Reliability = 18,
			                                              						Strength = 8,
			                                              						AmmoType = ".40S&W"
			                                              				}))));

			tempTemplates.Add("largeknife",
			                  new Tuple<string, Template>("meleeweapon",
			                                              new Template( //new Sprite("LARGE_KNIFE", Sprite.ITEMS_LAYER),
			                                              		new ItemRefId("largeknife"),
			                                              		new Identifier("Knife, Large", "A large knife."),
			                                              		new Item(
			                                              				new Item.Template
			                                              				{
			                                              						Type = ItemType.OneHandedWeapon,
			                                              						Value = 4000,
			                                              						Weight = 10,
			                                              						Size = 2,
			                                              						StackType = StackType.None,
			                                              						Slot = new List<string>()
			                                              						       {
			                                              						       		"Main Hand",
			                                              						       }
			                                              				}),
			                                              		new MeleeComponent(
			                                              				new MeleeComponent.Template
			                                              				{
			                                              						ComponentId = "largeknifeslash",
			                                              						ActionDescription = "slash",
			                                              						ActionDescriptionPlural = "slashes",
			                                              						Skill = "skill_knife",
			                                              						HitBonus = 0,
			                                              						Damage = Rand.Constant(-5),
			                                              						DamageType = Combat.DamageTypes["cut"],
			                                              						Penetration = 1,
			                                              						WeaponSpeed = 100,
			                                              						APToReady = 15,
			                                              						Reach = 1,
			                                              						Strength = 6,
			                                              						Parry = -1
			                                              				}))));
			tempTemplates.Add("smallknife",
			                  new Tuple<string, Template>("meleeweapon",
			                                              new Template( //new Sprite("SMALL_KNIFE", Sprite.ITEMS_LAYER),
			                                              		new ItemRefId("smallknife"),
			                                              		new Identifier("Knife", "A knife."),
			                                              		new Item(
			                                              				new Item.Template
			                                              				{
			                                              						Type = ItemType.OneHandedWeapon,
			                                              						Value = 3000,
			                                              						Weight = 5,
			                                              						Size = 1,
			                                              						StackType = StackType.None,
			                                              						Slot = new List<string>()
			                                              						       {
			                                              						       		"Main Hand",
			                                              						       }
			                                              				}),
			                                              		new MeleeComponent(
			                                              				new MeleeComponent.Template
			                                              				{
			                                              						ComponentId = "smallknifethrust",
			                                              						ActionDescription = "jab",
			                                              						ActionDescriptionPlural = "jabs",
			                                              						Skill = "skill_knife",
			                                              						HitBonus = 0,
			                                              						Damage = Rand.Constant(0),
			                                              						DamageType = Combat.DamageTypes["impale"],
			                                              						Penetration = 1,
			                                              						WeaponSpeed = 110,
			                                              						APToReady = 5,
			                                              						Reach = 1,
			                                              						Strength = 6,
			                                              						Parry = -1
			                                              				}))));
			tempTemplates.Add("axe",
			                  new Tuple<string, Template>("meleeweapon",
			                                              new Template( //new Sprite("AXE", Sprite.ITEMS_LAYER),
			                                              		new ItemRefId("axe"),
			                                              		new Identifier("Axe", "An axe."),

			                                              		new Item(
			                                              				new Item.Template
			                                              				{
			                                              						Type = ItemType.OneHandedWeapon,
			                                              						Value = 5000,
			                                              						Weight = 40,
			                                              						Size = 3,
			                                              						StackType = StackType.None,
			                                              						Slot = new List<string>()
			                                              						       {
			                                              						       		"Main Hand",
			                                              						       }
			                                              				}),
			                                              		new MeleeComponent(
			                                              				new MeleeComponent.Template
			                                              				{
			                                              						ComponentId = "axeswing",
			                                              						ActionDescription = "hack",
			                                              						ActionDescriptionPlural = "hacks",
			                                              						Skill = "skill_axe",
			                                              						HitBonus = 0,
			                                              						Damage = Rand.Constant(10),
			                                              						DamageType = Combat.DamageTypes["cut"],
			                                              						Penetration = 1,
			                                              						WeaponSpeed = 90,
			                                              						APToReady = 25,
			                                              						Reach = 1,
			                                              						Strength = 11,
			                                              						Parry = 0
			                                              				}))));
			tempTemplates.Add("hatchet",
			                  new Tuple<string, Template>("meleeweapon",
			                                              new Template( //new Sprite("HATCHET", Sprite.ITEMS_LAYER),
			                                              		new ItemRefId("hatchet"),
			                                              		new Identifier("Hatchet", "A hatchet."),
			                                              		new Item(
			                                              				new Item.Template
			                                              				{
			                                              						Type = ItemType.OneHandedWeapon,
			                                              						Value = 4000,
			                                              						Weight = 20,
			                                              						Size = 2,
			                                              						StackType = StackType.None,
			                                              						Slot = new List<string>()
			                                              						       {
			                                              						       		"Main Hand",
			                                              						       }
			                                              				}),
			                                              		new MeleeComponent(
			                                              				new MeleeComponent.Template
			                                              				{
			                                              						ComponentId = "hatchetswing",
			                                              						ActionDescription = "hack",
			                                              						ActionDescriptionPlural = "hacks",
			                                              						Skill = "skill_axe",
			                                              						HitBonus = 0,
			                                              						Damage = Rand.Constant(0),
			                                              						DamageType = Combat.DamageTypes["cut"],
			                                              						Penetration = 1,
			                                              						WeaponSpeed = 92,
			                                              						Reach = 1,
			                                              						Strength = 8,
			                                              						Parry = 0
			                                              				}))));
			tempTemplates.Add("brassknuckles",
			                  new Tuple<string, Template>("meleeweapon",
			                                              new Template( //new Sprite("BRASS_KNUCKLES", Sprite.ITEMS_LAYER),
			                                              		new ItemRefId("brassknuckles"),
			                                              		new Identifier("Brass Knuckles"),
			                                              		new Item(
			                                              				new Item.Template
			                                              				{
			                                              						Type = ItemType.OneHandedWeapon,
			                                              						Value = 1000,
			                                              						Weight = 20,
			                                              						StackType = StackType.None,
			                                              						Slot = new List<string>()
			                                              						       {
			                                              						       		"Main Hand",
			                                              						       }
			                                              				}),
			                                              		new MeleeComponent(
			                                              				new MeleeComponent.Template
			                                              				{
			                                              						ComponentId = "brassknucklesswing",
			                                              						ActionDescription = "punch",
			                                              						ActionDescriptionPlural = "punches",
			                                              						Skill = "skill_unarmed",
			                                              						HitBonus = 0,
			                                              						Damage = Rand.Constant(0),
			                                              						DamageType = Combat.DamageTypes["crush"],
			                                              						Penetration = 1,
			                                              						WeaponSpeed = 100,
			                                              						Reach = 0,
			                                              						Strength = 1,
			                                              						Parry = -1
			                                              				}))));
			tempTemplates.Add("bullet",
			                  new Tuple<string, Template>("item",
			                                              new Template( //new Sprite("BULLET", Sprite.ITEMS_LAYER),
			                                              		new ItemRefId("bullet"),
			                                              		new Identifier("Bullets"),
			                                              		new Item(
			                                              				new Item.Template
			                                              				{
			                                              						Type = ItemType.Ammo,
			                                              						Value = 30,
			                                              						Weight = 0,
			                                              						Size = 0,
			                                              						StackType = StackType.Hard
			                                              				}),
			                                              		new AmmoComponent(
			                                              				new AmmoComponent.Template
			                                              				{
			                                              						ComponentId = "bullet",
			                                              						ActionDescription = "load",
			                                              						ActionDescriptionPlural = "loads",
			                                              						Type = "bullet",
			                                              				}))));
			tempTemplates.Add("9x9mm",
			                  new Tuple<string, Template>("bullet",
			                                              new Template( //new Sprite("BULLET_9x19MM", Sprite.ITEMS_LAYER),
			                                              		new ItemRefId("9x9mm"),
			                                              		new Identifier("9x9mm", "9x19mm Parabellum bullet"),
			                                              		new Item(
			                                              				new Item.Template
			                                              				{
			                                              						Type = ItemType.Ammo,
			                                              						Value = 30,
			                                              						Weight = 0,
			                                              						Size = 0,
			                                              						StackType = StackType.Hard
			                                              				}),
			                                              		new AmmoComponent(
			                                              				new AmmoComponent.Template
			                                              				{
			                                              						ComponentId = "9x9mmbullet",
			                                              						ActionDescription = "load",
			                                              						ActionDescriptionPlural = "loads",
			                                              						Type = "9x19mm",
			                                              				}))));
			tempTemplates.Add(".40S&W",
			                  new Tuple<string, Template>("bullet",
			                                              new Template( //new Sprite("BULLET_.40S&W", Sprite.ITEMS_LAYER),
			                                              		new ItemRefId(".40S&W"),
			                                              		new Identifier(".40S&W", ".40 Smith & Wesson bullet"),
			                                              		new Item(
			                                              				new Item.Template
			                                              				{
			                                              						Type = ItemType.Ammo,
			                                              						Value = 30,
			                                              						Weight = 0,
			                                              						Size = 0,
			                                              						StackType = StackType.Hard
			                                              				}),
			                                              		new AmmoComponent(
			                                              				new AmmoComponent.Template
			                                              				{
			                                              						ComponentId = ".40S&Wbullet",
			                                              						ActionDescription = "load",
			                                              						ActionDescriptionPlural = "loads",
			                                              						Type = ".40S&W",
			                                              				}))));

			tempTemplates.Add("armor",
			                  new Tuple<string, Template>("item",
			                                              new Template(new Sprite("ARMOR", Sprite.ITEMS_LAYER),
			                                                           new ItemRefId("armor"),
			                                                           new Identifier("Generic Armor"),
																	   new Item(new Item.Template
			                                                                    {
			                                                                    		Type = ItemType.Armor,
			                                                                    		Value = 100,
			                                                                    		Weight = 10,
			                                                                    		Size = 11,
			                                                                    		StackType = StackType.None,
			                                                                    		Slot = new List<string>
			                                                                    		       {
			                                                                    		       		"Torso",
			                                                                    		       }
			                                                                    }),
			                                                           new ArmorComponent(new ArmorComponent.Template
			                                                                              {
			                                                                              		ComponentId = "armor",
			                                                                              		DonTime = 1,
			                                                                              		Defenses = new List<ArmorComponent.LocationProtected>
			                                                                              		           {
			                                                                              		           		new ArmorComponent.LocationProtected("Torso", 10, new Dictionary<DamageType, int>
			                                                                              		           		                                                  {
			                                                                              		           		                                                  		{Combat.DamageTypes["true"], 0},
			                                                                              		           		                                                  		{Combat.DamageTypes["cut"], 1},
			                                                                              		           		                                                  		{Combat.DamageTypes["crush"], 1},
			                                                                              		           		                                                  		{Combat.DamageTypes["impale"], 1},
			                                                                              		           		                                                  		{Combat.DamageTypes["pierce_small"], 1},
			                                                                              		           		                                                  		{Combat.DamageTypes["pierce"], 1},
			                                                                              		           		                                                  		{Combat.DamageTypes["pierce_large"], 1},
			                                                                              		           		                                                  		{Combat.DamageTypes["pierce_huge"], 1},
			                                                                              		           		                                                  		{Combat.DamageTypes["burn"], 1},
			                                                                              		           		                                                  })
			                                                                              		           }
			                                                                              }))));

			tempTemplates.Add("footballpads",
			                  new Tuple<string, Template>("armor",
			                                              new Template(new Sprite("FOOTBALL_SHOULDER_PADS", Sprite.ITEMS_LAYER),
			                                                           new ItemRefId("footballpads"),
			                                                           new Identifier("Football Shoulder Pads"),
			                                                           new Item(new Item.Template
			                                                                    {
			                                                                    		Type = ItemType.Armor,
			                                                                    		Value = 5000,
			                                                                    		Weight = 50,
			                                                                    		Size = 11,
			                                                                    		StackType = StackType.None,
			                                                                    		Slot = new List<string>
			                                                                    		       {
			                                                                    		       		"Torso",
			                                                                    		       }
			                                                                    }),
			                                                           new ArmorComponent(new ArmorComponent.Template
			                                                                              {
			                                                                              		ComponentId = "footballarmor",
			                                                                              		DonTime = 10,
			                                                                              		Defenses = new List<ArmorComponent.LocationProtected>
			                                                                              		           {
			                                                                              		           		new ArmorComponent.LocationProtected("Torso", 30, new Dictionary<DamageType, int>
			                                                                              		           		                                                  {
			                                                                              		           		                                                  		{Combat.DamageTypes["true"], 0},
			                                                                              		           		                                                  		{Combat.DamageTypes["cut"], 8},
			                                                                              		           		                                                  		{Combat.DamageTypes["crush"], 15},
			                                                                              		           		                                                  		{Combat.DamageTypes["impale"], 6},
			                                                                              		           		                                                  		{Combat.DamageTypes["pierce_small"], 4},
			                                                                              		           		                                                  		{Combat.DamageTypes["pierce"], 4},
			                                                                              		           		                                                  		{Combat.DamageTypes["pierce_large"], 4},
			                                                                              		           		                                                  		{Combat.DamageTypes["pierce_huge"], 4},
			                                                                              		           		                                                  		{Combat.DamageTypes["burn"], 5},
			                                                                              		           		                                                  })
			                                                                              		           }
			                                                                              }))));
		}
	}
}