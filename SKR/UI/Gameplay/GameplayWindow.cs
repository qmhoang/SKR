using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using DEngine.Extensions;
using Ogui.UI;
using SKR.UI.Gameplay.Systems;
using SKR.UI.Menus;
using SKR.Universe;
using SkrGame.Gameplay.Combat;
using SkrGame.Systems;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Features;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Entities.Items.Components;
using SkrGame.Universe.Factories;
using SkrGame.Universe.Locations;
using libtcod;
using log4net;

namespace SKR.UI.Gameplay {
	public class GameplayWindow : Window {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


		private readonly World world;

		public MapPanel MapPanel { get; private set; }
		public StatusPanel StatusPanel { get; private set; }
		public MessagePanel MessagePanel { get; private set; }
		public AssetsManager AssetsManager { get; private set; }

		public static WindowTemplate PromptTemplate;

		private Entity player;
		
		private EntityManager manager;

		public GameplayWindow(EntityManager manager, WindowTemplate template)
				: base(template) {
			world = World.Instance;
			AssetsManager = new AssetsManager();

			this.manager = manager;
			player = World.Instance.Player;
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

			MapPanel = new MapPanel(manager, AssetsManager, mapTemplate);

			AddControl(MapPanel);

			var statusTemplate =
					new PanelTemplate
					{
							Size = new Size(Size.Width - mapTemplate.Size.Width, mapTemplate.Size.Height),
							HasFrame = true,
					};
			statusTemplate.AlignTo(LayoutDirection.East, mapTemplate);

			StatusPanel = new StatusPanel(manager, statusTemplate);

			AddControl(StatusPanel);

			var msgTemplate =
					new PanelTemplate
					{
							HasFrame = true,
							Size = new Size(Size.Width, Size.Height - mapTemplate.Size.Height),
							TopLeftPos = mapTemplate.CalculateRect().BottomLeft.Shift(0, 1)
					};

			MessagePanel = new MessagePanel(msgTemplate);
			AddControl(MessagePanel);

			world.MessageAdded += (sender, args) => MessagePanel.HandleMessage(args.Data);

			PromptTemplate =
					new WindowTemplate
					{
							HasFrame = false,
							IsPopup = true,
							TopLeftPos = msgTemplate.TopLeftPos.Shift(1, 1),
							Size = new Size(Size.Width - 2, Size.Height - mapTemplate.Size.Height - 2)
					};

			AddManager(new PlayerMovementSystem(manager));
		}

		private void Move(Entity entity, Point direction) {
			Contract.Requires<ArgumentNullException>(entity != null, "entity");

			Point newPosition = entity.Get<Location>().Position + direction;

			var level = entity.Get<Location>().Level;

			var bumpablesAtNewLocation = level.GetEntitiesAt(newPosition, typeof(OnBump)).ToList();

			bool blockedMovement = false;
			foreach (var b in bumpablesAtNewLocation) {
				if (b.Get<OnBump>().Action(entity, b) == OnBump.BumpResult.BlockMovement) {
					blockedMovement = true;
				}
			}

			if (blockedMovement)
				return;

			if (level.IsWalkable(newPosition)) {
				var actorsAtNewLocation = level.GetEntitiesAt(newPosition, typeof(DefendComponent)).ToList();

				if (actorsAtNewLocation.Count > 0) {
					var weapons = FilterEquippedItems<MeleeComponent>(entity).ToList();
					if (weapons.Count > 1) {
						ParentApplication.Push(
								new OptionsSelectionPrompt<Entity>("With that weapon?",
								                                   weapons, e => e.Get<Identifier>().Name,
								                                   weapon => SelectMeleeTarget(entity, weapon, actorsAtNewLocation),
								                                   GameplayWindow.PromptTemplate));
					} else if (weapons.Count == 1)
						SelectMeleeTarget(entity, weapons.First(), actorsAtNewLocation);
					else {
						World.Instance.AddMessage("No possible way of attacking.");
						Logger.WarnFormat("Player is unable to melee attack, no unarmed component equipped or attached");
					}
				} else {
					entity.Get<Location>().Position = newPosition;
					entity.Get<ActionPoint>().ActionPoints -= World.SpeedToActionPoints(World.DEFAULT_SPEED);
				}
			} else
				World.Instance.AddMessage("There is something in the way.");
		}

