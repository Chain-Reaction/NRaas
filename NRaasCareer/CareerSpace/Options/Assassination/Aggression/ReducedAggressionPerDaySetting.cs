using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Skills;

namespace NRaas.CareerSpace.Options.Assassination.Aggression
{
    public class ReducedAggressionPerDaySetting : IntegerSettingOption<GameObject>, IAggressionOption
    {
        protected override int Value
        {
            get
            {
                return NRaas.CareerSpace.Skills.Assassination.Settings.mReducedAggressionPerDay;
            }
            set
            {
                NRaas.CareerSpace.Skills.Assassination.Settings.mReducedAggressionPerDay = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "ReducedAggressionPerDay";
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
