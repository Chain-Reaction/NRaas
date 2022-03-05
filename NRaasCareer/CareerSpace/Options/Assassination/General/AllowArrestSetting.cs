using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Skills;

namespace NRaas.CareerSpace.Options.Assassination.General
{
    public class AllowArrestSetting : BooleanSettingOption<GameObject>, IGeneralOption
    {
        protected override bool Value
        {
            get
            {
                return NRaas.CareerSpace.Skills.Assassination.Settings.mAllowArrest;
            }
            set
            {
                NRaas.CareerSpace.Skills.Assassination.Settings.mAllowArrest = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "AllowArrest";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return (Skills.Assassination.StaticGuid != SkillNames.None);
        }
    }
}
