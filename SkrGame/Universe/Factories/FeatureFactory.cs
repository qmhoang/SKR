using System;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entity;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Location;

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
							   new Position(0, 0),
							   new Sprite("OpenedDoor", 1),
					       		new Feature(),
								new UseableFeature("Close door", doorUsageAPCost,
							                           (f, user) => ActionDoor(f, user, "OpenedDoor", "ClosedDoor"))
					       };
				case "WALL_BRICK_DARK_DOOR_HORZ":
					return new Template()
					       {
					       		new Position(0, 0),
					       		new Sprite("WALL_BRICK_DARK_DOOR_HORZ", 1),
					       		new Feature(false, false),
					       		new UseableFeature("Open door", doorUsageAPCost,
					       		                   (f, user) => ActionDoor(f, user, "WALL_BRICK_DARK_DOOR_VERT","WALL_BRICK_DARK_DOOR_HORZ"))					       		                   
					       };

				case "WALL_BRICK_DARK_DOOR_VERT":
					return new Template { new Position(0, 0), new Sprite( "WALL_BRICK_DARK_DOOR_VERT", 1), new Feature(false, false),
							new UseableFeature("Open door", doorUsageAPCost,
							                           (f, user) => ActionDoor(f, user, "WALL_BRICK_DARK_DOOR_HORZ", "WALL_BRICK_DARK_DOOR_VERT"))};
				case "WINDOW_BRICK_DARK_VERT":
					return new Template { new Position(0, 0), new Sprite( "WINDOW_BRICK_DARK_VERT", 1), new Feature(false, false),
							new UseableFeature("Open window", windowUsageAPCost,
							                           (f, user) => ActionWindow(f, user, "WINDOW_BRICK_DARK_HORZ", "WINDOW_BRICK_DARK_VERT"))};
				case "WINDOW_BRICK_DARK_HORZ":
					return new Template { new Position(0, 0), new Sprite( "WINDOW_BRICK_DARK_HORZ", 1), new Feature(false, false),
							new UseableFeature("Open window", windowUsageAPCost,
							                           (f, user) => ActionWindow(f, user, "WINDOW_BRICK_DARK_VERT", "WINDOW_BRICK_DARK_HORZ"))};
				case "WINDOW_HOUSE_VERT":
					return new Template { new Position(0, 0), new Sprite( "WINDOW_HOUSE_VERT", 1), new Feature(false, false),
							new UseableFeature("Open window", windowUsageAPCost,
							                           (f, user) => ActionWindow(f, user, "WINDOW_HOUSE_HORZ", "WINDOW_HOUSE_VERT"))};
				case "WINDOW_HOUSE_HORZ":
					return new Template { new Position(0, 0), new Sprite( "WINDOW_HOUSE_HORZ", 1), new Feature(false, false),
							new UseableFeature("Open window", windowUsageAPCost,
							                           (f, user) => ActionWindow(f, user, "WINDOW_HOUSE_VERT", "WINDOW_HOUSE_HORZ"))};
				case "DOOR_APART_1_VERT":
					return new Template { new Position(0, 0), new Sprite( "DOOR_APART_1_VERT", 1), new Feature(false, false),
							new UseableFeature("Open window", windowUsageAPCost,
							                           (f, user) => ActionWindow(f, user, "DOOR_APART_1_HORZ", "DOOR_APART_1_VERT"))};
				case "DOOR_APART_1_HORZ":
					return new Template { new Position(0, 0), new Sprite( "DOOR_APART_1_HORZ", 1), new Feature(false, false),
							new UseableFeature("Open window", windowUsageAPCost,
							                           (f, user) => ActionWindow(f, user, "DOOR_APART_1_VERT", "DOOR_APART_1_HORZ"))};

					#endregion

					#region House Features

				case "COUNTER_WOOD_RED":
					return new Template { new Position(0, 0), new Sprite("COUNTER_WOOD_RED", 1), new Feature(false, false)};
				case "SINK":
					return new Template
					       {
					       		new Position(0, 0),
					       		new Sprite("SINK"),
					       		new UseableFeature("Wash hands", World.DEFAULT_SPEED, delegate(UseableFeature f, Actor user)
					       		                                                      {
					       		                                                      	//user.cleaniness += 5
					       		                                                      	return ActionResult.Aborted;
					       		                                                      })
					       };
				case "TOILET":
					return new Template { new Position(0, 0), new Sprite("TOILET"), new Feature(true, true),
							new PassiveFeature(delegate(PassiveFeature f, Actor actor, double distance)
							                   {
							                   	if (Math.Abs(distance - 0) < Double.Epsilon)
							                   		actor.World.AddMessage(String.Format("{0} stands on top of the toilet.  Ew.",
							                   		                                     actor.Name));
							                   })};
				case "BATHROOMSINK":
					return new Template { new Position(0, 0), new Sprite("BATHROOMSINK", 1), new Feature(false, false)};


				case "BATH":
					return new Template
					       {
					       		new Position(0, 0),
					       		new Sprite("BATH", 1), 
								new Feature(true, true),
					       		new PassiveFeature(delegate(PassiveFeature f, Actor actor, double distance)
					       		                   {
					       		                   	if (Math.Abs(distance - 0) < Double.Epsilon)
					       		                   		actor.World.AddMessage(String.Format("{0} steps into the bathtub.", actor.Name));
					       		                   }
					       				)
					       };
				case "SHOWER":
					return new Template { new Position(0, 0), new Sprite("SHOWER"), new Feature(true, true)};
				case "CHAIR_WOODEN":
					return new Template { new Position(0, 0), new Sprite("CHAIR_WOODEN"), new Feature(true, true)};
				case "TREE_SMALL":
					return new Template { new Position(0, 0), new Sprite("TREE_SMALL", 1), new Feature(false, false)};
				case "BED_WOODEN":
					return new Template
					       {
					       		new Position(0, 0),
					       		new Sprite("BED_WOODEN"),
								new Feature( true, true),
					       		new PassiveFeature(delegate(PassiveFeature f, Actor actor, double distance)
					       		                   {
					       		                   	if (Math.Abs(distance - 0) < Double.Epsilon)
					       		                   		actor.World.AddMessage(String.Format("{0} jumps on the bed.", actor.Name));
					       		                   })
					       };
				case "SHELF_WOOD":
					return new Template { new Position(0, 0), new Sprite("SHELF_WOOD", 1), new Feature(false, false)};
				case "SHELF_METAL":
					return new Template { new Position(0, 0), new Sprite("SHELF_METAL", 1), new Feature(false, false)};
				case "TELEVISION":
					return new Template
					       {
					       		new Position(0, 0),
					       		new Sprite("TELEVISION", 1),
					       		new Feature(false, false),
					       		new PassiveFeature(delegate(PassiveFeature f, Actor actor, double distance)
					       		                   {
					       		                   	if (distance < 5)
					       		                   		actor.World.AddMessage(String.Format("{0} hears the sound of television.",
					       		                   		                                     actor.Name));
					       		                   })
					       };
				case "FRIDGE":
					return new Template { new Position(0, 0), new Sprite("FRIDGE", 1), new Feature(false, false)};
				case "DESK_WOODEN":
					return new Template { new Position(0, 0), new Sprite("DESK_WOODEN", 1), new Feature(false, false)};
				case "CASH_REGISTER":
					return new Template { new Position(0, 0), new Sprite("CASH_REGISTER", 1), new Feature(false, false)};
				case "SOFA":
					return new Template
					       {
					       		new Position(0, 0),
					       		new Sprite("SOFA"),
								new Feature( true, true),
					       		new PassiveFeature(delegate(PassiveFeature f, Actor actor, double distance)
					       		                   {
					       		                   	if (Math.Abs(distance - 0) < Double.Epsilon)
					       		                   		actor.World.AddMessage(String.Format("{0} jumps on the sofa.  Whee!!", actor.Name));
					       		                   })
					       };
				case "OVEN":
					return new Template { new Position(0, 0), new Sprite("OVEN", 1), new Feature(false, false)};

				case "DOOR_GARAGE":
					return new Template { new Position(0, 0), new Sprite("DOOR_GARAGE", 1), new Feature(false, false)};
				case "FENCE_WOODEN":
					return new Template { new Position(0, 0), new Sprite("FENCE_WOODEN", 1), new Feature(false, true)};
				case "LAMP_STANDARD":
					return new Template { new Position(0, 0), new Sprite("LAMP_STANDARD"), new Feature(true, true)};
				case "TABLE_WOODEN":
					return new Template { new Position(0, 0), new Sprite("TABLE_WOODEN"), new Feature(true, true)};
				case "SAFE_SIMPLE":
					return new Template { new Position(0, 0), new Sprite("SAFE_SIMPLE"), new Feature(true, true)};

					#endregion

					#region Walls

				case "WALL_BRICK_DARK":
					return new Template { new Position(0, 0), new Sprite("WALL_BRICK_DARK", 1), new Feature(false, false)};
				case "WALL_BRICK_DARK_VERT":
					return new Template { new Position(0, 0), new Sprite("WALL_BRICK_DARK_VERT", 1), new Feature(false, false)};
				case "WALL_BRICK_DARK_HORZ":
					return new Template { new Position(0, 0), new Sprite("WALL_BRICK_DARK_HORZ", 1), new Feature(false, false)};
				case "WALL_BRICK_DARK_T_NORTH":
					return new Template { new Position(0, 0), new Sprite("WALL_BRICK_DARK_T_NORTH", 1), new Feature(false, false)};
				case "WALL_BRICK_DARK_T_SOUTH":
					return new Template { new Position(0, 0), new Sprite("WALL_BRICK_DARK_T_SOUTH", 1), new Feature(false, false)};
				case "WALL_BRICK_DARK_T_EAST":
					return new Template { new Position(0, 0), new Sprite("WALL_BRICK_DARK_T_EAST", 1), new Feature(false, false)};
				case "WALL_BRICK_DARK_T_WEST":
					return new Template { new Position(0, 0), new Sprite("WALL_BRICK_DARK_T_WEST", 1), new Feature(false, false)};
				case "WALL_BRICK_DARK_NORTHEAST":
					return new Template { new Position(0, 0), new Sprite("WALL_BRICK_DARK_NORTHEAST", 1), new Feature(false, false)};
				case "WALL_BRICK_DARK_NORTHWEST":
					return new Template { new Position(0, 0), new Sprite("WALL_BRICK_DARK_NORTHWEST", 1), new Feature(false, false)};
				case "WALL_BRICK_DARK_SOUTHWEST":
					return new Template { new Position(0, 0), new Sprite("WALL_BRICK_DARK_SOUTHWEST", 1), new Feature(false, false)};
				case "WALL_BRICK_DARK_SOUTHEAST":
					return new Template { new Position(0, 0), new Sprite("WALL_BRICK_DARK_SOUTHEAST", 1), new Feature(false, false)};
				case "WALL_DRY":
					return new Template { new Position(0, 0), new Sprite("WALL_DRY", 1), new Feature(false, false)};

					#endregion

					#region Stairs

				case "STAIR_WOODEN_UP":
					return new Template { new Position(0, 0), new Sprite("STAIR_WOODEN_UP"), new Feature(true, true)};
				case "STAIR_WOODEN_DOWN":
					return new Template { new Position(0, 0), new Sprite("STAIR_WOODEN_DOWN"), new Feature(true, true)};

					#endregion

					#region Misc Decorations

				case "PLANTPOT_FIXED":
					return new Template { new Position(0, 0), new Sprite("PLANTPOT_FIXED"), new Feature(true, true)};

					#endregion
			}

			return null;
		}
	}
}