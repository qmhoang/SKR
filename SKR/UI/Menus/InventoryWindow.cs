using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Core;
using DEngine.Extensions;
using DEngine.UI;
using SKR.Universe;
using SKR.Universe.Entities.Actor;
using SKR.Universe.Entities.Actor.PC;
using SKR.Universe.Entities.Items;
using libtcod;

namespace SKR.UI.Menus {
    public class ItemWindow : Window {
        #region ViewControl
//
//        private class ItemControlTemplate : ControlTemplate {
//            public Size Size { get; set; }
//            public bool HasFrameBorder { get; set; }
//            public List<Item> Items { get; set; }        
//
//            public override Size CalculateSize() {
//                return Size;
//            }
//        }
//
//        private class ItemControl : Control {
//            private Player player;
//            private int mouseOverIndex;
//            private int displayIndex;
//
//            private Dictionary<char, Item> items;
//
//            public ItemControl(Player player, ItemControlTemplate template)
//                    : base(template) {
//                this.player = player;
//                HasFrame = template.HasFrameBorder;
//                displayIndex = 0;
//                CanHaveKeyboardFocus = false;
//                items = new Dictionary<char, Item>();
//            }
//
//            protected override void OnSettingUp() {
//                base.OnSettingUp();
//
//                int count = 0;
//                foreach (var bodyPart in player.BodyParts) {
//                    items.Add((char) ('A' + count), bodyPart);
//                    count++;
//                }
//            }
//
//            protected override void OnMouseMoved(MouseData mouseData) {
//                base.OnMouseMoved(mouseData);
//
//                Point localPos = ScreenToLocal(mouseData.Position);
//                mouseOverIndex = localPos.Y / 2;
//            }
//
//            protected override void OnMouseButtonDown(MouseData mouseData) {
//                base.OnMouseButtonDown(mouseData);
//
//                if (mouseOverIndex >= 0 && mouseOverIndex < items.Count) {
//                    var bodyPart = items[(char) (mouseOverIndex + 'A')];
//                    selectIndex(bodyPart);
//                }
//            }
//
//            public void HandleKey(char key) {
//                char c = Char.ToUpper(key);
//                if (items.ContainsKey(c))
//                    selectIndex(items[c]);
//            }
//
//            private void selectIndex(Item bodyPart) {}
//
//            protected override void Redraw() {
//                base.Redraw();
//
////                int positionY = 0;
////                foreach (var bodyPart in items) {
////                    Canvas.PrintString(0, positionY, String.Format(String.Format("{0} - {1}", bodyPart.Key, bodyPart.Value.Name)));
////                    Canvas.PrintString(bodyPartWidth, positionY, ":");
////                    Canvas.PrintString(bodyPartWidth + 2, positionY, bodyPart.Value.Equipment == null ? "-" : bodyPart.Value.Equipment.Name);
////
////
////                    for (int i = 0; i < Canvas.Size.Width; i++) {
////                        if ((bodyPart.Key - 'A') == mouseOverIndex)
////                            Canvas.SetCharPigment(i, positionY, Pigments[PigmentType.ViewHilight]);
////                        else
////                            Canvas.SetCharPigment(i, positionY,
////                                                  bodyPart.Key % 2 == 0 ? Pigments[PigmentType.ViewNormal] : Pigments[PigmentType.ViewFocus]);
////                    }
////                    positionY += 2;
////                }
//            }
//        }

        #endregion

        private readonly Action<Item> itemSelected;
        private Dictionary<char, Item> itemsShortcut;
        private readonly IEnumerable<Item> itemsList;
        private readonly bool singleItem;
        private int mouseOverIndex;
        private int displayIndex;
        private static Point startingPoint = new Point(1, 5);

        public ItemWindow(IEnumerable<Item> items, bool selectSingleItem, WindowTemplate template, Action<Item> itemSelected)
            : base(template) {
            this.itemSelected = itemSelected;
            singleItem = selectSingleItem;
            this.itemsList = items;
            itemsShortcut = new Dictionary<char, Item>();
        }

        protected override void OnSettingUp() {
            base.OnSettingUp();            

            int count = 0;
            foreach (var item in itemsList) {
                itemsShortcut.Add((char)('A' + count), item);
                count++;
            }

            if (count <= 0) {
                World.Instance.InsertMessage("No items.");
                Quit();
            }
        }

