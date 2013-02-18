using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using Ogui.UI;
using SKR.UI.Menus;
using SKR.Universe;
using SkrGame.Actions;
using SkrGame.Actions.Features;
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

			StatusPanel = new StatusPanel(World.Player, statusTemplate);

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
			switch (keyboardEvent.KeyboardData.KeyCode) {
				case TCODKeyCode.Up:
				case TCODKeyCode.KeypadEight: // Up and 8 should have the same functionality
					Move(player, Direction.North);
					break;
				case TCODKeyCode.Down:
				case TCODKeyCode.KeypadTwo:
					Move(player, Direction.South);
					break;
				case TCODKeyCode.Left:
				case TCODKeyCode.KeypadFour:
					Move(player, Direction.West);
					break;
				case TCODKeyCode.KeypadFive:
					player.Get<ActorComponent>().Enqueue(new WaitAction(player));
					break;
				case TCODKeyCode.Right:
				case TCODKeyCode.KeypadSix:
					Move(player, Direction.East);
					break;
				case TCODKeyCode.KeypadSeven:
					Move(player, Direction.Northwest);
					break;
				case TCODKeyCode.KeypadNine:
					Move(player, Direction.Northeast);
					break;
				case TCODKeyCode.KeypadOne:
					Move(player, Direction.Southwest);
					break;
				case TCODKeyCode.KeypadThree:
					Move(player, Direction.Southeast);
					break;
				default:
				{
					var playerLocation = player.Get<Location>();
					if (keyboardEvent.KeyboardData.Character == 'f') {
						var weapons = FilterEquippedItems<RangeComponent>(player).ToList();

						if (weapons.Count > 1) {
							ParentApplication.Push(
									new OptionsSelectionPrompt<Entity>("With what weapon?",
									                                   weapons, e => e.Get<Identifier>().Name,
									                                   weapon => SelectRangeTarget(player, weapon),
									                                   PromptTemplate));
						} else if (weapons.Count == 1) {
							SelectRangeTarget(player, weapons.First());
						} else {
							World.Log.Normal("No possible way of shooting target.");
						}

					} else if (keyboardEvent.KeyboardData.Character == 'r') {
						var weapons = FilterEquippedItems<RangeComponent>(player).ToList();

						if (weapons.Count > 1) {
							ParentApplication.Push(
									new OptionsSelectionPrompt<Entity>("Reload what weapon?",
									                                   weapons, e => e.Get<Identifier>().Name,
									                                   weapon => Reload(player, weapon),
									                                   PromptTemplate));
						} else if (weapons.Count == 1) {
							Reload(player, weapons.First());
						} else {
							World.Log.Normal("No weapons to reload.");
						}
					} else if (keyboardEvent.KeyboardData.Character == 'u') {
						ParentApplication.Push(
								new DirectionalPrompt("What direction?",
								                      playerLocation.Point,
								                      p =>
								                      	{
								                      		var useables = playerLocation.Level.GetEntitiesAt(p).Where(e => e.Has<UseableFeature>() &&
								                      		                                                          e.Has<VisibleComponent>() &&
								                      		                                                          e.Get<VisibleComponent>().VisibilityIndex > 0);
								                      		if (useables.Count() > 1) {
								                      			ParentApplication.Push(
								                      					new OptionsSelectionPrompt<Entity>("What object do you want to use?",
								                      					                                   useables,
								                      					                                   Identifier.GetNameOrId,
								                      					                                   e => SelectUsableAction(player, e, e.Get<UseableFeature>().Uses.ToList()),
								                      					                                   PromptTemplate));
								                      		} else if (useables.Count() == 1)
								                      			SelectUsableAction(player, useables.First(), useables.First().Get<UseableFeature>().Uses.ToList());
								                      		else
								                      			World.Log.Normal("Nothing there to use.");
								                      	},
								                      PromptTemplate));
					} else if (keyboardEvent.KeyboardData.Character == 'd') {
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
							World.Log.Normal("You are carrying no items to drop.");
					} else if (keyboardEvent.KeyboardData.Character == 'g') {
						var inventory = player.Get<ContainerComponent>();

						// get all items that have a location (eg present on the map) that are at the location where are player is
						var items =
								playerLocation.Level.GetEntitiesAt(playerLocation.Point).Where(e => e.Has<Item>() &&
								                                                        e.Has<VisibleComponent>() &&
								                                                        e.Get<VisibleComponent>().VisibilityIndex > 0 &&
								                                                        (!inventory.Items.Contains(e)));

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
							World.Log.Normal("No items here to pick up.");
					} else if (keyboardEvent.KeyboardData.Character == 'i') {
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

					} else if (keyboardEvent.KeyboardData.Character == 'w')
						ParentApplication.Push(new InventoryWindow(new InventoryWindowTemplate
						                                           {
						                                           		Size = MapPanel.Size,
						                                           		IsPopup = true,
						                                           		HasFrame = true,
						                                           		World = World,
						                                           		Items = player.Get<EquipmentComponent>().Slots.ToList(),
						                                           }));
					else if (keyboardEvent.KeyboardData.Character == 'l') {
						if (keyboardEvent.KeyboardData.ControlKeys == ControlKeys.LeftControl) {} else
							ParentApplication.Push(
									new LookWindow(
											playerLocation.Point,
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
					} else if (keyboardEvent.KeyboardData.Character == 'z') {
						if (keyboardEvent.KeyboardData.ControlKeys == ControlKeys.LeftControl) {
							SetNPCActor();
						} else {
							player.Get<ActorComponent>().Enqueue(new UseFeatureAction(player, null, 1000));							
						}
//						World.Log.Special(
//								"Testing very long message for string wrapping.  We'll see how it works, hopefully very well; but if not we'll go in and fix it; won't we? Hmm, maybe I still need a longer message.  I'll just keep typing for now, hopefully making it very very very long.");
						//							player.Add(new LongAction(500, e => world.Log.Normal(String.Format("{0} completes long action", Identifier.GetNameOrId(e)))));
						//							player.Get<ActionPoint>().ActionPoints -= 100;
					}

					break;
				}
			}
		}

		private void SetNPCActor() {
			var playerLocation = player.Get<Location>();
			ParentApplication.Push(new TargetPrompt("Set AI of Entity",
			                                        playerLocation.Point,
			                                        p =>
			                                        	{
			                                        		var entitiesAtLocation = playerLocation.Level.GetEntitiesAt(p).Where(e => e.Has<ActorComponent>()).ToList();
			                                        		var npc = entitiesAtLocation.First();
			                                        		var options = new List<AbstractActor>
			                                        		              {
			                                        		              		new DoNothingActor(),
			                                        		              		new NPC()
			                                        		              };
			                                        		ParentApplication.Push(new OptionsSelectionPrompt<AbstractActor>("What AI?",
			                                        		                                                                 options, a => a.GetType().Name,
			                                        		                                                                 actor =>
			                                        		                                                                 	{
			                                        		                                                                 		var ap = npc.Get<ActorComponent>().AP;
			                                        		                                                                 		npc.Remove<ActorComponent>();
			                                        		                                                                 		npc.Add(new ActorComponent(actor, ap));
			                                        		                                                                 	},
			                                        		                                                                 PromptTemplate));
			                                        	},
			                                        MapPanel,
			                                        PromptTemplate));
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

		private void SelectMeleeTarget(Entity entity, Entity weapon, List<Entity> actorsAtNewLocation) {
			if (actorsAtNewLocation.Count == 1) {
				entity.Get<ActorComponent>().Enqueue(new MeleeAttackAction(entity, actorsAtNewLocation.First(), weapon, actorsAtNewLocation.First().Get<DefendComponent>().GetRandomPart()));
			} else {
				ParentApplication.Push(
						new OptionsSelectionPrompt<Entity>("Attack what?", actorsAtNewLocation,
						                                   e => e.ToString(),
						                                   e => entity.Get<ActorComponent>().Enqueue(new MeleeAttackAction(entity, e, weapon, e.Get<DefendComponent>().GetRandomPart())),
						                                   PromptTemplate));
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
			                                        				entity.Get<ActorComponent>().Enqueue(
			                                        						new RangeAttackAction(entity,
			                                        						                      entitiesAtLocation.First(),
			                                        						                      weapon,
			                                        						                      entitiesAtLocation.First().Get<DefendComponent>().GetRandomPart()));
			                                        			} else {
			                                        				ParentApplication.Push(
			                                        						new OptionsSelectionPrompt<Entity>("Shoot at what?", entitiesAtLocation,
			                                        						                                   e => e.ToString(),
			                                        						                                   e => entity.Get<ActorComponent>().Enqueue(
			                                        						                                   		new RangeAttackAction(entity,
			                                        						                                   		                      e,
			                                        						                                   		                      weapon,
			                                        						                                   		                      e.Get<DefendComponent>().GetRandomPart())),
			                                        						                                   PromptTemplate));
			                                        			}
			                                        		} else {
			                                        			World.Log.Normal("Nothing there to shoot.");
			                                        		}
			                                        	},
			                                        MapPanel,
			                                        PromptTemplate));
		}

		private void Reload(Entity user, Entity weapon) {
			Contract.Requires<ArgumentException>(user.Has<ContainerComponent>());

			var ammos = user.Get<ContainerComponent>().Items.Where(e => e.Has<AmmoComponent>() && e.Get<AmmoComponent>().Type == weapon.Get<RangeComponent>().AmmoType).ToList();

			if (ammos.Count > 1) {
				ParentApplication.Push(new OptionsSelectionPrompt<Entity>("What ammo?", ammos,
				                                                          ammo => ammo.Get<Identifier>().Name,
				                                                          ammo => user.Get<ActorComponent>().Enqueue(new ReloadAction(user, weapon, ammo)),
				                                                          PromptTemplate));

			} else if (ammos.Count == 1) {
				user.Get<ActorComponent>().Enqueue(new ReloadAction(user, weapon, ammos.First()));
			} else
				World.Log.Normal("No possible ammo for selected weapon.");
		}

		#region Pickup/Drop items

		private void PickUpItem(Entity user, Entity itemEntity) {
			var item = itemEntity.Get<Item>();

			if (item.StackType == StackType.Hard)
				ParentApplication.Push(
						new CountPrompt("How many items to pick up?",
						                amount => user.Get<ActorComponent>().Enqueue(new GetItemAction(user, itemEntity, amount)), item.Amount, 0, item.Amount, PromptTemplate));
			else {
				user.Get<ActorComponent>().Enqueue(new GetItemAction(user, itemEntity, 1));
			}
		}

		private void DropItem(Entity user, Entity itemEntity) {
			var item = itemEntity.Get<Item>();

			if (item.StackType == StackType.Hard)
				ParentApplication.Push(
						new CountPrompt("How many items to drop to the ground?",
						                amount => user.Get<ActorComponent>().Enqueue(new DropItemAction(user, itemEntity, amount)), item.Amount, 0, item.Amount, PromptTemplate));
			else {
				user.Get<ActorComponent>().Enqueue(new DropItemAction(user, itemEntity, 1));
			}
		}

		#endregion

		private void SelectUsableAction(Entity user, Entity thing, List<UseableFeature.UseAction> actions) {
			if (actions.Count > 1) {
				ParentApplication.Push(
						new OptionsSelectionPrompt<UseableFeature.UseAction>(String.Format("Do what with {0}?", Identifier.GetNameOrId(thing)),
						                                                     actions,
						                                                     a => a.Description, a => a.Use(user, thing, a),
						                                                     PromptTemplate));
			} else if (actions.Count == 1) {
				actions.First().Use(user, thing, actions.First());
			} else {
				World.Log.Normal(String.Format("No possible action on {0}", Identifier.GetNameOrId(thing)));
			}

		}

		private void Move(Entity user, Direction direction) {
			Contract.Requires<ArgumentNullException>(user != null, "user");

			Point newLocation = user.Get<Location>().Point + direction;

			// we check for attackables
			var actorsAtNewLocation = user.Get<Location>().Level.GetEntitiesAt(newLocation).Where(e => e.Has<DefendComponent>()).ToList();

			if (actorsAtNewLocation.Count > 0) {
				var weapons = FilterEquippedItems<MeleeComponent>(user).ToList();
				if (weapons.Count > 1) {
					ParentApplication.Push(
							new OptionsSelectionPrompt<Entity>("With that weapon?",
							                                   weapons, e => e.Get<Identifier>().Name,
							                                   weapon => SelectMeleeTarget(user, weapon, actorsAtNewLocation),
							                                   PromptTemplate));
				} else if (weapons.Count == 1)
					SelectMeleeTarget(user, weapons.First(), actorsAtNewLocation);
				else {
					World.Log.Normal("No possible way of attacking.");
					Logger.WarnFormat("Player is unable to melee attack, no unarmed component equipped or attached");
				}

				return;
			}

			user.Get<ActorComponent>().Enqueue(new BumpAction(user, direction));
		}
	}
}