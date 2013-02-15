using System;
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
using SkrGame.Actions;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;
using libtcod;

namespace SKR.UI.Menus {
	public class InventoryWindowTemplate : ListWindowTemplate<string> {
		
	}

	public class InventoryWindow : ListWindow<string> {
		private int bodyPartWidth;
		private ContainerComponent inventory;
		private EquipmentComponent equipment;
		private Entity player;

		private Rectangle sizeList;
		
		protected override Rectangle ListRect {
			get { return sizeList; }
		}

		public InventoryWindow(InventoryWindowTemplate template)
			: base(null, template) {
			Contract.Requires<ArgumentNullException>(template != null, "template");
			Contract.Requires<ArgumentNullException>(template.Items != null, "template.Items");
			Contract.Requires<ArgumentNullException>(template.World != null, "player");			

			inventory = template.World.Player.Get<ContainerComponent>();
			equipment = template.World.Player.Get<EquipmentComponent>();
			player = template.World.Player;
			bodyPartWidth = 25; // todo replace to code            
			sizeList = new Rectangle(new Point(1, 1), new Size(Size.Width - 2, Size.Height));
			
		}

		protected override void OnSelectItem(string slot) {
			if (equipment.IsSlotEquipped(slot)) {
				player.Get<ActorComponent>().Enqueue(new UnequipItem(player, slot));
			} else {
				var items = inventory.Items.Where(i => i.Has<Equipable>() && i.Get<Equipable>().Slots.Contains(slot)).ToList();
				if (items.Count > 0)
					ParentApplication.Push(new ItemWindow(new ItemWindowTemplate()
					                                      {
					                                      		Size = Size,
					                                      		IsPopup = IsPopup,
					                                      		HasFrame = true,
					                                      		Items = items,
					                                      		World = World,
					                                      		SelectSingleItem = true,
					                                      		ItemSelected = i => player.Get<ActorComponent>().Enqueue(new EquipItem(player, i, slot)),
					                                      }));					
				else
					World.Log.Normal("No items in inventory that go there.");
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

		protected override void CustomDraw(Rectangle rect) {
			int positionY = 0;
			char letter = 'A';
			foreach (var bodyPart in List) {
				Canvas.PrintString(rect.TopLeft.X, rect.TopLeft.Y + positionY, String.Format("{2}{0}{3} - {1}", letter, bodyPart, ColorPresets.Yellow.ForegroundCodeString, Color.StopColorCode));
				Canvas.PrintString(rect.TopLeft.X + bodyPartWidth, rect.TopLeft.Y + positionY, ":");

				if (equipment.IsSlotEquipped(bodyPart)) {
					var item = equipment.GetEquippedItemAt(bodyPart);			
					Canvas.PrintString(rect.TopLeft.X + bodyPartWidth + 2, rect.TopLeft.Y + positionY,
					                   String.Format("{0}{1}",
					                                 item.Get<Identifier>().Name,
					                                 (item.Has<RangeComponent>()
					                                  		? string.Format(" ({0}/{1})",
					                                  		                item.Get<RangeComponent>().ShotsRemaining, item.Get<RangeComponent>().Shots)
					                                  		: String.Empty)));
				} else {
					Canvas.PrintString(rect.TopLeft.X + bodyPartWidth + 2, rect.TopLeft.Y + positionY, "-");
				}


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