        protected override void OnMouseMoved(MouseData mouseData) {
            base.OnMouseMoved(mouseData);

            Point localPos = mouseData.Position;
            mouseOverIndex = localPos.Y;
        }

        protected override void OnMouseButtonDown(MouseData mouseData) {
            base.OnMouseButtonDown(mouseData);

            if (mouseOverIndex >= 0 && mouseOverIndex < itemsShortcut.Count && itemsShortcut.ContainsKey((char)(mouseOverIndex + 'A'))) {
                var item = itemsShortcut[(char) (mouseOverIndex + 'A')];
                SelectItem(item);
            }
        }

        protected override void OnKeyPressed(KeyboardData keyData) {
            base.OnKeyPressed(keyData);

            char c = Char.ToUpper(keyData.Character);
            if (itemsShortcut.ContainsKey(c))
                SelectItem(itemsShortcut[c]);

            if (keyData.KeyCode == TCODKeyCode.Escape) {
                Quit();
            }
        }

        private void SelectItem(Item item) {
            itemSelected(item);            
            if (singleItem)
                Quit();
        }

        protected override void Redraw() {
            base.Redraw();

            int y = 0;
            foreach (var item in itemsShortcut) {
                Canvas.PrintString(startingPoint.X, startingPoint.Y + y, String.Format("{2}{0}{3} - {1}", item.Key, item.Value.Name, ColorPresets.Yellow.ForegroundCodeString, Color.StopColorCode));

                for (int i = 0; i < Canvas.Size.Width; i++) {
                    if ((item.Key - 'A') == mouseOverIndex)
                        Canvas.Console.setCharBackground(i, y, Pigments[PigmentType.ViewHilight].Background.TCODColor);
                    else
                        Canvas.Console.setCharBackground(i, y,
                                                         item.Key % 2 == 0
                                                                 ? Pigments[PigmentType.ViewNormal].Background.TCODColor
                                                                 : Pigments[PigmentType.ViewFocus].Background.TCODColor);
                }

                y++;
            }


        }
    }

    public class SelectableListTemplate<T> : ControlTemplate {
        public Size Size { get; set; }
        public bool HasFrameBorder { get; set; }
        public List<T> Items { get; set; } 

        public override Size CalculateSize() {
            return Size;
        }
    }

    public abstract class SelectableList<T> : Control {
        private Action<T> selectItem;
        private int mouseOverIndex;

        private Dictionary<char, T> items;

        public SelectableList(Action<T> selectItem, SelectableListTemplate<T> template)
            : base(template) {
            this.selectItem = selectItem;
            HasFrame = template.HasFrameBorder;
            CanHaveKeyboardFocus = false;
            items = new Dictionary<char, T>();

            int count = 0;
            foreach (var i in template.Items) {
                items.Add((char)('A' + count), i);
                count++;
            }
        }


        protected override void OnMouseMoved(MouseData mouseData) {
            base.OnMouseMoved(mouseData);

            Point localPos = ScreenToLocal(mouseData.Position);
            mouseOverIndex = localPos.Y / 2;
        }

        protected override void OnMouseButtonDown(MouseData mouseData) {
            base.OnMouseButtonDown(mouseData);

            if (mouseOverIndex >= 0 && mouseOverIndex < items.Count && items.ContainsKey((char)(mouseOverIndex + 'A'))) {
                var t = items[(char)(mouseOverIndex + 'A')];
                selectItem(t);
            }
        }

        public void HandleKey(char key) {
            char c = Char.ToUpper(key);
            if (items.ContainsKey(c))
                selectItem(items[c]);
        }

        protected override void Redraw() {
            base.Redraw();

            CustomDraw();
        }

        protected abstract void CustomDraw();
    }

    public class InventoryWindow : Window {
        #region BodyPart class
        private class SelectableListTemplate : ControlTemplate {
            public Size Size { get; set; }
            public bool HasFrameBorder { get; set; }
            public int BodyPartWidth { get; set; }

            public override Size CalculateSize() {
                return Size;
            }
        }

        private class BodyPartList : Control {
            private Player player;
            private int bodyPartWidth;
            private int mouseOverIndex;

            private Dictionary<char, BodyPart> bodyParts;

