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
    public class BaseSatisfactionSetting : IntegerSettingOption<GameObject>, IKamaSimtraOption
    {
        protected override int Value
        {
            get
            {
                return Skills.KamaSimtra.Settings.mBaseSatisfaction;
            }
            set
            {
                Skills.KamaSimtra.Settings.mBaseSatisfaction = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "BaseSatisfaction";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
