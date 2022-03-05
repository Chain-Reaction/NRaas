using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.ChemistrySpace.Options.Performance
{
    public class ListingOption : InteractionOptionList<IPerformanceOption, GameObject>, IPrimaryOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "PerformanceRoot";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}
