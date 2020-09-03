using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace;
using NRaas.MasterControllerSpace.SelectionCriteria;
using NRaas.MasterControllerSpace.Settings;
using Sims3.Gameplay.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Settings.Filters
{
    public class AddCriteriaSetting : FilterSettingOption
    {
        public string mFilter = string.Empty;

        public AddCriteriaSetting()
        { }
        public AddCriteriaSetting(string filter)
        {
            mFilter = filter;
        }
        public AddCriteriaSetting(string filter, List<string> forbiddenAdditions)
        {
            mFilter = filter;
            base.mForbiddenCrit = forbiddenAdditions;
        }

        public override string GetTitlePrefix()
        {
            return "AddCriteria";
        }

        public OptionResult RunExternal(bool unused)
        {
            return this.Run(null);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            List<SimSelection.ICriteria> selCrit = new List<SimSelection.ICriteria>();

            List<string> existingCrit = new List<string>();
            SavedFilter filter;
            if (mFilter != string.Empty)
            {
                filter = MasterController.GetFilter(mFilter);

                if (filter == null) return OptionResult.Failure;                
            }
            else
            {
                SimSelection.ICriteria selection = base.RunFilterSelection(string.Empty);
                if (selection == null) return OptionResult.Failure;

                filter = MasterController.GetFilter(selection.Name);

                if (filter == null) return OptionResult.Failure;
            }

            foreach (SimSelection.ICriteria criteria in filter.Elements)
            {
                existingCrit.Add(criteria.GetType().Name);
            }

            base.mForbiddenCrit.AddRange(existingCrit);

            selCrit = base.RunCriteriaSelection(parameters, 20, true);

            foreach (SimSelection.ICriteria pick in selCrit)
            {
                filter.Elements.Add(pick);
            }

            return OptionResult.SuccessRetain;

        }
    }
}
