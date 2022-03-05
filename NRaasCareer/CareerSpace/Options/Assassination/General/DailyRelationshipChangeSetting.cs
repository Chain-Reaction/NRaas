using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Skills;

namespace NRaas.CareerSpace.Options.Assassination.General
{
    public class DailyRelationshipChangeSetting : IntegerSettingOption<GameObject>, IGeneralOption
    {
        protected override int Value
        {
            get
            {
                return NRaas.CareerSpace.Skills.Assassination.Settings.mDailyRelationshipChange;
            }
            set
            {
                NRaas.CareerSpace.Skills.Assassination.Settings.mDailyRelationshipChange = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "DailyRelationshipChange";
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
