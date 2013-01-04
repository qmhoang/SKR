using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Core;
using DEngine.Entity;
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
	public class ItemWindow : ListWindow<Entity> {        
		private readonly bool singleItem;        
		private int displayIndex;   //todo scrolling
		private Rect sizeList;

		protected override Rect ListRect {
			get { return sizeList; }
		}

		public ItemWindow(bool selectSingleItem, ListWindowTemplate<Entity> template, Action<Entity> itemSelected)
			: base(itemSelected, template) {
			Contract.Requires(Contract.ForAll(template.Items, i => i.Has<Item>()));
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
				var entity = charItemPair.Value;
				var item = entity.Get<Item>();

				Canvas.PrintString(rect.TopLeft.X, rect.TopLeft.Y + y, String.Format("{2}{0}{3} - {1}{4}", letter, item.Name, ColorPresets.Yellow.ForegroundCodeString, Color.StopColorCode,
					entity.Has<RangeComponent>() ? string.Format(" ({0}/{1})", entity.Get<RangeComponent>().ShotsRemaining, entity.Get<RangeComponent>().Shots) : ""));

				Canvas.PrintString(rect.TopRight.X - 4, rect.TopLeft.Y + y, String.Format("x{0}", entity.Get<Item>().Amount));

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

		protected override void OnSelectItem(Entity item) {
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

	public class InventoryWindow : ListWindow<string> {
		private int bodyPartWidth;
		private ContainerComponent inventory;

		private Rect sizeList;

		protected override Rect ListRect {
			get { return sizeList; }
		}

		public InventoryWindow(ListWindowTemplate<string> template)
			: base(null, template) {
			inventory = World.Instance.Player.Get<ContainerComponent>();
			bodyPartWidth = 25; // todo replace to code            
			sizeList = new Rect(new Point(1, 1), new Size(Size.Width - 2, Size.Height));
			
		}

		protected override void OnSelectItem(string slot) {
			if (inventory.IsSlotEquipped(slot)) {
				inventory.Unequip(slot);
			} else {
				var items = inventory.Items.Where(i => i.Get<Item>().Slots.Contains(slot)).ToList();
				if (items.Count > 0)
					ParentApplication.Push(new ItemWindow(true,
					                                      new ListWindowTemplate<Entity>
					                                      {
					                                      		Size = Size,
					                                      		IsPopup = IsPopup,
					                                      		HasFrame = true,
					                                      		Items = items,
					                                      },
														  delegate(Entity i)
					                                      {
					                                      	inventory.Equip(slot, i);
					                                      	//															  if ( == ActionResult.Aborted) // todo
					                                      	//																  World.Instance.AddMessage("Unable to equip item.");
					                                      }));
				else
					World.Instance.AddMessage("No items in inventory that go there.");

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
				var item = inventory.GetItemAt(bodyPartPair.Value);				

				Canvas.PrintString(rect.TopLeft.X, rect.TopLeft.Y + positionY, String.Format("{2}{0}{3} - {1}", bodyPartPair.Key, bodyPartPair.Value, ColorPresets.Yellow.ForegroundCodeString, Color.StopColorCode));
				Canvas.PrintString(rect.TopLeft.X + bodyPartWidth, rect.TopLeft.Y + positionY, ":");
				Canvas.PrintString(rect.TopLeft.X + bodyPartWidth + 2, rect.TopLeft.Y + positionY, inventory.IsSlotEquipped(bodyPartPair.Value) ?
					String.Format("{0}{1}", item.Get<Item>().Name, (item.Has<RangeComponent>() ? string.Format(" ({0}/{1})", item.Get<RangeComponent>().ShotsRemaining, item.Get<RangeComponent>().Shots) : "")) : 
					"-");

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
