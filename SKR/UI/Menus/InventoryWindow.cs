using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Core;
using DEngine.Extensions;
using OGUI.Core;
using OGUI.UI;
using SKR.Universe;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Actors.PC;
using SkrGame.Universe.Entities.Items;
using libtcod;

namespace SKR.UI.Menus {
    public class ItemWindow : ListWindow<Item> {        
        private readonly bool singleItem;        
        private int displayIndex;   //todo scrolling
        private Rect sizeList;

        protected override Rect ListRect {
            get { return sizeList; }
        }

        public ItemWindow(bool selectSingleItem, ListWindowTemplate<Item> template, Action<Item> itemSelected)
            : base(itemSelected, template) {                       
            singleItem = selectSingleItem;            
            sizeList = new Rect(new Point(1, 1), new Size(Size.Width - 2, Size.Height));
        }

        protected override int MouseToIndex(MouseData mouseData) {
            return mouseData.Position.Y - ListRect.Top;
        }

        protected override void CustomDraw(Rect rect) {
            int y = 0;
            
            foreach (var item in Items) {
                Canvas.PrintString(rect.TopLeft.X, rect.TopLeft.Y + y, String.Format("{2}{0}{3} - {1}", item.Key, item.Value.Name, ColorPresets.Yellow.ForegroundCodeString, Color.StopColorCode));

                for (int i = 0; i < rect.Size.Width; i++) {
                    if ((item.Key - 'A') == MouseOverIndex)
                        Canvas.Console.setCharBackground(rect.TopLeft.X + i, rect.TopLeft.Y + y, Pigments[PigmentType.ViewHilight].Background.TCODColor);
                    else
                        Canvas.Console.setCharBackground(rect.TopLeft.X + i, rect.TopLeft.Y + y,
                                                         item.Key % 2 == 0
                                                                 ? Pigments[PigmentType.ViewNormal].Background.TCODColor
                                                                 : Pigments[PigmentType.ViewFocus].Background.TCODColor);
                }

                y++;
            }
        }

        protected override void OnSettingUp() {
            base.OnSettingUp();

            if (Items.Count <= 0) {
                World.Instance.InsertMessage("No items.");
                Quit();
            }
        }

        protected override void OnSelectItem(Item item) {
            base.OnSelectItem(item);
            if (singleItem)
                Quit();
        }


        protected override void OnKeyPressed(KeyboardData keyData) {
            base.OnKeyPressed(keyData);

            if (keyData.KeyCode == TCODKeyCode.Escape) {
                Quit();
            }
        }

    }

    public class InventoryWindow : ListWindow<BodyPart> {
        private int bodyPartWidth;
        private Player player;

        private Rect sizeList;

        protected override Rect ListRect {
            get { return sizeList; }
        }

        public InventoryWindow(ListWindowTemplate<BodyPart> template)
            : base(null, template) {
            player = World.Instance.Player;
            bodyPartWidth = 25; // todo replace to code            
            sizeList = new Rect(new Point(1, 1), new Size(Size.Width - 2, Size.Height));
        }

        private void SelectIndex(BodyPart bodyPart) {
            if (player.IsItemEquipped(bodyPart.Type)) {
                if (player.Unequip(bodyPart.Type) == ActionResult.Aborted)  //todo
                    World.Instance.InsertMessage("Unable to unequip item, maybe you are carrying too much.");
            } else {
                var items = player.Items.Where(bodyPart.CanUseItem).ToList();
                if (items.Count > 0)
                    ParentApplication.Push(new ItemWindow(true,
                                                          new ListWindowTemplate<Item>
                                                          {
                                                              Size = Size,
                                                              IsPopup = IsPopup,
                                                              HasFrame = true,
                                                              Items = items,
                                                          },
                                                          delegate(Item i)
                                                          {
                                                              if (player.Equip(bodyPart.Type, i) == ActionResult.Aborted) // todo
                                                                  World.Instance.InsertMessage("Unable to equip item.");
                                                          }));
                else
                    World.Instance.InsertMessage("No items in inventory that go there.");

            }

        }

        protected override void OnKeyPressed(KeyboardData keyData) {
            base.OnKeyPressed(keyData);


            if (keyData.KeyCode == TCODKeyCode.Escape) {
                WindowState = WindowState.Quitting;
            }
        }



        protected override int MouseToIndex(MouseData mouseData) {
            return mouseData.Position.Y / 2;
        }

        protected override void CustomDraw(Rect rect) {
            int positionY = 0;
            foreach (var bodyPartPair in Items) {
                Canvas.PrintString(rect.TopLeft.X, rect.TopLeft.Y + positionY, String.Format("{2}{0}{3} - {1}", bodyPartPair.Key, bodyPartPair.Value.Name, ColorPresets.Yellow.ForegroundCodeString, Color.StopColorCode));
                Canvas.PrintString(rect.TopLeft.X + bodyPartWidth, rect.TopLeft.Y + positionY, ":");
                Canvas.PrintString(rect.TopLeft.X + bodyPartWidth + 2, rect.TopLeft.Y + positionY, player.IsItemEquipped(bodyPartPair.Value.Type) ? player.GetItemAtBodyPart(bodyPartPair.Value.Type).Name : "-");

                for (int i = 0; i < rect.Size.Width; i++) {
                    if ((bodyPartPair.Key - 'A') == MouseOverIndex)
                        Canvas.Console.setCharBackground(rect.TopLeft.X + i, rect.TopLeft.Y + positionY, Pigments[PigmentType.ViewHilight].Background.TCODColor);
                    else
                        Canvas.Console.setCharBackground(rect.TopLeft.X + i, rect.TopLeft.Y + positionY,
                                                         bodyPartPair.Key % 2 == 0
                                                                 ? Pigments[PigmentType.ViewNormal].Background.TCODColor
                                                                 : Pigments[PigmentType.ViewFocus].Background.TCODColor);
                }



                positionY += 2;
            }
        }
    }
}
