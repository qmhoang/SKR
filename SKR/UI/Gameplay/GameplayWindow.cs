using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using DEngine.Actions;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using DEngine.Extensions;
using Ogui.Core;
using Ogui.UI;
using SKR.UI.Menus;
using SKR.UI.Menus.Debug;
using SKR.UI.Prompts;
using SkrGame.Actions;
using SkrGame.Actions.Combat;
using SkrGame.Actions.Features;
using SkrGame.Actions.Items;
using SkrGame.Actions.Movement;
using SkrGame.Actions.Skills;
using SkrGame.Effects;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Controllers;
using SkrGame.Universe.Entities.Features;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Entities.Items.Equipables;
using SkrGame.Universe.Entities.Items.Tools;
using SkrGame.Universe.Entities.Useables;
using libtcod;
using log4net;

namespace SKR.UI.Gameplay {
	public class GameplayWindow : SkrWindow {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public MapPanel MapPanel { get; private set; }
		public StatusPanel StatusPanel { get; private set; }
		public LogPanel LogPanel { get; private set; }
		public AssetsManager AssetsManager { get; private set; }

		public PromptWindowTemplate PromptTemplate;

		private Entity _player;

		public GameplayWindow(SkrWindowTemplate template)
				: base(template) {
			AssetsManager = new AssetsManager();

			_player = template.World.Player;
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
			var playerLocation = _player.Get<GameObject>();

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
					Enqueue(new WaitAction(_player));
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
					if (keyboardEvent.KeyboardData.ControlKeys == ControlKeys.LeftControl || keyboardEvent.KeyboardData.ControlKeys == ControlKeys.RightControl) {
						switch (keyboardEvent.KeyboardData.Character) {
							case 'r':
							{
								Enqueue(new ChangePostureAction(_player, Posture.Run));
							}
								break;
							case 's':
							{
								Enqueue(new ChangePostureAction(_player, Posture.Stand));
							}
								break;
							case 'c':
							{
								Enqueue(new ChangePostureAction(_player, Posture.Crouch));
							}
								break;
							case 'p':
							{
								Enqueue(new ChangePostureAction(_player, Posture.Prone));
							}
								break;
							case 'z':
							{
								Targets("Set AI of Entity", p =>
								                            	{
																	var entitiesAtLocation = playerLocation.Level.GetEntitiesAt<ControllerComponent>(p).ToList();
								                            		var npc = entitiesAtLocation.First();
								                            		var options = new List<Controller>
								                            		              {
								                            		              		new DoNothing(),
								                            		              		new NPC()
								                            		              };
								                            		Options("What AI?",
								                            		        "No AI options.",
								                            		        options,
								                            		        o => o.GetType().Name,
								                            		        actor =>
								                            		        	{
								                            		        		var ap = npc.Get<ControllerComponent>().AP;
								                            		        		npc.Remove<ControllerComponent>();
								                            		        		npc.Add(new ControllerComponent(actor, ap));
								                            		        	});
								                            	});
							}
								break;
						}

					} else {
						switch (keyboardEvent.KeyboardData.Character) {
							case 'A': {
									Options("With that weapon?",
											"No possible way of attacking.",
											FilterEquippedItems<MeleeComponent>(_player).ToList(),
											Identifier.GetNameOrId,
											weapon => Directions("Attack where?",
																 p => Options("Attack where?",
																			  "Nothing at location to attack.",
																			  _player.Get<GameObject>().Level.GetEntitiesAt<BodyComponent, Creature>(p).ToList(),
																			  Identifier.GetNameOrId,
																			  defender => Options("Attack at what?",
																								  String.Format("{0} has no possible part to attack.  How did we get here?", Identifier.GetNameOrId(defender)),
																								  defender.Get<BodyComponent>().BodyPartsList,
																								  bp => bp.Name,
																								  bp => Enqueue(new MeleeAttackAction(_player,
																																	  defender,
																																	  weapon,
																																	  bp,
																																	  true))))));


								}
								break;
							case 'f': {
									Options("With what weapon?",
											"No possible way of shooting target.",
											FilterEquippedItems<RangeComponent>(_player).ToList(),
											Identifier.GetNameOrId,
											weapon => TargetsAt("Shoot where?",
																World.TagManager.IsRegistered("target") ? World.TagManager.GetEntity("target").Get<GameObject>().Location : playerLocation.Location,
																p => Options("Shoot at what?",
																			 "Nothing there to shoot.",
																			 _player.Get<GameObject>().Level.GetEntitiesAt<BodyComponent>(p),
																			 Identifier.GetNameOrId,
																			 delegate(Entity defender)
																			 {
																				 World.TagManager.Register("target", defender);
																				 Enqueue(new RangeAttackAction(_player, defender, weapon, defender.Get<BodyComponent>().GetRandomPart()));
																			 })));

								}
								break;
							case 'F': {
									Options("With what weapon?",
											"No possible way of shooting target.",
											FilterEquippedItems<RangeComponent>(_player).ToList(),
											Identifier.GetNameOrId,
											weapon => TargetsAt("Shoot where?",
																World.TagManager.IsRegistered("target") ? World.TagManager.GetEntity("target").Get<GameObject>().Location : playerLocation.Location,
																p => Options("Shoot at what?",
																			 "Nothing there to shoot.",
																			 _player.Get<GameObject>().Level.GetEntitiesAt<BodyComponent>(p),
																			 Identifier.GetNameOrId,
																			 defender => Options("What part?",
																								 String.Format("{0} has no possible part to attack.  How did we get here?", Identifier.GetNameOrId(defender)),
																								 defender.Get<BodyComponent>().BodyPartsList,
																								 bp => bp.Name,
																								 bp =>
																								 {
																									 World.TagManager.Register("target", defender);
																									 Enqueue(new RangeAttackAction(_player, defender, weapon, bp, true));
																								 }))));

								}
								break;
							case 'r': {
									Options("Reload what weapon?",
											"No weapons to reload.",
											FilterEquippedItems<RangeComponent>(_player).ToList(),
											Identifier.GetNameOrId, weapon => Options("What ammo?",
																					  "No possible ammo for selected weapon.",
																					  _player.Get<ItemContainerComponent>().Items.Where(e => e.Has<AmmoComponent>() &&
																																		e.Get<AmmoComponent>().Caliber == weapon.Get<RangeComponent>().AmmoCaliber).ToList(),
																					  Identifier.GetNameOrId,
																					  ammo => Enqueue(new ReloadAction(_player, weapon, ammo))));
								}
								break;
							case 'u':
								Directions("What direction?", p => Options("What object do you want to use?",
																		   "Nothing there to use.",
																		   playerLocation.Level.GetEntitiesAt<UseBroadcaster, VisibleComponent>(p).Where(e => e.Get<VisibleComponent>().VisibilityIndex > 0),
																		   Identifier.GetNameOrId,
																		   useable => Options(String.Format("Do what with {0}?", Identifier.GetNameOrId(useable)),
																							  String.Format("No possible action on {0}", Identifier.GetNameOrId(useable)),
																							  useable.Get<UseBroadcaster>().Actions.ToList(),
																							  use => use.Description,
																							  use => use.Use(_player, useable))));
								break;
							case 'd': {
									var inventory = _player.Get<ItemContainerComponent>();
									if (inventory.Count > 0)
										ParentApplication.Push(new ItemWindow(new ItemWindowTemplate
																			  {
																				  Size = MapPanel.Size,
																				  IsPopup = true,
																				  HasFrame = true,
																				  World = World,
																				  Items = inventory.Items,
																				  SelectSingleItem = false,
																				  ItemSelected = i => DropItem(_player, i),
																			  }));
									else
										World.Log.Aborted("You are carrying no items to drop.");
								}
								break;
							case 'g': {
									var inventory = _player.Get<ItemContainerComponent>();

									// get all items that have a location (eg present on the map) that are at the location where are player is
									var items = playerLocation.Level.GetEntitiesAt<Item, VisibleComponent>(playerLocation.Location).Where(e => e.Get<VisibleComponent>().VisibilityIndex > 0 &&
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
																				  ItemSelected = i => PickUpItem(_player, i),
																			  }));
									else
										World.Log.Aborted("No items here to pick up.");
								}
								break;
							case 'G': {
									var inventory = _player.Get<ItemContainerComponent>();

									// get all items that have a location (eg present on the map) that are at the location where are player is
									var items = playerLocation.Level.GetEntitiesAt<Item, VisibleComponent>(playerLocation.Location).Where(e => e.Get<VisibleComponent>().VisibilityIndex > 0 &&
																																			   !inventory.Items.Contains(e));

									if (items.Count() > 0)
										Enqueue(new GetAllItemsAction(_player, items));
									else
										World.Log.Aborted("No items here to pick up.");
								}
								break;
							case 'j': {
									Directions("Jump what direction?", p => Enqueue(new JumpOverAction(_player, p - playerLocation.Location)));
								}
								break;
							case 'i': {
									var inventory = _player.Get<ItemContainerComponent>();
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
																			   Items = _player.Get<EquipmentComponent>().Slots.ToList(),
																		   }));
								break;
							case 'p': {
									Options("With what?",
											"No lockpicks available.",
											_player.Get<ItemContainerComponent>().Items.Where(i => i.Has<Lockpick>()),
											Identifier.GetNameOrId,
											lockpick => Directions("What direction?", p => Options("Select what to lockpick.",
																								   "Nothing there to lockpick.",
																								   _player.Get<GameObject>().Level.GetEntitiesAt<LockedFeature>(p),
																								   Identifier.GetNameOrId,
																								   feature => Enqueue(new LockpickAction(_player, lockpick, feature)))));

								}
								break;
							case 'l':

