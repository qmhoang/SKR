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
using SkrGame.Actions;
using SkrGame.Actions.Items;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Entities.Items.Equipables;
using libtcod;

namespace SKR.UI.Menus {
	public class InventoryWindowTemplate : ListWindowTemplate<string> {
		
	}

	public class InventoryWindow : ListWindow<string> {
		private int _bodyPartWidth;
		private ContainerComponent _inventory;
		private EquipmentComponent _equipment;
		private Entity _player;

		private Rectangle _sizeList;
		
		protected override Rectangle ListRect {
			get { return _sizeList; }
		}

		public InventoryWindow(InventoryWindowTemplate template)
			: base(null, template) {
			Contract.Requires<ArgumentNullException>(template != null, "template");
			Contract.Requires<ArgumentNullException>(template.Items != null, "template.Items");
			Contract.Requires<ArgumentNullException>(template.World != null, "player");			

			_inventory = template.World.Player.Get<ContainerComponent>();
			_equipment = template.World.Player.Get<EquipmentComponent>();
			_player = template.World.Player;

			_bodyPartWidth = _equipment.Slots.Max(s => s.Length) + 5;  // todo replace to code            
			_sizeList = new Rectangle(new Point(1, 1), new Size(Size.Width - 2, Size.Height));
			
		}

		protected override void OnSelectItem(string slot) {
			if (_equipment.IsSlotEquipped(slot)) {
				_player.Get<ActorComponent>().Enqueue(new UnequipItemAction(_player, slot));
			} else {
				var items = _inventory.Items.Where(i => i.Has<Equipable>() && i.Get<Equipable>().SlotsOccupied.ContainsKey(slot)).ToList();
				if (items.Count > 0)
					ParentApplication.Push(new ItemWindow(new ItemWindowTemplate
					                                      {
					                                      		Size = Size,
					                                      		IsPopup = IsPopup,
					                                      		HasFrame = true,
					                                      		Items = items,
					                                      		World = World,
					                                      		SelectSingleItem = true,
					                                      		ItemSelected = item => Equip(item, slot),
					                                      }));					
				else
					World.Log.Aborted("No items in inventory that go there.");
			}

		}

		private void Equip(Entity item, string slot) {
			var canEquip = true;
			foreach (string slotNeeded in item.Get<Equipable>().SlotsOccupied[slot]) {
				if (_equipment.IsSlotEquipped(slotNeeded)) {
					canEquip = false;
					World.Log.AbortedFormat("{0} cannot go there, is takes multiple slots and {1} is already occupied.",
					                        Identifier.GetNameOrId(item),
					                        slotNeeded);

					break;
				}
			}

			if (canEquip)
				_player.Get<ActorComponent>().Enqueue(new EquipItemAction(_player, item, slot));
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
				Canvas.PrintString(rect.TopLeft.X + _bodyPartWidth, rect.TopLeft.Y + positionY, ":");

				if (_equipment.IsSlotEquipped(bodyPart)) {
					var item = _equipment.GetEquippedItemAt(bodyPart);			
					Canvas.PrintString(rect.TopLeft.X + _bodyPartWidth + 2, rect.TopLeft.Y + positionY,
					                   String.Format("{0}{1}",
					                                 item.Get<Identifier>().Name,
					                                 (item.Has<RangeComponent>()
					                                  		? string.Format(" ({0}/{1})",
					                                  		                item.Get<RangeComponent>().ShotsRemaining, item.Get<RangeComponent>().Shots)
					                                  		: String.Empty)));
				} else {
					Canvas.PrintString(rect.TopLeft.X + _bodyPartWidth + 2, rect.TopLeft.Y + positionY, "-");
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
