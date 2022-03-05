using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Skills;

namespace NRaas.CareerSpace.Options.Assassination.Autonomy
{
    public class ShowNoticeSetting : BooleanSettingOption<GameObject>, IAutonomyOption
    {
        protected override bool Value
        {
            get
            {
                return NRaas.CareerSpace.Skills.Assassination.Settings.mShowAutonomousNotice;
            }
            set
            {
                NRaas.CareerSpace.Skills.Assassination.Settings.mShowAutonomousNotice = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "ShowAutonomousNotice";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!NRaas.CareerSpace.Skills.Assassination.Settings.mAutonomous) return false;

            return (Skills.Assassination.StaticGuid != SkillNames.None);
        }
    }
}
