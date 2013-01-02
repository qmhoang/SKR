using System;
using System.Linq;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entity;
using Ogui.UI;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Actors.PC;
using SkrGame.Universe.Locations;
using libtcod;

namespace SKR.UI.Gameplay.Systems {
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

			var level = entity.As<Location>().Level;

			if (level.IsWalkable(newPosition)) {
				// todo fix hack cast?
				var actorsAtNewLocation = ((Level) level).EntityManager.Get(typeof (Actor), typeof (Location)).Where(actor => actor.As<Location>().Position == newPosition).ToList();

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
}