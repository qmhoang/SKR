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
using SKR.UI.Menus;
using SKR.Universe;
using SkrGame.Actions;
using SkrGame.Gameplay;
using SkrGame.Gameplay.Combat;
using SkrGame.Systems;
using SkrGame.Universe;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Features;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Factories;
using SkrGame.Universe.Locations;
using libtcod;
using log4net;
using Level = SkrGame.Universe.Locations.Level;

namespace SKR.UI.Gameplay {
	public class GameplayWindow : Window {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private readonly World world;

		public MapPanel MapPanel { get; private set; }
		public StatusPanel StatusPanel { get; private set; }
//		public MessagePanel MessagePanel { get; private set; }
		public LogPanel LogPanel { get; private set; }
		public AssetsManager AssetsManager { get; private set; }

		public static PromptWindowTemplate PromptTemplate;

		private Entity player;
		
		private EntityManager manager;

		public GameplayWindow(World world, WindowTemplate template)
				: base(template) {
			this.world = world;
			AssetsManager = new AssetsManager();

			this.manager = world.EntityManager;
			player = world.Player;
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

			MapPanel = new MapPanel(world, AssetsManager, mapTemplate);

			AddControl(MapPanel);

			var statusTemplate =
					new PanelTemplate
					{
							Size = new Size(Size.Width - mapTemplate.Size.Width, mapTemplate.Size.Height),
							HasFrame = true,
					};
			statusTemplate.AlignTo(LayoutDirection.East, mapTemplate);

			StatusPanel = new StatusPanel(world.Player, statusTemplate);

			AddControl(StatusPanel);

			var logTemplate = new LogPanelTemplate()
			                       {
			                       		HasFrame = true,
			                       		Log = world.Log,
			                       		Size = new Size(Size.Width, Size.Height - mapTemplate.Size.Height),
			                       		TopLeftPos = mapTemplate.CalculateRect().BottomLeft.Shift(0, 1)
			                       };
			LogPanel = new LogPanel(logTemplate);
			AddControl(LogPanel);

			PromptTemplate =
					new PromptWindowTemplate
					{
							HasFrame = false,
							IsPopup = true,
							TopLeftPos = logTemplate.TopLeftPos.Shift(1, 1),
							Log = world.Log,
							Size = new Size(Size.Width - 2, Size.Height - mapTemplate.Size.Height - 2)
					};

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
				Combat.MeleeAttack(entity, weapon, actorsAtNewLocation.First(), actorsAtNewLocation.First().Get<DefendComponent>().GetRandomPart());
			} else {
				ParentApplication.Push(
						new OptionsSelectionPrompt<Entity>("Attack what?", actorsAtNewLocation,
						                                   e => e.ToString(),
														   e => Combat.MeleeAttack(entity, weapon, e, e.Get<DefendComponent>().GetRandomPart()),
						                                   GameplayWindow.PromptTemplate));
			}

		}

		private void SelectRangeTarget(Entity entity, Entity weapon) {
			Contract.Requires<ArgumentNullException>(entity != null, "entity");

			ParentApplication.Push(new TargetPrompt("Shoot where?",
										entity.Get<Location>().Point,
										targetLocation =>
										{
											var level = entity.Get<Location>().Level;
											var entitiesAtLocation = level.GetEntitiesAt(targetLocation).Where(e => e.Has<DefendComponent>());

											if (entitiesAtLocation.Count() > 0) {
												if (entitiesAtLocation.Count() == 1) {
													Combat.RangeAttack(entity, weapon, entitiesAtLocation.First(), entitiesAtLocation.First().Get<DefendComponent>().GetRandomPart());
												} else {
													ParentApplication.Push(
															new OptionsSelectionPrompt<Entity>("Shoot at what?", entitiesAtLocation,
																							   e => e.ToString(),
																							   e => Combat.RangeAttack(entity, weapon, e, e.Get<DefendComponent>().GetRandomPart()),
																							   GameplayWindow.PromptTemplate));
												}
											} else {
												world.Log.Normal("Nothing there to shoot.");
											}
										},
										MapPanel,
										GameplayWindow.PromptTemplate));
		}

