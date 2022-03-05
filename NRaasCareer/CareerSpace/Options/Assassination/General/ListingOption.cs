using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;

namespace NRaas.CareerSpace.Options.Assassination.General
{
    public class ListingOption : InteractionOptionList<IGeneralOption, GameObject>, IAssassinationOption
    {
        public override string GetTitlePrefix()
        {
            return "GeneralInteraction";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}
