using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DEngine.Core;
using DEngine.Extensions;
using DEngine.UI;
using SKR.Gameplay.Combat;
using SKR.Gameplay.Talent;
using SKR.UI.Gameplay;
using SKR.Universe;
using SKR.Universe.Entities.Actors;
using SKR.Universe.Entities.Items;
using libtcod;
using log4net.Config;

namespace SKR {
    public class RoguelikeApp : Application {
        private World world;
        protected override void Setup(ApplicationInfo info) {
            base.Setup(info);
            world = World.Create();            
            Push(new GameplayWindow(new WindowTemplate()));
            //            SetWindow(new CharGenWindow(windowTemplate));
            //            SetWindow(mainWindow);
        }

        protected override void Update() {
            base.Update();
            world.Update();            
        }
    }

    public class Program {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public const double Epsilon = 0.001;
        public const int FpsLimit = 60;
        public const int GameUpdatePerSecond = 30;
        public const int GameUpdateHz = 1000 / GameUpdatePerSecond;
        public const int InitialDelay = 100;
        public const int IntervalDelay = 75;
        public static readonly Size ScreenSize = new Size(80, 60);

#if DEBUG
        public static bool SeeAll = false;
        public static bool GodMode = false;
#endif        

        public static void Main(string[] args) {                                        
            XmlConfigurator.Configure(new FileInfo("Log.xml"));                       

            Logger.InfoFormat("TCODConsole.root initialized {0}", ScreenSize);
            Logger.InfoFormat("Keyboard Repeat Limit.  Initial delay:: {0} milliseconds, Interval: {1} milliseconds", InitialDelay,
                                      IntervalDelay);
            Logger.InfoFormat("FPS Limit: {0}.", FpsLimit);
            using (RoguelikeApp app = new RoguelikeApp()) {
                app.Start(new ApplicationInfo()
                {
                    Title = "SKR",
                    ScreenSize = ScreenSize,
                    Font = "Data/Font/lucida10x10_gs_tc.png",
//                    Font = "Data/Font/consolas12x12_gs_tc.png",
                    FpsLimit = FpsLimit,
                    InitialDelay = InitialDelay,
                    IntervalDelay = IntervalDelay,
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
                    FontFlags = TCODFontFlags.Greyscale | TCODFontFlags.LayoutTCOD
                });
            }      
        }
    }
}
