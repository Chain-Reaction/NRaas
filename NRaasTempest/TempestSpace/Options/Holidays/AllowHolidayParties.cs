using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TempestSpace.Options.Holidays
{
    public class AllowHolidayParties : BooleanSettingOption<GameObject>, IHolidayOption
    {
        protected override bool Value
        {
            get
            {
                return Tempest.Settings.mAllowHolidayParties;
            }
            set
            {
                Tempest.Settings.mAllowHolidayParties = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "AllowHolidayParties";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
