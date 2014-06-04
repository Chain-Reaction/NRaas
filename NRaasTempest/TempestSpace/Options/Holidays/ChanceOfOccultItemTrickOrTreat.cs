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
    public class ChanceOfOccultItemTrickOrTreat : IntegerSettingOption<GameObject>, IHolidayOption
    {
        protected override int Value
        {
            get
            {
                return Tempest.Settings.mChanceOccultItemTrickOrTreat;
            }
            set
            {
                Tempest.Settings.mChanceOccultItemTrickOrTreat = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "ChanceOfOccultItemTrickOrTreat";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
