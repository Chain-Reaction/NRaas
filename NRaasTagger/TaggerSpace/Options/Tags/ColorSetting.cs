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

namespace NRaas.TaggerSpace.Options.Tags
{
    public class ColorSetting : IntegerSettingOption<GameObject>, ITagOption
    {
        TagStaticData mData;        

        public ColorSetting(TagStaticData data)
        {            
            mData = data;
        }

        protected override string GetValuePrefix()
        {
            return "ColorSetting";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }       

        public override bool ConvertFromString(string value, out CASAgeGenderFlags newValue)
        {
            return ParserFunctions.TryParseEnum<CASAgeGenderFlags>(value, out newValue, CASAgeGenderFlags.None);
        }

        public override string ConvertToString(CASAgeGenderFlags value)
        {
            return value.ToString();
        }

        public override string GetTitlePrefix()
        {
            return "TagColor";
        }

        protected override void PrivatePerform(IEnumerable<Item> results)
        {
            base.PrivatePerform(results);

            ServiceSettingKey key = Register.Settings.GetSettingsForService(mData);

            CASAgeGenderFlags validFlags = CASAgeGenderFlags.None;
            foreach (CASAgeGenderFlags flag in mAgeSpecies)
            {
                validFlags |= flag;
            }

            key.validAges = validFlags;
            key.SetSettings(mData);
        }
    }
}