using NRaas.CommonSpace.Options;
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
    public class SeedServicePoolWithProsSetting : BooleanSettingOption<GameObject>, IKamaSimtraOption
    {
        protected override bool Value
        {
            get
            {
                return Skills.KamaSimtra.Settings.mSeedServicePool;
            }
            set
            {
                Skills.KamaSimtra.Settings.mSeedServicePool = value;

                Skills.KamaSimtra.SeedServicePool(true);    
            }
        }

        public override string GetTitlePrefix()
        {
            return "SeedServicePool";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}