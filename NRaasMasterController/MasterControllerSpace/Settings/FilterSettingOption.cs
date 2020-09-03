using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.Settings.Filters;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Settings
{
    public abstract class FilterSettingOption : OptionItem, IFilterOption
    {
        public List<string> mForbiddenCrit = new List<string>();

        public virtual List<SimSelection.ICriteria> RunCriteriaSelection(GameHitParameters<GameObject> parameters, int maxSelection, bool withOptions)
        {
            List<SimSelection.ICriteria> selCrit = new List<SimSelection.ICriteria>();
            if (mForbiddenCrit.Count > 0)
            {
                foreach (SimSelection.ICriteria critItem in SelectionCriteria.SelectionOption.List)
                {
                    if (!mForbiddenCrit.Contains(critItem.GetType().Name))
                    {
                        selCrit.Add(critItem);
                    }
                }
            }
            else
            {
                selCrit = SelectionCriteria.SelectionOption.List;
            }

            SimSelection.CriteriaSelection.Results uncheckedCriteria = new SimSelection.CriteriaSelection(Name, selCrit).SelectMultiple(maxSelection);
            if (uncheckedCriteria.Count == 0)
            {
                if (uncheckedCriteria.mOkayed)
                {
                    return selCrit;
                }
                else
                {
                    return selCrit;
                }
            }

            bool showSpecial = false;
            foreach (SimSelection.ICriteria crit in uncheckedCriteria)
            {
                Common.Notify("Selected: " + crit.GetType().Name);
                if (crit is SimTypeOr)
                {
                    showSpecial = true;
                    break;
                }
            }

            Sim sim = parameters.mActor as Sim;

            List<IMiniSimDescription> simsList = new List<IMiniSimDescription>();
            foreach (List<IMiniSimDescription> sims in SimListing.AllSims<IMiniSimDescription>(sim.SimDescription, showSpecial).Values)
            {
                if (!showSpecial)
                {
                    sims.RemoveAll((e) => { return SimSelection.IsSpecial(e); });
                }

                simsList.AddRange(sims);
            }

            List<SimSelection.ICriteria> criteria = new List<SimSelection.ICriteria>();

            foreach (SimSelection.ICriteria crit in uncheckedCriteria)
            {
                if (withOptions)
                {
                    // Update changes the sims list, so we need a new copy for each call
                    List<IMiniSimDescription> newList = new List<IMiniSimDescription>(simsList);
                    if (crit.Update(sim.SimDescription, uncheckedCriteria, newList, false, false, false) != SimSelection.UpdateResult.Failure)
                    {
                        Common.Notify("Adding: " + crit.GetType().Name);
                        criteria.Add(crit);
                    }
                }
                else
                {
                    criteria.Add(crit);
                }
            }

            return criteria;
        }

        public virtual SimSelection.ICriteria RunFilterSelection(string callingMod)
        {
            List<SimSelection.ICriteria> filters = new List<SimSelection.ICriteria>();

            foreach (SavedFilter filter in NRaas.MasterController.Settings.mFilters)
            {
                filters.Add(new SavedFilter.Item(filter));
            }

            SimSelection.ICriteria selection = new SimSelection.CriteriaSelection(Name, filters, callingMod).SelectSingle();
            if (selection == null) return null;

            return selection;
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

    public class Item : ValueSettingOption<ICommonOptionItem>
    {
        public ICommonOptionItem mOption;
        public Item(ICommonOptionItem option)
            : base(option, option.Name, 0)
        {
            mOption = option;
        }
    }
}
