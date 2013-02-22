using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using DEngine.Actions;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using Ogui.Core;
using Ogui.UI;
using SKR.UI.Menus;
using SKR.Universe;
using SkrGame.Actions;
using SkrGame.Actions.Combat;
using SkrGame.Actions.Features;
using SkrGame.Actions.Items;
using SkrGame.Actions.Skills;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Features;
using SkrGame.Universe.Entities.Items;
using libtcod;
using log4net;
using Level = SkrGame.Universe.Locations.Level;

namespace SKR.UI.Gameplay {
	public class GameplayWindow : SkrWindow {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public MapPanel MapPanel { get; private set; }
		public StatusPanel StatusPanel { get; private set; }
		public LogPanel LogPanel { get; private set; }
		public AssetsManager AssetsManager { get; private set; }

		public PromptWindowTemplate PromptTemplate;

		private Entity player;

		public GameplayWindow(SkrWindowTemplate template)
				: base(template) {
			AssetsManager = new AssetsManager();

			player = template.World.Player;
		}

		protected override void OnSettingUp() {
			base.OnSettingUp();

			var mapTemplate =
					new PanelTemplate
					{
							Size = new Size(Size.Width - 15, Size.Height - 10),
							HasFrame = false,
							TopLeftPos = new Point(0, 0),
					};

			MapPanel = new MapPanel(World, AssetsManager, mapTemplate);

			AddControl(MapPanel);

			var statusTemplate =
					new PanelTemplate
					{
							Size = new Size(Size.Width - mapTemplate.Size.Width, mapTemplate.Size.Height),
							HasFrame = true,
					};
			statusTemplate.AlignTo(LayoutDirection.East, mapTemplate);

			StatusPanel = new StatusPanel(World, statusTemplate);

			AddControl(StatusPanel);

			var logTemplate = new LogPanelTemplate
			                  {
			                  		HasFrame = true,
			                  		Log = World.Log,
			                  		Size = new Size(Size.Width, Size.Height - mapTemplate.Size.Height),
			                  		TopLeftPos = mapTemplate.CalculateRect().BottomLeft
			                  };
			LogPanel = new LogPanel(logTemplate);
			AddControl(LogPanel);

			PromptTemplate =
					new PromptWindowTemplate
					{
							HasFrame = false,
							IsPopup = true,
							TopLeftPos = logTemplate.TopLeftPos.Shift(1, 1),
							Log = World.Log,
							Size = new Size(Size.Width - 2, Size.Height - mapTemplate.Size.Height - 2)
					};

			this.KeyPressed += GameplayWindow_KeyPressed;			
		}

