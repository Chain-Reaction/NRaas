using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;

namespace NRaas.CareerSpace.Options.Assassination.Aggression
{
    public class ListingOption : InteractionOptionList<IAggressionOption, GameObject>, IAssassinationOption
    {
        public override string GetTitlePrefix()
        {
            return "AggressionInteraction";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}
