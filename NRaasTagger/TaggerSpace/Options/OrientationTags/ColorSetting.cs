using NRaas.CommonSpace.Options;
using NRaas.TaggerSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TaggerSpace.Options.OrientationTags
{
    public class ColorSetting : ColorSettingOption<GameObject>, IOrientationTagOption
    {
        TagDataHelper.TagOrientationType mData;

        public ColorSetting(TagDataHelper.TagOrientationType data)
        {
            mData = data;
        }

        protected override uint Value
        {
            get
            {
                if (Tagger.Settings.mSimOrientationColorSettings.ContainsKey(mData))
                {
                    return Tagger.Settings.mSimOrientationColorSettings[mData];
                }

                return 0;
            }
            set
            {
                if (!Tagger.Settings.mSimOrientationColorSettings.ContainsKey(mData))
                {
                    Tagger.Settings.mSimOrientationColorSettings.Add(mData, value);
                }
                else
                {
                    Tagger.Settings.mSimOrientationColorSettings[mData] = value;
                }
            }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override string GetTitlePrefix()
        {
            return "TagColor";
        }
    }
}