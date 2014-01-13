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

namespace NRaas.RetunerSpace.Options.ITUN
{
    public class AllowUserDirected : BooleanSettingOption<GameObject>, IITUNOption
    {
        InteractionTuning mTuning;

        public AllowUserDirected(InteractionTuning tuning)
        {
            mTuning = tuning;
        }

        protected override bool Value
        {
            get
            {
                if (mTuning == null) return false;

                SeasonSettings.ITUNSettings settings = Retuner.SeasonSettings.GetSettings(mTuning, false);
                if (settings != null)
                {
                    bool result;
                    if (settings.GetUserDirected(out result)) return result;
                }

                return !mTuning.HasFlags(InteractionTuning.FlagField.DisallowUserDirected);
            }
            set
            {
                Retuner.SeasonSettings.GetSettings(mTuning, true).SetUserDirected(Retuner.SeasonSettings.Key, mTuning, value);
            }
        }

        public override string GetTitlePrefix()
        {
            return "AllowUserDirected";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}
