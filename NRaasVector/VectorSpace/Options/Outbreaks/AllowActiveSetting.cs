using NRaas.CommonSpace.Options;
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

namespace NRaas.VectorSpace.Options.Outbreaks
{
    public class AllowActiveSetting : BooleanSettingOption<GameObject>, IOutbreaksOption
    {
        protected override bool Value
        {
            get
            {
                return Vector.Settings.mOutbreakAllowActive;
            }
            set
            {
                Vector.Settings.mOutbreakAllowActive = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "OutbreakAllowActive";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
