using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Skills;

namespace NRaas.CareerSpace.Options.Assassination.Aggression
{
    public class AllowRepoSetting : BooleanSettingOption<GameObject>, IAggressionOption
    {
        protected override bool Value
        {
            get
            {
                return NRaas.CareerSpace.Skills.Assassination.Settings.mAllowRepo;
            }
            set
            {
                NRaas.CareerSpace.Skills.Assassination.Settings.mAllowRepo = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "AllowRepo";
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
