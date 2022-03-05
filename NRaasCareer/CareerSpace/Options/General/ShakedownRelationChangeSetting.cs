using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;

namespace NRaas.CareerSpace.Options.General
{
    public class ShakedownRelationChangeSetting : IntegerSettingOption<GameObject>, IGeneralOption
    {
        protected override int Value
        {
            get
            {
                return NRaas.Careers.Settings.mShakedownRelationChange;
            }
            set
            {
                NRaas.Careers.Settings.mShakedownRelationChange = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "ShakedownRelationChange";
        }

        protected override int Validate(int value)
        {
            if (value < 0)
            {
                value = -value;
            }

            return base.Validate(value);
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
