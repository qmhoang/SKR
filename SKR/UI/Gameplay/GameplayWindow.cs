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
using SkrGame.Core;
using SkrGame.Gameplay;
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
using Level = SkrGame.Universe.Locations.Level;

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
										entity.Get<Location>().Position,
										targetLocation =>
										{
											var level = entity.Get<Location>().Level;
											var entitiesAtLocation = level.GetEntitiesAt(targetLocation, typeof(DefendComponent)).ToList();

											if (entitiesAtLocation.Count > 0) {
												if (entitiesAtLocation.Count == 1) {
													Combat.RangeAttack(entity, weapon, entitiesAtLocation.First(), entitiesAtLocation.First().Get<DefendComponent>().GetRandomPart());
												} else {
													ParentApplication.Push(
															new OptionsSelectionPrompt<Entity>("Shoot at what?", entitiesAtLocation,
																							   e => e.ToString(),
																							   e => Combat.RangeAttack(entity, weapon, e, e.Get<DefendComponent>().GetRandomPart()),
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
			Movement.Wait(entity);
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
				World.Instance.AddMessage(String.Format("No possible action on {0}", Identifier.GetNameOrId(thing)));
			}

		}

		private void Move(Entity user, Point direction) {
			Contract.Requires<ArgumentNullException>(user != null, "user");

			var result = Movement.BumpDirection(user, direction);

			// if an entity prevents movement, we can't do anything
			if (!result) {
				return;
			}

			Point newPosition = user.Get<Location>().Position + direction;

			// we check for attackables
			var actorsAtNewLocation = user.Get<Location>().Level.GetEntitiesAt(newPosition, typeof(DefendComponent)).ToList();

			if (actorsAtNewLocation.Count > 0) {
				var weapons = FilterEquippedItems<MeleeComponent>(user).ToList();
				if (weapons.Count > 1) {
					ParentApplication.Push(
							new OptionsSelectionPrompt<Entity>("With that weapon?",
															   weapons, e => e.Get<Identifier>().Name,
															   weapon => SelectMeleeTarget(user, weapon, actorsAtNewLocation),
															   GameplayWindow.PromptTemplate));
				} else if (weapons.Count == 1)
					SelectMeleeTarget(user, weapons.First(), actorsAtNewLocation);
				else {
					World.Instance.AddMessage("No possible way of attacking.");
					Logger.WarnFormat("Player is unable to melee attack, no unarmed component equipped or attached");
				}

				return;
			}

			

			Movement.MoveEntity(user, newPosition);

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
									                      					                                   e => SelectUsableAction(player, e, e.Get<UseableFeature>().Uses.ToList()),
									                      					                                   GameplayWindow.PromptTemplate));
									                      		} else if (useables.Count == 1) {
									                      			SelectUsableAction(player, useables.First(), useables.First().Get<UseableFeature>().Uses.ToList());
									                      		} else {
									                      			World.Instance.AddMessage("Nothing there to use.");
									                      		}
									                      	},
									                      GameplayWindow.PromptTemplate));
						} else if (keyData.Character == 'd') {
							var inventory = player.Get<ContainerComponent>();
							if (inventory.Count > 0)
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

							} else
								ParentApplication.Push(
										new LookWindow(
												location.Position,
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
							player.Add(new LongAction(500, e => World.Instance.AddMessage(String.Format("{0} completes long action", Identifier.GetNameOrId(e)))));
							player.Get<ActionPoint>().ActionPoints -= 100;
						}
						
						break;
					}
				}
			}
		}
	}
}