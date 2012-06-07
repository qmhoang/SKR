﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using DEngine.Core;
using DEngine.Extensions;
using DEngine.UI;
using SKR.Gameplay.Combat;
using SKR.Gameplay.Talent;
using SKR.UI.Menus;
using SKR.Universe;
using SKR.Universe.Entities.Actors;
using SKR.Universe.Entities.Actors.PC;
using SKR.Universe.Entities.Items;
using libtcod;

namespace SKR.UI.Gameplay {
    public class StatusPanel : Panel {
        private Player player;

        public StatusPanel(Player player, PanelTemplate template)
                : base(template) {
            this.player = player;
        }

        protected override void Redraw() {
            base.Redraw();

            Canvas.PrintString(1, 1, World.Instance.Player.Name);
            Canvas.PrintString(1, 3, String.Format("H: {0}/{1}", World.Instance.Player.Characteristics.Health,
                                                   World.Instance.Player.Characteristics.MaxHealth));
        }
    }

    public class MsgPanel : Panel {
        public string Message { get; set; }

        public MsgPanel(PanelTemplate template) : base(template) {
        }

        public void Clear() {
            Message = null;
        }

        protected override void Redraw() {
            base.Redraw();
            if (string.IsNullOrEmpty(Message))
                return;

            Canvas.PrintString(1, 1, "*", new Pigment(ColorPresets.Red, Pigments[PigmentType.ViewNormal].Background));
            Canvas.Console.printRect(3, 1, Size.Width - 4, Size.Height - 2, Message);
//            Canvas.PrintStringAligned(3, 1, Message, HorizontalAlignment.Left, VerticalAlignment.Top, new Size(Size.Width - 4, Size.Height - 2));           

        }
    }

    public class GameplayWindow : Window {       
        private readonly World world;
        private Player player;

        public MapPanel MapPanel { get; private set; }
        public StatusPanel StatusPanel { get; private set; }
        public MsgPanel MessagePanel { get; private set; }


        public GameplayWindow(WindowTemplate template)
            : base(template) {                       
            player = World.Instance.Player;
            world = World.Instance;
        }

        private void HandleOptions(Talent t, Actor target) {
            RecursiveSelectOptionHelper(t, target, 0, new dynamic[Talent.MaxOptions]);
        }

        /// <summary>
        /// This function will recursively check for options
        /// 
        /// </summary>
        /// <param name="t">The talent being used</param>
        /// <param name="target">the target the talent is being used on</param>
        /// <param name="level"></param>
        /// <param name="args"></param>
        private void RecursiveSelectOptionHelper(Talent t, Actor target, int level, dynamic[] args) {
            if (level > t.NumberOfArgs)
                throw new Exception("We have somehow recursed on more levels that there are options");

            var options = t.GetArgParameters(target, level).ToList();
            if (options.Count > 0)  // todo talent needs a failure delegate to say that we failed on options
                ParentApplication.Push(
                        new OptionsPrompt<dynamic>("Options Arg" + level,
                                                   options,
                                                   o => t.TransformArgToString(target, o, level),
                                                   delegate(dynamic arg)
                                                       {
                                                           args[level] = arg;
                                                           if (t.ContainsArg(level + 1)) {
                                                               RecursiveSelectOptionHelper(t, target, level + 1, args);
                                                           } else
                                                               t.InvokeAction(target, args[0], args[1], args[2], args[3]);

                                                       }
                                ));
            else 
                player.World.InsertMessage("No options possible in arg" + level);
        }

        private void HandleTalent(Talent talent) {
            if (talent.RequiresTarget)
                ParentApplication.Push(
                    new TargetPrompt(talent.Name, player.Position,
                                                    delegate (Point p)
                                                        {
                                                            Actor target = player.Level.GetActorAtLocation(p);
                                                            HandleOptions(talent, target);
                                                        }, MapPanel));
            else
                HandleOptions(talent, player);

//            if (talent.Action is TargetBodyPartWithWeaponAction) {
//                ParentApplication.Push(
//                                new TargetPrompt(
//                                        talent.Name, player.Position,
//                                        delegate(Point p)
//                                        {
//                                            Person target = player.Level.GetActorAtLocation(p);
//                                            List<ItemComponent> attacks = new List<ItemComponent>
//                                                                                   {
//                                                                                           player.Characteristics.Kick,
//                                                                                           player.Characteristics.Punch,
//                                                                                   };
//
//                                            foreach (var bodyPart in player.BodyParts.Where(bodyPart => player.IsItemEquipped(bodyPart.Type))) {
//                                                var item = player.GetItemAtBodyPart(bodyPart.Type);
//                                                if (item.ContainsComponent(ItemAction.MeleeAttackSwing))
//                                                    attacks.Add(item.GetComponent(ItemAction.MeleeAttackSwing) as MeleeComponent);
//                                                if (item.ContainsComponent(ItemAction.MeleeAttackThrust))
//                                                    attacks.Add(item.GetComponent(ItemAction.MeleeAttackThrust) as MeleeComponent);
//                                            }
//
//                                            ParentApplication.Push(
//                                                    new OptionsPrompt<ItemComponent>(
//                                                            String.Format("Attacking {0}", target.Name),
//                                                            attacks,
//                                                            c => c.Item != null
//                                                                    ? String.Format("{0} with {1}", c.ActionDescription, c.Item.Name)
//                                                                    : c.ActionDescription,
//                                                            c => ParentApplication.Push(
//                                                                    new OptionsPrompt<BodyPart>("Select location",
//                                                                                                target.Characteristics.BodyPartsList.ToList(),
//                                                                                                bp => bp.Name,
//                                                                                                bp => ((TargetBodyPartWithWeaponAction) talent.Action).Action(player, target, bp, c)))));
//                                        },
//                                        MapPanel));
//            } else if (talent.Action is TargetPersonAction) {
//                ParentApplication.Push(
//                                new TargetPrompt(
//                                        talent.Name, player.Position,
//                                        delegate(Point p)
//                                        {
//                                            Person target = player.Level.GetActorAtLocation(p);
//                                            ((TargetPersonAction) talent.Action).Action(player, target);
//                                        },
//                                        MapPanel));
//            }
        }

