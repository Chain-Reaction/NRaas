using NRaas.CommonSpace.Options;
using NRaas.GoHereSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Options.DoorFilters
{
    public class FilterTypeOption : GenericSettingOption<DoorPortalComponentEx.DoorSettings.SettingType, GameObject>, IFilterTypeOption
    {
        GameObject mTarget;
        DoorPortalComponentEx.DoorSettings.SettingType mType;

        public FilterTypeOption(DoorPortalComponentEx.DoorSettings.SettingType type, GameObject obj)
        {
            mTarget = obj;
            mType = type;
        }

        protected override DoorPortalComponentEx.DoorSettings.SettingType Value
        {
            get
            {
                return GoHere.Settings.GetDoorSettings(mTarget.ObjectId).mType;
            }
            set
            {
                DoorPortalComponentEx.DoorSettings settings = GoHere.Settings.GetDoorSettings(mTarget.ObjectId);
                settings.mType = mType;
                GoHere.Settings.AddOrUpdateDoorSettings(mTarget.ObjectId, settings);
            }
        }

        public override string DisplayValue
        {
            get
            {
                return (GoHere.Settings.GetDoorSettings(mTarget.ObjectId).mType == mType ? Common.Localize("Boolean:True") : Common.Localize("Boolean:False"));
            }
        }

        public override void SetImportValue(string value)
        {
        }

        public override string GetTitlePrefix()
        {
            return mType.ToString();
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Value = Value;

            //Tagger.InitTags(false);

            Common.Notify(ToString());            

            return OptionResult.SuccessRetain;
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}
