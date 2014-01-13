using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MoverSpace
{
    [Persistable]
    public class PersistedSettings
    {
        [Tunable, TunableComment("Whether to allow the user to create households with more than eight sims")]
        protected static bool kAllowGreaterThanEight = true;

        [Tunable, TunableComment("Whether to allow the user to create households with no adult in them")]
        protected static bool kAllowNoAdult = true;

        [Tunable, TunableComment("Percentage of household funds to transfer per person")]
        protected static int kMovePerPersonPercentage = 30;

        [Tunable, TunableComment("Minimum funds to transfer")]
        protected static int kMinMoneyTransferred = 0;

        [Tunable, TunableComment("Maximum funds to transfer when splitting a non-rich family")]
        protected static int kMaxMoneyTransferred = 20000;

        [Tunable, TunableComment("Maximum funds to transfer when splitting a rich family")]
        protected static int kMaxMoneyTransferredRich = 50000;

        [Tunable, TunableComment("Whether to test home inspection requirements")]
        protected static bool kHomeInspection = true;

        [Tunable, TunableComment("Whether to require the purchasing of lots")]
        protected static bool kFreeRealEstate = false;

        [Tunable, TunableComment("Whether to prompt to transfer realestate")]
        protected static bool kPromptTransferRealEstate = true;

        [Tunable, TunableComment("Whether to retain dreams and opportunities during the switch")]
        protected static bool kDreamCatcher = false;

        public bool mAllowGreaterThanEight = kAllowGreaterThanEight;

        public bool mAllowNoAdult = kAllowNoAdult;

        public int mMovePerPersonPercentage = kMovePerPersonPercentage;

        public int mMinMoneyTransferred = kMinMoneyTransferred;

        public int mMaxMoneyTransferred = kMaxMoneyTransferred;

        public int mMaxMoneyTransferredRich = kMaxMoneyTransferredRich;

        public bool mHomeInspection = kHomeInspection;

        public bool mFreeRealEstate = kFreeRealEstate;

        public bool mPromptTransferRealEstate = kPromptTransferRealEstate;

        public bool mDreamCatcher = kDreamCatcher;

        protected bool mDebugging = Common.kDebugging;

        public bool Debugging
        {
            get
            {
                return mDebugging;
            }
            set
            {
                mDebugging = value;

                Common.kDebugging = value;
            }
        }
    }
}
