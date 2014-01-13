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

namespace NRaas.TempestSpace.Options.Plants
{
    public class HailKillsHarvestables : IntegerSettingOption<GameObject>, IPlantOption
    {
        protected override int Value
        {
            get
            {
                return Tempest.Settings.mHailKillsHarvestables;
            }
            set
            {
                Tempest.Settings.mHailKillsHarvestables = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "HailKillsHarvestables";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
