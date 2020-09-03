using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Settings.Filters
{
    public class DeleteCriteriaSetting : FilterSettingOption
    {
        public string mFilter = string.Empty;

        public DeleteCriteriaSetting()
        { }

        public override string GetTitlePrefix()
        {
            return "DeleteCriteria";
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
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

            List<string> existing = new List<string>();
            foreach (SimSelection.ICriteria crit in filter.Elements)
            {
                existing.Add(crit.GetType().Name);
            }

            List<string> forbidden = new List<string>();
            foreach (string crit in SelectionOption.StringList.Keys)
            {
                if (!existing.Contains(crit))
                {
                    forbidden.Add(crit);
                }
            }

            base.mForbiddenCrit = forbidden;

            List<SimSelection.ICriteria> selected = base.RunCriteriaSelection(parameters, 20, false);

            if (selected.Count == 0) return OptionResult.Failure;

            foreach (SimSelection.ICriteria crit in selected)
            {
                foreach (SimSelection.ICriteria element in new List<SimSelection.ICriteria>(filter.Elements))
                {
                    if (element.Name == crit.Name)
                    {
                        filter.Elements.Remove(element);
                        break;
                    }
                }
            }

            return OptionResult.SuccessRetain;
        }
    }
}
