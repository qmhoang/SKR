using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Core;
using DEngine.Extensions;
using DEngine.UI;
using SKR.Gameplay.Combat;
using SKR.UI.Menus;
using SKR.Universe;
using SKR.Universe.Entities.Actor;
using SKR.Universe.Entities.Actor.PC;
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
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
                        ParentApplication.Push(new InventoryWindow(new WindowTemplate
                                                                       {
                                                                               Size = MapPanel.Size,
                                                                               IsPopup = true,
                                                                               HasFrame = true,                                                                               
                                                                       }));
                    } else if (keyData.Character == 'a') {
                        Logger.Info("Pre push");
                        ParentApplication.Push(
                                new TargetPrompt("Attack", player.Position,
                                                 delegate(Point p)
                                                     {
                                                         Person actor = player.Level.GetActorAtLocation(p);
                                                         List<MeleeComponent> attacks = new List<MeleeComponent>
                                                                                            {
                                                                                                    player.Characteristics.Kick,
                                                                                                    player.Characteristics.Punch
                                                                                            };
                                                         
                                                         ParentApplication.Push(
                                                             new OptionsPrompt(
                                                                 String.Format("Attacking {0}", actor.Name), 
                                                                 attacks.Select(attack => attack.ActionDescription).ToList(), 
                                                                 delegate (int index)
                                                                     {
                                                                         var bps = actor.Characteristics.BodyPartsList.ToList();                                                                         
                                                                         ParentApplication.Push(
                                                                             new OptionsPrompt("Select location", 
                                                                                 bps.Select(bp => bp.Name).ToList(), 
                                                                                 i => MeleeCombat.AttackMeleeWithWeapon(player, actor, attacks[index], bps[i])));
                                                                     }));
                                                     }, 
                                                     MapPanel));
                        Logger.Info("Post push");
                    }

                    break;

            }

        }
    }
}
