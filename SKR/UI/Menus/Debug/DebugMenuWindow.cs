using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Core;
using Ogui.UI;
using libtcod;

namespace SKR.UI.Menus.Debug {
	public class DebugMenuWindow : SkrWindow {
		public DebugMenuWindow(SkrWindowTemplate template) : base(template) {
			
		}

		protected override void OnSettingUp() {
			base.OnSettingUp();

			_seeAllButton = new MenuButton(new MenuButtonTemplate()
									  {
										  Label = "See All",
										  VAlignment = VerticalAlignment.Top,
										  TopLeftPos = new Point(1, 1),
										  MinimumWidth = 20,
										  Tooltip = "Nothing will block vision, see the entire map.",
										  LabelAlignment = HorizontalAlignment.Left,
										  InitialSelection = Program.SeeAll.Enabled ? 0 : 1,
										  Items = new List<string>
										          {
										          		"On", "Off"
										          }
									  });

			_godModeButton = new MenuButton(new MenuButtonTemplate()
			                           {
			                           		Label = "God Mode",
			                           		VAlignment = VerticalAlignment.Top,
			                           		TopLeftPos = new Point(1, 4),
			                           		MinimumWidth = 20,
			                           		Tooltip = "Heal 1000 hp every second.",
			                           		LabelAlignment = HorizontalAlignment.Left,
											InitialSelection = Program.GodMod.Enabled ? 0 : 1,
											Items = new List<string>
										          {
										          		"On", "Off"
										          }
			                           });

			_freeWalkButton = new MenuButton(new MenuButtonTemplate()
			                                {
			                                		Label = "Free Walk",
			                                		VAlignment = VerticalAlignment.Top,
			                                		TopLeftPos = new Point(1, 7),
			                                		MinimumWidth = 20,
			                                		Tooltip = "Walk anywhere.",
			                                		LabelAlignment = HorizontalAlignment.Left,
			                                		InitialSelection = Program.FreeWalk.Enabled ? 0 : 1,
			                                		Items = new List<string>
			                                		        {
			                                		        		"On",
			                                		        		"Off"
			                                		        }
			                                });

			_seeAllButton.ButtonPushed += seeAllButton_ButtonPushed;
			_godModeButton.ButtonPushed += godModeButton_ButtonPushed;
			_freeWalkButton.ButtonPushed += freeWalkButton_ButtonPushed;

			AddControls(_seeAllButton, _godModeButton, _freeWalkButton);
		}
		
		protected override void OnKeyPressed(KeyboardData keyData) {
			base.OnKeyPressed(keyData);


			if (keyData.KeyCode == TCODKeyCode.Escape || keyData.Character == '`') {
				ExitWindow();
			}
		}

		void godModeButton_ButtonPushed(object sender, EventArgs e) {
			Program.GodMod.Enabled = !Program.GodMod.Enabled;			
		}

		void seeAllButton_ButtonPushed(object sender, EventArgs e) {
			Program.SeeAll.Enabled = !Program.SeeAll.Enabled;
		}

		void freeWalkButton_ButtonPushed(object sender, EventArgs e) {
			Program.FreeWalk.Enabled = !Program.FreeWalk.Enabled;
		}

		private Button _seeAllButton;
		private Button _godModeButton;
		private Button _freeWalkButton;

	}
}
