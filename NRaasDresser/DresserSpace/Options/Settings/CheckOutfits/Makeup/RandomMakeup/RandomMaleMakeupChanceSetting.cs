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

namespace NRaas.DresserSpace.Options.Settings.CheckOutfits.Makeup.RandomMakeup
{
    public class RandomMaleMakeupChanceSetting : IntegerSettingOption<GameObject>, IRandomMakeupOption
    {
        protected override int Value
        {
            get
            {
                return Dresser.Settings.mRandomMakeup.mMaleChance;
            }
            set
            {
                Dresser.Settings.mRandomMakeup.mMaleChance = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "MaleChance";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
