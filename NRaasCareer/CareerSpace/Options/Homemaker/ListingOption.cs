using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;

namespace NRaas.CareerSpace.Options.Homemaker
{
    public class ListingOption : InteractionOptionList<IHomemakerOption, GameObject>, IPrimaryOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "HomemakerInteraction";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}
