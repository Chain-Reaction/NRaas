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

namespace NRaas.TaggerSpace.Options.CustomTags
{
    public class ColorSetting : ColorSettingOption<GameObject>, ITagOption
    {
        TagStaticData mData;        

        public ColorSetting(TagStaticData data)
        {            
            mData = data;
        }

        protected override uint Value
        {
            get
            {
                if (Tagger.Settings.TypeHasCustomSettings(mData.GUID))
                {
                    return Tagger.Settings.mCustomTagSettings[mData.GUID].Color;
                }

                return 0;
            }
            set
            {
                if (!Tagger.Settings.TypeHasCustomSettings(mData.GUID))
                {
                    TagSettingKey key = new TagSettingKey();
                    key.SetColor(value);
                    Tagger.Settings.mCustomTagSettings.Add(mData.GUID, key);
                }
                else
                {
                    Tagger.Settings.mCustomTagSettings[mData.GUID].SetColor(value);
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