		private IEnumerable<Entity> FilterEquippedItems<T>(Entity entity) where T : DEngine.Entities.Component {
			List<Entity> items = new List<Entity>();

			if (entity.Has<EquipmentComponent>()) {
				var equipment = entity.Get<EquipmentComponent>();

				items.AddRange(from slot in equipment.Slots where equipment.IsSlotEquipped(slot) && equipment[slot].Has<T>() select equipment[slot]);
			}

			if (items.Count == 0 && entity.Has<T>())
				items.Add(entity);			// natural weapon

			return items;
		}

		private void SelectMeleeTarget(Entity entity, Entity weapon, List<Entity> actorsAtNewLocation) {
			if (actorsAtNewLocation.Count == 1) {
				Combat.MeleeAttack(entity, weapon, actorsAtNewLocation.First(), actorsAtNewLocation.First().Get<DefendComponent>().DefaultPart, World.MEAN);
			} else {
				ParentApplication.Push(
						new OptionsSelectionPrompt<Entity>("Attack what?", actorsAtNewLocation,
						                                   e => e.ToString(),
						                                   e => Combat.MeleeAttack(entity, weapon, e, e.Get<DefendComponent>().DefaultPart, World.MEAN),
						                                   GameplayWindow.PromptTemplate));
			}

		}

		private void SelectRangeTarget(Entity entity, Entity weapon) {
			Contract.Requires<ArgumentNullException>(entity != null, "entity");

			ParentApplication.Push(new TargetPrompt("Shoot where?",
										entity.Get<Location>().Position,
										targetLocation =>
										{
											var level = entity.Get<Location>().Level;
											var entitiesAtLocation = level.GetEntitiesAt(targetLocation, typeof(DefendComponent)).ToList();

											if (entitiesAtLocation.Count > 0) {
												if (entitiesAtLocation.Count == 1) {
													Combat.RangeAttack(entity, weapon, entitiesAtLocation.First(), entitiesAtLocation.First().Get<DefendComponent>().DefaultPart, World.MEAN);
												} else {
													ParentApplication.Push(
															new OptionsSelectionPrompt<Entity>("Shoot at what?", entitiesAtLocation,
																							   e => e.ToString(),
																							   e => Combat.RangeAttack(entity, weapon, e, e.Get<DefendComponent>().DefaultPart, World.MEAN),
																							   GameplayWindow.PromptTemplate));
												}
											} else {
												World.Instance.AddMessage("Nothing there to shoot.");
											}
										},
										MapPanel,
										GameplayWindow.PromptTemplate));
		}

		private void Reload(Entity user, Entity weapon) {
			Contract.Requires<ArgumentException>(user.Has<ContainerComponent>());

			var ammos = user.Get<ContainerComponent>().Where(e => e.Has<AmmoComponent>() && e.Get<AmmoComponent>().Type == weapon.Get<RangeComponent>().AmmoType).ToList();

			if (ammos.Count > 1) {
				ParentApplication.Push(new OptionsSelectionPrompt<Entity>("What ammo?", ammos,
																		  ammo => ammo.Get<Identifier>().Name,
																		  ammo => Combat.ReloadWeapon(user, weapon, ammo),
																		  GameplayWindow.PromptTemplate));
			} else if (ammos.Count == 1)
				Combat.ReloadWeapon(user, weapon, ammos.First());
			else
				World.Instance.AddMessage("No possible ammo for selected weapon.");
		}

		private void Wait(Entity entity) {
			entity.Get<ActionPoint>().ActionPoints -= World.SpeedToActionPoints(World.DEFAULT_SPEED);	
		}

		#region Pickup/Drop items
		private void PickUpItem(Entity inventoryEntity, Entity itemEntityFromLevel, ICollection<Entity> itemsOnDisplay) {
			Contract.Requires<ArgumentNullException>(itemsOnDisplay != null, "items");

			var inventory = inventoryEntity.Get<ContainerComponent>();
			var item = itemEntityFromLevel.Get<Item>();

			if (item.StackType == StackType.Hard)
				ParentApplication.Push(
						new CountPrompt("How many items to pick up?",
						                amount => PickUpStackedItem(inventoryEntity, itemEntityFromLevel, amount, itemsOnDisplay), item.Amount, 0, item.Amount, PromptTemplate));
			else {
				inventory.Add(itemEntityFromLevel);
				itemsOnDisplay.Remove(itemEntityFromLevel);
			}
		}

