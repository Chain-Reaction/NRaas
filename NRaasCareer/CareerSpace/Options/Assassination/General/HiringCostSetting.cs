using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Skills;

namespace NRaas.CareerSpace.Options.Assassination.General
{
    public class HiringCostSetting : IntegerSettingOption<GameObject>, IGeneralOption
    {
        protected override int Value
        {
            get
            {
                return NRaas.CareerSpace.Skills.Assassination.Settings.mHiringCost;
            }
            set
            {
                NRaas.CareerSpace.Skills.Assassination.Settings.mHiringCost = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "AssassinationHiringCost";
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
