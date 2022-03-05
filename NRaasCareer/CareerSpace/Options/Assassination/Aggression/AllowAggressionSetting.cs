using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Skills;

namespace NRaas.CareerSpace.Options.Assassination.Aggression
{
    public class AllowAggressionSetting : BooleanSettingOption<GameObject>, IAggressionOption
    {
        protected override bool Value
        {
            get
            {
                return NRaas.CareerSpace.Skills.Assassination.Settings.mAllowAggression;
            }
            set
            {
                NRaas.CareerSpace.Skills.Assassination.Settings.mAllowAggression = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "AllowAggression";
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
