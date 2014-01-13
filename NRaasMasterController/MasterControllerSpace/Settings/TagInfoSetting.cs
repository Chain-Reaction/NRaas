using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Settings
{
    public class TagInfoSetting : EnumSettingOption<PersistedSettings.TagInfo,GameObject>, ISettingOption
    {
        protected override PersistedSettings.TagInfo Value
        {
            get
            {
                return NRaas.MasterController.Settings.mTagInfo;
            }
            set
            {
                NRaas.MasterController.Settings.mTagInfo = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "TagInfoSetting";
        }

        public override PersistedSettings.TagInfo Default
        {
            get { return PersistedSettings.TagInfo.Limited; }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new SimListingOption(); }
        }

        public override string GetLocalizedValue(PersistedSettings.TagInfo value)
        {
            switch (value)
            {
                case PersistedSettings.TagInfo.Personal:
                    return Common.Localize("PersonalStatus:MenuName");
                case PersistedSettings.TagInfo.Career:
                    return Common.Localize("CareerStatus:MenuName");
                case PersistedSettings.TagInfo.Household:
                    return Common.Localize("HouseStatus:MenuName");
                case PersistedSettings.TagInfo.Relationship:
                    return Common.Localize("RelationshipStatus:MenuName");
            }

            return base.GetLocalizedValue(value);
        }
    }
}
