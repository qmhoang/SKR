using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using DEngine.Actions;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Actions.Features;
using SkrGame.Actions.Movement;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Features;
using SkrGame.Universe.Locations;

namespace SkrGame.Universe.Factories {
	public static class FeatureFactory {
		private const int WINDOW_USAGE_AP_COST = World.DEFAULT_SPEED;

		private static Opening Door(string openAsset, string closedAsset) {
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(closedAsset));
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(openAsset));
			return new Opening(openAsset, closedAsset, "opens the door", "closes the door");
		}

		private static Opening Window(string openAsset, string closedAsset) {
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(closedAsset));
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(openAsset));
			return new Opening(openAsset, closedAsset, "opens the window", "closes the window", false, WINDOW_USAGE_AP_COST);
		}

		public static OnBump.BumpResult DoorOnBump(Entity user, Entity door) {
			Contract.Requires<ArgumentNullException>(door != null, "door");
			Contract.Requires<ArgumentNullException>(user != null, "user");
			Contract.Requires<ArgumentException>(door.Has<Opening>());
			if (door.Get<Opening>().Status == Opening.OpeningStatus.Closed) {
				user.Get<ActorComponent>().Enqueue(new OpenDoorAction(user, door));	
				return OnBump.BumpResult.BlockMovement;
			} else {
				return OnBump.BumpResult.NormalMovement;
			}
		}

		public static OnBump.BumpResult WindowOnBump(Entity user, Entity door) {
			Contract.Requires<ArgumentNullException>(door != null, "door");
			Contract.Requires<ArgumentNullException>(user != null, "user");
			user.Get<ActorComponent>().Enqueue(new ToggleDoorAction(user, door));
			return OnBump.BumpResult.BlockMovement;
		}

		private static void DoorsAndWindows(EntityFactory ef) {
			ef.Inherits("Door", "feature",
			            new Sprite("ClosedDoor", Sprite.FEATURES_LAYER),
			            new Identifier("Door", "A basic door"),
			            Door("OpenedDoor", "ClosedDoor"),
			            new OnBump(DoorOnBump),
			            new UseableFeature(new List<UseableFeature.UseAction>
			                               {
			                               		new UseableFeature.UseAction("Use door", (user, featureEntity, action) =>
			                               		                                         	{
			                               		                                         		if (featureEntity.Has<Opening>())
			                               		                                         			user.Get<ActorComponent>().Enqueue(new ToggleDoorAction(user, featureEntity));
			                               		                                         		return ActionResult.Aborted;
			                               		                                         	})
			                               }));

			ef.Inherits("LockedDoor", "Door",
						new LockedFeature(0),
						new DefendComponent(10, new List<DefendComponent.AttackablePart>
						                        {
						                        		new DefendComponent.AttackablePart("Lock", 10, 10, -World.STANDARD_DEVIATION * 2)
						                        }));

			ef.Inherits("WALL_BRICK_DARK_DOOR_HORZ", "Door",
			            new Sprite("WALL_BRICK_DARK_DOOR_HORZ", Sprite.FEATURES_LAYER),
			            Door("WALL_BRICK_DARK_DOOR_VERT", "WALL_BRICK_DARK_DOOR_HORZ"));

			ef.Inherits("WALL_BRICK_DARK_DOOR_VERT", "Door",
			            new Sprite("WALL_BRICK_DARK_DOOR_VERT", Sprite.FEATURES_LAYER),
			            Door("WALL_BRICK_DARK_DOOR_HORZ", "WALL_BRICK_DARK_DOOR_VERT"));

			ef.Inherits("DOOR_APART_1_VERT", "Door",
			            new Sprite("DOOR_APART_1_VERT", Sprite.FEATURES_LAYER),
			            Door("DOOR_APART_1_HORZ", "DOOR_APART_1_VERT"));

			ef.Inherits("DOOR_APART_1_HORZ", "Door",
			            new Sprite("DOOR_APART_1_HORZ", Sprite.FEATURES_LAYER),
			            Door("DOOR_APART_1_VERT", "DOOR_APART_1_HORZ"));

			ef.Inherits("Window", "feature",
			            new Sprite("ClosedWindow", Sprite.FEATURES_LAYER),
			            Window("OpenedWindow", "ClosedWindow"),
			            new Identifier("Window", "A window door"),
			            new OnBump(WindowOnBump),
			            new UseableFeature(new List<UseableFeature.UseAction>
			                               {
			                               		new UseableFeature.UseAction("Use window", (user, featureEntity, action) =>
			                               		                                           	{
			                               		                                           		if (featureEntity.Has<Opening>())
			                               		                                           			user.Get<ActorComponent>().Enqueue(new ToggleDoorAction(user, featureEntity));
			                               		                                           		return ActionResult.Aborted;
			                               		                                           	})
			                               }));

			ef.Inherits("WINDOW_HOUSE_VERT", "Window",
			            new Sprite("WINDOW_HOUSE_VERT", Sprite.FEATURES_LAYER),
			            Window("WINDOW_HOUSE_HORZ", "WINDOW_HOUSE_VERT"));

			ef.Inherits("WINDOW_HOUSE_HORZ", "Window",
			            new Sprite("WINDOW_HOUSE_HORZ", Sprite.FEATURES_LAYER),
			            Window("WINDOW_HOUSE_VERT", "WINDOW_HOUSE_HORZ"));

			ef.Inherits("WINDOW_BRICK_DARK_VERT", "Window",
			            new Sprite("WINDOW_BRICK_DARK_VERT", Sprite.FEATURES_LAYER),
			            Window("WINDOW_BRICK_DARK_HORZ", "WINDOW_BRICK_DARK_VERT"));

			ef.Inherits("WINDOW_BRICK_DARK_HORZ", "Window",
			            new Sprite("WINDOW_BRICK_DARK_HORZ", Sprite.FEATURES_LAYER),
			            Window("WINDOW_BRICK_DARK_VERT", "WINDOW_BRICK_DARK_HORZ"));
		}

		public static void Walls(EntityFactory ef) {
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
		}

		public static void Init(EntityFactory ef) {
			Contract.Requires<ArgumentNullException>(ef != null, "ef");
			ef.Add("feature",
			       new VisibleComponent(10),
			       new Sprite("FEATURE", Sprite.FEATURES_LAYER),
			       new Identifier("Feature"),
			       new Scenery(false, false));

			ef.Inherits("nonblockingfeature", "feature", new Scenery());
			
			DoorsAndWindows(ef);

			#region House Features

			ef.Inherits("COUNTER_WOOD_RED", "feature",
			            new Sprite("COUNTER_WOOD_RED", Sprite.FEATURES_LAYER));

			ef.Inherits("SINK", "feature",
			            new Sprite("SINK", Sprite.FEATURES_LAYER),
						new Identifier("Sink", "A sink."),
			            new UseableFeature(new UseableFeature.UseAction("Wash hands",
			                                                            (entity, user, action) =>
			                                                            	{
//																				World.Instance.Log.Normal(String.Format("{0} uses the sink.", Identifier.GetNameOrId(user)));
			                                                            		return ActionResult.Success;
			                                                            	})));

			ef.Inherits("TOILET", "nonblockingfeature",
			            new Sprite("TOILET", Sprite.FEATURES_LAYER),
			            new UseableFeature(new UseableFeature.UseAction("Use toilet",
			                                                            (entity, user, action) =>
			                                                            	{
//																				World.Instance.Log.Normal(String.Format("{0} uses the toilet.", Identifier.GetNameOrId(user)));
			                                                            		return ActionResult.Success;
			                                                            	})),
			            new PassiveFeature(delegate(Entity entityNear, Entity featureEntity)
			                               	{
//			                               		var distanceTo = entityNear.Get<Location>().DistanceTo(featureEntity.Get<Location>());
//			                               		if (Math.Abs(distanceTo - 0) < Double.Epsilon)
//													World.Instance.Log.Normal(String.Format("{0} stands on top of the toilet.  Ew.",
//			                               			                                        Identifier.GetNameOrId(entityNear)));
			                               	}));

			ef.Inherits("BATHROOMSINK", "SINK",
			            new Sprite("BATHROOMSINK", Sprite.FEATURES_LAYER));

			ef.Inherits("BATH", "nonblockingfeature",
			            new Sprite("BATH", Sprite.FEATURES_LAYER),
			            new PassiveFeature(delegate(Entity entityNear, Entity featureEntity)
			                               	{
//			                               		if (Math.Abs(entityNear.Get<Location>().DistanceTo(featureEntity.Get<Location>()) - 0) < Double.Epsilon)
//													World.Instance.Log.Normal(String.Format("{0} steps into the bathtub.", Identifier.GetNameOrId(entityNear)));
			                               	}));

			ef.Inherits("SHOWER", "nonblockingfeature",
			            new Sprite("SHOWER", Sprite.FEATURES_LAYER));

			ef.Inherits("CHAIR_WOODEN", "nonblockingfeature",
			            new Sprite("CHAIR_WOODEN", Sprite.FEATURES_LAYER));

			ef.Inherits("TREE_SMALL", "nonblockingfeature",
			            new Sprite("TREE_SMALL", Sprite.FEATURES_LAYER));

			ef.Inherits("BED_WOODEN", "nonblockingfeature",
			            new Sprite("BED_WOODEN", Sprite.FEATURES_LAYER),
			            new PassiveFeature(delegate(Entity entityNear, Entity featureEntity)
			                               	{
//			                               		if (Math.Abs(entityNear.Get<Location>().DistanceTo(featureEntity.Get<Location>()) - 0) < Double.Epsilon)
//													World.Instance.Log.Normal(String.Format("{0} jumps on the bed.", Identifier.GetNameOrId(entityNear)));
			                               	}));

			ef.Inherits("SHELF_WOOD", "feature",
			            new Sprite("SHELF_WOOD", Sprite.FEATURES_LAYER));

			ef.Inherits("SHELF_METAL", "feature",
			            new Sprite("SHELF_METAL", Sprite.FEATURES_LAYER));

			ef.Inherits("TELEVISION", "feature",
			            new Sprite("TELEVISION", Sprite.FEATURES_LAYER),
			            new PassiveFeature(delegate(Entity entityNear, Entity featureEntity)
			                               	{
//			                               		if (entityNear.Get<Location>().DistanceTo(featureEntity.Get<Location>()) < 5)
//													World.Instance.Log.Normal(String.Format("{0} hears the sound of television.",
//			                               			                                        Identifier.GetNameOrId(entityNear)));
			                               	}));

			ef.Inherits("FRIDGE", "feature",
			            new Sprite("FRIDGE", Sprite.FEATURES_LAYER));

			ef.Inherits("DESK_WOODEN", "feature",
			            new Sprite("DESK_WOODEN", Sprite.FEATURES_LAYER));

			ef.Inherits("CASH_REGISTER", "feature",
			            new Sprite("CASH_REGISTER", Sprite.FEATURES_LAYER));

			ef.Inherits("SOFA", "nonblockingfeature",
			            new Sprite("SOFA", Sprite.FEATURES_LAYER),
			            new PassiveFeature(delegate(Entity entityNear, Entity featureEntity)
			                               	{
//			                               		if (Math.Abs(entityNear.Get<Location>().DistanceTo(featureEntity.Get<Location>()) - 0) < Double.Epsilon)
//													World.Instance.Log.Normal(String.Format("{0} jumps on the sofa.  Whee!!", Identifier.GetNameOrId(entityNear)));
			                               	}));

			ef.Inherits("OVEN", "feature",
			            new Sprite("OVEN", Sprite.FEATURES_LAYER));

			ef.Inherits("DOOR_GARAGE", "feature",
			            new Sprite("DOOR_GARAGE", Sprite.FEATURES_LAYER));

			ef.Inherits("FENCE_WOODEN", "feature",
			            new Sprite("FENCE_WOODEN", Sprite.FEATURES_LAYER), 
						new Scenery(true, false, 1),
						new OnBump(delegate (Entity user, Entity entity)
						           	{
										user.Get<ActorComponent>().Enqueue(new JumpOverAction(user, entity));
						           		return OnBump.BumpResult.BlockMovement;
						           	}));

			ef.Inherits("LAMP_STANDARD", "nonblockingfeature",
			            new Sprite("LAMP_STANDARD", Sprite.FEATURES_LAYER));

			ef.Inherits("TABLE_WOODEN", "nonblockingfeature",
			            new Sprite("TABLE_WOODEN", Sprite.FEATURES_LAYER));

			ef.Inherits("SAFE_SIMPLE", "nonblockingfeature",
			            new Sprite("SAFE_SIMPLE", Sprite.FEATURES_LAYER));

			#endregion

			Walls(ef);

			#region Stairs

			ef.Inherits("STAIR_WOODEN_UP", "feature", new Sprite("STAIR_WOODEN_UP", Sprite.FEATURES_LAYER), new Scenery(true, true));
			ef.Inherits("STAIR_WOODEN_DOWN", "feature", new Sprite("STAIR_WOODEN_DOWN", Sprite.FEATURES_LAYER), new Scenery(true, true));

			#endregion

			#region Misc Decorations

			ef.Inherits("PLANTPOT_FIXED", "feature", new Sprite("PLANTPOT_FIXED", Sprite.FEATURES_LAYER), new Scenery(true, true));

			#endregion
		}
	}
}