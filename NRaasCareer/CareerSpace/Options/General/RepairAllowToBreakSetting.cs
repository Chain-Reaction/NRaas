using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;

namespace NRaas.CareerSpace.Options.General
{
    public class RepairAllowToBreakSetting : BooleanSettingOption<GameObject>, IGeneralOption
    {
        protected override bool Value
        {
            get
            {
                return NRaas.Careers.Settings.mRepairAllowToBreak;
            }
            set
            {
                NRaas.Careers.Settings.mRepairAllowToBreak = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "RepairAllowToBreak";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
