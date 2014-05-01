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

namespace NRaas.RegisterSpace.Options.Service
{
    public class AgeSetting : ListedSettingOption<CASAgeGenderFlags, GameObject>, IServiceOption
    {
        Sims3.Gameplay.Services.Service mData;
        protected List<CASAgeGenderFlags> mAgeSpecies = null;

        public AgeSetting(Sims3.Gameplay.Services.Service data)
        {
            mAgeSpecies = Register.Settings.GetSettingsForService(data).AgeSpeciesToList();
            mData = data;
        }

        protected override string GetValuePrefix()
        {
            return "ValidAges";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override Proxy GetList()
        {
            return new ListProxy(mAgeSpecies);
        }

        protected override bool Allow(CASAgeGenderFlags value)
        {
            switch (value)
            {
                case CASAgeGenderFlags.Teen:
                case CASAgeGenderFlags.YoungAdult:
                case CASAgeGenderFlags.Adult:
                case CASAgeGenderFlags.Elder:
                    return Register.AllowForService(mData.ServiceType, value);
                case CASAgeGenderFlags.None:
                    return false;
                default:
                    return false;
            }           
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
            return "ServiceValidAges";
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
