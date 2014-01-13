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

namespace NRaas.CareerSpace
{
    [Persistable]
    public class PersistedSettings
    {
        [Tunable, TunableComment("The maximum amount of funds allowed to transfer during a Shakedown")]
        protected static int kMaxShakedown = 500;

        [Tunable, TunableComment("The amount of relationship lost during a shakedown")]
        protected static int kShakedownRelationChange = -50;

        [Tunable, TunableComment("The level for busker at which a sim can perform concerts")]
        protected static int kBuskerLevelToGetPaidForConcerts = 7;

        [Tunable, TunableComment("Allow the mod to create a broken object during the 'Find Broken' interaction")]
        protected static bool kRepairAllowToBreak = true;

        [Tunable, TunableComment("How much cash per homemaker mark should be paid daily")]
        protected static int kHomemakerPayPerMark = 1;

        [Tunable, TunableComment("The Homemaker level at which a sim is immune to Stir Crazy")]
        protected static int kHomemakerLevelStirCrazy = 2;
        [Tunable, TunableComment("The Homemaker level at which purchases are discounted")]
        protected static int kHomemakerLevelDiscount = 3;
        [Tunable, TunableComment("The Homemaker level at which lifetime rewards are provided")]
        protected static int kHomemakerLevelLifetimeRewards = 5;

        [Tunable, TunableComment("The Homemaker discount level as a percent")]
        protected static int kHomemakerDiscountRate = 20;

        [Tunable, TunableComment("The amount of performance gained per Home Schooling homework submission")]
        protected static int kPerformancePerHomework = 20;

        public int mMaxShakedown = kMaxShakedown;
        public int mShakedownRelationChange = kShakedownRelationChange;

        public int mBuskerLevelToGetPaidForConcerts = kBuskerLevelToGetPaidForConcerts;

        public bool mRepairAllowToBreak = kRepairAllowToBreak;

        public int mHomemakerPayPerMark = kHomemakerPayPerMark;
        public int mHomemakerLevelStirCrazy = kHomemakerLevelStirCrazy;
        public int mHomemakerLevelDiscount = kHomemakerLevelDiscount;
        public int mHomemakerLevelLifetimeRewards = kHomemakerLevelLifetimeRewards;
        public int mHomemakerDiscountRate = kHomemakerDiscountRate;

        public int mPerformancePerHomework = kPerformancePerHomework;

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
