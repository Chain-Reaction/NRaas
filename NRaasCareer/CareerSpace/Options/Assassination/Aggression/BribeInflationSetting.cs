using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Skills;

namespace NRaas.CareerSpace.Options.Assassination.Aggression
{
    public class BribeInflationSetting : FloatSettingOption<GameObject>, IAggressionOption
    {
        protected override float Value
        {
            get
            {
                return NRaas.CareerSpace.Skills.Assassination.Settings.mBribeInflation;
            }
            set
            {
                NRaas.CareerSpace.Skills.Assassination.Settings.mBribeInflation = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "BribeInflation";
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
