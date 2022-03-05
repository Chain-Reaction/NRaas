using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Skills;

namespace NRaas.CareerSpace.Options.Assassination.Aggression
{
    public class RabbitHoleCorruptionSetting : FloatSettingOption<GameObject>, IAggressionOption
    {
        protected override float Value
        {
            get
            {
                return NRaas.CareerSpace.Skills.Assassination.Settings.mRabbitHoleCorruption;
            }
            set
            {
                NRaas.CareerSpace.Skills.Assassination.Settings.mRabbitHoleCorruption = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "RabbitHoleCorruption";
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
