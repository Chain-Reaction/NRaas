using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;

namespace NRaas.CareerSpace.Options.General
{
    public class MaxShakedownSetting : IntegerSettingOption<GameObject>, IGeneralOption
    {
        protected override int Value
        {
            get
            {
                return NRaas.Careers.Settings.mMaxShakedown;
            }
            set
            {
                NRaas.Careers.Settings.mMaxShakedown = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "MaxShakedown";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
