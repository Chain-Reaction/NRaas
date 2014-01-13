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
    public class AllowNonGreetedOutsideAutonomous : BooleanSettingOption<GameObject>, IITUNOption
    {
        InteractionTuning mTuning;

        public AllowNonGreetedOutsideAutonomous(InteractionTuning tuning)
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
                    if (settings.HasAvailability(Availability.FlagField.AllowNonGreetedSimsIfObjectOutsideAutonomous, out result)) return result;
                }

                return mTuning.Availability.HasFlags(Availability.FlagField.AllowNonGreetedSimsIfObjectOutsideAutonomous);
            }
            set
            {
                Retuner.SeasonSettings.GetSettings(mTuning, true).SetAvailability(Retuner.SeasonSettings.Key, mTuning, Availability.FlagField.AllowNonGreetedSimsIfObjectOutsideAutonomous, value);
            }
        }

        public override string GetTitlePrefix()
        {
            return "AllowNonGreetedOutsideAutonomous";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}
