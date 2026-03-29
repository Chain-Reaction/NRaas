using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.ChemistrySpace.Options.DebuggingLevel
{
    public class ListingOption : InteractionOptionList<IOverallDebuggingOption, GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "DebuggingLevel";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}