		private void PickUpStackedItem(Entity entityPicking, Entity itemEntityFromLevel, int amount, ICollection<Entity> itemsOnDisplay) {
			if (amount == 0)
				return;

			if (amount < itemEntityFromLevel.Get<Item>().Amount) {
				var ne = Item.Split(itemEntityFromLevel, amount);
				entityPicking.Get<ContainerComponent>().Add(ne);
			} else {
				entityPicking.Get<ContainerComponent>().Add(itemEntityFromLevel);
				itemsOnDisplay.Remove(itemEntityFromLevel);
			}

//
//			var inventory = entityPickingUp.Get<ContainerComponent>();
//			var item = itemEntityFromLevel.Get<Item>();
//			
//			// if an item doesn't exist in the inventory
//			if (!inventory.Exist(e => e.Get<ReferenceId>() == itemEntityFromLevel.Get<ReferenceId>())) {
//
//				// and if we're splitting an item, create a new one
//				if (amount < item.Amount) {
//					item.Amount -= amount;
//
//					var tempItem = itemEntityFromLevel.Copy();					
//					tempItem.Get<VisibleComponent>().VisibilityIndex = -1;
//
//					tempItem.Get<Item>().Amount = amount;	// amount starts out as 1
//					inventory.Add(tempItem);
//				} else {
//					inventory.Add(itemEntityFromLevel);
//					items.Remove(itemEntityFromLevel);
//				}
//
//			} else {
//				inventory.GetItem(e => e.Get<ReferenceId>() == itemEntityFromLevel.Get<ReferenceId>()).Get<Item>().Amount += amount;
//
//				if (amount < item.Amount) {
//					item.Amount -= amount;
//				} else {
//					manager.Remove(itemEntityFromLevel);
//					items.Remove(itemEntityFromLevel);
//				}
//
//			}
		}

		private void DropItem(Entity inventoryEntity, Entity itemEntityFromInventory, ICollection<Entity> itemsOnDisplay) {
			Contract.Requires<ArgumentNullException>(itemsOnDisplay != null, "items");
			var inventory = inventoryEntity.Get<ContainerComponent>();
			var item = itemEntityFromInventory.Get<Item>();

			if (item.StackType == StackType.Hard)
				ParentApplication.Push(
						new CountPrompt("How many items to drop to the ground?",
						                amount => DropStackedItem(inventoryEntity, itemEntityFromInventory, amount, itemsOnDisplay), item.Amount, 0, item.Amount, GameplayWindow.PromptTemplate));
			else {
				inventory.Remove(itemEntityFromInventory);
				itemsOnDisplay.Remove(itemEntityFromInventory);
			}
		}


		private void DropStackedItem(Entity entityDropping, Entity itemEntityFromInventory, int amount, ICollection<Entity> itemsOnDisplay) {
			Contract.Requires<ArgumentException>(amount >= 0);
			if (amount == 0)
				return;

			var inventory = entityDropping.Get<ContainerComponent>();

			if (amount < itemEntityFromInventory.Get<Item>().Amount) {
				var ne = Item.Split(itemEntityFromInventory, amount);

				var level = entityDropping.Get<Location>().Level;
				var itemsInLevel = level.GetEntitiesAt(entityDropping.Get<Location>().Position, typeof(Item), typeof(VisibleComponent)).Where(e => e.Get<VisibleComponent>().VisibilityIndex > 0).ToList();

				if (itemsInLevel.Exists(e => e.Get<ReferenceId>() == ne.Get<ReferenceId>())) {
					itemsInLevel.First(e => e.Get<ReferenceId>() == ne.Get<ReferenceId>()).Get<Item>().Amount += amount;
				} else {
					ne.Get<VisibleComponent>().Reset();
				}

			} else {
				inventory.Remove(itemEntityFromInventory);
				itemsOnDisplay.Remove(itemEntityFromInventory);
			}


//			var inventory = entityDropping.Get<ContainerComponent>();
//			var item = itemEntityFromInventory.Get<Item>();
//			
//			var level = entityDropping.Get<Location>().Level;
//
//			// this only gets items that are visible at that location.  entities that are being carried aren't visible
//			var itemsInLevel = level.EntityManager.Get(typeof(Location), typeof(Item), typeof(VisibleComponent)).Where(i => i.Get<VisibleComponent>().VisibilityIndex > 0).ToList();
//
//			// if an item doesn't exist in the at the location, create one
//			if (!itemsInLevel.Exists(e => e.Get<ReferenceId>() == itemEntityFromInventory.Get<ReferenceId>() && e.Get<Location>() == entityDropping.Get<Location>())) {
//
//				// if amount drop is less than currently carrying, just substract it, otherwise remove it
//				if (amount < item.Amount) {
//					item.Amount -= amount;
//
//					var tempItem = itemEntityFromInventory.Copy();
//					inventory.Remove(tempItem);
//					tempItem.Get<Item>().Amount = amount;	// amount starts out as 1
//
//				} else {
//					// if we're removing everything, just remove from the inventory and show it
//					inventory.Remove(itemEntityFromInventory);
//					items.Remove(itemEntityFromInventory);
//				}
//				
//			} else {
//				itemsInLevel.First(e => e.Get<ReferenceId>() == itemEntityFromInventory.Get<ReferenceId>() && e.Get<Location>() == entityDropping.Get<Location>()).Get<Item>().Amount += amount;
//				if (amount < item.Amount) {
//					item.Amount -= amount;
//				} else {
//					items.Remove(itemEntityFromInventory);
//					manager.Remove(itemEntityFromInventory);	// WARNING: will render itemEntityFromInventory componentless
//				}		
//			}

		}
		#endregion

