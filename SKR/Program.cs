using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using DEngine.Extensions;
using Ogui.Core;
using Ogui.UI;
using SKR.UI.Gameplay;
using SKR.UI.Menus;
using SKR.Universe;
using SkrGame.Actions;
using SkrGame.Gameplay.Combat;
using SkrGame.Universe;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;
using libtcod;
using log4net.Config;

namespace SKR {
	public class SKRApp : Application {
		private World world;

		protected override void Setup(ApplicationInfo info) {
			base.Setup(info);

			world = new World();


			world.CurrentLevel = world.MapFactory.Construct("TestMap");

			var player = world.EntityManager.Create(new List<DEngine.Entities.Component>
			                                  {
			                                  		new Sprite("player", Sprite.PLAYER_LAYER),
			                                  		new Identifier("Player"),
			                                  		new Location(0, 0, world.CurrentLevel),
			                                  		new ActorComponent(new Player(), new AP()),
			                                  		new Person(),
			                                  		new DefendComponent(),
			                                  		new ContainerComponent(),
			                                  		new EquipmentComponent(),
			                                  		new VisibleComponent(10),
			                                  		new SightComponent()
			                                  });
			
			var punch =
					new MeleeComponent.Template
					{
						ComponentId = "punch",
						ActionDescription = "punch",
						ActionDescriptionPlural = "punches",
						Skill = "skill_unarmed",
						HitBonus = 0,
						Damage = Rand.Constant(-5),
						DamageType = Combat.DamageTypes["crush"],
						Penetration = 1,
						WeaponSpeed = 100,
						APToReady = 1,
						Reach = 0,
						Strength = 1,
						Parry = 0
					};

			player.Add(new MeleeComponent(punch));

			world.Player = player;

			var npc = world.EntityManager.Create(new List<DEngine.Entities.Component>
			                               {
			                               		new Sprite("npc", Sprite.ACTOR_LAYER),
			                               		new Identifier("npc"),
			                               		new Location(6, 2, world.CurrentLevel),
												new ActorComponent(new NPC(), new AP()),
			                               		new Person(),
			                               		new DefendComponent(),
			                               		new VisibleComponent(10),
			                               		new ContainerComponent(),
			                               		new EquipmentComponent(),
			                               });

			world.EntityManager.Create(world.EntityFactory.Get("smallknife")).Add(new Location(1, 1, world.CurrentLevel));
			world.EntityManager.Create(world.EntityFactory.Get("axe")).Add(new Location(1, 1, world.CurrentLevel));
			world.EntityManager.Create(world.EntityFactory.Get("glock17")).Add(new Location(1, 1, world.CurrentLevel));
			var ammo = world.EntityManager.Create(world.EntityFactory.Get("9x9mm")).Add(new Location(1, 1, world.CurrentLevel));
			ammo.Get<Item>().Amount = 30;
			world.EntityManager.Create(world.EntityFactory.Get("bullet")).Add(new Location(1, 1, world.CurrentLevel));

			var armor = world.EntityManager.Create(world.EntityFactory.Get("footballpads")).Add(new Location(1, 1, world.CurrentLevel));
			//			npc.Get<ContainerComponent>().Add(armor);
			npc.Get<ActorComponent>().Enqueue(new EquipItem(npc, armor, "Torso", true));
			npc.Add(new MeleeComponent(punch));

			world.Initialize();
			Push(new CharGen(world, new WindowTemplate()));
		}

		protected override void Update() {
			base.Update();
			world.UpdateSystems();
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
			using (SKRApp app = new SKRApp())
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