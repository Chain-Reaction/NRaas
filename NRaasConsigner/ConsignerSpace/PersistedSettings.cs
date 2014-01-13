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

namespace NRaas.ConsignerSpace
{
    [Persistable]
    public class PersistedSettings
    {
        [Tunable, TunableComment("Whether to simply make everything sellable")]
        public static bool kAllowSellAll = false;

        [Tunable, TunableComment("Base price for a cat or dog { Child, Adult, Elder }")]
        public static int[] kSellPrice = { 100, 200, 0 };

        [Tunable, TunableComment("Bonus price for good traits")]
        public static int kGoodTraitBonus = 100;

        [Tunable, TunableComment("Penalty price for negative traits")]
        public static int kBadTraitPenalty = 100;

        [Tunable, TunableComment("Bonus price per skill level")]
        public static int kSkillLevelBonus = 25;

        [Tunable, TunableComment("Bonus price per prey caught")]
        public static int kPerPreyBonus = 25;

        [Tunable, TunableComment("Bonus price for being a ghost")]
        public static int kOccultBonus = 250;

        [Tunable, TunableComment("Bonus for each level of unbroken ancestry")]
        public static int kPedigreeBonus = 250;

        [Tunable, TunableComment("Penalty for each ancestor that appears multiple times in the family tree")]
        public static int kInbredPenalty = 100;

        [Tunable, TunableComment("The minimum sale value required to display a notice for inactives")]
        public static int kReportGate = 200;

        public bool mAllowSellAll = kAllowSellAll;

        public int[] mSellPrice = kSellPrice;
        public int mGoodTraitBonus = kGoodTraitBonus;
        public int mBadTraitPenalty = kBadTraitPenalty;
        public int mSkillLevelBonus = kSkillLevelBonus;
        public int mPerPreyBonus = kPerPreyBonus;
        public int mOccultBonus = kOccultBonus;
        public int mPedigreeBonus = kPedigreeBonus;
        public int mInbredPenalty = kInbredPenalty;

        public int mReportGate = kReportGate;
    }
}