		private void Reload(Entity user, Entity weapon) {
			Contract.Requires<ArgumentException>(user.Has<ContainerComponent>());

			var ammos = user.Get<ContainerComponent>().Items.Where(e => e.Has<AmmoComponent>() && e.Get<AmmoComponent>().Type == weapon.Get<RangeComponent>().AmmoType).ToList();

			if (ammos.Count > 1) {
				ParentApplication.Push(new OptionsSelectionPrompt<Entity>("What ammo?", ammos,
																		  ammo => ammo.Get<Identifier>().Name,
																		  ammo => Combat.ReloadWeapon(user, weapon, ammo),
																		  GameplayWindow.PromptTemplate));
			} else if (ammos.Count == 1)
				Combat.ReloadWeapon(user, weapon, ammos.First());
			else
				world.Log.Normal("No possible ammo for selected weapon.");
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
		}

		private void DropItem(Entity inventoryEntity, Entity itemEntityFromInventory) {
			var inventory = inventoryEntity.Get<ContainerComponent>();
			var item = itemEntityFromInventory.Get<Item>();

			if (item.StackType == StackType.Hard)
				ParentApplication.Push(
						new CountPrompt("How many items to drop to the ground?",
						                amount => DropStackedItem(inventoryEntity, itemEntityFromInventory, amount), item.Amount, 0, item.Amount, GameplayWindow.PromptTemplate));
			else {
				inventory.Remove(itemEntityFromInventory);				
			}
		}


		private void DropStackedItem(Entity entityDropping, Entity itemEntityFromInventory, int amount) {
			Contract.Requires<ArgumentException>(amount >= 0);
			if (amount == 0)
				return;

			var inventory = entityDropping.Get<ContainerComponent>();

			if (amount < itemEntityFromInventory.Get<Item>().Amount) {
				var ne = Item.Split(itemEntityFromInventory, amount);

				var level = entityDropping.Get<Location>().Level;
				var itemsInLevel = level.GetEntitiesAt(entityDropping.Get<Location>().Point).Where(e => e.Has<Item>() &&
				                                                                                           e.Has<VisibleComponent>() &&
				                                                                                           e.Get<VisibleComponent>().VisibilityIndex > 0).ToList();

				if (itemsInLevel.Exists(e => e.Get<ReferenceId>() == ne.Get<ReferenceId>())) {
					itemsInLevel.First(e => e.Get<ReferenceId>() == ne.Get<ReferenceId>()).Get<Item>().Amount += amount;
				} else {
					ne.Get<VisibleComponent>().Reset();
				}

			} else {
				inventory.Remove(itemEntityFromInventory);				
			}
		}
		#endregion

		private void SelectUsableAction(Entity user, Entity thing, List<UseableFeature.UseAction> actions) {
			if (actions.Count > 1) {
				ParentApplication.Push(
						new OptionsSelectionPrompt<UseableFeature.UseAction>(String.Format("Do what with {0}?", Identifier.GetNameOrId(thing)),
						                                                     actions,
						                                                     a => a.Description, a => a.Use(user, thing, a),
						                                                     GameplayWindow.PromptTemplate));
			} else if (actions.Count == 1) {
				actions.First().Use(user, thing, actions.First());
			} else {
				world.Log.Normal(String.Format("No possible action on {0}", Identifier.GetNameOrId(thing)));
			}

		}

//		private void Move(Entity user, Direction direction) {
//			Contract.Requires<ArgumentNullException>(user != null, "user");
//
//			var result = Movement.BumpDirection(user, direction);
//
//			// if an entity prevents movement, we can't do anything
//			if (!result) {
//				return;
//			}
//
//			Point newLocation = user.Get<Location>().Point + direction;
//
//			// we check for attackables
//			var actorsAtNewLocation = user.Get<Location>().Level.GetEntitiesAt(newLocation).Where(e => e.Has<DefendComponent>()).ToList();
//
//			if (actorsAtNewLocation.Count > 0) {
//				var weapons = FilterEquippedItems<MeleeComponent>(user).ToList();
//				if (weapons.Count > 1) {
//					ParentApplication.Push(
//							new OptionsSelectionPrompt<Entity>("With that weapon?",
//															   weapons, e => e.Get<Identifier>().Name,
//															   weapon => SelectMeleeTarget(user, weapon, actorsAtNewLocation),
//															   GameplayWindow.PromptTemplate));
//				} else if (weapons.Count == 1)
//					SelectMeleeTarget(user, weapons.First(), actorsAtNewLocation);
//				else {
//					world.Log.Normal("No possible way of attacking.");
//					Logger.WarnFormat("Player is unable to melee attack, no unarmed component equipped or attached");
//				}
//
//				return;
//			}
//
//			Movement.MoveEntity(user, newLocation);
//
//		}

