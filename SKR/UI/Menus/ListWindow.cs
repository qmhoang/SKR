using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Collections.Generic;
using DEngine.Core;
using DEngine.Extensions;
using Ogui.UI;
using SkrGame.Universe;

namespace SKR.UI.Menus {
	public class ListWindowTemplate<T> : SkrWindowTemplate {
		public IEnumerable<T> Items { get; set; }
	}

	public abstract class ListWindow<T> : SkrWindow {
		private readonly Action<T> _selectItem;
		protected int MouseOverIndex;

		/// <summary>
		/// The rectangle in which the list will be drawn
		/// </summary>
		protected abstract Rectangle ListRect { get; }

		protected readonly IEnumerable<T> List;

		protected ListWindow(Action<T> selectItem, ListWindowTemplate<T> template)
				: base(template) {
			Contract.Requires<ArgumentNullException>(template != null, "template");
			Contract.Requires<ArgumentNullException>(template.Items != null, "template.Items");

			this._selectItem = selectItem;
			HasFrame = template.HasFrame;
			List = template.Items;
		}

		[ContractInvariantMethod]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
		private void ObjectInvariant() {
			Contract.Invariant(List != null);
		}

		protected override void OnMouseMoved(MouseData mouseData) {
			base.OnMouseMoved(mouseData);
			MouseOverIndex = MouseToIndex(mouseData);
		}

		protected abstract int MouseToIndex(MouseData mouseData);

		protected override void OnMouseButtonUp(MouseData mouseData) {
			base.OnMouseButtonDown(mouseData);			

			int index = MouseToIndex(mouseData);
			if (index >= 0 && index < List.Count()) {
				var t = List.ElementAt(index);
				OnSelectItem(t);
			}
		}

		protected override void OnKeyPressed(KeyboardData keyData) {
			if (Char.IsLetter(keyData.Character)) {
				int index = Char.ToUpper(keyData.Character) - 'A';
				if (index < List.Count())
					OnSelectItem(List.ElementAt(index));
			}
		}

		protected override void Redraw() {
			base.Redraw();

			CustomDraw(ListRect);
		}

		protected override void Update() {
			base.Update();

			if (List.IsEmpty())
				ExitWindow();
		}

		protected abstract void CustomDraw(Rectangle rect);

		protected virtual void OnSelectItem(T item) {
			if (_selectItem != null)
				_selectItem(item);

			if (List.IsEmpty())
				ExitWindow();
		}
	}
}