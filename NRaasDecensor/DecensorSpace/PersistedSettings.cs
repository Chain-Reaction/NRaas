using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.DecensorSpace
{
    [Persistable]
    public class PersistedSettings
    {
        [Tunable, TunableComment("Whether to display the censor while on the toilet")]
        protected static bool kCensorOnToilet = false;

        [Tunable, TunableComment("Whether to display the censor while woohooing in the shower")]
        protected static bool kCensorForShowerWoohoo = true;

        [Tunable, TunableComment("Whether to display the censor while on the potty")]
        protected static bool kCensorOnPotty = false;

        [Tunable, TunableComment("Whether to display the censor while horse woohooing")]
        protected static bool kCensorForHorseWoohoo = true;

        [Tunable, TunableComment("Whether to display the interactions in the Sim Menu")]
        protected static bool kShowSimMenu = true;

        [Tunable, TunableComment("Whether to disable the mod")]
        protected static bool kDisable = false;

        [Tunable, TunableComment("Whether to disable the censors for males")]
        protected static bool kCensorForMales = false;

        [Tunable, TunableComment("Whether to disable the censors for females")]
        protected static bool kCensorForFemales = false;

        [Tunable, TunableComment("Whether to disable the censors for a specific age")]
        protected static CASAgeGenderFlags[] kCensorByAge = new CASAgeGenderFlags[] { CASAgeGenderFlags.Baby, CASAgeGenderFlags.Toddler, CASAgeGenderFlags.Child };

        [Tunable, TunableComment("Whether to disable the censors for a specific species")]
        protected static CASAgeGenderFlags[] kCensorBySpecies = new CASAgeGenderFlags[0];

        [Tunable, TunableComment("The delay between censor checks in real-time milliseconds")]
        protected static int kDelay = 250;

        public bool mCensorOnToilet = kCensorOnToilet;

        public bool mCensorForShowerWoohoo = kCensorForShowerWoohoo;

        public bool mCensorOnPotty = kCensorOnPotty;

        public bool mCensorForHorseWoohoo = kCensorForHorseWoohoo;

        public bool mShowSimMenu = kShowSimMenu;

        public int mDelay = kDelay;

        public bool mDisable = kDisable;

        public bool mCensorForMales = kCensorForMales;

        public bool mCensorForFemales = kCensorForFemales;

        public List<CASAgeGenderFlags> mCensorByAge = new List<CASAgeGenderFlags>(kCensorByAge);

        public List<CASAgeGenderFlags> mCensorBySpecies = new List<CASAgeGenderFlags>(kCensorBySpecies);
    }
}