		private void GameplayWindow_KeyPressed(object sender, KeyboardEventArgs keyboardEvent) {
			var playerLocation = player.Get<GameObject>();
			
			switch (keyboardEvent.KeyboardData.KeyCode) {
				case TCODKeyCode.Up:
				case TCODKeyCode.KeypadEight: // Up and 8 should have the same functionality
					Move(Direction.North);
					break;
				case TCODKeyCode.Down:
				case TCODKeyCode.KeypadTwo:
					Move(Direction.South);
					break;
				case TCODKeyCode.Left:
				case TCODKeyCode.KeypadFour:
					Move(Direction.West);
					break;
				case TCODKeyCode.KeypadFive:
					Enqueue(new WaitAction(player));
					break;
				case TCODKeyCode.Right:
				case TCODKeyCode.KeypadSix:
					Move(Direction.East);
					break;
				case TCODKeyCode.KeypadSeven:
					Move(Direction.Northwest);
					break;
				case TCODKeyCode.KeypadNine:
					Move(Direction.Northeast);
					break;
				case TCODKeyCode.KeypadOne:
					Move(Direction.Southwest);
					break;
				case TCODKeyCode.KeypadThree:
					Move(Direction.Southeast);
					break;
				default:
				{
					switch (keyboardEvent.KeyboardData.Character) {
						case 'f':
						{
							Options(FilterEquippedItems<RangeComponent>(player).ToList(),
							        Identifier.GetNameOrId,
							        weapon => Targets(p => Options(player.Get<GameObject>().Level.GetEntitiesAt(p).Where(e => e.Has<DefendComponent>()),
							                                       Identifier.GetNameOrId,
							                                       defender => Enqueue(
							                                       		new RangeAttackAction(player,
							                                       		                      defender,
							                                       		                      weapon,
							                                       		                      defender.Get<DefendComponent>().GetRandomPart())),
							                                       "Shoot at what?",
							                                       "Nothing there to shoot."),
							                          "Shoot where?"),
							        "With what weapon?",
							        "No possible way of shooting target.");

						}
							break;
						case 'F':
						{
							Options(FilterEquippedItems<RangeComponent>(player).ToList(),
							        Identifier.GetNameOrId,
							        weapon => Targets(p => Options(player.Get<GameObject>().Level.GetEntitiesAt(p).Where(e => e.Has<DefendComponent>()),
							                                       Identifier.GetNameOrId,
							                                       defender => Options(defender.Get<DefendComponent>().BodyPartsList,
							                                                           bp => bp.Name,
							                                                           bp => Enqueue(new RangeAttackAction(player,
							                                                                                                                            defender,
							                                                                                                                            weapon,
							                                                                                                                            bp,
							                                                                                                                            true)),
							                                                           "What part?",
							                                                           String.Format("{0} has no possible part to attack.  How did we get here?", Identifier.GetNameOrId(defender))),
							                                       "Shoot at what?",
							                                       "Nothing there to shoot."),
							                          "Shoot where?"),
							        "With what weapon?",
							        "No possible way of shooting target.");

						}
							break;
						case 'r':
						{
							Options(FilterEquippedItems<RangeComponent>(player).ToList(),
							        Identifier.GetNameOrId,
							        weapon => Options(player.Get<ContainerComponent>().Items.Where(e => e.Has<AmmoComponent>() &&
							                                                                            e.Get<AmmoComponent>().Type == weapon.Get<RangeComponent>().AmmoType).ToList(),
							                          Identifier.GetNameOrId,
							                          ammo => Enqueue(new ReloadAction(player, weapon, ammo)),
							                          "What ammo?",
							                          "No possible ammo for selected weapon."),
							        "Reload what weapon?",
							        "No weapons to reload.");
						}
							break;
						case 'u':
							Directions(p => Options(playerLocation.Level.GetEntitiesAt(p).Where(e => e.Has<UseableFeature>() &&
							                                                                         e.Has<VisibleComponent>() &&
							                                                                         e.Get<VisibleComponent>().VisibilityIndex > 0),
							                        Identifier.GetNameOrId,
							                        useable => Options(useable.Get<UseableFeature>().Uses.ToList(),
							                                           use => use.Description,
							                                           use => use.Use(player, useable, use),
							                                           String.Format("Do what with {0}?", Identifier.GetNameOrId(useable)),
							                                           String.Format("No possible action on {0}", Identifier.GetNameOrId(useable))),
							                        "What object do you want to use?",
							                        "Nothing there to use."),
							           "What direction?");
							break;
						case 'd':
						{
							var inventory = player.Get<ContainerComponent>();
							if (inventory.Count > 0)
								ParentApplication.Push(new ItemWindow(new ItemWindowTemplate
								                                      {
								                                      		Size = MapPanel.Size,
								                                      		IsPopup = true,
								                                      		HasFrame = true,
								                                      		World = World,
								                                      		Items = inventory.Items,
								                                      		SelectSingleItem = false,
								                                      		ItemSelected = i => DropItem(player, i),
								                                      }));
							else
								World.Log.Fail("You are carrying no items to drop.");
						}
							break;
						case 'g':
						{
							var inventory = player.Get<ContainerComponent>();

							// get all items that have a location (eg present on the map) that are at the location where are player is
							var items = playerLocation.Level.GetEntitiesAt(playerLocation.Location).Where(e => e.Has<Item>() &&
							                                                                                e.Has<VisibleComponent>() &&
							                                                                                e.Get<VisibleComponent>().VisibilityIndex > 0 &&
							                                                                                !inventory.Items.Contains(e));

							if (items.Count() > 0)
								ParentApplication.Push(new ItemWindow(new ItemWindowTemplate
								                                      {
								                                      		Size = MapPanel.Size,
								                                      		IsPopup = true,
								                                      		HasFrame = true,
								                                      		World = World,
								                                      		Items = items,
								                                      		SelectSingleItem = false,
								                                      		ItemSelected = i => PickUpItem(player, i),
								                                      }));
							else
								World.Log.Fail("No items here to pick up.");
						}
							break;
						case 'G':
						{
							var inventory = player.Get<ContainerComponent>();

							// get all items that have a location (eg present on the map) that are at the location where are player is
							var items = playerLocation.Level.GetEntitiesAt(playerLocation.Location).Where(e => e.Has<Item>() &&
							                                                                                e.Has<VisibleComponent>() &&
							                                                                                e.Get<VisibleComponent>().VisibilityIndex > 0 &&
							                                                                                !inventory.Items.Contains(e));

							if (items.Count() > 0)
								Enqueue(new GetAllItemsAction(player, items));
							else
								World.Log.Fail("No items here to pick up.");
						}
							break;
						case 'i':
						{
							var inventory = player.Get<ContainerComponent>();
							ParentApplication.Push(new ItemWindow(new ItemWindowTemplate
							                                      {
							                                      		Size = MapPanel.Size,
							                                      		IsPopup = true,
							                                      		HasFrame = true,
							                                      		World = World,
							                                      		Items = inventory.Items,
							                                      		SelectSingleItem = false,
							                                      		ItemSelected = i => World.Log.Normal(String.Format("This is a {0}, it weights {1}.", i.Get<Identifier>().Name, i.Get<Item>().Weight)),
							                                      }));

						}
							break;
						case 'w':
							ParentApplication.Push(new InventoryWindow(new InventoryWindowTemplate
							                                           {
							                                           		Size = MapPanel.Size,
							                                           		IsPopup = true,
							                                           		HasFrame = true,
							                                           		World = World,
							                                           		Items = player.Get<EquipmentComponent>().Slots.ToList(),
							                                           }));
							break;
						case 'p':
						{
							Options(player.Get<ContainerComponent>().Items.Where(i => i.Has<Lockpick>()),
							        Identifier.GetNameOrId,
							        lockpick => Directions(p => Options(player.Get<GameObject>().Level.GetEntitiesAt(p).Where(e => e.Has<LockedFeature>()),
							                                            Identifier.GetNameOrId,
							                                            feature => Enqueue(new LockpickAction(player, lockpick, feature)),
							                                            "Select what to lockpick.",
							                                            "Nothing there to lockpick."),
							                               "What direction?"),
							        "With what?",
							        "No lockpicks available.");

						}
							break;
						case 'l':
							if (keyboardEvent.KeyboardData.ControlKeys == ControlKeys.LeftControl) {
								//
							} else
								ParentApplication.Push(
										new LookWindow(
												playerLocation.Location,
												delegate(Point p)
													{
														StringBuilder sb = new StringBuilder();
														var entitiesAtLocation = playerLocation.Level.GetEntitiesAt(p);
														sb.AppendLine(playerLocation.Level.GetTerrain(p).Definition);
														foreach (var entity in entitiesAtLocation) {
															sb.AppendFormat("Entity: {0} ", entity.Id);
															sb.AppendFormat("Name: {0} ", Identifier.GetNameOrId(entity));
															if (entity.Has<Blocker>())
																sb.AppendFormat("Transparent: {0}, Walkable: {1} ", entity.Get<Blocker>().Transparent, entity.Get<Blocker>().Walkable);

															sb.AppendLine();
														}

														return sb.ToString();
													},
												MapPanel,
												PromptTemplate));
							break;
						case 'z':
							if (keyboardEvent.KeyboardData.ControlKeys == ControlKeys.LeftControl) {
								Targets(p =>
								        	{
								        		var entitiesAtLocation = playerLocation.Level.GetEntitiesAt(p).Where(e => e.Has<ActorComponent>()).ToList();
								        		var npc = entitiesAtLocation.First();
								        		var options = new List<AbstractActor>
								        		              {
								        		              		new DoNothingActor(),
								        		              		new NPC()
								        		              };
								        		Options(options,
								        		        o => o.GetType().Name,
								        		        actor =>
								        		        	{
								        		        		var ap = npc.Get<ActorComponent>().AP;
								        		        		npc.Remove<ActorComponent>();
								        		        		npc.Add(new ActorComponent(actor, ap));
								        		        	},
								        		        "What AI?",
								        		        "No AI options.");
								        	},
								        "Set AI of Entity");
							} else {

							}
							break;
					}

					break;
				}
			}
		}

