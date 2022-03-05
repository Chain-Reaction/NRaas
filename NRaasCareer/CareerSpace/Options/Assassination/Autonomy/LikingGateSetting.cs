using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Skills;

namespace NRaas.CareerSpace.Options.Assassination.Autonomy
{
    public class LikingGateSetting : IntegerSettingOption<GameObject>, IAutonomyOption
    {
        protected override int Value
        {
            get
            {
                return NRaas.CareerSpace.Skills.Assassination.Settings.mLikingGate;
            }
            set
            {
                NRaas.CareerSpace.Skills.Assassination.Settings.mLikingGate = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "AssassinationLikingGate";
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
