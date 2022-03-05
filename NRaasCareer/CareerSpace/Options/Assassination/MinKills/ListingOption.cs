using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;

namespace NRaas.CareerSpace.Options.Assassination.MinKills
{
    public class ListingOption : InteractionOptionList<IMinKillsOption, GameObject>, IAssassinationOption
    {
        public override string GetTitlePrefix()
        {
            return "MinKillsInteraction";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}
