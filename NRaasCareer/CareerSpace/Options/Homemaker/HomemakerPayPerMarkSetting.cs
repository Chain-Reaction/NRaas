using NRaas.CareerSpace.Booters;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;

namespace NRaas.CareerSpace.Options.Homemaker
{
    public class HomemakerPayPerMarkSetting : IntegerSettingOption<GameObject>, IHomemakerOption
    {
        protected override int Value
        {
            get
            {
                return NRaas.Careers.Settings.mHomemakerPayPerMark;
            }
            set
            {
                NRaas.Careers.Settings.mHomemakerPayPerMark = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "HomemakerPayPerMark";
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
