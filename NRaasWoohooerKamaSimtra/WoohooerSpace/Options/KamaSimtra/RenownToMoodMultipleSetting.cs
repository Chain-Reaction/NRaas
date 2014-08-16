using NRaas.CommonSpace.Options;
using NRaas.WoohooerSpace.Skills;
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

namespace NRaas.WoohooerSpace.Options.KamaSimtra
{
    public class RenownToMoodMultipleSetting : FloatSettingOption<GameObject>, IKamaSimtraOption
    {
        protected override float Value
        {
            get
            {
                return Skills.KamaSimtra.Settings.mRenownToMoodMultiple;
            }
            set
            {
                Skills.KamaSimtra.Settings.mRenownToMoodMultiple = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "RenownToMoodMultiple";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
