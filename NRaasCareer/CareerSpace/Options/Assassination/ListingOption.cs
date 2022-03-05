using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;

namespace NRaas.CareerSpace.Options.Assassination
{
    public class ListingOption : InteractionOptionList<IAssassinationOption, GameObject>, IPrimaryOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "AssassinationInteraction";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}
