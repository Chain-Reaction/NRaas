using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Skills;

namespace NRaas.CareerSpace.Options.Assassination.General
{
    public class AllowTeenSetting : BooleanSettingOption<GameObject>, IGeneralOption
    {
        protected override bool Value
        {
            get
            {
                return NRaas.CareerSpace.Skills.Assassination.Settings.mAllowTeen;
            }
            set
            {
                NRaas.CareerSpace.Skills.Assassination.Settings.mAllowTeen = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "AllowTeen";
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