		private IEnumerable<Entity> FilterEquippedItems<T>(Entity entity) where T : DEngine.Entities.Component {
			List<Entity> items = new List<Entity>();

			if (entity.Has<EquipmentComponent>()) {
				var equipment = entity.Get<EquipmentComponent>();

				items.AddRange(equipment.Slots.Where(slot => equipment.IsSlotEquipped(slot) && equipment[slot].Has<T>()).Select(slot => equipment[slot]));
			}

			if (items.Count == 0 && entity.Has<T>())
				items.Add(entity); // natural weapon

			return items;
		}
		
		private void Move(Direction direction) {
			Point newLocation = player.Get<GameObject>().Location + direction;

			// we check for attackables
			var actorsAtNewLocation = player.Get<GameObject>().Level.GetEntitiesAt(newLocation).Where(e => e.Has<DefendComponent>()).ToList();

			if (actorsAtNewLocation.Count > 0) {
				Options(FilterEquippedItems<MeleeComponent>(player).ToList(),
					Identifier.GetNameOrId,
					weapon => Options(actorsAtNewLocation,
					                  Identifier.GetNameOrId,
					                  target => Enqueue(new MeleeAttackAction(player, target, weapon, target.Get<DefendComponent>().GetRandomPart())),
					                  "Attack what?",
					                  "Nothing at location to attack."),
						"With that weapon?",
						"No possible way of attacking.");

				return;
			}

			Enqueue(new BumpAction(player, direction));
		}

