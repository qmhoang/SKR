using System;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entity;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Locations;

namespace SkrGame.Universe.Factories {
	public abstract class FeatureFactory : Factory<string, Template> {}

	public class SourceFeatureFactory : FeatureFactory {
		private ActionResult ActionDoor(UseableFeature f, Actor user, string assetOpened, string assetClosed) {
//			if (f.Action == "Open door") {
//				f.Owner.Asset = assetOpened;
//				f.Action = "Close door";
//				f.Owner.Transparent = f.Owner.Walkable = true;
//				user.World.AddMessage(String.Format("{0} opens the door.", user.Name))};
//				return ActionResult.Success;
//			}
//			if (!f.Owner.Level.DoesActorExistAtLocation(f.Owner.Position)) {
//				f.Owner.Asset = assetClosed;
//				f.Action = "Open door";
//				f.Owner.Transparent = f.Owner.Walkable = false;
//				user.World.AddMessage(String.Format("{0} closes the door.", user.Name))};
//				return ActionResult.Success;
//			}

			user.World.AddMessage("There's something blocking the way.");
			return ActionResult.Aborted;
		}

		private ActionResult ActionWindow(UseableFeature f, Actor user, string assetOpened, string assetClosed) {
//			if (f.Action == "Open window") {
//				f.Owner.Asset = assetOpened;
//				f.Action = "Close window";
//				f.Owner.Transparent = true;
//				user.World.AddMessage(String.Format("{0} opens the window.", user.Name))};
//				return ActionResult.Success;
//			} else {
//				f.Owner.Asset = assetClosed;
//				f.Action = "Open window";
//				f.Owner.Transparent = false;
//				user.World.AddMessage(String.Format("{0} closes the window.", user.Name))};
//				return ActionResult.Success;
//			}
			return ActionResult.Success;
		}

		private int doorUsageAPCost = World.DEFAULT_SPEED;
		private int windowUsageAPCost = World.DEFAULT_SPEED;

