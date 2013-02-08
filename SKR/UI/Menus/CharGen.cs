﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Core;
using Ogui.Core;
using Ogui.UI;
using SKR.UI.Gameplay;
using SkrGame.Universe;
using libtcod;

namespace SKR.UI.Menus {
	public class CharGen : Window {
		private ListBox occupationListBox;
		private MenuButton sexButton;
		private TextEntry nameEntry;
		private World world;

		public CharGen(World world, WindowTemplate template) : base(template) {
			this.world = world;
		}

		protected override void OnSettingUp() {
			base.OnSettingUp();

			var nameTemplate = new TextEntryTemplate()
			                   {
			                   		Label = "Name: ",
			                   		MaximumCharacters = 20,
//                                  HasFrameBorder = false,
			                   		TopLeftPos = new Point(2, 2),
			                   		Tooltip = "Enter your character's name",
			                   		StartingField = "player",
			                   		CanHaveKeyboardFocus = true,
			                   		Pigments = new PigmentAlternatives()
			                   		           {
			                   		           		{PigmentType.FrameNormal, new Pigment(ColorPresets.White, ColorPresets.Black)},
			                   		           		{PigmentType.ViewNormal, new Pigment(ColorPresets.White, ColorPresets.Black)},
			                   		           		{PigmentType.ViewSelected, new Pigment(ColorPresets.Green, ColorPresets.Black)}
			                   		           }
			                   };
			nameEntry = new TextEntry(nameTemplate);

			AddControl(nameEntry);

			var jobs = new List<ListItemData>();

			for (int i = 0; i < 2; i++) {
				jobs.Add(new ListItemData("jobs" + i));
			}

			var occupationTemplate = new ListBoxTemplate()
			                         {
			                         		Title = "Occupation",
			                         		TitleAlignment = HorizontalAlignment.Center,
			                         		MinimumListBoxWidth = 15,
			                         		FrameTitle = true,
			                         		AutoSizeOverride = new Size(15, 10),
			                         		Items = jobs
			                         };
			occupationTemplate.AlignTo(LayoutDirection.South, nameTemplate);
			occupationListBox = new ListBox(occupationTemplate);
			AddControl(occupationListBox);
			jobs.Add(new ListItemData("test"));

			var sexTemplate = new MenuButtonTemplate()
			                  {
			                  		Label = "Sex",
//                                  MinimumWidth = 15,
			                  		TopLeftPos = nameTemplate.CalculateRect().TopRight.Shift(3, 0),
			                  		RightClickMenu = true,
//                                  Tooltip = "Click to switch sex, right click to choose from a menu",
			                  		Items = new List<string>()
			                  		        {
			                  		        		"Male",
			                  		        		"Female"
			                  		        },
			                  };
			sexButton = new MenuButton(sexTemplate);
			AddControl(sexButton);

			var nodes = new List<TreeNode>();
			int index = 0;
			for (int i = 0; i < 5; i++) {
				var treeNode = new TreeNode("category" + i);
				nodes.Add(treeNode);

				for (int j = 0; j < 7; j++) {
					treeNode.AddChild(new TreeNode("jobs" + index));
					index++;
				}
			}

			var treeTemplate = new TreeViewTemplate()
			                   {
			                   		Title = "Occupation",
			                   		TitleAlignment = HorizontalAlignment.Center,
			                   		MinimumListBoxWidth = 15,
			                   		FrameTitle = true,
			                   		AutoSizeOverride = new Size(15, 10),
			                   		Items = nodes
			                   };

			treeTemplate.AlignTo(LayoutDirection.South, sexTemplate, 1);
			AddControl(new TreeView(treeTemplate));

			var sliderTestTemplate = new SliderTemplate()
			                         {
			                         		Label = "Slider",
			                         		ShowLabel = true,
			                         		HasFrameBorder = false,
			                         		MinimumWidth = 22,
			                         		MaximumValue = 100,
			                         		StartingValue = 50,											
			                         };

			sliderTestTemplate.AlignTo(LayoutDirection.South, occupationTemplate);
			var sliderTest = new Slider(sliderTestTemplate);
			AddControl(sliderTest);


			var doneTemplate = new ButtonTemplate()
			                   {
			                   		Label = "Press [Enter] Finish",
			                   };
			doneTemplate.SetBottomRight(ScreenRect.BottomRight.Shift(-1, -1));
			var doneButton = new Button(doneTemplate);
			doneButton.ButtonPushed += StartNewGame;

			AddControl(doneButton);
		}

		private void StartNewGame(object sender, EventArgs e) {
			this.ExitWindow();

			ParentApplication.Push(new GameplayWindow(world, new WindowTemplate()));
		}

		protected override void OnKeyPressed(KeyboardData keyData) {
			if ((keyData.KeyCode == TCODKeyCode.Enter || keyData.KeyCode == TCODKeyCode.KeypadEnter) && !nameEntry.HasKeyboardFocus)
				StartNewGame(this, EventArgs.Empty);
			base.OnKeyPressed(keyData);

		}
	}
}
