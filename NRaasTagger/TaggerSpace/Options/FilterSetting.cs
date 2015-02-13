using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TaggerSpace.Options
{
    public abstract class FilterSetting<TObject> : ListedSettingOption<string, TObject>
        where TObject : class, IGameObject
    {
        public override string GetLocalizedValue(string value)
        {
            return value;
        }

        public override bool ConvertFromString(string value, out string newValue)
        {
            newValue = value;
            return true;
        }

        public override string ConvertToString(string value)
        {
            return value;
        }

        protected override List<ListedSettingOption<string, TObject>.Item> GetOptions()
        {
            List<Item> results = new List<Item>();

            Dictionary<string, bool> filters = FilterHelper.GetFilters();

            foreach (KeyValuePair<string, bool> value in filters)
            {
                results.Add(new Item(this, value.Key));
            }

            return results;
        }

        protected override bool Allow(GameHitParameters<TObject> parameters)
        {
            return FilterHelper.GetFilters().Count > 0;
        }
    }
}