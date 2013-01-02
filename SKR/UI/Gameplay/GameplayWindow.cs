using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entity;
using DEngine.Extensions;
using Ogui.UI;
using SKR.UI.Menus;
using SKR.Universe;
using SkrGame.Gameplay.Talent;
using SkrGame.Gameplay.Talent.Components;
using SkrGame.Systems;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Actors.PC;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Locations;
using libtcod;

namespace SKR.UI.Gameplay {
	public class PlayerMovementSystem : Manager {
		private Entity player;


		public PlayerMovementSystem(EntityManager entityManager) {
					player = entityManager.Get<PlayerMarker>().ToList()[0];
				}

//				public bool IsVisibleTo(Actor actor) {
//					return actor.CanSpot(this);
//				}
//		
//				public bool HasLineOfSight(Actor target) {
//					return HasLineOfSight(target.Position);
//				}

		private void Move(Entity entity, Point direction) {
			Point newPosition = entity.As<Location>().Position + direction;

			var level = entity.As<LevelComponent>().Level;

			if (level.IsWalkable(newPosition)) {
				var actorsAtNewLocation = level.EntityManager.Get(typeof (Actor), typeof (Location)).Where(actor => actor.As<Location>().Position == newPosition).ToList();

				if (actorsAtNewLocation.Count > 1)
					throw new Exception("We somehow have had 2 actors occupying the same location");
				else if (actorsAtNewLocation.Count == 1) {
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

		protected override void OnKeyReleased(KeyboardData keyData) {
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
						break;
					}
				}
			}

			//				default:
			//					if (keyData.Character == 'w')
			//						ParentApplication.Push(new InventoryWindow(new ListWindowTemplate<BodyPart>
			//						{
			//							Size = MapPanel.Size,
			//							IsPopup = true,
			//							HasFrame = true,
			//							Items = player.BodyPartsList,
			//						}));
			//					else if (keyData.Character == 'r')
			//						HandleActiveTalent(player.ReloadWeapon().As<ActiveTalentComponent>());
			//					else if (keyData.Character == 'f')
			//						HandleActiveTalent(player.RangeAttack().As<ActiveTalentComponent>());
			//					else if (keyData.Character == 'l') {
			//						if (keyData.ControlKeys == ControlKeys.LeftControl) {
			//							ParentApplication.Push(new LookWindow(player.Position,
			//																  p => string.Format("{0}\n{1}\nVisible: {2}, Transparent: {3}, Walkable: {4}", player.Level.GetTerrain(p).Definition,
			//																					 (player.Level.DoesFeatureExistAtLocation(p) ? player.Level.GetFeatureAtLocation(p).Asset : ""),
			//																					 player.Level.IsVisible(p), player.Level.IsTransparent(p), player.Level.IsWalkable(p)),
			//																  MapPanel, promptTemplate));
			//						} else
			//							ParentApplication.Push(new LookWindow(player.Position,
			//							                                      p => string.Format("{0}\n{1}", player.Level.GetTerrain(p).Definition,
			//							                                                         (player.Level.DoesFeatureExistAtLocation(p) ? player.Level.GetFeatureAtLocation(p).Asset : "")),
			//							                                      MapPanel, promptTemplate));
			//					}
			//							//                    } else if (keyData.Character == 'a') {
			//						//                        HandleTalent(player.GetTalent(Skill.TargetAttack));
			//					else if (keyData.Character == 'd')
			//						if (player.Items.Count() > 0)
			//							ParentApplication.Push(new ItemWindow(false,
			//																  new ListWindowTemplate<Item>
			//																  {
			//																	  Size = MapPanel.Size,
			//																	  IsPopup = true,
			//																	  HasFrame = true,
			//																	  Items = player.Items,
			//																  },
			//																  DropItem));
			//						else
			//							World.Instance.AddMessage("You are carrying no items to drop.");
			//					else if (keyData.Character == 'g')
			//						if (player.Level.Items.Where(tuple => tuple.Item1 == player.Position).Count() > 0)
			//							ParentApplication.Push(new ItemWindow(false,
			//																  new ListWindowTemplate<Item>
			//																  {
			//																	  Size = MapPanel.Size,
			//																	  IsPopup = true,
			//																	  HasFrame = true,
			//																	  Items = player.Level.Items.Where(tuple => tuple.Item1 == player.Position).Select(tuple => tuple.Item2),
			//																  },
			//																  PickUpItem));
			//						else
			//							World.Instance.AddMessage("No items here to pick up.");
			//					else if (keyData.Character == 'u')
			//						HandleActiveTalent(player.Activate().As<ActiveTalentComponent>());
			//					else if (keyData.Character == 'i')
			//						ParentApplication.Push(new ItemWindow(false,
			//															  new ListWindowTemplate<Item>
			//															  {
			//																  Size = MapPanel.Size,
			//																  IsPopup = true,
			//																  HasFrame = true,
			//																  Items = player.Items,
			//															  },
			//															  i => player.World.AddMessage(String.Format("This is a {0}, it weights {1}.", i.Name, i.Weight))));
			//
			//					break;
			//			}

		}
	}

