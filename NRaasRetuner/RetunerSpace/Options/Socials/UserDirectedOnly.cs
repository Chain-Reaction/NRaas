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
    public class UserDirectedOnly : BooleanSettingOption<GameObject>, ISocialOption
    {
        ActionData mData;

        public UserDirectedOnly(ActionData data)
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
                    if (settings.GetUserDirected(out result)) return result;
                }

                return mData.UserDirectedOnly;
            }
            set
            {
                Retuner.SeasonSettings.GetSettings(mData, true).SetUserDirected(Retuner.SeasonSettings.Key, mData, value);
            }
        }

        public override string GetTitlePrefix()
        {
            return "UserDirectedOnly";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}
