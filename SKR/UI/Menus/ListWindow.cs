using System;
using System.Collections.Generic;
using DEngine.Core;
using DEngine.UI;

namespace SKR.UI.Menus {
    public class SelectableListTemplate<T> : WindowTemplate {        
        public List<T> Items { get; set; }
    }

    public abstract class ListWindow<T> : Window {
        protected Action<T> SelectItem;
        protected int MouseOverIndex;
        protected abstract Rect ListRect { get; }

        protected Dictionary<char, T> Items;

        protected ListWindow(Action<T> selectItem, SelectableListTemplate<T> template)
                : base(template) {
            this.SelectItem = selectItem;
            HasFrame = template.HasFrame;            
            Items = new Dictionary<char, T>();

            int count = 0;
            foreach (var i in template.Items) {
                Items.Add((char)('A' + count), i);
                count++;
            }
        }


        protected override void OnMouseMoved(MouseData mouseData) {
            base.OnMouseMoved(mouseData);
            MouseOverIndex = MouseToIndex(mouseData);
        }

        protected abstract int MouseToIndex(MouseData mouseData);

        protected override void OnMouseButtonDown(MouseData mouseData) {
            base.OnMouseButtonDown(mouseData);

            int index = MouseToIndex(mouseData);
            if (index >= 0 && index < Items.Count && Items.ContainsKey((char)(index + 'A'))) {
                var t = Items[(char)(index + 'A')];
                SelectItem(t);
            }
        }

        protected override void OnKeyPressed(KeyboardData keyData) {
            char c = Char.ToUpper(keyData.Character);
            if (Items.ContainsKey(c))
                SelectItem(Items[c]);
        }

        protected override void Redraw() {
            base.Redraw();

            CustomDraw(ListRect);
        }

        protected abstract void CustomDraw(Rect rect);
    }
}