using NRaas.CommonSpace.Options;
using NRaas.GoHereSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Options.DoorFilters
{
    public class FilterTypeOption : GenericSettingOption<DoorPortalComponentEx.DoorSettings.SettingType, GameObject>, IDoorOption
    {
        GameObject mTarget;

        protected override DoorPortalComponentEx.DoorSettings.SettingType Value
        {
            get
            {
                DoorPortalComponentEx.DoorSettings settings = GoHere.Settings.GetDoorSettings(mTarget.ObjectId);
                if (settings != null)
                {
                    return settings.mType;
                }

                return DoorPortalComponentEx.DoorSettings.SettingType.Allow;
            }
            set
            {
                if (mTarget != null)
                {
                    DoorPortalComponentEx.DoorSettings settings = GoHere.Settings.GetDoorSettings(mTarget.ObjectId);
                    settings.mType = (settings.mType == DoorPortalComponentEx.DoorSettings.SettingType.Deny ? DoorPortalComponentEx.DoorSettings.SettingType.Allow : DoorPortalComponentEx.DoorSettings.SettingType.Deny);
                    GoHere.Settings.AddOrUpdateDoorSettings(mTarget.ObjectId, settings, true);
                }
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            mTarget = parameters.mTarget;
            return base.Allow(parameters);
        }

        public override string DisplayValue
        {
            get
            {
                if(mTarget != null)
                    return Common.Localize("DoorFilterType:" + GoHere.Settings.GetDoorSettings(mTarget.ObjectId).mType.ToString());

                return null;
            }
        }

        public override void SetImportValue(string value)
        {
        }

        public override string GetTitlePrefix()
        {
            return "DoorFilterType";
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Value = Value;            

            Common.Notify(ToString());            

            return OptionResult.SuccessRetain;
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}
