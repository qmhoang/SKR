﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using DEngine.Actions;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using DEngine.Extensions;
using Ogui;
using Ogui.Core;
using Ogui.UI;
using SKR.UI.Gameplay;
using SKR.UI.Menus;
using SkrGame.Actions;
using SkrGame.Actions.Items;
using SkrGame.Gameplay.Combat;
using SkrGame.Universe;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Controllers;
using SkrGame.Universe.Entities.Items;
using libtcod;
using log4net.Config;

namespace SKR {
	public class RoguelikeApp : Application {
		private World world;

		protected override void Setup(ApplicationInfo info) {
			base.Setup(info);

			world = new World();
			Push(new CharGen(new SkrWindowTemplate
			                 {
			                 		World = world
			                 }));
		}

		protected override void Update(uint elapsedTime) {
			base.Update(elapsedTime);
			if (world.Status == WorldStatus.Initialized) {
				world.UpdateSystems();				
			}
		}
	}


	public static class Program {
		private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		
		public const int FpsLimit = 60;
		public const int GameUpdatePerSecond = 100;
		public const int InitialDelay = 100;
		public const int IntervalDelay = 100;
		public static readonly Size ScreenSize = new Size(80, 60);

		public static BooleanSwitch SeeAll = new BooleanSwitch("SeeAll", "See everything (no FOV checking)");
		public static BooleanSwitch GodMod = new BooleanSwitch("GodMode", "God mode");
		public static BooleanSwitch FreeWalk = new BooleanSwitch("Free Walk", "Lets the player walk anywhere.");

		public static void Main(string[] args) {
			XmlConfigurator.Configure(new FileInfo("Log.xml"));	
			
			Logger.InfoFormat("TCODConsole.root initialized {0}", ScreenSize);
			Logger.InfoFormat("Keyboard Repeat Limit.  Initial delay:: {0} milliseconds, Interval: {1} milliseconds", InitialDelay,
			                  IntervalDelay);
			Logger.InfoFormat("FPS Limit: {0}.", FpsLimit);
			using (RoguelikeApp app = new RoguelikeApp())
				app.Start(new ApplicationInfo()
				          {
				          		Title = "SKR",
				          		ScreenSize = ScreenSize,
//                    Font = "Data/Font/lucida10x10_gs_tc.png",
//                    Font = "Data/Font/consolas12x12_gs_tc.png",
				          		Font = "Data/Font/terminal12x12_gs_ro.png",
				          		UpdatesPerSecondLimit = GameUpdatePerSecond,
								FpsLimit = FpsLimit,
				          		InitialDelay = InitialDelay,
				          		IntervalDelay = IntervalDelay,
				          		Pigments = new PigmentAlternatives
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
				          		FontFlags = TCODFontFlags.Greyscale | TCODFontFlags.LayoutAsciiInRow,
								RendererType = TCODRendererType.OpenGL
				          });
		}
	}
}