using System;
using System.Linq;
using System.Collections.Generic;
using DEngine.Core;
using Ogui.UI;

namespace SKR.UI.Menus {
	public class ListWindowTemplate<T> : WindowTemplate {
		public ICollection<T> Items { get; set; }
	}

	public abstract class ListWindow<T> : Window {
		private readonly Action<T> selectItem;
		protected int MouseOverIndex;

		/// <summary>
		/// The rectangle in which the list will be drawn
		/// </summary>
		protected abstract Rect ListRect { get; }

//		protected Dictionary<char, T> Items;
		protected ICollection<T> List;

		protected ListWindow(Action<T> selectItem, ListWindowTemplate<T> template)
				: base(template) {
			this.selectItem = selectItem;
			HasFrame = template.HasFrame;
			List = template.Items;
//			Items = new Dictionary<char, T>();

//			int count = 0;
//			foreach (var i in template.Items) {
//				Items.Add((char) ('A' + count), i);
//				count++;
//			}
		}

		protected override void OnMouseMoved(MouseData mouseData) {
			base.OnMouseMoved(mouseData);
			MouseOverIndex = MouseToIndex(mouseData);
		}

		protected abstract int MouseToIndex(MouseData mouseData);		

		protected override void OnMouseButtonUp(MouseData mouseData) {
			base.OnMouseButtonDown(mouseData);			

			int index = MouseToIndex(mouseData);
			if (index >= 0 && index < List.Count) {				
				var t = List.ElementAt(index);
				OnSelectItem(t);
			}
		}

		protected override void OnKeyPressed(KeyboardData keyData) {
			if (Char.IsLetter(keyData.Character)) {
				int index = Char.ToUpper(keyData.Character) - 'A';
				if (index < List.Count)
					OnSelectItem(List.ElementAt(index));
			}
		}

		protected override void Redraw() {
			base.Redraw();

			CustomDraw(ListRect);
		}

		protected override void Update() {
			base.Update();

			if (List.Count() == 0)
				ExitWindow();
		}

		protected abstract void CustomDraw(Rect rect);

		protected virtual void OnSelectItem(T item) {
			selectItem(item);

			if (List.Count() <= 0)
				ExitWindow();
//			else {
//				Items.Clear();
//
//				int count = 0;
//				foreach (var i in List) {
//					Items.Add((char) ('A' + count), i);
//					count++;
//				}
//			}
		}
	}
}