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

namespace NRaas.RegisterSpace.Options.Service
{
    public class ReoccurrenceSetting : BooleanSettingOption<GameObject>, IServiceOption
    {
        Sims3.Gameplay.Services.Service mData;

        public ReoccurrenceSetting(Sims3.Gameplay.Services.Service data)
        {
            mData = data;
        }

        protected override bool Value
        {
            get
            {
                if (mData == null) return false;

                return Register.Settings.GetSettingsForService(mData).reoccuring;
            }
            set
            {
                ServiceSettingKey key = Register.Settings.GetSettingsForService(mData);
                key.reoccuring = value;
                key.SetSettings(mData);
            }
        }

        public override string GetTitlePrefix()
        {
            return "ServiceReoccuring";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}
