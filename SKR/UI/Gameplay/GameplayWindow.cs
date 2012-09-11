using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using DEngine.Core;
using DEngine.Extensions;
using OGUI.Core;
using OGUI.UI;
using SKR.UI.Menus;
using SKR.Universe;
using SkrGame.Gameplay.Talent;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Actors.PC;
using SkrGame.Universe.Entities.Items;
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
                        HandleTalent(player.GetTalent(Skill.Range));

                    } else if (keyData.Character == 'l') {
                        ParentApplication.Push(new LookWindow(player.Position, MapPanel));
//                    } else if (keyData.Character == 'a') {
//                        HandleTalent(player.GetTalent(Skill.TargetAttack));
                    } else if (keyData.Character == 'd') {
                        ParentApplication.Push(new ItemWindow(false,
                                                              new ListWindowTemplate<Item>()
                                                                  {
                                                                          Size = MapPanel.Size,
                                                                          IsPopup = true,
                                                                          HasFrame = true,
                                                                          Items = player.Items,
                                                                  },
                                                              DropItem));
                    } else if (keyData.Character == 'g') {
                        ParentApplication.Push(new ItemWindow(false,
                                                              new ListWindowTemplate<Item>()
                                                              {
                                                                  Size = MapPanel.Size,
                                                                  IsPopup = true,
                                                                  HasFrame = true,
                                                                  Items = player.Level.Items.Where(tuple => tuple.Item1 == player.Position).Select(tuple => tuple.Item2),
                                                              },
                                                              PickUpItem));
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

        private void PickUpItem(Item item) {
            if (item.StackType == StackType.Hard)
                ParentApplication.Push(
                        new CountPrompt("How many items to pick up?",
                                        delegate(int amount)
                                            {
                                                if (amount < item.Amount)
                                                    item.Amount -= amount;
                                                else
                                                    player.Level.RemoveItem(item);

                                                // if an item doesn't exist in the inventory, create one
                                                if (!player.Items.ToList().Exists(i => i.RefId == item.RefId)) {
                                                    var tempItem = World.Instance.CreateItem(item.RefId);
                                                    tempItem.Amount += amount - 1;
                                                    player.AddItem(tempItem);
                                                } else
                                                    player.Items.First(i => i.RefId == item.RefId).Amount += amount;                                                

                                                // additional testing
                                                if (item.Amount < 0)
                                                    throw new Exception("Should not be possible: have negative amount");

                                            }, item.Amount, 0, item.Amount));
            else {
                player.Level.RemoveItem(item);
                player.AddItem(item);
            }
        }

        private void DropItem(Item item) {
            if (item.StackType == StackType.Hard)
                ParentApplication.Push(
                        new CountPrompt("How many items to drop to the ground?",
                                        delegate(int amount)
                                        {
                                            if (amount < item.Amount)
                                                item.Amount -= amount;
                                            else
                                                player.RemoveItem(item);

                                            // if an item doesn't exist in the at the location, create one
                                            if (!player.Level.Items.ToList().Exists(t => t.Item1 == player.Position && t.Item2.RefId == item.RefId)) {
                                                var tempItem = World.Instance.CreateItem(item.RefId);
                                                tempItem.Amount += amount - 1;
                                                player.Level.AddItem(tempItem, player.Position);
                                            } else
                                                player.Level.Items.First(t => t.Item1 == player.Position && t.Item2.RefId == item.RefId).Item2.Amount += amount;                                              

                                            // additional testing
                                            if (item.Amount < 0)
                                                throw new Exception("Should not be possible: have negative amount");

                                        }, item.Amount, 0, item.Amount));
            else {
                player.RemoveItem(item);
                player.Level.AddItem(item, player.Position);
            }
        }
    }
}
