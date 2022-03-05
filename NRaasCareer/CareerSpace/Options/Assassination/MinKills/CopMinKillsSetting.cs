using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Skills;

namespace NRaas.CareerSpace.Options.Assassination.MinKills
{
    public class CopMinKillsSetting : IntegerSettingOption<GameObject>, IMinKillsOption
    {
        protected override int Value
        {
            get
            {
                return NRaas.CareerSpace.Skills.Assassination.Settings.mCopMinKills;
            }
            set
            {
                NRaas.CareerSpace.Skills.Assassination.Settings.mCopMinKills = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "CopMinKills";
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
