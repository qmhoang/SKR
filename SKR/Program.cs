using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Core;
using DEngine.Extensions;
using Ogui.Core;
using Ogui.UI;
using SKR.UI.Gameplay;
using SKR.Universe;
using SkrGame.Universe;
using libtcod;
using log4net.Config;

namespace SKR {
	public class RoguelikeApp : Application {
		private World world;

		protected override void Setup(ApplicationInfo info) {
			base.Setup(info);
			world = World.Create();
			Push(new GameplayWindow(new WindowTemplate()));
		}

		protected override void Update() {
			base.Update();
			world.Update();
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