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
						case 'A':
						{
							Options("With that weapon?",
							        "No possible way of attacking.",
							        FilterEquippedItems<MeleeComponent>(player).ToList(),
							        Identifier.GetNameOrId,
							        weapon => Directions("Attack where?",
							                             p => Options("Attack where?",
							                                          "Nothing at location to attack.",
							                                          player.Get<GameObject>().Level.GetEntitiesAt(p).Where(e => e.Has<DefendComponent>() && e.Has<Person>()).ToList(),
							                                          Identifier.GetNameOrId,
							                                          defender => Options("Attack at what?",
							                                                              String.Format("{0} has no possible part to attack.  How did we get here?", Identifier.GetNameOrId(defender)),
							                                                              defender.Get<DefendComponent>().BodyPartsList,
							                                                              bp => bp.Name,
							                                                              bp => Enqueue(new MeleeAttackAction(player,
							                                                                                                  defender,
							                                                                                                  weapon,
							                                                                                                  bp,
							                                                                                                  true))))));


						}
							break;
						case 'f':
						{
							Options("With what weapon?",
							        "No possible way of shooting target.",
							        FilterEquippedItems<RangeComponent>(player).ToList(),
							        Identifier.GetNameOrId,
							        weapon => TargetsAt("Shoot where?",
							                            World.TagManager.IsRegistered("target") ? World.TagManager.GetEntity("target").Get<GameObject>().Location : playerLocation.Location,
							                            p => Options("Shoot at what?",
							                                         "Nothing there to shoot.",
							                                         player.Get<GameObject>().Level.GetEntitiesAt(p).Where(e => e.Has<DefendComponent>()),
							                                         Identifier.GetNameOrId,
							                                         delegate(Entity defender)
							                                         	{
							                                         		World.TagManager.Register(defender, "target");
							                                         		Enqueue(new RangeAttackAction(player, defender, weapon, defender.Get<DefendComponent>().GetRandomPart()));
							                                         	})));

						}
							break;
						case 'F': {
								Options("With what weapon?",
										"No possible way of shooting target.",
										FilterEquippedItems<RangeComponent>(player).ToList(),
										Identifier.GetNameOrId,
										weapon => TargetsAt("Shoot where?",
															World.TagManager.IsRegistered("target") ? World.TagManager.GetEntity("target").Get<GameObject>().Location : playerLocation.Location,
															p => Options("Shoot at what?",
																		 "Nothing there to shoot.",
																		 player.Get<GameObject>().Level.GetEntitiesAt(p).Where(e => e.Has<DefendComponent>()),
																		 Identifier.GetNameOrId,
																		 defender => Options("What part?",
																							 String.Format("{0} has no possible part to attack.  How did we get here?", Identifier.GetNameOrId(defender)),
																							 defender.Get<DefendComponent>().BodyPartsList,
																							 bp => bp.Name,
																							 bp =>
																							 {
																								 World.TagManager.Register(defender, "target");
																								 Enqueue(new RangeAttackAction(player, defender, weapon, bp, true));
																							 }))));

							}
							break;
						case 'r': {
								Options("Reload what weapon?",
										"No weapons to reload.",
										FilterEquippedItems<RangeComponent>(player).ToList(),
										Identifier.GetNameOrId, weapon => Options("What ammo?",
																				  "No possible ammo for selected weapon.",
																				  player.Get<ContainerComponent>().Items.Where(e => e.Has<AmmoComponent>() &&
																																	e.Get<AmmoComponent>().Type == weapon.Get<RangeComponent>().AmmoType).ToList(),
																				  Identifier.GetNameOrId,
																				  ammo => Enqueue(new ReloadAction(player, weapon, ammo))));
							}
							break;
						case 'u':
							Directions("What direction?", p => Options("What object do you want to use?",
																	   "Nothing there to use.",
																	   playerLocation.Level.GetEntitiesAt(p).Where(e => e.Has<UseableFeature>() &&
																														e.Has<VisibleComponent>() &&
																														e.Get<VisibleComponent>().VisibilityIndex > 0), Identifier.GetNameOrId,
																	   useable => Options(String.Format("Do what with {0}?", Identifier.GetNameOrId(useable)),
																						  String.Format("No possible action on {0}", Identifier.GetNameOrId(useable)),
																						  useable.Get<UseableFeature>().Uses.ToList(),
																						  use => use.Description,
																						  use => use.Use(player, useable, use))));
							break;
						case 'd': {
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
						case 'g': {
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
						case 'G': {
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
						case 'i': {
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
						case 'p': {
								Options("With what?",
										"No lockpicks available.",
										player.Get<ContainerComponent>().Items.Where(i => i.Has<Lockpick>()),
										Identifier.GetNameOrId,
										lockpick => Directions("What direction?", p => Options("Select what to lockpick.",
																							   "Nothing there to lockpick.",
																							   player.Get<GameObject>().Level.GetEntitiesAt(p).Where(e => e.Has<LockedFeature>()),
																							   Identifier.GetNameOrId,
																							   feature => Enqueue(new LockpickAction(player, lockpick, feature)))));

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
								Targets("Set AI of Entity", p =>
																{
																	var entitiesAtLocation = playerLocation.Level.GetEntitiesAt(p).Where(e => e.Has<ActorComponent>()).ToList();
																	var npc = entitiesAtLocation.First();
																	var options = new List<AbstractActor>
								                            		              {
								                            		              		new DoNothingActor(),
								                            		              		new NPC()
								                            		              };
																	Options("What AI?",
																			"No AI options.",
																			options,
																			o => o.GetType().Name,
																			actor =>
																			{
																				var ap = npc.Get<ActorComponent>().AP;
																				npc.Remove<ActorComponent>();
																				npc.Add(new ActorComponent(actor, ap));
																			});
																});
							} else { }
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
			var actorsAtNewLocation = player.Get<GameObject>().Level.GetEntitiesAt(newLocation).Where(e => e.Has<DefendComponent>() && e.Has<Person>()).ToList();

			if (actorsAtNewLocation.Count > 0) {
				Options("With that weapon?",
				        "No possible way of attacking.",
				        FilterEquippedItems<MeleeComponent>(player).ToList(),
				        Identifier.GetNameOrId,
				        weapon => Options("Attack what?",
				                          "Nothing at location to attack.", actorsAtNewLocation,
				                          Identifier.GetNameOrId,
				                          target =>
				                          Enqueue(new MeleeAttackAction(player,
				                                                        target,
				                                                        weapon,
				                                                        target.Get<DefendComponent>().GetRandomPart()))));

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
					                      player.Get<GameObject>().Location,										  
					                      action,
					                      PromptTemplate));
		}

		private void Targets(string message, Action<Point> action) {
			ParentApplication.Push(
					new TargetPrompt(message,
					                 player.Get<GameObject>().Location,
					                 action,
					                 MapPanel,
					                 PromptTemplate));
		}

		private void TargetsAt(string message, Point point, Action<Point> action) {
			ParentApplication.Push(
					new TargetPrompt(message,
									 player.Get<GameObject>().Location,
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