using NRaas.CareerSpace.Booters;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;

namespace NRaas.CareerSpace.Options.Homemaker
{
    public class HomemakeStirCrazySetting : IntegerSettingOption<GameObject>, IHomemakerOption
    {
        protected override int Value
        {
            get
            {
                return NRaas.Careers.Settings.mHomemakerLevelStirCrazy;
            }
            set
            {
                NRaas.Careers.Settings.mHomemakerLevelStirCrazy = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "HomemakeStirCrazy";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!HomemakerBooter.HasValue) return false;

            return base.Allow(parameters);
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
