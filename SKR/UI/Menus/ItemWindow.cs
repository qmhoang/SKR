using System;
using System.Diagnostics.Contracts;
using System.Linq;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using Ogui.Core;
using Ogui.UI;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Entities.Items.Equipables;
using libtcod;

namespace SKR.UI.Menus {
	public class ItemWindowTemplate : ListWindowTemplate<Entity> {
		public Action<Entity> ItemSelected { get; set; }
		public bool SelectSingleItem { get; set; }
	}

	public class ItemWindow : ListWindow<Entity> {
		private readonly bool _singleItem;
		private Rectangle _sizeList;	

		protected override Rectangle ListRect {
			get { return _sizeList; }
		}

		public ItemWindow(ItemWindowTemplate template)
				: base(template.ItemSelected, template) {
			Contract.Requires<ArgumentNullException>(template != null, "template");
			Contract.Requires(Contract.ForAll(template.Items, i => i.Has<Item>()));
			_singleItem = template.SelectSingleItem;            
			_sizeList = new Rectangle(new Point(1, 1), new Size(Size.Width - 2, Size.Height));
		}

		protected override int MouseToIndex(MouseData mouseData) {
			return mouseData.Position.Y - ListRect.Top;
		}

		protected override void CustomDraw(Rectangle rect) {
			int y = 0;
			char letter = 'A';
			foreach (var entity in List) {
				var item = entity.Get<Item>();

				Canvas.PrintString(rect.TopLeft.X, rect.TopLeft.Y + y, String.Format("{2}{0}{3} - {1}{4}", letter, entity.Get<Identifier>().Name, ColorPresets.Yellow.ForegroundCodeString, Color.StopColorCode,
				                                                                     entity.Has<RangeComponent>() ? string.Format(" ({0}/{1})", entity.Get<RangeComponent>().ShotsRemaining, entity.Get<RangeComponent>().Shots) : ""));

				Canvas.PrintString(rect.TopRight.X - 3, rect.TopLeft.Y + y, String.Format("x{0}", entity.Get<Item>().Amount));

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
				letter++;
			}
		}

		protected override void OnSettingUp() {
			base.OnSettingUp();
			
			if (List.Count() <= 0) {
				World.Log.Normal("No items.");
				ExitWindow();
			}
		}

		protected override void OnSelectItem(Entity item) {
			base.OnSelectItem(item);
			if (_singleItem)
				ExitWindow();
		}


		protected override void OnKeyPressed(KeyboardData keyData) {
			base.OnKeyPressed(keyData);

			if (keyData.KeyCode == TCODKeyCode.Escape) {
				ExitWindow();
			}
		}

	}
}