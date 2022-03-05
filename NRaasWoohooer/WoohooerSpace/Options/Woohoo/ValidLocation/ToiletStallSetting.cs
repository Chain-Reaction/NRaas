using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using System;
namespace NRaas.WoohooerSpace.Options.Woohoo.ValidLocation
{
    public class ToiletStallSetting : BooleanSettingOption<GameObject>, IValidLocationOption, IOptionItem, IInteractionOptionItem<IActor, GameObject, GameHitParameters<GameObject>>, ICommonOptionItem
    {
        protected override bool Value
        {
            get
            {
                return Woohooer.Settings.mAutonomousToiletStall;
            }
            set
            {
                Woohooer.Settings.mAutonomousToiletStall = value;
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
                return Common.Localize("Location:ToiletStall");
            }
        }
        public override string GetTitlePrefix()
        {
            return "AutonomousToiletStall";
        }
    }
}