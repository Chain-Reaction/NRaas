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

namespace NRaas.TaggerSpace.Options.RelationshipStatusTags
{
    public class ColorSetting : ColorSettingOption<GameObject>, IRelationshipStatusTagOption
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
                if (Tagger.Settings.mSimStatusColorSettings.ContainsKey(mData))
                {
                    return Tagger.Settings.mSimStatusColorSettings[mData];
                }

                return 0;
            }
            set
            {
                if (!Tagger.Settings.mSimStatusColorSettings.ContainsKey(mData))
                {                    
                    Tagger.Settings.mSimStatusColorSettings.Add(mData, value);
                    Tagger.Settings.mSimStatusColorSettingsSave.Add(mData.ToString(), value);
                }
                else
                {                    
                    Tagger.Settings.mSimStatusColorSettings[mData] = value;
                    Tagger.Settings.mSimStatusColorSettingsSave[mData.ToString()] = value;
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