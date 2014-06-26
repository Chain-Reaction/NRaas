using NRaas.CommonSpace.Options;
using NRaas.TaggerSpace.Options;
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

namespace NRaas.TaggerSpace.Options.AgeTags
{
    public class ColorSetting : ColorSettingOption<GameObject>, IAgeTagOption
    {
        CASAgeGenderFlags mData;

        public ColorSetting(CASAgeGenderFlags data)
        {
            mData = data;
        }

        protected override uint Value
        {
            get
            {
                if (Tagger.Settings.mAgeColorSettings.ContainsKey(mData))
                {                    
                    return Tagger.Settings.mAgeColorSettings[mData];
                }
                
                return 0;                
            }
            set
            {
                if (!Tagger.Settings.mAgeColorSettings.ContainsKey(mData))
                {                    
                    Tagger.Settings.mAgeColorSettings.Add(mData, value);
                }
                else
                {                    
                    Tagger.Settings.mAgeColorSettings[mData] = value;
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