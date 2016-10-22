using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Options.DoorFiltersGlobal
{
    public abstract class FilerListingOption<TObject> : ListedSettingOption<string, TObject>
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
            return value.ToString();
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
    }
}
