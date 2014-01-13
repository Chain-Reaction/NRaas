using NRaas.CommonSpace.Helpers;
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

namespace NRaas.RetunerSpace.Options.ITUN.Outputs
{
    public class CommodityChangeOption : FloatSettingOption<GameObject>, IOutputOption
    {
        InteractionTuning mTuning;

        CommodityChange mChange;

        public CommodityChangeOption(InteractionTuning tuning, CommodityChange change)
            : base(CommoditiesEx.GetMotiveLocalizedName(change.Commodity))
        {
            mTuning = tuning;
            mChange = change;
        }

        protected override float Value
        {
            get
            {
                if (mTuning == null) return 0;

                SeasonSettings.ITUNSettings settings = Retuner.SeasonSettings.GetSettings(mTuning, false);
                if (settings != null)
                {
                    float result;
                    if (settings.GetAdvertised(mChange.Commodity, out result)) return result;
                }

                return mChange.mConstantChange;
            }
            set
            {
                Retuner.SeasonSettings.GetSettings(mTuning, true).SetAdvertised(mChange.Commodity, value);
            }
        }

        public override string GetTitlePrefix()
        {
            return null;
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}
