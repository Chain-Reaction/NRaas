using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.HybridSpace.Options
{
    public class SpecialWerewolfOutfitSetting : BooleanSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        protected override bool Value
        {
            get
            {
                return Hybrid.Settings.mSpecialWerewolfOutfit;
            }
            set
            {
                Hybrid.Settings.mSpecialWerewolfOutfit = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "SpecialWerewolfOutfit";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return base.Allow(parameters);
        }
    }
}
