using NRaas.CommonSpace.Options;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.VectorSpace.Options.Enabled
{
    public class EnabledSetting : BooleanSettingOption<GameObject>, IEnabledOption
    {
        string mGuid;

        public EnabledSetting(string guid)
        {
            mGuid = guid;
        }

        protected override bool Value
        {
            get
            {
                return Vector.Settings.IsEnabled(mGuid);
            }
            set
            {
                Vector.Settings.SetEnabled(mGuid, value);
            }
        }

        public override string GetTitlePrefix()
        {
            return "Enabled";
        }

        public override string Name
        {
            get
            {
                return VectorBooter.GetVector(mGuid).GetLocalizedName(false);
            }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
