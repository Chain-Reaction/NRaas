using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.RelativitySpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.RelativitySpace.Options.MotiveDecayDelta.AddNewFactor
{
    public class ListingOption : InteractionOptionList<IAddNewFactorOption, GameObject>, IMotiveDecayDeltaOption
    {
        public override string GetTitlePrefix()
        {
            return "AddNewFactorRoot";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new MotiveDecayDelta.ListingOption(); }
        }
    }
}