		public override Template Construct(string key) {
			switch (key) {
					#region Doors & Windows

				case "Door":
					return new Template()
					       {
					       		
					       		new Sprite("OpenedDoor", Sprite.FEATURES_LAYER),
					       		new Blocker(),
					       		new UseableFeature("Close door", doorUsageAPCost,
					       		                   (f, user) => ActionDoor(f, user, "OpenedDoor", "ClosedDoor"))
					       };
				case "WALL_BRICK_DARK_DOOR_HORZ":
					return new Template()
					       {
					       		
					       		new Sprite("WALL_BRICK_DARK_DOOR_HORZ", Sprite.FEATURES_LAYER),
					       		new Blocker(false, false),
					       		new UseableFeature("Open door", doorUsageAPCost,
					       		                   (f, user) => ActionDoor(f, user, "WALL_BRICK_DARK_DOOR_VERT", "WALL_BRICK_DARK_DOOR_HORZ"))
					       };

				case "WALL_BRICK_DARK_DOOR_VERT":
					return new Template
					       {
					       		
					       		new Sprite("WALL_BRICK_DARK_DOOR_VERT", Sprite.FEATURES_LAYER),
					       		new Blocker(false, false),
					       		new UseableFeature("Open door", doorUsageAPCost,
					       		                   (f, user) => ActionDoor(f, user, "WALL_BRICK_DARK_DOOR_HORZ", "WALL_BRICK_DARK_DOOR_VERT"))
					       };
				case "WINDOW_BRICK_DARK_VERT":
					return new Template
					       {
					       		
					       		new Sprite("WINDOW_BRICK_DARK_VERT", Sprite.FEATURES_LAYER),
					       		new Blocker(false, false),
					       		new UseableFeature("Open window", windowUsageAPCost,
					       		                   (f, user) => ActionWindow(f, user, "WINDOW_BRICK_DARK_HORZ", "WINDOW_BRICK_DARK_VERT"))
					       };
				case "WINDOW_BRICK_DARK_HORZ":
					return new Template
					       {
					       		
					       		new Sprite("WINDOW_BRICK_DARK_HORZ", Sprite.FEATURES_LAYER),
					       		new Blocker(false, false),
					       		new UseableFeature("Open window", windowUsageAPCost,
					       		                   (f, user) => ActionWindow(f, user, "WINDOW_BRICK_DARK_VERT", "WINDOW_BRICK_DARK_HORZ"))
					       };
				case "WINDOW_HOUSE_VERT":
					return new Template
					       {
					       		
					       		new Sprite("WINDOW_HOUSE_VERT", Sprite.FEATURES_LAYER),
					       		new Blocker(false, false),
					       		new UseableFeature("Open window", windowUsageAPCost,
					       		                   (f, user) => ActionWindow(f, user, "WINDOW_HOUSE_HORZ", "WINDOW_HOUSE_VERT"))
					       };
				case "WINDOW_HOUSE_HORZ":
					return new Template
					       {
					       		
					       		new Sprite("WINDOW_HOUSE_HORZ", Sprite.FEATURES_LAYER),
					       		new Blocker(false, false),
					       		new UseableFeature("Open window", windowUsageAPCost,
					       		                   (f, user) => ActionWindow(f, user, "WINDOW_HOUSE_VERT", "WINDOW_HOUSE_HORZ"))
					       };
				case "DOOR_APART_1_VERT":
					return new Template
					       {
					       		
					       		new Sprite("DOOR_APART_1_VERT", Sprite.FEATURES_LAYER),
					       		new Blocker(false, false),
					       		new UseableFeature("Open window", windowUsageAPCost,
					       		                   (f, user) => ActionWindow(f, user, "DOOR_APART_1_HORZ", "DOOR_APART_1_VERT"))
					       };
				case "DOOR_APART_1_HORZ":
					return new Template
					       {
					       		
					       		new Sprite("DOOR_APART_1_HORZ", Sprite.FEATURES_LAYER),
					       		new Blocker(false, false),
					       		new UseableFeature("Open window", windowUsageAPCost,
					       		                   (f, user) => ActionWindow(f, user, "DOOR_APART_1_VERT", "DOOR_APART_1_HORZ"))
					       };

					#endregion

					#region House Features

				case "COUNTER_WOOD_RED":
					return new Template { new Sprite("COUNTER_WOOD_RED", Sprite.FEATURES_LAYER), new Blocker(false, false)};
				case "SINK":
					return new Template
					       {
					       		
					       		new Sprite("SINK", Sprite.FEATURES_LAYER),
					       		new UseableFeature("Wash hands", World.DEFAULT_SPEED, delegate(UseableFeature f, Actor user)
					       		                                                      {
					       		                                                      	//user.cleaniness += 5
					       		                                                      	return ActionResult.Aborted;
					       		                                                      })
					       };
				case "TOILET":
					return new Template
					       {
					       		
					       		new Sprite("TOILET", Sprite.FEATURES_LAYER),
					       		new Blocker(true, true),
					       		new PassiveFeature(delegate(PassiveFeature f, Actor actor, double distance)
					       		                   {
					       		                   	if (Math.Abs(distance - 0) < Double.Epsilon)
					       		                   		actor.World.AddMessage(String.Format("{0} stands on top of the toilet.  Ew.",
					       		                   		                                     actor.Name));
					       		                   })
					       };
				case "BATHROOMSINK":
					return new Template { new Sprite("BATHROOMSINK", Sprite.FEATURES_LAYER), new Blocker(false, false)};


				case "BATH":
					return new Template
					       {
					       		
					       		new Sprite("BATH", Sprite.FEATURES_LAYER),
					       		new Blocker(true, true),
					       		new PassiveFeature(delegate(PassiveFeature f, Actor actor, double distance)
					       		                   {
					       		                   	if (Math.Abs(distance - 0) < Double.Epsilon)
					       		                   		actor.World.AddMessage(String.Format("{0} steps into the bathtub.", actor.Name));
					       		                   }
					       				)
					       };
				case "SHOWER":
					return new Template { new Sprite("SHOWER", Sprite.FEATURES_LAYER), new Blocker(true, true) };
				case "CHAIR_WOODEN":
					return new Template { new Sprite("CHAIR_WOODEN", Sprite.FEATURES_LAYER), new Blocker(true, true) };
				case "TREE_SMALL":
					return new Template { new Sprite("TREE_SMALL", Sprite.FEATURES_LAYER), new Blocker(false, false)};
				case "BED_WOODEN":
					return new Template
					       {
					       		
					       		new Sprite("BED_WOODEN", Sprite.FEATURES_LAYER),
					       		new Blocker(true, true),
					       		new PassiveFeature(delegate(PassiveFeature f, Actor actor, double distance)
					       		                   {
					       		                   	if (Math.Abs(distance - 0) < Double.Epsilon)
					       		                   		actor.World.AddMessage(String.Format("{0} jumps on the bed.", actor.Name));
					       		                   })
					       };
				case "SHELF_WOOD":
					return new Template { new Sprite("SHELF_WOOD", Sprite.FEATURES_LAYER), new Blocker(false, false)};
				case "SHELF_METAL":
					return new Template { new Sprite("SHELF_METAL", Sprite.FEATURES_LAYER), new Blocker(false, false)};
				case "TELEVISION":
					return new Template
					       {
					       		
					       		new Sprite("TELEVISION", Sprite.FEATURES_LAYER),
					       		new Blocker(false, false),
					       		new PassiveFeature(delegate(PassiveFeature f, Actor actor, double distance)
					       		                   {
					       		                   	if (distance < 5)
					       		                   		actor.World.AddMessage(String.Format("{0} hears the sound of television.",
					       		                   		                                     actor.Name));
					       		                   })
					       };
				case "FRIDGE":
					return new Template { new Sprite("FRIDGE", Sprite.FEATURES_LAYER), new Blocker(false, false)};
				case "DESK_WOODEN":
					return new Template { new Sprite("DESK_WOODEN", Sprite.FEATURES_LAYER), new Blocker(false, false)};
				case "CASH_REGISTER":
					return new Template { new Sprite("CASH_REGISTER", Sprite.FEATURES_LAYER), new Blocker(false, false)};
				case "SOFA":
					return new Template
					       {
					       		
					       		new Sprite("SOFA", Sprite.FEATURES_LAYER),
					       		new Blocker(true, true),
					       		new PassiveFeature(delegate(PassiveFeature f, Actor actor, double distance)
					       		                   {
					       		                   	if (Math.Abs(distance - 0) < Double.Epsilon)
					       		                   		actor.World.AddMessage(String.Format("{0} jumps on the sofa.  Whee!!", actor.Name));
					       		                   })
					       };
				case "OVEN":
					return new Template { new Sprite("OVEN", Sprite.FEATURES_LAYER), new Blocker(false, false)};

				case "DOOR_GARAGE":
					return new Template { new Sprite("DOOR_GARAGE", Sprite.FEATURES_LAYER), new Blocker(false, false)};
				case "FENCE_WOODEN":
					return new Template { new Sprite("FENCE_WOODEN", Sprite.FEATURES_LAYER), new Blocker(false, true)};
				case "LAMP_STANDARD":
					return new Template { new Sprite("LAMP_STANDARD", Sprite.FEATURES_LAYER), new Blocker(true, true) };
				case "TABLE_WOODEN":
					return new Template { new Sprite("TABLE_WOODEN", Sprite.FEATURES_LAYER), new Blocker(true, true) };
				case "SAFE_SIMPLE":
					return new Template { new Sprite("SAFE_SIMPLE", Sprite.FEATURES_LAYER), new Blocker(true, true) };

					#endregion

					#region Walls

				case "WALL_BRICK_DARK":
					return new Template { new Sprite("WALL_BRICK_DARK", Sprite.FEATURES_LAYER), new Blocker(false, false)};
				case "WALL_BRICK_DARK_VERT":
					return new Template { new Sprite("WALL_BRICK_DARK_VERT", Sprite.FEATURES_LAYER), new Blocker(false, false)};
				case "WALL_BRICK_DARK_HORZ":
					return new Template { new Sprite("WALL_BRICK_DARK_HORZ", Sprite.FEATURES_LAYER), new Blocker(false, false)};
				case "WALL_BRICK_DARK_T_NORTH":
					return new Template { new Sprite("WALL_BRICK_DARK_T_NORTH", Sprite.FEATURES_LAYER), new Blocker(false, false)};
				case "WALL_BRICK_DARK_T_SOUTH":
					return new Template { new Sprite("WALL_BRICK_DARK_T_SOUTH", Sprite.FEATURES_LAYER), new Blocker(false, false)};
				case "WALL_BRICK_DARK_T_EAST":
					return new Template { new Sprite("WALL_BRICK_DARK_T_EAST", Sprite.FEATURES_LAYER), new Blocker(false, false)};
				case "WALL_BRICK_DARK_T_WEST":
					return new Template { new Sprite("WALL_BRICK_DARK_T_WEST", Sprite.FEATURES_LAYER), new Blocker(false, false)};
				case "WALL_BRICK_DARK_NORTHEAST":
					return new Template { new Sprite("WALL_BRICK_DARK_NORTHEAST", Sprite.FEATURES_LAYER), new Blocker(false, false)};
				case "WALL_BRICK_DARK_NORTHWEST":
					return new Template { new Sprite("WALL_BRICK_DARK_NORTHWEST", Sprite.FEATURES_LAYER), new Blocker(false, false)};
				case "WALL_BRICK_DARK_SOUTHWEST":
					return new Template { new Sprite("WALL_BRICK_DARK_SOUTHWEST", Sprite.FEATURES_LAYER), new Blocker(false, false)};
				case "WALL_BRICK_DARK_SOUTHEAST":
					return new Template { new Sprite("WALL_BRICK_DARK_SOUTHEAST", Sprite.FEATURES_LAYER), new Blocker(false, false)};
				case "WALL_DRY":
					return new Template { new Sprite("WALL_DRY", Sprite.FEATURES_LAYER), new Blocker(false, false)};

					#endregion

					#region Stairs

				case "STAIR_WOODEN_UP":
					return new Template { new Sprite("STAIR_WOODEN_UP", Sprite.FEATURES_LAYER), new Blocker(true, true) };
				case "STAIR_WOODEN_DOWN":
					return new Template { new Sprite("STAIR_WOODEN_DOWN", Sprite.FEATURES_LAYER), new Blocker(true, true) };

					#endregion

					#region Misc Decorations

				case "PLANTPOT_FIXED":
					return new Template { new Sprite("PLANTPOT_FIXED", Sprite.FEATURES_LAYER), new Blocker(true, true) };

					#endregion
			}

			return null;
		}
	}
}