using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entity;
using DEngine.Extensions;
using Ogui.UI;
using SKR.UI.Gameplay.Systems;
using SKR.UI.Menus;
using SKR.Universe;
using SkrGame.Gameplay.Combat;
using SkrGame.Systems;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Factories;
using SkrGame.Universe.Locations;
using libtcod;

namespace SKR.UI.Gameplay {
	public class GameplayWindow : Window {
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

//		/// <summary>
//		/// This function will recursively check for options
//		/// 
//		/// </summary>
//		/// <param name="activeTalent">The talent being used</param>
//		/// <param name="targetLocation">the target the talent is being used on</param>
//		/// <param name="index"></param>
//		/// <param name="args"></param>
//		private void RecursiveSelectOptionHelper(ActiveTalentComponent activeTalent, Point targetLocation, int index, List<dynamic> argSelected) {
//			if (index > activeTalent.NumberOfArgs)
//				throw new Exception("We have somehow recursed on more levels that there are options");
//
//			var user = activeTalent.Talent.Owner;
//			var talentArg = activeTalent.Args.ElementAt(index);
//
//			var options = talentArg.ArgFunction(activeTalent, user, targetLocation);
//
//			if (options == null || options.Count() == 0)
//				if (talentArg.Required) {
//					Logger.DebugFormat("{0} used talent: {1} without any possible options", user.FullId, activeTalent.Talent.RefId);
//					world.AddMessage("no possible options");
//				} else {
//					// no options, but its not required
//					argSelected.Add(null);
//					InvokeOrRecurse(activeTalent, targetLocation, index, argSelected);
//				} else if (options.Count() == 1) {
//				// only one option, select it automatically
//				argSelected.Add(options.ElementAt(0));
//
//				InvokeOrRecurse(activeTalent, targetLocation, index, argSelected);
//			} else if (options.Count() > 0)
//				ParentApplication.Push(
//						new OptionsSelectionPrompt<dynamic>(String.IsNullOrEmpty(talentArg.PromptDescription) ? "Options" : talentArg.PromptDescription,
//															options.ToList(),
//															arg => talentArg.ArgDesciptor(activeTalent, user, targetLocation, arg),
//															arg =>
//															{
//																argSelected.Add(arg);
//																InvokeOrRecurse(activeTalent, targetLocation, index, argSelected);
//															},
//															promptTemplate));
//			else
//				world.AddMessage("No options possible in arg");
//		}
//
//		private void InvokeOrRecurse(ActiveTalentComponent activeTalent, Point targetLocation, int index, List<dynamic> argSelected) {
//			if (activeTalent.ContainsArg(index + 1))
//				RecursiveSelectOptionHelper(activeTalent, targetLocation, index + 1, argSelected);
//			else
//				activeTalent.InvokeAction(targetLocation, argSelected.ToArray());
//		}
//
//		private void HandleActiveTalent(ActiveTalentComponent activeTalent) {
//			if (activeTalent.RequiresTarget == TargetType.Positional)
//				ParentApplication.Push(
//						new TargetPrompt(activeTalent.Talent.Name, player.Position, p => RecursiveSelectOptionHelper(activeTalent, p, 0, new List<dynamic>()), MapPanel,
//										 promptTemplate));
//			else if (activeTalent.RequiresTarget == TargetType.Directional)
//				ParentApplication.Push(
//						new DirectionalPrompt(activeTalent.Talent.Name, player.Position, p => RecursiveSelectOptionHelper(activeTalent, p, 0, new List<dynamic>()), promptTemplate));
//			else
//				RecursiveSelectOptionHelper(activeTalent, player.Position, 0, new List<dynamic>());
//		}

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
			Point newPosition = entity.Get<Location>().Position + direction;

			var level = entity.Get<Location>().Level;

			if (level.IsWalkable(newPosition)) {
		
				var actorsAtNewLocation = level.EntityManager.Get(typeof (DefendComponent), typeof (Location)).Where(actor => actor.Get<Location>().Position == newPosition).ToList();

				if (actorsAtNewLocation.Count > 1)
					throw new Exception("We somehow have had 2 actors occupying the same location");
				else if (actorsAtNewLocation.Count == 1) {
					Combat.MeleeAttack(player, actorsAtNewLocation.First(), actorsAtNewLocation.First().Get<DefendComponent>().DefaultPart, World.MEAN);
				} else
					entity.Get<Location>().Position = newPosition;

				entity.Get<ActionPoint>().ActionPoints -= World.SpeedToActionPoints(World.DEFAULT_SPEED);
			} else
				World.Instance.AddMessage("There is something in the way.");
		}

		private void Wait(Entity entity) {
			entity.Get<ActionPoint>().ActionPoints -= World.SpeedToActionPoints(World.DEFAULT_SPEED);	
		}

		private void PickUpItem(Entity inventoryEntity, Entity itemEntityFromLevel, ICollection<Entity> items) {
			Contract.Requires(items != null);
			var inventory = inventoryEntity.Get<ContainerComponent>();
			var item = itemEntityFromLevel.Get<Item>();

			if (item.StackType == StackType.Hard)
				ParentApplication.Push(
						new CountPrompt("How many items to pick up?",
						                amount => PickUpStackedItem(inventoryEntity, itemEntityFromLevel, amount, items), item.Amount, 0, item.Amount, PromptTemplate));
			else {
				inventory.Add(itemEntityFromLevel);
				itemEntityFromLevel.Get<VisibleComponent>().VisibilityIndex = -1;
				items.Remove(itemEntityFromLevel);
			}
		}

