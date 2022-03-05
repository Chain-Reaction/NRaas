using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Skills;

namespace NRaas.CareerSpace.Options.Assassination.MinKills
{
    public class PsychopathMinKillsSetting : IntegerSettingOption<GameObject>, IMinKillsOption
    {
        protected override int Value
        {
            get
            {
                return NRaas.CareerSpace.Skills.Assassination.Settings.mPsychopathMinKills;
            }
            set
            {
                NRaas.CareerSpace.Skills.Assassination.Settings.mPsychopathMinKills = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "PsychopathMinKills";
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
