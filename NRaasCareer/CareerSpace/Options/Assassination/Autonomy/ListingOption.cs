using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;

namespace NRaas.CareerSpace.Options.Assassination.Autonomy
{
    public class ListingOption : InteractionOptionList<IAutonomyOption, GameObject>, IAssassinationOption
    {
        public override string GetTitlePrefix()
        {
            return "AutonomyInteraction";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}