            public BodyPartList(Player player, SelectableListTemplate template)
                : base(template) {
                this.player = player;
                HasFrame = template.HasFrameBorder;
                bodyPartWidth = template.BodyPartWidth;                
                CanHaveKeyboardFocus = false;
                bodyParts = new Dictionary<char, BodyPart>();
            }

            protected override void OnSettingUp() {
                base.OnSettingUp();

                int count = 0;
                foreach (var bodyPart in player.BodyParts) {
                    bodyParts.Add((char)('A' + count), bodyPart);
                    count++;
                }
            }

            protected override void OnMouseMoved(MouseData mouseData) {
                base.OnMouseMoved(mouseData);

                Point localPos = ScreenToLocal(mouseData.Position);
                mouseOverIndex = localPos.Y / 2;
            }

            protected override void OnMouseButtonDown(MouseData mouseData) {
                base.OnMouseButtonDown(mouseData);

                Logger.Info(mouseOverIndex);

                if (mouseOverIndex >= 0 && mouseOverIndex < bodyParts.Count && bodyParts.ContainsKey((char)(mouseOverIndex + 'A'))) {            
                    var bodyPart = bodyParts[(char)(mouseOverIndex + 'A')];
                    selectIndex(bodyPart);
                }
            }

            public void HandleKey(char key) {
                char c = Char.ToUpper(key);
                if (bodyParts.ContainsKey(c))
                    selectIndex(bodyParts[c]);
            }

            private void selectIndex(BodyPart bodyPart) {
                if (player.IsSlotFull(bodyPart.Type)) {
                    if (!player.Unequip(bodyPart.Type))
                        World.Instance.InsertMessage("Unable to unequip item, maybe you are carrying too much.");
                } else {
                    var items = player.Items.Where(bodyPart.CanUseItem).ToList();                    
                    if (items.Count > 0)
                        ParentWindow.ParentApplication.Push(new ItemWindow(items, true,
                                                                           new WindowTemplate
                                                                               {
                                                                                       Size = Size,
                                                                                       IsPopup = ParentWindow.IsPopup,
                                                                               },
                                                                           delegate(Item i)
                                                                               {
                                                                                   if (!player.Equip(bodyPart.Type, i))
                                                                                       World.Instance.InsertMessage("Unable to equip item.");
                                                                               }));
                }

            }

            protected override void Redraw() {
                base.Redraw();

                int positionY = 0;
                foreach (var bodyPartPair in bodyParts) {
                    Canvas.PrintString(0, positionY, String.Format("{2}{0}{3} - {1}", bodyPartPair.Key, bodyPartPair.Value.Name, ColorPresets.Yellow.ForegroundCodeString, Color.StopColorCode));
                    Canvas.PrintString(bodyPartWidth, positionY, ":");
                    Canvas.PrintString(bodyPartWidth + 2, positionY, player.IsSlotFull(bodyPartPair.Value.Type) ? player.GetItemAtBodyPart(bodyPartPair.Value.Type).Name : "-");

                    for (int i = 0; i < Canvas.Size.Width; i++) {
                        if ((bodyPartPair.Key - 'A') == mouseOverIndex)
                            Canvas.Console.setCharBackground(i, positionY, Pigments[PigmentType.ViewHilight].Background.TCODColor);
                        else
                            Canvas.Console.setCharBackground(i, positionY,
                                                             bodyPartPair.Key % 2 == 0
                                                                     ? Pigments[PigmentType.ViewNormal].Background.TCODColor
                                                                     : Pigments[PigmentType.ViewFocus].Background.TCODColor);
                    }
                    


                    positionY += 2;
                }
            }
        }
    #endregion
        private Player player;
        private BodyPartList control;

        public InventoryWindow(WindowTemplate template) : base(template) {
            player = World.Instance.Player;
        }

        protected override void OnSettingUp() {
            base.OnSettingUp();

            control = new BodyPartList(player, new SelectableListTemplate
                                                      {
                                                              Size = new Size(Size.Width - 2, Size.Height - 10),
                                                              TopLeftPos = new Point(1, 5),
                                                              BodyPartWidth = 25,
                                                      });
            AddControl(control);
        }

        protected override void OnKeyPressed(KeyboardData keyData) {
            base.OnKeyPressed(keyData);


            control.HandleKey(keyData.Character);

            if (keyData.KeyCode == TCODKeyCode.Escape) {
                ParentApplication.RemoveWindow(this);
            }
        }
    }
}
