using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;

namespace NRaas.CareerSpace.Options
{
    public class DebuggingSetting : BooleanSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        protected override bool Value
        {
            get
            {
                return NRaas.Careers.Settings.Debugging;
            }
            set
            {
                NRaas.Careers.Settings.Debugging = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "Debugging";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}
