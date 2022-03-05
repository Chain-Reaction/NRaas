using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.ChemistrySpace.Options.Attraction
{
    public class ListingOption : InteractionOptionList<IAttractionOption, GameObject>, IPrimaryOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "Attraction";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}