		private void DoWhat(Entity user, Entity @object, List<UseableFeature.UseAction> actions) {
			if (actions.Count > 1) {
				ParentApplication.Push(
						new OptionsSelectionPrompt<UseableFeature.UseAction>(String.Format("Do what with {0}?", Identifier.GetNameOrId(@object)),
						                                                     actions,
						                                                     a => a.Description, a => a.Use(user, @object, a),
						                                                     GameplayWindow.PromptTemplate));
			} else if (actions.Count == 1) {
				actions.First().Use(user, @object, actions.First());
			} else {
				World.Instance.AddMessage(String.Format("No possible action on {0}", Identifier.GetNameOrId(@object)));
			}

		}

		protected override void OnKeyPressed(KeyboardData keyData) {
			base.OnKeyReleased(keyData);
			if (player.Get<ActionPoint>().Updateable) {
				switch (keyData.KeyCode) {
					case TCODKeyCode.Up:
					case TCODKeyCode.KeypadEight: // Up and 8 should have the same functionality
						Move(player, Point.North);
						break;
					case TCODKeyCode.Down:
					case TCODKeyCode.KeypadTwo:
						Move(player, Point.South);
						break;
					case TCODKeyCode.Left:
					case TCODKeyCode.KeypadFour:
						Move(player, Point.West);
						break;
					case TCODKeyCode.KeypadFive:
						Wait(player);
						break;
					case TCODKeyCode.Right:
					case TCODKeyCode.KeypadSix:
						Move(player, Point.East);
						break;
					case TCODKeyCode.KeypadSeven:
						Move(player, Point.Northwest);
						break;
					case TCODKeyCode.KeypadNine:
						Move(player, Point.Northeast);
						break;
					case TCODKeyCode.KeypadOne:
						Move(player, Point.Southwest);
						break;
					case TCODKeyCode.KeypadThree:
						Move(player, Point.Southeast);
						break;
					default:
					{
						var location = player.Get<Location>();
						if (keyData.Character == 'f') {
							var weapons = FilterEquippedItems<RangeComponent>(player).ToList();

							if (weapons.Count > 1) {
								ParentApplication.Push(
										new OptionsSelectionPrompt<Entity>("With what weapon?",
																		   weapons, e => e.Get<Identifier>().Name,
																		   weapon => SelectRangeTarget(player, weapon),
																		   GameplayWindow.PromptTemplate));
							} else if (weapons.Count == 1) {
								SelectRangeTarget(player, weapons.First());
							} else {
								World.Instance.AddMessage("No possible way of shooting target.");
							}

						} else if (keyData.Character == 'r') {
							var weapons = FilterEquippedItems<RangeComponent>(player).ToList();

							if (weapons.Count > 1) {
								ParentApplication.Push(
										new OptionsSelectionPrompt<Entity>("Reload what weapon?",
										                                   weapons, e => e.Get<Identifier>().Name,
										                                   weapon => Reload(player, weapon),
										                                   GameplayWindow.PromptTemplate));
							} else if (weapons.Count == 1) {
								Reload(player, weapons.First());
							} else {
								World.Instance.AddMessage("No weapons to reload.");
							}
						} else if (keyData.Character == 'u') {
							ParentApplication.Push(
									new DirectionalPrompt("What direction?",
									                      location.Position,
									                      p =>
									                      {
									                      	var useables = location.Level.GetEntitiesAt(p, typeof(UseableFeature), typeof(VisibleComponent)).Where(e => e.Get<VisibleComponent>().VisibilityIndex > 0).ToList();

															if (useables.Count > 1) {
																ParentApplication.Push(
																		new OptionsSelectionPrompt<Entity>("What object do you want to use?",
																		                                   useables,
																		                                   Identifier.GetNameOrId,
																		                                   e => DoWhat(player, e, e.Get<UseableFeature>().Uses.ToList()), 
																										   GameplayWindow.PromptTemplate));
															} else if (useables.Count == 1) {
																DoWhat(player, useables.First(), useables.First().Get<UseableFeature>().Uses.ToList());
															} else {
																World.Instance.AddMessage("Nothing there to use.");
															}
									                      },
									                      GameplayWindow.PromptTemplate));
						} else if (keyData.Character == 'd') {
							var inventory = player.Get<ContainerComponent>();
							if (inventory.Count() > 0)
								ParentApplication.Push(new ItemWindow(false,
																	  new ListWindowTemplate<Entity>
																	  {
																		  Size = MapPanel.Size,
																		  IsPopup = true,
																		  HasFrame = true,
																		  Items = inventory,
																	  }, i => DropItem(player, i, inventory)));
							else
								World.Instance.AddMessage("You are carrying no items to drop.");
						} else if (keyData.Character == 'g') {
							var level = location.Level;
							var inventory = player.Get<ContainerComponent>();

							// get all items that have a location (eg present on the map) that are at the location where are player is
							var items = level.GetEntitiesAt(location.Position, typeof(Item), typeof(VisibleComponent)).Where(e => e.Get<VisibleComponent>().VisibilityIndex > 0 && (!inventory.Items.Contains(e))).ToList();

							if (items.Count() > 0)
								ParentApplication.Push(new ItemWindow(false,
																	  new ListWindowTemplate<Entity>
																	  {
																		  Size = MapPanel.Size,
																		  IsPopup = true,
																		  HasFrame = true,
																		  Items = items,
																	  },
																	  i => PickUpItem(player, i, items)));
							else
								World.Instance.AddMessage("No items here to pick up.");
						} else if (keyData.Character == 'i') {
							var inventory = player.Get<ContainerComponent>();

							ParentApplication.Push(new ItemWindow(false,
																  new ListWindowTemplate<Entity>
																  {
																	  Size = MapPanel.Size,
																	  IsPopup = true,
																	  HasFrame = true,
																	  Items = inventory,
																  },
																  i => World.Instance.AddMessage(String.Format("This is a {0}, it weights {1}.", i.Get<Identifier>().Name, i.Get<Item>().Weight))));
						} else if (keyData.Character == 'w')
							ParentApplication.Push(new InventoryWindow(new ListWindowTemplate<string>
																	   {
																		   Size = MapPanel.Size,
																		   IsPopup = true,
																		   HasFrame = true,
																		   Items = player.Get<EquipmentComponent>().Slots.ToList(),
																	   }));
						else if (keyData.Character == 'l') {
							if (keyData.ControlKeys == ControlKeys.LeftControl) {
								//									ParentApplication.Push(new LookWindow(location.Position,
								//									                                      p => string.Format("{0}\n{1}\nVisible: {2}, Transparent: {3}, Walkable: {4}", player.Level.GetTerrain(p).Definition,
								//									                                                         (player.Level.DoesFeatureExistAtLocation(p) ? player.Level.GetFeatureAtLocation(p).Asset : ""),
								//									                                                         player.Level.IsVisible(p), player.Level.IsTransparent(p), player.Level.IsWalkable(p)),
								//									                                      MapPanel, GameplayWindow.PromptTemplate));
							} else
								ParentApplication.Push(
										new LookWindow(
												location.Position,
												delegate(Point p)
												{
													StringBuilder sb = new StringBuilder();
													var entitiesAtLocation = location.Level.GetEntitiesAt(p);
													sb.AppendLine(((Level)location.Level).GetTerrain(p).Definition);
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
												GameplayWindow.PromptTemplate));
						}
					
						break;
					}
				}
			}
		}
	}
}