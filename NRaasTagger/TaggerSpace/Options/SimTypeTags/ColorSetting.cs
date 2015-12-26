using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
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

namespace NRaas.TaggerSpace.Options.SimTypeTags
{
    public class ColorSetting : ColorSettingOption<GameObject>, ISimTypeTagOption
    {
        SimType mData;

        public ColorSetting(SimType data)
        {
            mData = data;
        }

        protected override uint Value
        {
            get
            {
                if (Tagger.Settings.mSimTypeColorSettings.ContainsKey(mData))
                {
                    return Tagger.Settings.mSimTypeColorSettings[mData];
                }

                return 0;
            }
            set
            {
                if (!Tagger.Settings.mSimTypeColorSettings.ContainsKey(mData))
                {
                   Tagger.Settings.mSimTypeColorSettings.Add(mData, value);
                   Tagger.Settings.mSimTypeColorSettingsSave.Add(mData.ToString(), value);
                }
                else
                {                    
                    Tagger.Settings.mSimTypeColorSettings[mData] = value;
                    Tagger.Settings.mSimTypeColorSettingsSave[mData.ToString()] = value;
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