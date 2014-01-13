using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.CAS
{
    public class TattooTuning
    {
        [Tunable, TunableComment("Default AnkleOuterL Scale")]
        public static float kDefaultAnkleOuterLScales = 0.5f;
        [Tunable, TunableComment("Default AnkleOuterR Scale")]
        public static float kDefaultAnkleOuterRScales = 0.5f;
        [Tunable, TunableComment("Default BreastUpperL Scale")]
        public static float kDefaultBreastUpperLScales = 0.7f;
        [Tunable, TunableComment("Default BreastUpperR Scale")]
        public static float kDefaultBreastUpperRScales = 0.7f;
        [Tunable, TunableComment("Default ButtLeft Scale")]
        public static float kDefaultButtLeftScales = 0.7f;
        [Tunable, TunableComment("Default ButtRight Scale")]
        public static float kDefaultButtRightScales = 0.7f;
        [Tunable, TunableComment("Default CalfBackL Scale")]
        public static float kDefaultCalfBackLScales = 0.7f;
        [Tunable, TunableComment("Default CalfBackR Scale")]
        public static float kDefaultCalfBackRScales = 0.7f;
        [Tunable, TunableComment("Default CalfFrontL Scale")]
        public static float kDefaultCalfFrontLScales = 0.7f;
        [Tunable, TunableComment("Default CalfFrontR Scale")]
        public static float kDefaultCalfFrontRScales = 0.7f;
        [Tunable, TunableComment("Default CheekLeft Scale")]
        public static float kDefaultCheekLeftScales = 0.5f;
        [Tunable, TunableComment("Default CheekRight Scale")]
        public static float kDefaultCheekRightScales = 0.5f;
        [Tunable, TunableComment("Default FootLeft Scale")]
        public static float kDefaultFootLeftScales = 0.7f;
        [Tunable, TunableComment("Default FootRight Scale")]
        public static float kDefaultFootRightScales = 0.7f;
        [Tunable, TunableComment("Default Forehead Scale")]
        public static float kDefaultForeheadScales = 0.5f;
        [Tunable, TunableComment("Default FullBody Scale")]
        public static float kDefaultFullBodyScales = 1f;
        [Tunable, TunableComment("Default FullFace Scale")]
        public static float kDefaultFullFaceScales = 1f;
        [Tunable, TunableComment("Default HandLeft Scale")]
        public static float kDefaultHandLeftScales = 0.4f;
        [Tunable, TunableComment("Default HandRight Scale")]
        public static float kDefaultHandRightScales = 0.4f;
        [Tunable, TunableComment("Default HipLeft Scale")]
        public static float kDefaultHipLeftScales = 0.7f;
        [Tunable, TunableComment("Default HipRight Scale")]
        public static float kDefaultHipRightScales = 0.7f;
        [Tunable, TunableComment("Default LowerBelly Scale")]
        public static float kDefaultLowerBellyScales = 0.7f;
        [Tunable, TunableComment("Default LowerLowBack Scale")]
        public static float kDefaultLowerLowBackScales = 0.7f;
        [Tunable, TunableComment("Default PalmLeft Scale")]
        public static float kDefaultPalmLeftScales = 0.4f;
        [Tunable, TunableComment("Default PalmRight Scale")]
        public static float kDefaultPalmRightScales = 0.4f;
        [Tunable, TunableComment("Default RibsLeft Scale")]
        public static float kDefaultRibsLeftScales = 0.7f;
        [Tunable, TunableComment("Default RibsRight Scale")]
        public static float kDefaultRibsRightScales = 0.7f;
        [Tunable, TunableComment("Default ShoulderBackL Scale")]
        public static float kDefaultShoulderBackLScales = 0.7f;
        [Tunable, TunableComment("Default ShoulderBackR Scale")]
        public static float kDefaultShoulderBackRScales = 0.7f;
        [Tunable, TunableComment("Default ThighBackL Scale")]
        public static float kDefaultThighBackLScales = 0.7f;
        [Tunable, TunableComment("Default ThighBackR Scale")]
        public static float kDefaultThighBackRScales = 0.7f;
        [Tunable, TunableComment("Default ThighFrontL Scale")]
        public static float kDefaultThighFrontLScales = 0.7f;
        [Tunable, TunableComment("Default ThighFrontR Scale")]
        public static float kDefaultThighFrontRScales = 0.7f;
        [Tunable, TunableComment("Default Throat Scale")]
        public static float kDefaultThroatScales = 0.7f;
        [Tunable, TunableComment("Default Pubic Scale")]
        public static float kDefaultPubicScales = 0.5f;
        [Tunable, TunableComment("Default Left Nipple Scale")]
        public static float kDefaultNippleLeftScales = 0.5f;
        [Tunable, TunableComment("Default Right Nipple Scale")]
        public static float kDefaultNippleRightScales = 0.5f;
    }
}
