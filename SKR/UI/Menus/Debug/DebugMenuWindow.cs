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

			seeAllButton = new MenuButton(new MenuButtonTemplate()
									  {
										  Label = "See All",
										  VAlignment = VerticalAlignment.Top,
										  TopLeftPos = new Point(1, 1),
										  MinimumWidth = 20,
										  Tooltip = "Nothing will block vision, see the entire map.",
										  LabelAlignment = HorizontalAlignment.Left,
										  Items = new List<string>
										          {
										          		"Off", "On"
										          }
									  });

			godModeButton = new MenuButton(new MenuButtonTemplate()
			                           {
			                           		Label = "God Mode",
			                           		VAlignment = VerticalAlignment.Top,
			                           		TopLeftPos = new Point(1, 4),
			                           		MinimumWidth = 20,
			                           		Tooltip = "Heal 1000 hp every second.",
			                           		LabelAlignment = HorizontalAlignment.Left,
											Items = new List<string>
										          {
										          		"Off", "On"
										          }
			                           });

			seeAllButton.ButtonPushed += seeAllButton_ButtonPushed;
			godModeButton.ButtonPushed += godModeButton_ButtonPushed;

			AddControls(seeAllButton, godModeButton);
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

		private Button seeAllButton;
		private Button godModeButton;
	}
}
