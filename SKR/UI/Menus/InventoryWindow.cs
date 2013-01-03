using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Core;
using DEngine.Extensions;
using Ogui.Core;
using Ogui.UI;
using SKR.Universe;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Actors.PC;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Entities.Items.Components;
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
            
            foreach (var charItemPair in Items) {
                var letter = charItemPair.Key;
                var item = charItemPair.Value;

//                Canvas.PrintString(rect.TopLeft.X, rect.TopLeft.Y + y, String.Format("{2}{0}{3} - {1}{4}", letter, item.Name, ColorPresets.Yellow.ForegroundCodeString, Color.StopColorCode,
//                    item.Is(typeof(RangeComponent)) ? string.Format(" ({0}/{1})", item.As<RangeComponent>().ShotsRemaining, item.As<RangeComponent>().Shots) : ""));

                Canvas.PrintString(rect.TopRight.X - 4, rect.TopLeft.Y + y, String.Format("x{0}", item.Amount));

                for (int i = 0; i < rect.Size.Width; i++) {
                    if ((letter - 'A') == MouseOverIndex)
                        Canvas.Console.setCharBackground(rect.TopLeft.X + i, rect.TopLeft.Y + y, Pigments[PigmentType.ViewHilight].Background.TCODColor);
                    else
                        Canvas.Console.setCharBackground(rect.TopLeft.X + i, rect.TopLeft.Y + y,
                                                         letter % 2 == 0
                                                                 ? Pigments[PigmentType.ViewNormal].Background.TCODColor
                                                                 : Pigments[PigmentType.ViewFocus].Background.TCODColor);
                }

                y++;
            }
        }

        protected override void OnSettingUp() {
            base.OnSettingUp();

            if (List.Count() <= 0) {
                World.Instance.AddMessage("No items.");
                ExitWindow();
            }
        }

        protected override void OnSelectItem(Item item) {
            base.OnSelectItem(item);
            if (singleItem)
                ExitWindow();
        }


        protected override void OnKeyPressed(KeyboardData keyData) {
            base.OnKeyPressed(keyData);

            if (keyData.KeyCode == TCODKeyCode.Escape) {
                ExitWindow();
            }
        }

    }

//    public class InventoryWindow : ListWindow<BodyPart> {
//        private int bodyPartWidth;
//        private BodyComponent bodyParts;
//
//        private Rect sizeList;
//
//        protected override Rect ListRect {
//            get { return sizeList; }
//        }
//
//        public InventoryWindow(ListWindowTemplate<BodyPart> template)
//            : base(null, template) {
//            bodyParts = World.Instance.Player;
//            bodyPartWidth = 25; // todo replace to code            
//            sizeList = new Rect(new Point(1, 1), new Size(Size.Width - 2, Size.Height));
//            
//        }
//
//        protected override void OnSelectItem(BodyPart bodyPart) {
//            if (bodyParts.GetBodyPart(bodyPart.Type).Equipped) {
//                if (bodyParts.Unequip(bodyPart.Type) == ActionResult.Aborted)  //todo
//                    World.Instance.AddMessage("Unable to unequip item, maybe you are carrying too much.");
//            } else {
//                var items = bodyParts.Items.Where(bodyPart.CanUseItem).ToList();
//                if (items.Count > 0)
//                    ParentApplication.Push(new ItemWindow(true,
//                                                          new ListWindowTemplate<Item>
//                                                          {
//                                                              Size = Size,
//                                                              IsPopup = IsPopup,
//                                                              HasFrame = true,
//                                                              Items = items,
//                                                          },
//                                                          delegate(Item i)
//                                                          {
//                                                              if (bodyParts.Equip(bodyPart.Type, i) == ActionResult.Aborted) // todo
//                                                                  World.Instance.AddMessage("Unable to equip item.");
//                                                          }));
//                else
//                    World.Instance.AddMessage("No items in inventory that go there.");
//
//            }
//
//        }
//
//        protected override void OnKeyPressed(KeyboardData keyData) {
//            base.OnKeyPressed(keyData);
//
//
//            if (keyData.KeyCode == TCODKeyCode.Escape) {
//                WindowState = WindowState.Quitting;
//            }
//        }
//        
//        protected override int MouseToIndex(MouseData mouseData) {
//            return mouseData.Position.Y / 2;
//        }
//
//        protected override void CustomDraw(Rect rect) {
//            int positionY = 0;
//            foreach (var bodyPartPair in Items) {
//                var item = bodyPartPair.Value.Item;
//
//                Canvas.PrintString(rect.TopLeft.X, rect.TopLeft.Y + positionY, String.Format("{2}{0}{3} - {1}", bodyPartPair.Key, bodyPartPair.Value.Name, ColorPresets.Yellow.ForegroundCodeString, Color.StopColorCode));
//                Canvas.PrintString(rect.TopLeft.X + bodyPartWidth, rect.TopLeft.Y + positionY, ":");
//                Canvas.PrintString(rect.TopLeft.X + bodyPartWidth + 2, rect.TopLeft.Y + positionY, bodyPartPair.Value.Equipped ?
//                    String.Format("{0}{1}", item.Name, (item.Is(typeof(RangeComponent)) ? string.Format(" ({0}/{1})", item.As<RangeComponent>().ShotsRemaining, item.As<RangeComponent>().Shots) : "")) : 
//                    "-");
//
//                for (int i = 0; i < rect.Size.Width; i++) {
//                    if ((bodyPartPair.Key - 'A') == MouseOverIndex)
//                        Canvas.Console.setCharBackground(rect.TopLeft.X + i, rect.TopLeft.Y + positionY, Pigments[PigmentType.ViewHilight].Background.TCODColor);
//                    else
//                        Canvas.Console.setCharBackground(rect.TopLeft.X + i, rect.TopLeft.Y + positionY,
//                                                         bodyPartPair.Key % 2 == 0
//                                                                 ? Pigments[PigmentType.ViewNormal].Background.TCODColor
//                                                                 : Pigments[PigmentType.ViewFocus].Background.TCODColor);
//                }
//
//
//
//                positionY += 2;
//            }
//        }
//    }
}
