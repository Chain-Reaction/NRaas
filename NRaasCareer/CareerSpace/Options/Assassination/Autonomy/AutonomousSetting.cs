using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Skills;

namespace NRaas.CareerSpace.Options.Assassination.Autonomy
{
    public class AutonomousSetting : BooleanSettingOption<GameObject>, IAutonomyOption
    {
        protected override bool Value
        {
            get
            {
                return NRaas.CareerSpace.Skills.Assassination.Settings.mAutonomous;
            }
            set
            {
                NRaas.CareerSpace.Skills.Assassination.Settings.mAutonomous = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "Autonomous";
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
