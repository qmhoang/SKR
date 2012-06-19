using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Core;
using DEngine.Extensions;
using SKR.Universe.Entities.Actors;
using SKR.Universe.Location;
using libtcod;

namespace SKR.Universe.Factories {
    public abstract class FeatureFactory : Factory<string, Feature> {
        
    }
    public class SourceFeatureFactory : FeatureFactory {
        public override Feature Construct(string key) {
            switch (key)
            {
                #region Doors & Windows
                case "Door":
                    return new Feature(key, "OpenedDoor").Add(new ActiveFeatureComponent("Close door", delegate(ActiveFeatureComponent f, Actor user)
                    {
                        if (f.Action == "Open door")
                        {
                            f.Owner.Asset = "OpenedDoor";
                            f.Action = "Close door";
                            f.Owner.Transparent = f.Owner.Walkable = true;
                            user.World.InsertMessage(String.Format("{0} opens the door.", user.Name));
                        }
                        else
                        {
                            if (!f.Owner.Level.DoesActorExistAtLocation(f.Owner.Position))
                            {
                                f.Owner.Asset = "ClosedDoor";
                                f.Action = "Open door";
                                f.Owner.Transparent = f.Owner.Walkable = false;
                                user.World.InsertMessage(String.Format("{0} closes the door.", user.Name));
                            }
                            else
                                user.World.InsertMessage("There's something blocking the way.");

                        }

                    }));
                case "WALL_BRICK_DARK_DOOR_HORZ":
                    return new Feature(key, "WALL_BRICK_DARK_DOOR_HORZ", false, false).Add(new ActiveFeatureComponent("Open door", delegate(ActiveFeatureComponent f, Actor user)
                    {
                        if (f.Action == "Open door")
                        {
                            f.Owner.Asset = "WALL_BRICK_DARK_DOOR_VERT";
                            f.Action = "Close door";
                            f.Owner.Transparent = f.Owner.Walkable = true;
                            user.World.InsertMessage(
                                    String.Format("{0} opens the door.", user.Name));
                        }
                        else
                        {
                            if (
                                    !f.Owner.Level.DoesActorExistAtLocation(
                                            f.Owner.Position))
                            {
                                f.Owner.Asset = "WALL_BRICK_DARK_DOOR_HORZ";
                                f.Action = "Open door";
                                f.Owner.Transparent = f.Owner.Walkable = false;
                                user.World.InsertMessage(
                                        String.Format("{0} closes the door.",
                                                      user.Name));
                            }
                            else
                                user.World.InsertMessage(
                                        "There's something blocking the way.");
                        }
                    }));
                case "WALL_BRICK_DARK_DOOR_VERT":
                    return new Feature(key, "WALL_BRICK_DARK_DOOR_VERT", false, false).Add(new ActiveFeatureComponent("Open door", delegate(ActiveFeatureComponent f, Actor user)
                    {
                        if (f.Action == "Open door")
                        {
                            f.Owner.Asset = "WALL_BRICK_DARK_DOOR_HORZ";
                            f.Action = "Close door";
                            f.Owner.Transparent = f.Owner.Walkable = true;
                            user.World.InsertMessage(
                                    String.Format("{0} opens the door.", user.Name));
                        }
                        else
                        {
                            if (
                                    !f.Owner.Level.DoesActorExistAtLocation(
                                            f.Owner.Position))
                            {
                                f.Owner.Asset = "WALL_BRICK_DARK_DOOR_VERT";
                                f.Action = "Open door";
                                f.Owner.Transparent = f.Owner.Walkable = false;
                                user.World.InsertMessage(
                                        String.Format("{0} closes the door.",
                                                      user.Name));
                            }
                            else
                                user.World.InsertMessage(
                                        "There's something blocking the way.");
                        }
                    }));
                case "WINDOW_BRICK_DARK_VERT":
                    return new Feature(key, "WINDOW_BRICK_DARK_VERT", false, false).Add(
                            new ActiveFeatureComponent("Open window", delegate(ActiveFeatureComponent f, Actor user)
                            {
                                if (f.Action == "Open window")
                                {
                                    f.Owner.Asset = "WINDOW_BRICK_DARK_HORZ";
                                    f.Action = "Close window";
                                    f.Owner.Transparent = true;
                                    user.World.InsertMessage(
                                            String.Format("{0} opens the window.", user.Name));
                                }
                                else
                                {
                                    f.Owner.Asset = "WINDOW_BRICK_DARK_VERT";
                                    f.Action = "Open window";
                                    f.Owner.Transparent = false;
                                    user.World.InsertMessage(
                                            String.Format("{0} closes the window.", user.Name));

                                }
                            }));
                case "WINDOW_BRICK_DARK_HORZ":
                    return new Feature(key, "WINDOW_BRICK_DARK_HORZ", false, false).Add(
                            new ActiveFeatureComponent("Open window", delegate(ActiveFeatureComponent f, Actor user)
                            {
                                if (f.Action == "Open window")
                                {
                                    f.Owner.Asset = "WINDOW_BRICK_DARK_VERT";
                                    f.Action = "Close window";
                                    f.Owner.Transparent = true;
                                    user.World.InsertMessage(
                                            String.Format("{0} opens the window.", user.Name));
                                }
                                else
                                {
                                    f.Owner.Asset = "WINDOW_BRICK_DARK_HORZ";
                                    f.Action = "Open window";
                                    f.Owner.Transparent = false;
                                    user.World.InsertMessage(
                                            String.Format("{0} closes the window.", user.Name));

                                }
                            }));
                case "WINDOW_HOUSE_VERT":
                    return new Feature(key, "WINDOW_HOUSE_VERT", false, false).Add(
                            new ActiveFeatureComponent("Open window", delegate(ActiveFeatureComponent f, Actor user)
                            {
                                if (f.Action == "Open window")
                                {
                                    f.Owner.Asset = "WINDOW_HOUSE_HORZ";
                                    f.Action = "Close window";
                                    f.Owner.Transparent = true;
                                    user.World.InsertMessage(
                                            String.Format("{0} opens the window.", user.Name));
                                }
                                else
                                {
                                    f.Owner.Asset = "WINDOW_HOUSE_VERT";
                                    f.Action = "Open window";
                                    f.Owner.Transparent = false;
                                    user.World.InsertMessage(
                                            String.Format("{0} closes the window.", user.Name));

                                }
                            }));
                case "WINDOW_HOUSE_HORZ":
                    return new Feature(key, "WINDOW_HOUSE_HORZ", false, false).Add(
                            new ActiveFeatureComponent("Open window", delegate(ActiveFeatureComponent f, Actor user)
                            {
                                if (f.Action == "Open window")
                                {
                                    f.Owner.Asset = "WINDOW_HOUSE_VERT";
                                    f.Action = "Close window";
                                    f.Owner.Transparent = true;
                                    user.World.InsertMessage(
                                            String.Format("{0} opens the window.", user.Name));
                                }
                                else
                                {
                                    f.Owner.Asset = "WINDOW_HOUSE_HORZ";
                                    f.Action = "Open window";
                                    f.Owner.Transparent = false;
                                    user.World.InsertMessage(
                                            String.Format("{0} closes the window.", user.Name));

                                }
                            }));
                case "DOOR_APART_1_VERT":
                    return new Feature(key, "DOOR_APART_1_VERT", false, false).Add(
                            new ActiveFeatureComponent("Open window", delegate(ActiveFeatureComponent f, Actor user)
                            {
                                if (f.Action == "Open window")
                                {
                                    f.Owner.Asset = "DOOR_APART_1_HORZ";
                                    f.Action = "Close window";
                                    f.Owner.Transparent = f.Owner.Walkable = true;

                                    user.World.InsertMessage(
                                            String.Format("{0} opens the window.", user.Name));
                                }
                                else
                                {
                                    f.Owner.Asset = "DOOR_APART_1_VERT";
                                    f.Action = "Open window";
                                    f.Owner.Transparent = f.Owner.Walkable = false;

                                    user.World.InsertMessage(
                                            String.Format("{0} closes the window.", user.Name));

                                }
                            }));
                case "DOOR_APART_1_HORZ":
                    return new Feature(key, "DOOR_APART_1_HORZ", false, false).Add(
                            new ActiveFeatureComponent("Open window", delegate(ActiveFeatureComponent f, Actor user)
                            {
                                if (f.Action == "Open window")
                                {
                                    f.Owner.Asset = "DOOR_APART_1_VERT";
                                    f.Action = "Close window";
                                    f.Owner.Transparent = f.Owner.Walkable = true;

                                    user.World.InsertMessage(
                                            String.Format("{0} opens the window.", user.Name));
                                }
                                else
                                {
                                    f.Owner.Asset = "DOOR_APART_1_HORZ";
                                    f.Action = "Open window";
                                    f.Owner.Transparent = f.Owner.Walkable = false;

                                    user.World.InsertMessage(
                                            String.Format("{0} closes the window.", user.Name));

                                }
                            }));
                #endregion

                #region House Features
                case "COUNTER_WOOD_RED":
                    return new Feature(key, "COUNTER_WOOD_RED", false, false);
                case "SINK":
                    return new Feature(key, "SINK").Add(new ActiveFeatureComponent("Wash hands", delegate(ActiveFeatureComponent f, Actor user)
                                                                                                     {
                                                                                                         //user.cleaniness += 5
                                                                                                     }));
                case "TOILET":
                    return new Feature(key, "TOILET", true, true).Add(new PassiveFeatureComponent(delegate(PassiveFeatureComponent f, Actor actor, double distance)
                    {
                        if (Math.Abs(distance - 0) < Program.Epsilon)
                            actor.World.InsertMessage(String.Format("{0} stands on top of the toilet.  Ew.",
                                                                    actor.Name));
                    }));
                case "BATHROOMSINK":
                    return new Feature(key, "BATHROOMSINK", false, false);


                case "BATH":
                    return new Feature(key, "BATH", true, true).Add(new PassiveFeatureComponent(delegate(PassiveFeatureComponent f, Actor actor, double distance)
                    {
                        if (Math.Abs(distance - 0) < Program.Epsilon)
                            actor.World.InsertMessage(String.Format("{0} steps into the bathtub.", actor.Name));
                    }));
                case "SHOWER":
                    return new Feature(key, "SHOWER", true, true);
                case "CHAIR_WOODEN":
                    return new Feature(key, "CHAIR_WOODEN", true, true);
                case "TREE_SMALL":
                    return new Feature(key, "TREE_SMALL", false, false);
                case "BED_WOODEN":
                    return new Feature(key, "BED_WOODEN", true, true).Add(new PassiveFeatureComponent(delegate(PassiveFeatureComponent f, Actor actor, double distance)
                    {
                        if (Math.Abs(distance - 0) < Program.Epsilon)
                            actor.World.InsertMessage(String.Format("{0} jumps on the bed.", actor.Name));
                    }));
                case "SHELF_WOOD":
                    return new Feature(key, "SHELF_WOOD", false, false);
                case "SHELF_METAL":
                    return new Feature(key, "SHELF_METAL", false, false);
                case "TELEVISION":
                    return new Feature(key, "TELEVISION", false, false).Add(new PassiveFeatureComponent(delegate(PassiveFeatureComponent f, Actor actor, double distance)
                    {
                        if (distance < 5)
                            actor.World.InsertMessage(String.Format("{0} hears the sound of television.",
                                                                    actor.Name));
                    }));
                case "FRIDGE":
                    return new Feature(key, "FRIDGE", false, false);
                case "DESK_WOODEN":
                    return new Feature(key, "DESK_WOODEN", false, false);
                case "CASH_REGISTER":
                    return new Feature(key, "CASH_REGISTER", false, false);
                case "SOFA":
                    return new Feature(key, "SOFA", true, true).Add(new PassiveFeatureComponent(delegate(PassiveFeatureComponent f, Actor actor, double distance)
                    {
                        if (Math.Abs(distance - 0) < Program.Epsilon)
                            actor.World.InsertMessage(String.Format("{0} jumps on the sofa.  Whee!!", actor.Name));
                    }));
                case "OVEN":
                    return new Feature(key, "OVEN", false, false);

                case "DOOR_GARAGE":
                    return new Feature(key, "DOOR_GARAGE", false, false);
                case "FENCE_WOODEN":
                    return new Feature(key, "FENCE_WOODEN", false, true);
                case "LAMP_STANDARD":
                    return new Feature(key, "LAMP_STANDARD", true, true);
                case "TABLE_WOODEN":
                    return new Feature(key, "TABLE_WOODEN", true, true);
                case "SAFE_SIMPLE":
                    return new Feature(key, "SAFE_SIMPLE", true, true);
                #endregion
                #region Walls
                case "WALL_BRICK_DARK":
                    return new Feature(key, "WALL_BRICK_DARK", false, false);
                case "WALL_BRICK_DARK_VERT":
                    return new Feature(key, "WALL_BRICK_DARK_VERT", false, false);
                case "WALL_BRICK_DARK_HORZ":
                    return new Feature(key, "WALL_BRICK_DARK_HORZ", false, false);
                case "WALL_BRICK_DARK_T_NORTH":
                    return new Feature(key, "WALL_BRICK_DARK_T_NORTH", false, false);
                case "WALL_BRICK_DARK_T_SOUTH":
                    return new Feature(key, "WALL_BRICK_DARK_T_SOUTH", false, false);
                case "WALL_BRICK_DARK_T_EAST":
                    return new Feature(key, "WALL_BRICK_DARK_T_EAST", false, false);
                case "WALL_BRICK_DARK_T_WEST":
                    return new Feature(key, "WALL_BRICK_DARK_T_WEST", false, false);
                case "WALL_BRICK_DARK_NORTHEAST":
                    return new Feature(key, "WALL_BRICK_DARK_NORTHEAST", false, false);
                case "WALL_BRICK_DARK_NORTHWEST":
                    return new Feature(key, "WALL_BRICK_DARK_NORTHWEST", false, false);
                case "WALL_BRICK_DARK_SOUTHWEST":
                    return new Feature(key, "WALL_BRICK_DARK_SOUTHWEST", false, false);
                case "WALL_BRICK_DARK_SOUTHEAST":
                    return new Feature(key, "WALL_BRICK_DARK_SOUTHEAST", false, false);
                case "WALL_DRY":
                    return new Feature(key, "WALL_DRY", false, false);
                #endregion

                #region Stairs
                case "STAIR_WOODEN_UP":
                    return new Feature(key, "STAIR_WOODEN_UP", true, true);
                case "STAIR_WOODEN_DOWN":
                    return new Feature(key, "STAIR_WOODEN_DOWN", true, true);
                #endregion
                #region Misc Decorations
                case "PLANTPOT_FIXED":
                    return new Feature(key, "PLANTPOT_FIXED", true, true);
                #endregion
            }

            return null;
        }
    }
}