		public void Enqueue(IAction action) {
			player.Get<ActorComponent>().Enqueue(action);
		}
		
		#region Pickup/Drop items

		private void PickUpItem(Entity user, Entity itemEntity) {
			var item = itemEntity.Get<Item>();

			if (item.StackType == StackType.Hard)
				if (item.Amount == 1) {
					user.Get<ActorComponent>().Enqueue(new GetItemAction(user, itemEntity));
				} else {
					ParentApplication.Push(
							new CountPrompt("How many items to pick up?",
							                amount => user.Get<ActorComponent>().Enqueue(new GetItemAction(user, itemEntity, amount)),
							                item.Amount,
							                0,
							                item.Amount,
							                PromptTemplate));
				}

			else {
				user.Get<ActorComponent>().Enqueue(new GetItemAction(user, itemEntity));
			}
		}

		private void DropItem(Entity user, Entity itemEntity) {
			var item = itemEntity.Get<Item>();

			if (item.StackType == StackType.Hard)
				if (item.Amount == 1) {
					user.Get<ActorComponent>().Enqueue(new DropItemAction(user, itemEntity));
				} else {
					ParentApplication.Push(
							new CountPrompt("How many items to drop to the ground?",
							                amount => user.Get<ActorComponent>().Enqueue(new DropItemAction(user, itemEntity, amount)),
							                item.Amount,
							                0,
							                item.Amount,
							                PromptTemplate));
				}

			else {
				user.Get<ActorComponent>().Enqueue(new DropItemAction(user, itemEntity));
			}
		}

		#endregion
		
		#region Prompts
		private void Options<T>(IEnumerable<T> entities, Func<T, string> descriptionFunc, Action<T> action, string prompt, string fail) {
			ParentApplication.Push(
					new OptionsPrompt<T>(prompt,
					                     fail,
					                     entities,
					                     descriptionFunc,
					                     action,
					                     PromptTemplate));

		}

		private void Directions(Action<Point> action, string prompt) {
			ParentApplication.Push(
					new DirectionalPrompt(prompt,
					                      player.Get<GameObject>().Location,										  
					                      action,
					                      PromptTemplate));
		}

		private void Targets(Action<Point> action, string prompt) {
			ParentApplication.Push(
					new TargetPrompt(prompt,
					                 player.Get<GameObject>().Location,
					                 action,
					                 MapPanel,
					                 PromptTemplate));
		}

		private void Number(Action<int> action, int min, int max, int initial, string prompt) {
			ParentApplication.Push(new CountPrompt(prompt, action, max, min, initial, PromptTemplate));
		}

		private void Boolean(Action<bool> action, bool defaultBool, string prompt) {
			ParentApplication.Push(new BooleanPrompt(prompt, defaultBool, action, PromptTemplate));
		}
		#endregion
	}
}