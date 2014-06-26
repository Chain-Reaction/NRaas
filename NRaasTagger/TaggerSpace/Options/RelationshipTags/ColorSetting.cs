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

namespace NRaas.TaggerSpace.Options.RelationshipTags
{
    public class ColorSetting : ColorSettingOption<GameObject>, IRelationshipTagOption
    {
        TagDataHelper.TagRelationshipType mData;

        public ColorSetting(TagDataHelper.TagRelationshipType data)
        {
            mData = data;
        }

        protected override uint Value
        {
            get
            {
                if (Tagger.Settings.mRelationshipColorSettings.ContainsKey(mData))
                {
                    return Tagger.Settings.mRelationshipColorSettings[mData];
                }

                return 0;
            }
            set
            {
                if (!Tagger.Settings.mRelationshipColorSettings.ContainsKey(mData))
                {
                    Tagger.Settings.mRelationshipColorSettings.Add(mData, value);
                }
                else
                {
                    Tagger.Settings.mRelationshipColorSettings[mData] = value;
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