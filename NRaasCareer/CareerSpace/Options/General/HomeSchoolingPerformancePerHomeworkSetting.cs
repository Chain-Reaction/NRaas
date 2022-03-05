using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Utilities;

namespace NRaas.CareerSpace.Options.General
{
    public class HomeSchoolingPerformancePerHomeworkSetting : IntegerSettingOption<GameObject>, IGeneralOption
    {
        protected override int Value
        {
            get
            {
                return NRaas.Careers.Settings.mPerformancePerHomework;
            }
            set
            {
                NRaas.Careers.Settings.mPerformancePerHomework = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "HomeSchoolingPerformancePerHomework";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!Localization.HasLocalizationString("NRaas.Careers.HomeSchoolingPerformancePerHomework:MenuName")) return false;

            return base.Allow(parameters);
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
