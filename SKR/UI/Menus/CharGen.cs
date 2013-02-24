using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using Ogui.Core;
using Ogui.UI;
using SKR.UI.Gameplay;
using SkrGame.Actions.Items;
using SkrGame.Gameplay.Combat;
using SkrGame.Universe;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Controllers;
using SkrGame.Universe.Entities.Items;
using libtcod;

namespace SKR.UI.Menus {
	public class CharGen : SkrWindow {
		private ListBox occupationListBox;
		private MenuButton sexButton;
		private TextEntry nameEntry;

		public CharGen(SkrWindowTemplate template) : base(template) {}

		protected override void OnSettingUp() {
			base.OnSettingUp();

			var nameTemplate = new TextEntryTemplate
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
			                  		TopLeftPos = nameTemplate.CalculateRect().TopRight.Shift(2, 0),
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

				for (int j = 0; j < 10; j++) {
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

			var treeTemplate1 = new TreeViewTemplate
			                    {
			                    		Title = "Occupation",
			                    		TitleAlignment = HorizontalAlignment.Center,
			                    		MinimumListBoxWidth = 15,
			                    		FrameTitle = true,
			                    		AutoSizeOverride = new Size(15, 10),
			                    		Items = nodes.Where((n, i) => i % 10 == 0).ToList(),
			                    };

			treeTemplate1.AlignTo(LayoutDirection.South, treeTemplate, 1);
			AddControl(new TreeView(treeTemplate1));

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

			KeyPressed += CharGen_KeyPressed;
		}

		private void CharGen_KeyPressed(object sender, KeyboardEventArgs e) {
			if ((e.KeyboardData.KeyCode == TCODKeyCode.Enter || e.KeyboardData.KeyCode == TCODKeyCode.KeypadEnter) && !nameEntry.HasKeyboardFocus)
				StartNewGame(this, EventArgs.Empty);
		}


		private void StartNewGame(object sender, EventArgs e) {
			this.ExitWindow();
			World.CurrentLevel = World.MapFactory.Construct("TestHouse");

			var player = World.EntityManager.Create(new List<DEngine.Entities.Component>
			                                        {
			                                        		new Sprite("player", Sprite.PLAYER_LAYER),
			                                        		new Identifier("Player"),
			                                        		new GameObject(0, 0, World.CurrentLevel),
			                                        		new ActorComponent(new Player(), new AP()),
			                                        		new Person(),
			                                        		DefendComponent.CreateHuman(50),
			                                        		new ContainerComponent(),
			                                        		new EquipmentComponent(),
			                                        		new VisibleComponent(10),
			                                        		new SightComponent()
			                                        });

			player.Add(new MeleeComponent(new MeleeComponent.Template
			                              {
			                              		ActionDescription = "punch",
			                              		ActionDescriptionPlural = "punches",
			                              		Skill = "skill_unarmed",
			                              		HitBonus = 0,
			                              		Damage = Rand.Constant(-5),
			                              		DamageType = Combat.DamageTypes["crush"],
			                              		Penetration = 1,
			                              		WeaponSpeed = 100,
			                              		APToReady = 1,
			                              		Reach = 0,
			                              		Strength = 1,
			                              		Parry = 0
			                              }));

			World.Player = player;

			var npc = World.EntityManager.Create(new List<DEngine.Entities.Component>
			                                     {
			                                     		new Sprite("npc", Sprite.ACTOR_LAYER),
			                                     		new Identifier("npc"),
			                                     		new GameObject(6, 2, World.CurrentLevel),
			                                     		new ActorComponent(new DoNothing(), new AP()),
			                                     		new Person(),
			                                     		DefendComponent.CreateHuman(50),
			                                     		new VisibleComponent(10),
			                                     		new ContainerComponent(),
			                                     		new EquipmentComponent(),
			                                     });

			World.EntityManager.Create(World.EntityFactory.Get("smallknife")).Add(new GameObject(1, 1, World.CurrentLevel));
			World.EntityManager.Create(World.EntityFactory.Get("axe")).Add(new GameObject(1, 1, World.CurrentLevel));
			World.EntityManager.Create(World.EntityFactory.Get("glock17")).Add(new GameObject(1, 1, World.CurrentLevel));
			World.EntityManager.Create(World.EntityFactory.Get("lockpick")).Add(new GameObject(1, 1, World.CurrentLevel));
			var ammo = World.EntityManager.Create(World.EntityFactory.Get("9x9mm")).Add(new GameObject(1, 1, World.CurrentLevel));
			ammo.Get<Item>().Amount = 30;
			World.EntityManager.Create(World.EntityFactory.Get("bullet")).Add(new GameObject(1, 1, World.CurrentLevel));

			var armor = World.EntityManager.Create(World.EntityFactory.Get("footballpads")).Add(new GameObject(1, 1, World.CurrentLevel));
			new EquipItemAction(npc, armor, "Torso", true).OnProcess();
			npc.Add(new MeleeComponent(new MeleeComponent.Template
			                           {
			                           		ActionDescription = "punch",
			                           		ActionDescriptionPlural = "punches",
			                           		Skill = "skill_unarmed",
			                           		HitBonus = 0,
			                           		Damage = Rand.Constant(-5),
			                           		DamageType = Combat.DamageTypes["crush"],
			                           		Penetration = 1,
			                           		WeaponSpeed = 100,
			                           		APToReady = 1,
			                           		Reach = 0,
			                           		Strength = 1,
			                           		Parry = 0
			                           }));
			World.Initialize();

			ParentApplication.Push(new GameplayWindow(new SkrWindowTemplate()
			                                          {
			                                          		World = World
			                                          }));
		}
	}
}
