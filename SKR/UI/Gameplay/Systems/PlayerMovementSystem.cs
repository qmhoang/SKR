using System;
using System.Diagnostics.Contracts;
using System.Linq;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entity;
using Ogui.UI;
using SKR.UI.Menus;
using SkrGame.Gameplay.Combat;
using SkrGame.Universe;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Actors.PC;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Locations;
using libtcod;

namespace SKR.UI.Gameplay.Systems {
	public class PlayerMovementSystem : Manager {
		private Entity player;


		public PlayerMovementSystem(EntityManager entityManager) {
			player = World.Instance.Player;
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

//		}
	}
}