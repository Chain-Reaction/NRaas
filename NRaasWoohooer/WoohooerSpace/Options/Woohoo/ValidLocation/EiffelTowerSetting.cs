using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using System;
namespace NRaas.WoohooerSpace.Options.Woohoo.ValidLocation
{
    public class EiffelTowerSetting : BooleanSettingOption<GameObject>, IValidLocationOption, IOptionItem, IInteractionOptionItem<IActor, GameObject, GameHitParameters<GameObject>>, ICommonOptionItem
    {
        protected override bool Value
        {
            get
            {
                return Woohooer.Settings.mAutonomousEiffelTower;
            }
            set
            {
                Woohooer.Settings.mAutonomousEiffelTower = value;
            }
        }
        public override ITitlePrefixOption ParentListingOption
        {
            get
            {
                return new ListingOption();
            }
        }
        public override string Name
        {
            get
            {
                return Common.Localize("Location:EiffelTower");
            }
        }
        public override string GetTitlePrefix()
        {
            return "AutonomousEiffelTower";
        }
    }
}