        protected override void OnSettingUp() {
            base.OnSettingUp();


            var mapTemplate = new PanelTemplate()
            {
                Size = new Size(Size.Width - 15, Size.Height - 10),
                HasFrame = false,
                TopLeftPos = new Point(0, 0),
            };

            MapPanel = new MapPanel(mapTemplate, player);

            AddControl(MapPanel);

            var statusTemplate = new PanelTemplate()
                                     {
                                             Size = new Size(Size.Width - mapTemplate.Size.Width, mapTemplate.Size.Height),
                                             HasFrame = true,

                                     };
            statusTemplate.AlignTo(LayoutDirection.East, mapTemplate);

            StatusPanel = new StatusPanel(player, statusTemplate);

            AddControl(StatusPanel);

            MessagePanel = new MsgPanel(new PanelTemplate()
                                           {
                                                   HasFrame = true,
                                                   Size = new Size(Size.Width, Size.Height - mapTemplate.Size.Height),
                                                   TopLeftPos = mapTemplate.CalculateRect().BottomRight.Shift(0, 1)
                                           });                  
            AddControl(MessagePanel);

            world.MessageAdded += (sender, arg) =>
                                      {
                                          MessagePanel.Message = arg.Data;
                                      };            
        }



        protected override void OnKeyPressed(KeyboardData keyData) {
            base.OnKeyPressed(keyData);
            MessagePanel.Clear();
            switch (keyData.KeyCode) {
                case TCODKeyCode.Up:
                case TCODKeyCode.KeypadEight:
                    player.Move(Point.North);
                    break;
                case TCODKeyCode.Down:
                case TCODKeyCode.KeypadTwo:
                    player.Move(Point.South);
                    break;
                case TCODKeyCode.Left:
                case TCODKeyCode.KeypadFour:
                    player.Move(Point.West);
                    break;
                case TCODKeyCode.KeypadFive:
                    player.Wait();
                    break;
                case TCODKeyCode.Right:
                case TCODKeyCode.KeypadSix:
                    player.Move(Point.East);
                    break;
                case TCODKeyCode.KeypadSeven:
                    player.Move(Point.Northwest);
                    break;
                case TCODKeyCode.KeypadNine:
                    player.Move(Point.Northeast);
                    break;
                case TCODKeyCode.KeypadOne:
                    player.Move(Point.Southwest);
                    break;
                case TCODKeyCode.KeypadThree:
                    player.Move(Point.Southeast);
                    break;
                default:
                    if (keyData.Character == 'w') {
                        ParentApplication.Push(new InventoryWindow(new ListWindowTemplate<BodyPart>
                                                                       {
                                                                               Size = MapPanel.Size,
                                                                               IsPopup = true,
                                                                               HasFrame = true,
                                                                               Items = player.BodyParts,
                                                                       }));
                    } else if (keyData.Character == 'r') {
                        HandleTalent(player.GetTalent(Skill.Reload));

                    } else if (keyData.Character == 'f') {
//                        HandleTalent(attack);
                        HandleTalent(player.GetTalent(Skill.RangeTargetAttack));

                    } else if (keyData.Character == 'a') {
                        HandleTalent(player.GetTalent(Skill.TargetAttack));
                    } else if (keyData.Character == 'i') {
                        ParentApplication.Push(new ItemWindow(false, new ListWindowTemplate<Item>
                                                                         {
                                                                             Size = MapPanel.Size,
                                                                             IsPopup = true,
                                                                             HasFrame = true,   
                                                                             Items = player.Items,
                                                                         }, i => player.World.InsertMessage(String.Format("This is a {0}, it weights {1}", i.Name, i.Weight))));
                    }

                    break;

            }

        }
    }
}