	public class ActionSystemWrapper : Manager {
		private ActionPointSystem system;

		public ActionSystemWrapper(EntityManager manager) {
			system = new ActionPointSystem(manager);
		}

		protected override void Update() {
			base.Update();
			system.Update();
		}
	}

	public class GameplayWindow : Window {
		private readonly World world;

		public MapPanel MapPanel { get; private set; }
		public StatusPanel StatusPanel { get; private set; }
		public MessagePanel MessagePanel { get; private set; }
		public AssetsManager AssetsManager { get; private set; }

		private WindowTemplate promptTemplate;

		private EntityManager manager;

		public GameplayWindow(EntityManager manager, WindowTemplate template)
				: base(template) {
			world = World.Instance;
			AssetsManager = new AssetsManager();

			this.manager = manager;
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

			promptTemplate =
					new WindowTemplate
					{
							HasFrame = false,
							IsPopup = true,
							TopLeftPos = msgTemplate.TopLeftPos.Shift(1, 1),
							Size = new Size(Size.Width - 2, Size.Height - mapTemplate.Size.Height - 2)
					};

			AddManager(new ActionSystemWrapper(manager));
			AddManager(new PlayerMovementSystem(manager));
		}

//		private void PickUpItem(Item item) {
//			if (item.StackType == StackType.Hard)
//				ParentApplication.Push(
//						new CountPrompt("How many items to pick up?",
//						                delegate(int amount)
//						                {
//						                	if (amount < 1)
//						                		return;
//
//						                	if (amount < item.Amount)
//						                		item.Amount -= amount;
//						                	else
//						                		player.Level.RemoveItem(item);
//
//						                	// if an item doesn't exist in the inventory, create one
//						                	if (!player.Items.ToList().Exists(i => i.RefId == item.RefId)) {
//						                		var tempItem = World.Instance.CreateItem(item.RefId);
//						                		tempItem.Amount += amount - 1;
//						                		player.AddItem(tempItem);
//						                	} else
//						                		player.Items.First(i => i.RefId == item.RefId).Amount += amount;
//
//						                	// additional testing
//						                	if (item.Amount < 0)
//						                		throw new Exception("Should not be possible: have negative amount");
//						                }, item.Amount, 0, item.Amount, promptTemplate));
//			else {
//				player.Level.RemoveItem(item);
//				player.AddItem(item);
//			}
//		}
//
//		private void DropItem(Item item) {
//			if (item.StackType == StackType.Hard)
//				ParentApplication.Push(
//						new CountPrompt("How many items to drop to the ground?",
//										delegate(int amount)
//										{
//											if (amount < 1)
//												return;
//
//											if (amount < item.Amount)
//												item.Amount -= amount;
//											else
//												player.RemoveItem(item);
//
//											// if an item doesn't exist in the at the location, create one
//											if (!player.Level.Items.ToList().Exists(t => t.Item1 == player.Position && t.Item2.RefId == item.RefId)) {
//												var tempItem = World.Instance.CreateItem(item.RefId);
//												tempItem.Amount += amount - 1;
//												player.Level.AddItem(tempItem, player.Position);
//											} else
//												player.Level.Items.First(t => t.Item1 == player.Position && t.Item2.RefId == item.RefId).Item2.Amount += amount;
//
//											// additional testing
//											if (item.Amount < 0)
//												throw new Exception("Should not be possible: have negative amount");
//										}, item.Amount, 0, item.Amount, promptTemplate));
//			else {
//				player.RemoveItem(item);
//				player.Level.AddItem(item, player.Position);
//			}
//		}
	}
}