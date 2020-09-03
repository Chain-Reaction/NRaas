using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.CAS;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Settings.Filters
{
    public class DeleteOptionsSetting : FilterSettingOption
    {
        public string mFilter = string.Empty;
        public string mCriteria = string.Empty;

        public DeleteOptionsSetting()
        { }
        public DeleteOptionsSetting(string filter, string criteria)
        {
            mFilter = filter;
            mCriteria = criteria;
        }

        public override string GetTitlePrefix()
        {
            return "DeleteOptions";
        }

        public OptionResult RunExternal(bool unused)
        {
            return this.Run(null);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            SavedFilter filter;
            SimSelection.ICriteria criteria = null;
            if (mFilter != string.Empty && mCriteria != string.Empty)
            {
                filter = MasterController.GetFilter(mFilter);

                if (filter == null) return OptionResult.Failure;

                foreach (SimSelection.ICriteria crit in filter.Elements)
                {
                    if(crit.GetType().Name == mCriteria)
                    {
                        criteria = crit;
                        break;
                    }
                }

                if (criteria == null) return OptionResult.Failure;
            }
            else
            {
                SimSelection.ICriteria selection = base.RunFilterSelection(string.Empty);
                if (selection == null) return OptionResult.Failure;

                filter = MasterController.GetFilter(selection.Name);

                if (filter == null) return OptionResult.Failure;

                List<string> nonexistingCrit = new List<string>();

                foreach (SimSelection.ICriteria crit in SelectionOption.List)
                {
                    foreach (SimSelection.ICriteria crit2 in filter.Elements)
                    {
                        if (crit.Name == crit2.Name) goto Skip;
                    }

                    nonexistingCrit.Add(crit.GetType().Name);

                    Skip: ;
                }

                base.mForbiddenCrit.AddRange(nonexistingCrit);

                List<SimSelection.ICriteria> selCrit = base.RunCriteriaSelection(parameters, 1, false);

                if (selCrit.Count == 0) return OptionResult.Failure;

                foreach (SimSelection.ICriteria crit2 in filter.Elements)
                {
                    if (selCrit[0].Name == crit2.Name)
                    {
                        criteria = crit2;
                        break;
                    }
                }
            }

            if (!SelectionOption.StringList.ContainsKey(criteria.GetType().Name)) return OptionResult.Failure;

            List<IMiniSimDescription> sims = new List<IMiniSimDescription>();
            Dictionary<ulong, SimDescription> residents = SimListing.GetResidents(false);

            foreach (KeyValuePair<ulong, SimDescription> sim in residents)
            {
                sims.Add(sim.Value as IMiniSimDescription);
            }

            List<ICommonOptionItem> existingOptions = criteria.GetOptions(null, filter.Elements, sims);

            if (existingOptions == null) return OptionResult.Failure;

            List<Item> opts = new List<Item>();

            foreach (ICommonOptionItem opt in existingOptions)
            {
                opts.Add(new Item(opt));
            }

            CommonSelection<Item>.Results optSelection = new CommonSelection<Item>(Name, opts).SelectMultiple(20);

            if (optSelection == null || optSelection.Count == 0)
            {
                return OptionResult.Failure;
            }

            foreach (Item item in optSelection)
            {
                existingOptions.Remove(item.mOption);
            }

            criteria.SetOptions(existingOptions);

            return OptionResult.SuccessRetain;
        }
    }
}
