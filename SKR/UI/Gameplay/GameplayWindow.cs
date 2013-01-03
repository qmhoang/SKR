using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection.Emit;
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
using SkrGame.Gameplay.Talent;
using SkrGame.Gameplay.Talent.Components;
using SkrGame.Systems;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;
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
			Point newPosition = entity.As<Location>().Position + direction;

			var level = entity.As<Location>().Level;

			if (level.IsWalkable(newPosition)) {
		
				var actorsAtNewLocation = level.EntityManager.Get(typeof (DefendComponent), typeof (Location)).Where(actor => actor.As<Location>().Position == newPosition).ToList();

				if (actorsAtNewLocation.Count > 1)
					throw new Exception("We somehow have had 2 actors occupying the same location");
				else if (actorsAtNewLocation.Count == 1) {
					Combat.MeleeAttack(player, actorsAtNewLocation.First(), actorsAtNewLocation.First().As<DefendComponent>().DefaultPart, World.MEAN);
					//				MeleeAttack().As<ActiveTalentComponent>().InvokeAction(newPosition);
				} else
					entity.As<Location>().Position = newPosition;

				entity.As<ActionPoint>().ActionPoints -= World.SpeedToActionPoints(World.DEFAULT_SPEED);
			} else
				World.Instance.AddMessage("There is something in the way.");
		}

		private void Wait(Entity entity) {
			entity.As<ActionPoint>().ActionPoints -= World.SpeedToActionPoints(World.DEFAULT_SPEED);	
		}

		private void PickUpItem(Entity inventoryEntity, Entity itemEntity) {
			var inventory = inventoryEntity.As<ContainerComponent>();
			var item = itemEntity.As<Item>();

			if (item.StackType == StackType.Hard)
				ParentApplication.Push(
						new CountPrompt("How many items to pick up?", amount => PickUpStackedItem(inventoryEntity, inventoryEntity, amount), item.Amount, 0, item.Amount, PromptTemplate));
			else {
				itemEntity.Remove<Location>();
				inventory.AddItem(itemEntity);
			}
		}

		private void PickUpStackedItem(Entity inventoryEntity, Entity itemEntity, int amount) {

			var inventory = inventoryEntity.As<ContainerComponent>();
			var item = itemEntity.As<Item>();



			if (amount < item.Amount)
				item.Amount -= amount;
			else
				itemEntity.Remove<Location>();

			// if an item doesn't exist in the inventory, create one
			if (inventory.Exist(e => e.As<Item>().RefId == item.RefId)) {
				var tempItem = inventoryEntity.As<Location>().Level.EntityManager.Create(World.Instance.ItemFactory.Construct(item.RefId));
				
				tempItem.As<Item>().Amount += amount - 1;	// amount starts out as 1
				inventory.AddItem(tempItem);
			} else {
				inventory.GetItem(e => e.As<Item>().RefId == item.RefId).As<Item>().Amount += amount;
			}
			Contract.Requires(amount > 1);
			Contract.Ensures(item.Amount >= 0, "item afterwards cannot have negative amounts");				
		}

		private void DropItem(Entity inventoryEntity, Entity itemEntity) {
			var inventory = inventoryEntity.As<ContainerComponent>();
			var item = itemEntity.As<Item>();

			if (item.StackType == StackType.Hard)
				ParentApplication.Push(
						new CountPrompt("How many items to drop to the ground?",
						                amount => DropStackedItem(inventoryEntity, itemEntity, amount), item.Amount, 0, item.Amount, GameplayWindow.PromptTemplate));
			else {
				inventory.RemoveItem(itemEntity);
				itemEntity.Add(new Location(inventoryEntity.As<Location>().Position, inventoryEntity.As<Location>().Level));				
			}
		}

		private void DropStackedItem(Entity inventoryEntity, Entity itemEntity, int amount) {
			var inventory = inventoryEntity.As<ContainerComponent>();
			var item = itemEntity.As<Item>();

			Contract.Requires(amount > 1);
			Contract.Ensures(item.Amount >= 0, "item afterwards cannot have negative amounts");

			// if amount drop is less than currently carrying, just substract it, otherwise remove it
			if (amount < item.Amount)
				item.Amount -= amount;
			else
				inventory.RemoveItem(itemEntity);

			// if an item doesn't exist in the at the location, create one
			var level = inventoryEntity.As<Location>().Level;
			var itemsInLevel = level.EntityManager.Get(typeof (Location), typeof (Item)).ToList();
			if (!itemsInLevel.Exists(e => e.As<Item>().RefId == item.RefId && e.As<Location>() == inventoryEntity.As<Location>())) {
				var tempItem = inventoryEntity.As<Location>().Level.EntityManager.Create(World.Instance.ItemFactory.Construct(item.RefId));
				tempItem.As<Item>().Amount += amount - 1;	// amount starts out as 1

				tempItem.Add(new Location(inventoryEntity.As<Location>().Position, inventoryEntity.As<Location>().Level));
			} else {
				itemsInLevel.First(e => e.As<Item>().RefId == item.RefId && e.As<Location>() == inventoryEntity.As<Location>()).As<Item>().Amount += amount;
				// todo improve				
			}

		}

		protected override void OnKeyPressed(KeyboardData keyData) {
			base.OnKeyReleased(keyData);
			if (player.As<ActionPoint>().Updateable) {
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
						if (keyData.Character == 'd') {
							var inventory = player.As<ContainerComponent>();
							if (inventory.Items.Count() > 0)
								ParentApplication.Push(new ItemWindow(false,
																	  new ListWindowTemplate<Entity>
																	  {
																		  Size = MapPanel.Size,
																		  IsPopup = true,
																		  HasFrame = true,
																		  Items = inventory.Items,
																	  }, i => DropItem(player, i)));
							else
								World.Instance.AddMessage("You are carrying no items to drop.");
						} else {
							var location = player.As<Location>();
							if (keyData.Character == 'g') {
								var level = location.Level;
								// get all items that have a location (eg present on the map) that are at the location where are player is
								var items = level.EntityManager.Get(typeof(Item), typeof(Location)).Where(e => e.As<Location>().Position == location.Position).ToList();
								if (items.Count() > 0)
									ParentApplication.Push(new ItemWindow(false,
									                                      new ListWindowTemplate<Entity>
									                                      {
									                                      		Size = MapPanel.Size,
									                                      		IsPopup = true,
									                                      		HasFrame = true,
									                                      		Items = items,
									                                      },
									                                      i =>
									                                      {
									                                      	PickUpItem(player, i);
									                                      	items.Remove(i);
									                                      }));
								else
									World.Instance.AddMessage("No items here to pick up.");
							} else if (keyData.Character == 'i') {
								var inventory = player.As<ContainerComponent>();

								ParentApplication.Push(new ItemWindow(false,
								                                      new ListWindowTemplate<Entity>
								                                      {
								                                      		Size = MapPanel.Size,
								                                      		IsPopup = true,
								                                      		HasFrame = true,
								                                      		Items = inventory.Items,
								                                      },
								                                      i => World.Instance.AddMessage(String.Format("This is a {0}, it weights {1}.", i.As<Item>().Name, i.As<Item>().Weight))));
							} else if (keyData.Character == 'w')
								ParentApplication.Push(new InventoryWindow(new ListWindowTemplate<string>
								                                           {
								                                           		Size = MapPanel.Size,
								                                           		IsPopup = true,
								                                           		HasFrame = true,
								                                           		Items = player.As<ContainerComponent>().Slots,
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
															if (entity.Is<Blocker>())
																sb.AppendFormat("Transparent: {0}, Walkable: {1} ", entity.As<Blocker>().Transparent, entity.As<Blocker>().Walkable);
															if (entity.Is<Item>())
																sb.AppendFormat("Item: {0}", entity.As<Item>().Name);
															sb.AppendLine();
														}

														return sb.ToString();
													},
													MapPanel,
													GameplayWindow.PromptTemplate));
							}
						}
						break;
					}
				}
			}
		}
	}
}