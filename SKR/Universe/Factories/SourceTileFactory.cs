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
            switch (key) {
                case "BlockWall":
                    return new Feature(key, "Wall");
                    //                case "TestTrap":
                    //                    return new Feature(key, "Trap").Add(new AnotherWalk(a => a.World.InsertMessage("The tile squeaks")));
                case "Door":
                    return new Feature(key, "OpenedDoor").Add(new ActiveFeatureComponent("Close door", delegate(ActiveFeatureComponent f, Actor user)
                                                                                                           {
                                                                                                               if (f.Action == "Open door") {
                                                                                                                   f.Owner.Asset = "OpenedDoor";
                                                                                                                   f.Action = "Close door";
                                                                                                                   f.Owner.Transparent = f.Owner.Walkable = true;
                                                                                                                   user.World.InsertMessage(String.Format("{0} opens the door.", user.Name));
                                                                                                               } else {
                                                                                                                   if (!f.Owner.Level.DoesActorExistAtLocation(f.Owner.Position)) {
                                                                                                                       f.Owner.Asset = "ClosedDoor";
                                                                                                                       f.Action = "Open door";
                                                                                                                       f.Owner.Transparent = f.Owner.Walkable = false;
                                                                                                                       user.World.InsertMessage(String.Format("{0} closes the door.", user.Name));
                                                                                                                   } else
                                                                                                                       user.World.InsertMessage("There's something blocking the way.");

                                                                                                               }

                                                                                                           }));
                case "SINK":
                    return new Feature(key, "SINK").Add(new ActiveFeatureComponent("Wash hands", delegate(ActiveFeatureComponent f, Actor user)
                                                                                                     {
                                                                                                         //user.cleaniness += 5
                                                                                                     }));
                case "Tree":
                    return new Feature(key, "Tree", false, false);
                case "LockDoor":
                    return new Feature(key, "ClosedDoor", false, false).
                            Add(new ActiveFeatureComponent("Open door", delegate(ActiveFeatureComponent f, Actor user)
                                                                            {
                                                                                if (f.Action == "Open door") {
                                                                                    f.Owner.Asset = "OpenedDoor";
                                                                                    f.Action = "Close door";
                                                                                    f.Owner.Transparent = f.Owner.Walkable = true;
                                                                                    user.World.InsertMessage(String.Format("{0} opens the door.", user.Name));
                                                                                } else {
                                                                                    if (!f.Owner.Level.DoesActorExistAtLocation(f.Owner.Position)) {
                                                                                        f.Owner.Asset = "ClosedDoor";
                                                                                        f.Action = "Open door";
                                                                                        f.Owner.Transparent = f.Owner.Walkable = false;
                                                                                        user.World.InsertMessage(String.Format("{0} closes the door.", user.Name));
                                                                                    } else
                                                                                        user.World.InsertMessage("There's something blocking the way.");
                                                                                }
                                                                            }),
                                new ActiveFeatureComponent("Unlock", delegate(ActiveFeatureComponent f, Actor user)
                                                                         {

                                                                         }),
                                new SwitchFeaturecomponent());
                case "WALL_BRICK_DARK":
                    return new Feature(key, "WALL_BRICK_DARK", false, false);
                case "COUNTER_WOOD_RED":
                    return new Feature(key, "COUNTER_WOOD_RED", false, false);
                case "WALL_BRICK_DARK_DOOR":
                    return new Feature(key, "WALL_BRICK_DARK_DOOR_CLOSE", false, false).Add(new ActiveFeatureComponent("Open door", delegate(ActiveFeatureComponent f, Actor user)
                                                                                                                                        {
                                                                                                                                            if (f.Action == "Open door") {
                                                                                                                                                f.Owner.Asset = "WALL_BRICK_DARK_DOOR_OPEN";
                                                                                                                                                f.Action = "Close door";
                                                                                                                                                f.Owner.Transparent = f.Owner.Walkable = true;
                                                                                                                                                user.World.InsertMessage(
                                                                                                                                                        String.Format("{0} opens the door.", user.Name));
                                                                                                                                            } else {
                                                                                                                                                if (
                                                                                                                                                        !f.Owner.Level.DoesActorExistAtLocation(
                                                                                                                                                                f.Owner.Position)) {
                                                                                                                                                    f.Owner.Asset = "WALL_BRICK_DARK_DOOR_CLOSE";
                                                                                                                                                    f.Action = "Open door";
                                                                                                                                                    f.Owner.Transparent = f.Owner.Walkable = false;
                                                                                                                                                    user.World.InsertMessage(
                                                                                                                                                            String.Format("{0} closes the door.",
                                                                                                                                                                          user.Name));
                                                                                                                                                } else
                                                                                                                                                    user.World.InsertMessage(
                                                                                                                                                            "There's something blocking the way.");
                                                                                                                                            }
                                                                                                                                        }));
                case "WINDOW_BRICK_DARK":
                    return new Feature(key, "WINDOW_BRICK_DARK_CLOSE", false, false).Add(
                            new ActiveFeatureComponent("Open window", delegate(ActiveFeatureComponent f, Actor user)
                                                                          {
                                                                              if (f.Action == "Open window") {
                                                                                  f.Owner.Asset = "WINDOW_BRICK_DARK_OPEN";
                                                                                  f.Action = "Close window";
                                                                                  f.Owner.Transparent = true;
                                                                                  user.World.InsertMessage(
                                                                                          String.Format("{0} opens the window.", user.Name));
                                                                              } else {
                                                                                  f.Owner.Asset = "WINDOW_BRICK_DARK_CLOSE";
                                                                                  f.Action = "Open window";
                                                                                  f.Owner.Transparent = false;
                                                                                  user.World.InsertMessage(
                                                                                          String.Format("{0} closes the window.", user.Name));

                                                                              }
                                                                          }));
                case "TOILET":
                    return new Feature(key, "TOILET", true, true).Add(new PassiveFeatureComponent(delegate(PassiveFeatureComponent f, Actor actor, double distance)
                                                                                                      {
                                                                                                          if (Math.Abs(distance - 0) < Program.Epsilon)
                                                                                                              actor.World.InsertMessage(String.Format("{0} stands on top of the toilet.  Ew.",
                                                                                                                                                      actor.Name));
                                                                                                      }));
                case "CHAIR_WOODEN":
                    return new Feature(key, "CHAIR_WOODEN", true, true);
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
                case "STAIR_WOODEN_UP":
                    return new Feature(key, "STAIR_WOODEN_UP", true, true);
                case "STAIR_WOODEN_DOWN":
                    return new Feature(key, "STAIR_WOODEN_DOWN", true, true);
                case "DOOR_GARAGE":
                    return new Feature(key, "DOOR_GARAGE", false, false);
                case "FENCE_WOODEN":
                    return new Feature(key, "FENCE_WOODEN", false, true);
                case "LAMP_STANDARD":
                    return new Feature(key, "LAMP_STANDARD", true, true);
                case "TABLE_WOODEN":
                    return new Feature(key, "TABLE_WOODEN", true, true);
                case "PLANTPOT_FIXED":
                    return new Feature(key, "PLANTPOT_FIXED", true, true);
            }

            return null;
        }
    }
}
