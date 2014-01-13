using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Settings
{
    public class DefaultSpeciesFilterSetting : ListedSettingOption<CASAgeGenderFlags, GameObject>, ISettingOption
    {
        public override string GetTitlePrefix()
        {
            return "DefaultSpeciesFilter";
        }

        public override string GetLocalizedValue(CASAgeGenderFlags value)
        {
            return Common.Localize("Species:" + value);
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new SimListingOption(); }
        }

        protected override List<Item> GetOptions()
        {
            List<Item> results = new List<Item>();

            foreach (CASAgeGenderFlags species in Species.GetSpecies())
            {
                results.Add(new Item(this, species));
            }

            return results;
        }

        protected override bool Allow(CASAgeGenderFlags value)
        {
            if (value == CASAgeGenderFlags.SpeciesMask) return false;

            return ((value & CASAgeGenderFlags.SpeciesMask) == value);
        }

        protected override Proxy GetList()
        {
            return new ListProxy(MasterController.Settings.mDefaultSpecies);
        }

        public override string ConvertToString(CASAgeGenderFlags value)
        {
            return value.ToString();
        }

        public override bool ConvertFromString(string value, out CASAgeGenderFlags newValue)
        {
            return ParserFunctions.TryParseEnum<CASAgeGenderFlags>(value, out newValue, CASAgeGenderFlags.None);
        }
    }
}