		private void PickUpStackedItem(Entity inventoryEntity, Entity itemEntityFromLevel, int amount, ICollection<Entity> items) {
			Contract.Requires(amount > 0);

			if (amount == 0)
				return;

			var inventory = inventoryEntity.Get<ContainerComponent>();
			var item = itemEntityFromLevel.Get<Item>();
			
			// if an item doesn't exist in the inventory
			if (!inventory.Exist(e => e.Get<ItemRefId>() == itemEntityFromLevel.Get<ItemRefId>())) {

				// and if we're splitting an item, create a new one
				if (amount < item.Amount) {
					item.Amount -= amount;

					var tempItem = itemEntityFromLevel.Copy();					
					tempItem.Get<VisibleComponent>().VisibilityIndex = -1;

					tempItem.Get<Item>().Amount = amount;	// amount starts out as 1
					inventory.Add(tempItem);
				} else {
					inventory.Add(itemEntityFromLevel);
					itemEntityFromLevel.Get<VisibleComponent>().VisibilityIndex = -1;
					items.Remove(itemEntityFromLevel);
				}

			} else {
				inventory.GetItem(e => e.Get<ItemRefId>() == itemEntityFromLevel.Get<ItemRefId>()).Get<Item>().Amount += amount;

				if (amount < item.Amount) {
					item.Amount -= amount;
				} else {
					manager.Remove(itemEntityFromLevel);
					items.Remove(itemEntityFromLevel);
				}

			}
			Contract.Ensures(item.Amount >= 0, "item afterwards cannot have negative amounts");				
		}

		private void DropItem(Entity inventoryEntity, Entity itemEntityFromInventory, ICollection<Entity> items) {
			Contract.Requires(items != null);
			var inventory = inventoryEntity.Get<ContainerComponent>();
			var item = itemEntityFromInventory.Get<Item>();

			if (item.StackType == StackType.Hard)
				ParentApplication.Push(
						new CountPrompt("How many items to drop to the ground?",
						                amount => DropStackedItem(inventoryEntity, itemEntityFromInventory, amount, items), item.Amount, 0, item.Amount, GameplayWindow.PromptTemplate));
			else {
				inventory.Remove(itemEntityFromInventory);
				itemEntityFromInventory.Get<VisibleComponent>().Reset();
				items.Remove(itemEntityFromInventory);
			}
		}

		private void DropStackedItem(Entity inventoryEntity, Entity itemEntityFromInventory, int amount, ICollection<Entity> items) {
			Contract.Requires(amount > 0);

			if (amount == 0)
				return;

			var inventory = inventoryEntity.Get<ContainerComponent>();
			var item = itemEntityFromInventory.Get<Item>();

			Contract.Ensures(item.Amount >= 0, "item afterwards cannot have negative amounts");
			
			var level = inventoryEntity.Get<Location>().Level;

			// this only gets items that are visible at that location.  entities that are being carried aren't visible
			var itemsInLevel = level.EntityManager.Get(typeof(Location), typeof(Item), typeof(VisibleComponent)).Where(i => i.Get<VisibleComponent>().VisibilityIndex > 0).ToList();

			// if an item doesn't exist in the at the location, create one
			if (!itemsInLevel.Exists(e => e.Get<ItemRefId>() == itemEntityFromInventory.Get<ItemRefId>() && e.Get<Location>() == inventoryEntity.Get<Location>())) {

				// if amount drop is less than currently carrying, just substract it, otherwise remove it
				if (amount < item.Amount) {
					item.Amount -= amount;

					var tempItem = itemEntityFromInventory.Copy();
					inventory.Remove(tempItem);
					tempItem.Get<Item>().Amount = amount;	// amount starts out as 1

				} else {
					// if we're removing everything, just remove from the inventory and show it
					inventory.Remove(itemEntityFromInventory);
					itemEntityFromInventory.Get<VisibleComponent>().Reset();
					items.Remove(itemEntityFromInventory);
				}
				
			} else {
				itemsInLevel.First(e => e.Get<ItemRefId>() == itemEntityFromInventory.Get<ItemRefId>() && e.Get<Location>() == inventoryEntity.Get<Location>()).Get<Item>().Amount += amount;
				if (amount < item.Amount) {
					item.Amount -= amount;
				} else {
					items.Remove(itemEntityFromInventory);
					manager.Remove(itemEntityFromInventory);	// WARNING: will render itemEntityFromInventory componentless
				}		
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

						if (keyData.Character == 'd') {
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
							                                      i => World.Instance.AddMessage(String.Format("This is a {0}, it weights {1}.", i.Get<Item>().Name, i.Get<Item>().Weight))));
						} else if (keyData.Character == 'w')
							ParentApplication.Push(new InventoryWindow(new ListWindowTemplate<string>
							                                           {
							                                           		Size = MapPanel.Size,
							                                           		IsPopup = true,
							                                           		HasFrame = true,
							                                           		Items = player.Get<ContainerComponent>().Slots.ToList(),
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
													sb.AppendLine(((Level) location.Level).GetTerrain(p).Definition);
													foreach (var entity in entitiesAtLocation) {
														sb.AppendFormat("Entity: {0} ", entity.Id);
														if (entity.Has<Blocker>())
															sb.AppendFormat("Transparent: {0}, Walkable: {1} ", entity.Get<Blocker>().Transparent, entity.Get<Blocker>().Walkable);
														if (entity.Has<Item>())
															sb.AppendFormat("Item: {0}", entity.Get<Item>().Name);
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