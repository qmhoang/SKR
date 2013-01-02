using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entity;
using DEngine.Extensions;
using Ogui.Core;
using Ogui.UI;
using SKR.UI.Gameplay;
using SKR.Universe;
using SkrGame.Universe;
using libtcod;
using log4net.Config;

namespace SKR {
//	public class MonsterMovement : Manager {
//		private FilteredCollection updateables;
//
//		public MonsterMovement(EntityManager entityManager) {
//			updateables = entityManager.Get(typeof (ActionPoint), typeof(Position));
//		}
//
//		protected override void Update() {
//			base.Update();
//
//			foreach (var updateable in updateables) {
//				if (!updateable.Is<PlayerMarker>()) {
//					while (updateable.As<ActionPoint>().Updateable) {
//						updateable.As<Position>().X++;
//						updateable.As<ActionPoint>().ActionPoints -= 100;
//					}
//				}
//
//			}
//		}
//	}
//
//	public class PlayerMovement : Manager {
//		private Entity player;
//
//
//		public PlayerMovement(EntityManager entityManager) {
//			player = entityManager.Get<PlayerMarker>().ToList()[0];
//		}
//
//		protected override void OnKeyReleased(KeyboardData keyData) {
//			base.OnKeyReleased(keyData);
//			if (player.As<ActionPoint>().Updateable) {
//				switch (keyData.KeyCode) {
//					case TCODKeyCode.Up:
//					case TCODKeyCode.KeypadEight: // Up and 8 should have the same functionality
//						player.As<Position>().Y--;
//						player.As<ActionPoint>().ActionPoints -= 100;
//						break;
//					case TCODKeyCode.Down:
//					case TCODKeyCode.KeypadTwo:
//						player.As<Position>().Y++;
//						player.As<ActionPoint>().ActionPoints -= 100;
//						break;
//					case TCODKeyCode.Left:
//					case TCODKeyCode.KeypadFour:
//						player.As<Position>().X--;
//						player.As<ActionPoint>().ActionPoints -= 100;
//						break;
//					case TCODKeyCode.KeypadFive:
//						break;
//					case TCODKeyCode.Right:
//					case TCODKeyCode.KeypadSix:
//						player.As<Position>().X++;
//						player.As<ActionPoint>().ActionPoints -= 100;
//						break;
//					case TCODKeyCode.KeypadSeven:
//						break;
//					case TCODKeyCode.KeypadNine:
//						break;
//					case TCODKeyCode.KeypadOne:
//						break;
//					case TCODKeyCode.KeypadThree:
//						break;
//				}
//			}
//
//		}
//	}

	public class RoguelikeApp : Application {
		private World world;

//		private EntityManager entityManager;

		protected override void Setup(ApplicationInfo info) {
			base.Setup(info);
			world = World.Create();
			Push(new GameplayWindow(world.EntityManager, new WindowTemplate()));

//			entityManager = new EntityManager();
//
//			entityManager.Create().Add(new ActionPoint()).Add(new Sprite("", '@', 1)).Add(new Position(3, 4)).Add(new PlayerMarker());
//			entityManager.Create().Add(new ActionPoint()).Add(new Sprite("", '$', 1)).Add(new Position(9, 4));
//			var drawing = new DrawingSystem(entityManager, new WindowTemplate());
//			drawing.AddManager(new APSystem(entityManager));
//			drawing.AddManager(new MonsterMovement(entityManager));
//			drawing.AddManager(new PlayerMovement(entityManager));
//			Push(drawing);
			
		}

		protected override void Update() {
			base.Update();
//			world.Update();
		}
	}


	public class Program {
		private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		
		public const int FPS_LIMIT = 60;
		public const int GAME_UPDATE_PER_SECOND = 30;
		public const int GAME_UPDATE_HZ = 1000 / GAME_UPDATE_PER_SECOND;
		public const int INITIAL_DELAY = 100;
		public const int INTERVAL_DELAY = 75;
		public static readonly Size ScreenSize = new Size(80, 60);

		public static BooleanSwitch SeeAll = new BooleanSwitch("SeeAll", "See everything (no FOV checking)");
		public static BooleanSwitch GodMod = new BooleanSwitch("GodMode", "God mode");

		public static void Main(string[] args) {
			XmlConfigurator.Configure(new FileInfo("Log.xml"));

			Logger.InfoFormat("TCODConsole.root initialized {0}", ScreenSize);
			Logger.InfoFormat("Keyboard Repeat Limit.  Initial delay:: {0} milliseconds, Interval: {1} milliseconds", INITIAL_DELAY,
			                  INTERVAL_DELAY);
			Logger.InfoFormat("FPS Limit: {0}.", FPS_LIMIT);
			using (RoguelikeApp app = new RoguelikeApp())
				app.Start(new ApplicationInfo()
				          {
				          		Title = "SKR",
				          		ScreenSize = ScreenSize,
//                    Font = "Data/Font/lucida10x10_gs_tc.png",
//                    Font = "Data/Font/consolas12x12_gs_tc.png",
				          		Font = "Data/Font/terminal12x12_gs_ro.png",
				          		FpsLimit = FPS_LIMIT,
				          		InitialDelay = INITIAL_DELAY,
				          		IntervalDelay = INTERVAL_DELAY,
				          		Pigments = new PigmentAlternatives()
				          		           {
				          		           		{PigmentType.FrameFocus, new Pigment(ColorPresets.White, ColorPresets.Black)},
				          		           		{PigmentType.FrameNormal, new Pigment(ColorPresets.White, ColorPresets.Black)},
				          		           		{PigmentType.FrameInactive, new Pigment(ColorPresets.White, ColorPresets.Black)},
				          		           		{PigmentType.FrameHilight, new Pigment(ColorPresets.White, ColorPresets.Black)},
				          		           		{PigmentType.FrameDepressed, new Pigment(ColorPresets.White, ColorPresets.Black)},
				          		           		{PigmentType.FrameSelected, new Pigment(ColorPresets.White, ColorPresets.Black)},
				          		           		{PigmentType.ViewFocus, new Pigment(ColorPresets.LightGray, ColorPresets.DarkerGrey)},
				          		           		{PigmentType.ViewNormal, new Pigment(ColorPresets.White, ColorPresets.Black)},
				          		           		{PigmentType.ViewInactive, new Pigment(ColorPresets.White, ColorPresets.Black)},
				          		           		{PigmentType.ViewHilight, new Pigment(ColorPresets.White, ColorPresets.DarkGrey)},
				          		           		{PigmentType.ViewDepressed, new Pigment(ColorPresets.White, ColorPresets.DarkestGrey)},
				          		           		{PigmentType.ViewSelected, new Pigment(ColorPresets.Green, ColorPresets.Black)}
				          		           },
				          		FontFlags = TCODFontFlags.Greyscale | TCODFontFlags.LayoutAsciiInRow
				          });
		}
	}
}