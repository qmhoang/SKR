using System;
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
        public AssetsManager AssetsManager { get; private set; }

        public GameplayWindow(WindowTemplate template)
            : base(template) {                       
            player = World.Instance.Player;
            world = World.Instance;
            AssetsManager = new AssetsManager();
        }

        private void HandleOptions(Talent t, Point target) {
            RecursiveSelectOptionHelper(t, target, 0, new dynamic[Talent.MaxOptions]);
        }

        /// <summary>
        /// This function will recursively check for options
        /// 
        /// </summary>
        /// <param name="t">The talent being used</param>
        /// <param name="target">the target the talent is being used on</param>
        /// <param name="index"></param>
        /// <param name="args"></param>
        private void RecursiveSelectOptionHelper(Talent t, Point target, int index, dynamic[] args) {
            if (index > t.NumberOfArgs)
                throw new Exception("We have somehow recursed on more levels that there are options");

            var options = t.GetArgsParameters(target, index);            
            if (options == null)
                player.World.InsertMessage("no possible options");                
            else {
                var optionsList = options.ToList();
                if (optionsList.Count == 1) {   // only one option, select it automatically
                    args[index] = optionsList[0];
                    if (t.ContainsArg(index + 1)) {
                        RecursiveSelectOptionHelper(t, target, index + 1, args);
                    } else
                        t.InvokeAction(target, args);
                } else if (optionsList.Count > 0)  // todo talent needs a failure delegate to say that we failed on options
                    ParentApplication.Push(
                            new OptionsPrompt<dynamic>("Options Arg" + index,
                                                       optionsList,
                                                       o => t.TransformArgToString(target, o, index),
                                                       delegate(dynamic arg)
                                                       {
                                                           args[index] = arg;
                                                           if (t.ContainsArg(index + 1)) {
                                                               RecursiveSelectOptionHelper(t, target, index + 1, args);
                                                           } else
                                                               t.InvokeAction(target, args);

                                                       }
                                    ));
                else
                    player.World.InsertMessage("No options possible in arg" + index);
            }
            
        }

        private void HandleTalent(Talent talent) {
            if (talent.RequiresTarget == TargetType.Positional)
                ParentApplication.Push(
                    new TargetPrompt(talent.Name, player.Position, p => HandleOptions(talent, p), MapPanel));
            else if (talent.RequiresTarget == TargetType.Directional) {
                ParentApplication.Push(
                    new DirectionalPrompt(talent.Name, player.Position, p => HandleOptions(talent, p)));
            } else
                HandleOptions(talent, player.Position);
        }

        protected override void OnSettingUp() {
            base.OnSettingUp();


            var mapTemplate = new PanelTemplate()
            {
                Size = new Size(Size.Width - 15, Size.Height - 10),
                HasFrame = false,
                TopLeftPos = new Point(0, 0),
            };

            MapPanel = new MapPanel(mapTemplate, player, AssetsManager);

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
                        HandleTalent(player.GetTalent(Skill.RangeTargetAttack));

                    } else if (keyData.Character == 'a') {
                        HandleTalent(player.GetTalent(Skill.TargetAttack));
                    } else if (keyData.Character == 'u') {
                        HandleTalent(player.GetTalent(Skill.UseFeature));
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
