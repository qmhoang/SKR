using System;
using System.Collections.Generic;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Universe.Locations;

namespace SkrGame.Universe.Factories {
	public class FeatureFactory {
		private static ActionResult ActionDoor(Entity user, Entity door, UseableFeature.UseAction action, string assetOpened, string assetClosed) {
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

		private static ActionResult ActionWindow(Entity user, Entity window, UseableFeature.UseAction action, string assetOpened, string assetClosed) {
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

		public static void Init(EntityFactory ef) {
			ef.Add("feature",
			       new VisibleComponent(10),
			       new Sprite("FEATURE", Sprite.FEATURES_LAYER),
			       new Identifier("Feature"),
			       new Blocker(false, false));

			ef.Inherits("nonblockingfeature", "feature", new Blocker());

			#region Doors and Windows

			ef.Inherits("door", "feature",
			            new Sprite("ClosedDoor", Sprite.FEATURES_LAYER),
			            new Identifier("Door", "A basic door"),
			            new UseableFeature(new List<UseableFeature.UseAction>
			                               {
			                               		new UseableFeature.UseAction("Open door", (user, d, action) => ActionDoor(user, d, action, "OpenedDoor", "ClosedDoor"))
			                               }));

			ef.Inherits("WALL_BRICK_DARK_DOOR_HORZ", "door",
			            new Sprite("WALL_BRICK_DARK_DOOR_HORZ", Sprite.FEATURES_LAYER),
			            new UseableFeature(new List<UseableFeature.UseAction>
			                               {
			                               		new UseableFeature.UseAction("Open door",
			                               		                             (user, d, action) => ActionDoor(user, d, action, "WALL_BRICK_DARK_DOOR_VERT", "WALL_BRICK_DARK_DOOR_HORZ"))
			                               }));

			ef.Inherits("WALL_BRICK_DARK_DOOR_VERT", "door",
			            new Sprite("WALL_BRICK_DARK_DOOR_VERT", Sprite.FEATURES_LAYER),
			            new UseableFeature(new List<UseableFeature.UseAction>
			                               {
			                               		new UseableFeature.UseAction("Open door",
			                               		                             (user, d, action) =>
			                               		                             ActionDoor(user, d, action, "WALL_BRICK_DARK_DOOR_HORZ",
			                               		                                        "WALL_BRICK_DARK_DOOR_VERT"))
			                               }));

			ef.Inherits("WINDOW_BRICK_DARK_VERT", "door",
			            new Sprite("WINDOW_BRICK_DARK_VERT", Sprite.FEATURES_LAYER),
			            new UseableFeature(new List<UseableFeature.UseAction>
			                               {
			                               		new UseableFeature.UseAction("Open window",
			                               		                             (w, user, action) =>
			                               		                             ActionWindow(user, w, action, "WINDOW_BRICK_DARK_HORZ",
			                               		                                          "WINDOW_BRICK_DARK_VERT"))
			                               }));

			ef.Inherits("WINDOW_BRICK_DARK_HORZ", "door",
			            new Sprite("WINDOW_BRICK_DARK_HORZ", Sprite.FEATURES_LAYER),
			            new UseableFeature(new List<UseableFeature.UseAction>
			                               {
			                               		new UseableFeature.UseAction("Open window",
			                               		                             (w, user, action) =>
			                               		                             ActionWindow(user, w, action, "WINDOW_BRICK_DARK_VERT",
			                               		                                          "WINDOW_BRICK_DARK_HORZ"))
			                               }));

			ef.Inherits("WINDOW_HOUSE_VERT", "door",
			            new Sprite("WINDOW_HOUSE_VERT", Sprite.FEATURES_LAYER),
			            new UseableFeature(new List<UseableFeature.UseAction>()
			                               {
			                               		new UseableFeature.UseAction("Open window",
			                               		                             (w, user, action) =>
			                               		                             ActionWindow(user, w, action, "WINDOW_HOUSE_HORZ", "WINDOW_HOUSE_VERT"))
			                               }));

			ef.Inherits("WINDOW_HOUSE_HORZ", "door",
			            new Sprite("WINDOW_HOUSE_HORZ", Sprite.FEATURES_LAYER),
			            new UseableFeature(new List<UseableFeature.UseAction>
			                               {
			                               		new UseableFeature.UseAction("Open window",
			                               		                             (w, user, action) =>
			                               		                             ActionWindow(user, w, action, "WINDOW_HOUSE_VERT", "WINDOW_HOUSE_HORZ"))
			                               }));

			ef.Inherits("DOOR_APART_1_VERT", "door",
			            new Sprite("DOOR_APART_1_VERT", Sprite.FEATURES_LAYER),
			            new UseableFeature(new List<UseableFeature.UseAction>
			                               {
			                               		new UseableFeature.UseAction("Open window",
			                               		                             (w, user, action) =>
			                               		                             ActionWindow(user, w, action, "DOOR_APART_1_HORZ", "DOOR_APART_1_VERT"))
			                               }));

			ef.Inherits("DOOR_APART_1_HORZ", "door",
			            new Sprite("DOOR_APART_1_HORZ", Sprite.FEATURES_LAYER),
			            new UseableFeature(new List<UseableFeature.UseAction>
			                               {
			                               		new UseableFeature.UseAction("Open window",
			                               		                             (w, user, action) =>
			                               		                             ActionWindow(user, w, action, "DOOR_APART_1_VERT", "DOOR_APART_1_HORZ"))
			                               }));

			#endregion

			#region House Features

			ef.Inherits("COUNTER_WOOD_RED", "feature", 
				new Sprite("COUNTER_WOOD_RED", Sprite.FEATURES_LAYER));

			ef.Inherits("SINK", "feature",
			            new Sprite("SINK", Sprite.FEATURES_LAYER),
			            new UseableFeature(new List<UseableFeature.UseAction>
			                               {
			                               		new UseableFeature.UseAction("Wash hands",
			                               		                             (entity, user, action) =>
			                               		                             {
			                               		                             	World.Instance.AddMessage(String.Format("{0} uses the sink.", Identifier.GetNameOrId(user)));
			                               		                             	return ActionResult.Success;
			                               		                             })
			                               }));

			ef.Inherits("TOILET", "nonblockingfeature",
			            new Sprite("TOILET", Sprite.FEATURES_LAYER),
			            new PassiveFeature(delegate(Entity entityNear, Entity featureEntity, PassiveFeature passiveFeature)
			                               {
			                               	if (Math.Abs(entityNear.Get<Location>().DistanceTo(featureEntity.Get<Location>()) - 0) < Double.Epsilon)
			                               		World.Instance.AddMessage(String.Format("{0} stands on top of the toilet.  Ew.",
			                               		                                        Identifier.GetNameOrId(entityNear)));
			                               }));

			ef.Inherits("BATHROOMSINK", "nonblockingfeature", 
				new Sprite("BATHROOMSINK", Sprite.FEATURES_LAYER));

			ef.Inherits("BATH", "nonblockingfeature",
			            new Sprite("BATH", Sprite.FEATURES_LAYER),
			            new PassiveFeature(delegate(Entity entityNear, Entity featureEntity, PassiveFeature passiveFeature)
			                               	{
			                               		if (Math.Abs(entityNear.Get<Location>().DistanceTo(featureEntity.Get<Location>()) - 0) < Double.Epsilon)
			                               			World.Instance.AddMessage(String.Format("{0} steps into the bathtub.", Identifier.GetNameOrId(entityNear)));
			                               	}));

			ef.Inherits("SHOWER", "nonblockingfeature", 
				new Sprite("SHOWER", Sprite.FEATURES_LAYER));

			ef.Inherits("CHAIR_WOODEN", "nonblockingfeature", 
				new Sprite("CHAIR_WOODEN", Sprite.FEATURES_LAYER));

			ef.Inherits("TREE_SMALL", "nonblockingfeature", 
				new Sprite("TREE_SMALL", Sprite.FEATURES_LAYER));

			ef.Inherits("BED_WOODEN", "nonblockingfeature",
			            new Sprite("BED_WOODEN", Sprite.FEATURES_LAYER),
			            new PassiveFeature(delegate(Entity entityNear, Entity featureEntity, PassiveFeature passiveFeature)
			                               	{
			                               		if (Math.Abs(entityNear.Get<Location>().DistanceTo(featureEntity.Get<Location>()) - 0) < Double.Epsilon)
			                               			World.Instance.AddMessage(String.Format("{0} jumps on the bed.", Identifier.GetNameOrId(entityNear)));
			                               	}));

			ef.Inherits("SHELF_WOOD", "feature", 
				new Sprite("SHELF_WOOD", Sprite.FEATURES_LAYER));

			ef.Inherits("SHELF_METAL", "feature", 
				new Sprite("SHELF_METAL", Sprite.FEATURES_LAYER));

			ef.Inherits("TELEVISION", "feature",
			            new Sprite("TELEVISION", Sprite.FEATURES_LAYER),
			            new PassiveFeature(delegate(Entity entityNear, Entity featureEntity, PassiveFeature passiveFeature)
			                               	{
			                               		if (entityNear.Get<Location>().DistanceTo(featureEntity.Get<Location>()) < 5)
			                               			World.Instance.AddMessage(String.Format("{0} hears the sound of television.",
			                               			                                        Identifier.GetNameOrId(entityNear)));
			                               	}));

			ef.Inherits("FRIDGE", "feature", 
				new Sprite("FRIDGE", Sprite.FEATURES_LAYER));

			ef.Inherits("DESK_WOODEN", "feature", 
				new Sprite("DESK_WOODEN", Sprite.FEATURES_LAYER));

			ef.Inherits("CASH_REGISTER", "feature", 
				new Sprite("CASH_REGISTER", Sprite.FEATURES_LAYER));

			ef.Inherits("SOFA", "nonblockingfeature",
			            new Sprite("SOFA", Sprite.FEATURES_LAYER),
			            new PassiveFeature(delegate(Entity entityNear, Entity featureEntity, PassiveFeature passiveFeature)
			                               	{
			                               		if (Math.Abs(entityNear.Get<Location>().DistanceTo(featureEntity.Get<Location>()) - 0) < Double.Epsilon)
			                               			World.Instance.AddMessage(String.Format("{0} jumps on the sofa.  Whee!!", Identifier.GetNameOrId(entityNear)));
			                               	}));

			ef.Inherits("OVEN", "feature", 
				new Sprite("OVEN", Sprite.FEATURES_LAYER));

			ef.Inherits("DOOR_GARAGE", "feature", 
				new Sprite("DOOR_GARAGE", Sprite.FEATURES_LAYER));

			ef.Inherits("FENCE_WOODEN", "feature", 
				new Sprite("FENCE_WOODEN", Sprite.FEATURES_LAYER), new Blocker(false, true));

			ef.Inherits("LAMP_STANDARD", "nonblockingfeature", 
				new Sprite("LAMP_STANDARD", Sprite.FEATURES_LAYER));

			ef.Inherits("TABLE_WOODEN", "nonblockingfeature", 
				new Sprite("TABLE_WOODEN", Sprite.FEATURES_LAYER));

			ef.Inherits("SAFE_SIMPLE", "nonblockingfeature",
				new Sprite("SAFE_SIMPLE", Sprite.FEATURES_LAYER));

			#endregion

			#region Walls

			ef.Inherits("WALL_BRICK_DARK", "feature",
			            new Sprite("WALL_BRICK_DARK", Sprite.FEATURES_LAYER));
			ef.Inherits("WALL_BRICK_DARK_VERT", "feature",
			            new Sprite("WALL_BRICK_DARK_VERT", Sprite.FEATURES_LAYER));
			ef.Inherits("WALL_BRICK_DARK_HORZ", "feature",
			            new Sprite("WALL_BRICK_DARK_HORZ", Sprite.FEATURES_LAYER));
			ef.Inherits("WALL_BRICK_DARK_T_NORTH", "feature",
			            new Sprite("WALL_BRICK_DARK_T_NORTH", Sprite.FEATURES_LAYER));
			ef.Inherits("WALL_BRICK_DARK_T_SOUTH", "feature",
			            new Sprite("WALL_BRICK_DARK_T_SOUTH", Sprite.FEATURES_LAYER));
			ef.Inherits("WALL_BRICK_DARK_T_EAST", "feature",
			            new Sprite("WALL_BRICK_DARK_T_EAST", Sprite.FEATURES_LAYER));
			ef.Inherits("WALL_BRICK_DARK_T_WEST", "feature",
			            new Sprite("WALL_BRICK_DARK_T_WEST", Sprite.FEATURES_LAYER));
			ef.Inherits("WALL_BRICK_DARK_NORTHEAST", "feature",
			            new Sprite("WALL_BRICK_DARK_NORTHEAST", Sprite.FEATURES_LAYER));
			ef.Inherits("WALL_BRICK_DARK_NORTHWEST", "feature",
			            new Sprite("WALL_BRICK_DARK_NORTHWEST", Sprite.FEATURES_LAYER));
			ef.Inherits("WALL_BRICK_DARK_SOUTHWEST", "feature",
			            new Sprite("WALL_BRICK_DARK_SOUTHWEST", Sprite.FEATURES_LAYER));
			ef.Inherits("WALL_BRICK_DARK_SOUTHEAST", "feature",
			            new Sprite("WALL_BRICK_DARK_SOUTHEAST", Sprite.FEATURES_LAYER));
			ef.Inherits("WALL_DRY", "feature",
			            new Sprite("WALL_DRY", Sprite.FEATURES_LAYER));

			#endregion

			#region Stairs

			ef.Inherits("STAIR_WOODEN_UP", "feature",new Sprite("STAIR_WOODEN_UP", Sprite.FEATURES_LAYER), new Blocker(true, true));
			ef.Inherits("STAIR_WOODEN_DOWN", "feature",new Sprite("STAIR_WOODEN_DOWN", Sprite.FEATURES_LAYER), new Blocker(true, true));

			#endregion

			#region Misc Decorations

			ef.Inherits("PLANTPOT_FIXED", "feature", new Sprite("PLANTPOT_FIXED", Sprite.FEATURES_LAYER), new Blocker(true, true));

			#endregion
		}
	}
}