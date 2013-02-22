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

		protected override void Update() {
			base.Update();
			if (!World.RequireNewPrompt)
				return;

			switch (World.CurrentAction.RequiresPrompt) {
				case PromptType.None:
					throw new ArgumentOutOfRangeException();
				case PromptType.Boolean:
				{
					World.RequireNewPrompt = false;
					var action = (IBooleanAction) World.CurrentAction;
					Boolean(action.Message, action.Default, action.SetBoolean);
				}
					break;
				case PromptType.Number:
				{
					World.RequireNewPrompt = false;
					var action = (INumberAction) World.CurrentAction;
					Number(action.Message, action.Mininum, action.Maximum, action.Initial, action.SetNumber);
				}
					break;
				case PromptType.Location:
				{
					World.RequireNewPrompt = false;
					var action = (ILocationAction) World.CurrentAction;
					Targets(action.Message, action.SetLocation, MapPanel);
				}
					break;
				case PromptType.Direction:
				{
					World.RequireNewPrompt = false;
					var action = (IDirectionAction) World.CurrentAction;
					Directions(action.Message, p => action.SetDirection(p - player.Get<GameObject>().Location));
				}

					break;
				case PromptType.Options:
				{
					World.RequireNewPrompt = false;

					var action = (IOptionsAction) World.CurrentAction;
					Options(action.Message, action.Options, s => s, action.SetOption, action.Fail);
				}
					break;
				case PromptType.PlayerInput:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void GameplayWindow_KeyPressed(object sender, KeyboardEventArgs keyboardEvent) {
			if (World.CurrentAction.RequiresPrompt != PromptType.PlayerInput)
				return;
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
							Options("With what weapon?", FilterEquippedItems<RangeComponent>(player).ToList(), Identifier.GetNameOrId, weapon => Targets("Shoot where?",
							                                                                                                                             p => Options("Shoot at what?", player.Get<GameObject>().Level.GetEntitiesAt(p).Where(e => e.Has<DefendComponent>()), Identifier.GetNameOrId, defender => Enqueue(
							                                                                                                                             		new RangeAttackAction(player,
							                                                                                                                             		                      defender,
							                                                                                                                             		                      weapon,
							                                                                                                                             		                      defender.Get<DefendComponent>().GetRandomPart())), () => World.Log.Fail("Nothing there to shoot.")), MapPanel), () => World.Log.Fail("No possible way of shooting target."));

						}
							break;
						case 'F':
						{
							Options("With what weapon?", FilterEquippedItems<RangeComponent>(player).ToList(), Identifier.GetNameOrId, weapon => Targets("Shoot where?",
							                                                                                                                             p => Options("Shoot at what?", player.Get<GameObject>().Level.GetEntitiesAt(p).Where(e => e.Has<DefendComponent>()), Identifier.GetNameOrId, defender => Options("What part?", defender.Get<DefendComponent>().BodyPartsList, bp => bp.Name, bp => Enqueue(new RangeAttackAction(player,
							                                                                                                                                                                                                                                                                                                                                                                                                              defender,
							                                                                                                                                                                                                                                                                                                                                                                                                              weapon,
							                                                                                                                                                                                                                                                                                                                                                                                                              bp,
							                                                                                                                                                                                                                                                                                                                                                                                                              true)), () => World.Log.Fail(String.Format("{0} has no possible part to attack.  How did we get here?", Identifier.GetNameOrId(defender)))), () => World.Log.Fail("Nothing there to shoot.")), MapPanel), () => World.Log.Fail("No possible way of shooting target."));

						}
							break;
						case 'r':
						{
							Options("Reload what weapon?", FilterEquippedItems<RangeComponent>(player).ToList(), Identifier.GetNameOrId, weapon => Options("What ammo?", player.Get<ContainerComponent>().Items.Where(e => e.Has<AmmoComponent>() &&
							                                                                                                                                                                                               e.Get<AmmoComponent>().Type == weapon.Get<RangeComponent>().AmmoType).ToList(), Identifier.GetNameOrId, ammo => Enqueue(new ReloadAction(player, weapon, ammo)), () => World.Log.Fail("No possible ammo for selected weapon.")), () => World.Log.Fail("No weapons to reload."));
						}
							break;
						case 'u':
							Directions("What direction?", p => Options("What object do you want to use?", playerLocation.Level.GetEntitiesAt(p).Where(e => e.Has<UseableFeature>() &&
							                                                                                                                               e.Has<VisibleComponent>() &&
							                                                                                                                               e.Get<VisibleComponent>().VisibilityIndex > 0), Identifier.GetNameOrId, useable => Options(String.Format("Do what with {0}?", Identifier.GetNameOrId(useable)), useable.Get<UseableFeature>().Uses.ToList(), use => use.Description, use => use.Use(player, useable, use), () => World.Log.Fail(String.Format("No possible action on {0}", Identifier.GetNameOrId(useable)))), () => World.Log.Fail("Nothing there to use.")));
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
							Options("With what?", player.Get<ContainerComponent>().Items.Where(i => i.Has<Lockpick>()), Identifier.GetNameOrId, lockpick => Directions("What direction?", p => Options("Select what to lockpick.", player.Get<GameObject>().Level.GetEntitiesAt(p).Where(e => e.Has<LockedFeature>()), Identifier.GetNameOrId, feature => Enqueue(new LockpickAction(player, lockpick, feature)), () => World.Log.Fail("Nothing there to lockpick."))), () => World.Log.Fail("No lockpicks available."));

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
								Targets("Set AI of Entity",
								        p =>
								        	{
								        		var entitiesAtLocation = playerLocation.Level.GetEntitiesAt(p).Where(e => e.Has<ActorComponent>()).ToList();
								        		var npc = entitiesAtLocation.First();
								        		var options = new List<AbstractActor>
								        		              {
								        		              		new DoNothingActor(),
								        		              		new NPC()
								        		              };
								        		Options("What AI?", options, o => o.GetType().Name, actor =>
								        		                                                    	{
								        		                                                    		var ap = npc.Get<ActorComponent>().AP;
								        		                                                    		npc.Remove<ActorComponent>();
								        		                                                    		npc.Add(new ActorComponent(actor, ap));
								        		                                                    	}, () => World.Log.Fail("No AI options."));
								        	}, MapPanel);
							} else {
								Enqueue(new PlayerPreAttack(player, Direction.N, true));
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
				Options("With that weapon?", FilterEquippedItems<MeleeComponent>(player).ToList(), Identifier.GetNameOrId, weapon => Options("Attack what?", actorsAtNewLocation, Identifier.GetNameOrId, target => Enqueue(new MeleeAttackAction(player, target, weapon, target.Get<DefendComponent>().GetRandomPart())), () => World.Log.Fail("Nothing at location to attack.")), () => World.Log.Fail("No possible way of attacking."));

				return;
			}

			Enqueue(new BumpAction(player, direction));
		}

		public void Enqueue(IAction action) {
			player.Get<ActorComponent>().Enqueue(action);
			((IPlayerInputAction)World.CurrentAction).SetFinished();
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
	
	}
}