		protected override void OnKeyPressed(KeyboardData keyData) {
			base.OnKeyReleased(keyData);
//			if (!player.Get<ActionPoint>().Updateable)
//				return;
			switch (keyData.KeyCode) {
				case TCODKeyCode.Up:
				case TCODKeyCode.KeypadEight: // Up and 8 should have the same functionality
//						Move(player, Direction.North);
					player.Get<ActorComponent>().Enqueue(new BumpAction(player, Direction.N));
					break;
				case TCODKeyCode.Down:
				case TCODKeyCode.KeypadTwo:
//						Move(player, Direction.South);
					player.Get<ActorComponent>().Enqueue(new BumpAction(player, Direction.S));
					break;
				case TCODKeyCode.Left:
				case TCODKeyCode.KeypadFour:
//						Move(player, Direction.West);
					player.Get<ActorComponent>().Enqueue(new BumpAction(player, Direction.W));
					break;
				case TCODKeyCode.KeypadFive:
					player.Get<ActorComponent>().Enqueue(new WaitAction(player));
					break;
				case TCODKeyCode.Right:
				case TCODKeyCode.KeypadSix:
//						Move(player, Direction.East);
					player.Get<ActorComponent>().Enqueue(new BumpAction(player, Direction.E));
					break;
				case TCODKeyCode.KeypadSeven:
//						Move(player, Direction.Northwest);
					player.Get<ActorComponent>().Enqueue(new BumpAction(player, Direction.NW));
					break;
				case TCODKeyCode.KeypadNine:
//						Move(player, Direction.Northeast);
					player.Get<ActorComponent>().Enqueue(new BumpAction(player, Direction.NE));
					break;
				case TCODKeyCode.KeypadOne:
//						Move(player, Direction.Southwest);
					player.Get<ActorComponent>().Enqueue(new BumpAction(player, Direction.SW));
					break;
				case TCODKeyCode.KeypadThree:
//						Move(player, Direction.Southeast);
					player.Get<ActorComponent>().Enqueue(new BumpAction(player, Direction.SE));
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
							world.Log.Normal("No possible way of shooting target.");
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
							world.Log.Normal("No weapons to reload.");
						}
					} else if (keyData.Character == 'u') {
						ParentApplication.Push(
								new DirectionalPrompt("What direction?",
								                      location.Point,
								                      p =>
								                      	{
								                      		var useables = location.Level.GetEntitiesAt(p).Where(e => e.Has<UseableFeature>() &&
								                      		                                                          e.Has<VisibleComponent>() &&
								                      		                                                          e.Get<VisibleComponent>().VisibilityIndex > 0);									                      		
								                      		if (useables.Count() > 1) {
								                      			ParentApplication.Push(
								                      					new OptionsSelectionPrompt<Entity>("What object do you want to use?",
								                      					                                   useables,
								                      					                                   Identifier.GetNameOrId,
								                      					                                   e => SelectUsableAction(player, e, e.Get<UseableFeature>().Uses.ToList()),
								                      					                                   GameplayWindow.PromptTemplate));
								                      		} else if (useables.Count() == 1) {
								                      			SelectUsableAction(player, useables.First(), useables.First().Get<UseableFeature>().Uses.ToList());
								                      		} else {
								                      			world.Log.Normal("Nothing there to use.");
								                      		}
								                      	},
								                      GameplayWindow.PromptTemplate));
					} else if (keyData.Character == 'd') {
						var inventory = player.Get<ContainerComponent>();
						if (inventory.Count > 0)
							ParentApplication.Push(new ItemWindow(new ItemWindowTemplate()
							                                      {
							                                      		Size = MapPanel.Size,
							                                      		IsPopup = true,
							                                      		HasFrame = true,
							                                      		Items = inventory.Items,
							                                      		SelectSingleItem = false,
							                                      		ItemSelected = i => DropItem(player, i),
							                                      }));
						else
							world.Log.Normal("You are carrying no items to drop.");
					} else if (keyData.Character == 'g') {
						var level = location.Level;
						var inventory = player.Get<ContainerComponent>();

						// get all items that have a location (eg present on the map) that are at the location where are player is
						var items =
								location.Level.GetEntitiesAt(location.Point).Where(e => e.Has<Item>() &&
								                                                        e.Has<VisibleComponent>() &&
								                                                        e.Get<VisibleComponent>().VisibilityIndex > 0 &&
								                                                        (!inventory.Items.Contains(e))).ToList();

						if (items.Count() > 0)
							ParentApplication.Push(new ItemWindow(new ItemWindowTemplate()
							                                      {
							                                      		Size = MapPanel.Size,
							                                      		IsPopup = true,
							                                      		HasFrame = true,
							                                      		Items = inventory.Items,
							                                      		SelectSingleItem = false,
							                                      		ItemSelected = i => PickUpItem(player, i, items),
							                                      }));
						else
							world.Log.Normal("No items here to pick up.");
					} else if (keyData.Character == 'i') {
						var inventory = player.Get<ContainerComponent>();
						ParentApplication.Push(new ItemWindow(new ItemWindowTemplate()
						                                      {
						                                      		Size = MapPanel.Size,
						                                      		IsPopup = true,
						                                      		HasFrame = true,
						                                      		Items = inventory.Items,
						                                      		SelectSingleItem = false,
						                                      		ItemSelected = i => world.Log.Normal(String.Format("This is a {0}, it weights {1}.", i.Get<Identifier>().Name, i.Get<Item>().Weight)),
						                                      }));

					} else if (keyData.Character == 'w')
						ParentApplication.Push(new InventoryWindow(new InventoryWindowTemplate()
						                                           {
						                                           		Size = MapPanel.Size,
						                                           		IsPopup = true,
						                                           		HasFrame = true,
						                                           		World = world,
						                                           		Items = player.Get<EquipmentComponent>().Slots.ToList(),
						                                           }));
					else if (keyData.Character == 'l') {
						if (keyData.ControlKeys == ControlKeys.LeftControl) {

						} else
							ParentApplication.Push(
									new LookWindow(
											location.Point,
											delegate(Point p)
												{
													StringBuilder sb = new StringBuilder();
													var entitiesAtLocation = location.Level.GetEntitiesAt(p);
													sb.AppendLine(((Level) location.Level).GetTerrain(p).Definition);
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
					} else if (keyData.Character == 'z') {
						world.Log.Special("Testing very long message for string wrapping.  We'll see how it works, hopefully very well; but if not we'll go in and fix it; won't we? Hmm, maybe I still need a longer message.  I'll just keep typing for now, hopefully making it very very very long.");
//							player.Add(new LongAction(500, e => world.Log.Normal(String.Format("{0} completes long action", Identifier.GetNameOrId(e)))));
//							player.Get<ActionPoint>().ActionPoints -= 100;
					}
						
					break;
				}
			}
		}
	}
}