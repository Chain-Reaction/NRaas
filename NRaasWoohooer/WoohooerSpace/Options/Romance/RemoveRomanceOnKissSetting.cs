using NRaas.CommonSpace.Options;
using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay.Abstracts;

namespace NRaas.WoohooerSpace.Options.Romance
{
    public class RemoveRomanceOnKissSetting : BooleanSettingOption<GameObject>, IRomanceOption, Common.IWorldLoadFinished
    {
        protected override bool Value
        {
            get
            {
                return Woohooer.Settings.mRemoveRomanceOnKiss;
            }
            set
            {
                Woohooer.Settings.mRemoveRomanceOnKiss = value;

                CommonSocials.ToggleKissRules();
            }
        }

        public override string GetTitlePrefix()
        {
            return "RemoveRomanceOnKiss";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public void OnWorldLoadFinished()
        {
            CommonSocials.ToggleKissRules();
        }
    }
}