								ParentApplication.Push(
										new LookWindow(
												playerLocation.Location,
												delegate(Point p)
												{
													StringBuilder sb = new StringBuilder();
													var entitiesAtLocation = playerLocation.Level.GetEntitiesAt(p);
													sb.AppendLine(playerLocation.Level.GetTerrain(p).Definition);
													foreach (var entity in entitiesAtLocation) {
														sb.AppendFormat("Entity: {0} ", entity.Has<ReferenceId>() ? entity.Get<ReferenceId>().RefId : entity.Id.ToString());
														sb.AppendFormat("Name: {0} ", Identifier.GetNameOrId(entity));
														if (entity.Has<Scenery>())
															sb.AppendFormat(entity.Get<Scenery>().ToString());

														sb.AppendLine();
													}

													return sb.ToString();
												},
												MapPanel,
												PromptTemplate));
								break;
							case 'z':
							{
								_player.Get<ControllerComponent>().Cancel();
							} break;
							case '`': {
									ParentApplication.Push(new DebugMenuWindow(new SkrWindowTemplate()
																			   {
																				   World = World
																			   }));
								}
								break;
						}
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
			Point newLocation = _player.Get<GameObject>().Location + direction;

			// we check for attackables
			var actorsAtNewLocation = _player.Get<GameObject>().Level.GetEntitiesAt<BodyComponent, Creature>(newLocation).ToList();

			if (actorsAtNewLocation.Count > 0) {
				Options("With that weapon?",
				        "No possible way of attacking.",
				        FilterEquippedItems<MeleeComponent>(_player).ToList(),
				        Identifier.GetNameOrId,
				        weapon => Options("Attack what?",
				                          "Nothing at location to attack.", actorsAtNewLocation,
				                          Identifier.GetNameOrId,
				                          target =>
				                          Enqueue(new MeleeAttackAction(_player,
				                                                        target,
				                                                        weapon,
				                                                        target.Get<BodyComponent>().GetRandomPart()))));

				return;
			}

			Enqueue(new BumpAction(_player, direction));
		}

		public void Enqueue(IAction action) {
			_player.Get<ControllerComponent>().Enqueue(action);
		}

		#region Pickup/Drop items

		private void PickUpItem(Entity user, Entity itemEntity) {
			var item = itemEntity.Get<Item>();

			if (item.StackType == StackType.Hard)
				if (item.Amount == 1) {
					user.Get<ControllerComponent>().Enqueue(new GetItemAction(user, itemEntity));
				} else {
					ParentApplication.Push(
							new CountPrompt("How many items to pick up?",
							                amount => user.Get<ControllerComponent>().Enqueue(new GetItemAction(user, itemEntity, amount)),
							                item.Amount,
							                0,
							                item.Amount,
							                PromptTemplate));
				}
			else {
				user.Get<ControllerComponent>().Enqueue(new GetItemAction(user, itemEntity));
			}
		}

		private void DropItem(Entity user, Entity itemEntity) {
			var item = itemEntity.Get<Item>();

			if (item.StackType == StackType.Hard)
				if (item.Amount == 1) {
					user.Get<ControllerComponent>().Enqueue(new DropItemAction(user, itemEntity));
				} else {
					ParentApplication.Push(
							new CountPrompt("How many items to drop to the ground?",
							                amount => user.Get<ControllerComponent>().Enqueue(new DropItemAction(user, itemEntity, amount)),
							                item.Amount,
							                0,
							                item.Amount,
							                PromptTemplate));
				}
			else {
				user.Get<ControllerComponent>().Enqueue(new DropItemAction(user, itemEntity));
			}
		}

		#endregion

		#region Prompts

		private void Options<T>(string message, string fail, IEnumerable<T> entities, Func<T, string> descriptionFunc, Action<T> action) {
			ParentApplication.Push(
					new OptionsPrompt<T>(message,
					                     fail,
					                     entities,
					                     descriptionFunc,
					                     action,
					                     PromptTemplate));

		}
		

		private void Directions(string message, Action<Point> action) {
			ParentApplication.Push(
					new DirectionalPrompt(message,
					                      _player.Get<GameObject>().Location,
					                      action,
					                      PromptTemplate));
		}

		private void Targets(string message, Action<Point> action) {
			ParentApplication.Push(
					new TargetPrompt(message,
					                 _player.Get<GameObject>().Location,
					                 action,
					                 MapPanel,
					                 PromptTemplate));
		}

		private void TargetsAt(string message, Point point, Action<Point> action) {
			ParentApplication.Push(
					new TargetPrompt(message,
					                 _player.Get<GameObject>().Location,
					                 point,
					                 action,
					                 MapPanel,
					                 PromptTemplate));
		}

		private void Number(string message, int min, int max, int initial, Action<int> action) {
			ParentApplication.Push(new CountPrompt(message, action, max, min, initial, PromptTemplate));
		}

		private void Boolean(string message, bool defaultBool, Action<bool> action) {
			ParentApplication.Push(new BooleanPrompt(message, defaultBool, action, PromptTemplate));
		}

		#endregion
	}
}