﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
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
			char letter = 'A';
			foreach (var entity in List) {								
				var item = entity.Get<Item>();

				Canvas.PrintString(rect.TopLeft.X, rect.TopLeft.Y + y, String.Format("{2}{0}{3} - {1}{4}", letter, entity.Get<Identifier>().Name, ColorPresets.Yellow.ForegroundCodeString, Color.StopColorCode,
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
				letter++;
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
		private EquipmentComponent equipment;

		private Rect sizeList;

		protected override Rect ListRect {
			get { return sizeList; }
		}

		public InventoryWindow(ListWindowTemplate<string> template)
			: base(null, template) {
			inventory = World.Instance.Player.Get<ContainerComponent>();
			equipment = World.Instance.Player.Get<EquipmentComponent>();
			bodyPartWidth = 25; // todo replace to code            
			sizeList = new Rect(new Point(1, 1), new Size(Size.Width - 2, Size.Height));
			
		}

		protected override void OnSelectItem(string slot) {
			if (equipment.IsSlotEquipped(slot)) {
				Entity removed;
				equipment.Unequip(slot, out removed);

				if (removed != null) {
					inventory.Add(removed);
				}
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
					                                      	equipment.Equip(slot, i);					                                      	
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
			char letter = 'A';
			foreach (var bodyPart in List) {
				var item = equipment.GetEquippedItemAt(bodyPart);				

				Canvas.PrintString(rect.TopLeft.X, rect.TopLeft.Y + positionY, String.Format("{2}{0}{3} - {1}", letter, bodyPart, ColorPresets.Yellow.ForegroundCodeString, Color.StopColorCode));
				Canvas.PrintString(rect.TopLeft.X + bodyPartWidth, rect.TopLeft.Y + positionY, ":");
				Canvas.PrintString(rect.TopLeft.X + bodyPartWidth + 2, rect.TopLeft.Y + positionY, equipment.IsSlotEquipped(bodyPart) ?
					String.Format("{0}{1}", item.Get<Identifier>().Name, (item.Has<RangeComponent>() ? string.Format(" ({0}/{1})", item.Get<RangeComponent>().ShotsRemaining, item.Get<RangeComponent>().Shots) : "")) : 
					"-");

				for (int i = 0; i < rect.Size.Width; i++) {
					if ((letter - 'A') == MouseOverIndex)
						Canvas.Console.setCharBackground(rect.TopLeft.X + i, rect.TopLeft.Y + positionY, Pigments[PigmentType.ViewHilight].Background.TCODColor);
					else
						Canvas.Console.setCharBackground(rect.TopLeft.X + i, rect.TopLeft.Y + positionY,
														 letter % 2 == 0
																 ? Pigments[PigmentType.ViewNormal].Background.TCODColor
																 : Pigments[PigmentType.ViewFocus].Background.TCODColor);
				}


				letter++;
				positionY += 2;
			}
		}
	}
}
