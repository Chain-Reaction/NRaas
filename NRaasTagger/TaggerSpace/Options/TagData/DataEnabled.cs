using NRaas.CommonSpace.Options;
using NRaas.TaggerSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TaggerSpace.Options.TagData
{
    public class DataEnabled : BooleanSettingOption<GameObject>, ITagDataOption
    {
        TagDataHelper.TagDataType mData;

        public DataEnabled(TagDataHelper.TagDataType data)
        {
            mData = data;
        }

        protected override bool Value
        {
            get
            {
                return Tagger.Settings.mTagDataSettings[mData];
            }
            set
            {
                Tagger.Settings.mTagDataSettings[mData] = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "DataEnabled";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}