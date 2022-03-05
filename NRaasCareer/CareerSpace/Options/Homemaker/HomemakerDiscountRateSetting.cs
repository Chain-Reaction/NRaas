using NRaas.CareerSpace.Booters;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;

namespace NRaas.CareerSpace.Options.Homemaker
{
    public class HomemakerDiscountRateSetting : IntegerSettingOption<GameObject>, IHomemakerOption
    {
        protected override int Value
        {
            get
            {
                return NRaas.Careers.Settings.mHomemakerDiscountRate;
            }
            set
            {
                NRaas.Careers.Settings.mHomemakerDiscountRate = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "HomemakerDiscountRate";
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
