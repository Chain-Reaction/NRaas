using NRaas.CommonSpace.Options;
using NRaas.OverwatchSpace.Interfaces;
using NRaas.OverwatchSpace.Loadup;
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

namespace NRaas.OverwatchSpace.Settings
{
    public class StopPetAdoptionSetting : BooleanOption
    {
        public override string GetTitlePrefix()
        {
            return "StopPetAdoptionSetting";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new NRaas.OverwatchSpace.Settings.ListingOption(); }
        }

        protected override bool Value
        {
            get
            {
                return NRaas.Overwatch.Settings.mStopPetAdoption;
            }
            set
            {
                NRaas.Overwatch.Settings.mStopPetAdoption = value;

                if (value)
                {
                    //new CleanupPetAdoption().OnWorldLoadFinished();
                }
            }
        }
    }
}
