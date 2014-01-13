using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Settings
{
    public abstract class FilterSettingOption : OptionItem, ISettingOption
    {
        public class Item : ValueSettingOption<SavedFilter>
        {
            public Item(SavedFilter filter)
                : base(filter, filter.Name, 0)
            { }
        }

        public SavedFilter Find(string name)
        {
            name = name.ToLower();

            foreach (SavedFilter filter in NRaas.MasterController.Settings.mFilters)
            {
                if (filter.Name.ToLower() == name)
                {
                    return filter;
                }
            }

            return null;
        }

        public bool Delete(string name)
        {
            name = name.ToLower();

            List<SavedFilter> filters = NRaas.MasterController.Settings.mFilters;

            int index = 0;
            while (index < filters.Count)
            {
                SavedFilter filter = filters[index];

                if (filter.Name.ToLower() == name)
                {
                    filters.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }

            return true;
        }
    }
}
