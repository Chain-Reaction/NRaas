using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.RetunerSpace.Options.Socials
{
    public class AllowAutonomous : BooleanSettingOption<GameObject>, ISocialOption
    {
        ActionData mData;

        public AllowAutonomous(ActionData data)
        {
            mData = data;
        }

        protected override bool Value
        {
            get
            {
                if (mData == null) return false;

                SeasonSettings.ActionDataSetting settings = Retuner.SeasonSettings.GetSettings(mData, false);
                if (settings != null)
                {
                    bool result;
                    if (settings.GetAutonomous(out result)) return result;
                }

                return mData.Autonomous;
            }
            set
            {
                Retuner.SeasonSettings.GetSettings(mData, true).SetAutonomous(Retuner.SeasonSettings.Key, mData, value);
            }
        }

        public override string GetTitlePrefix()
        {
            return "AllowAutonomous";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}
