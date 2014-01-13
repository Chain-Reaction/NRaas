using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.SelectionCriteria;
using NRaas.MasterControllerSpace.Sims;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Settings
{
    public class DeleteFilterSetting : FilterSettingOption
    {
        public override string GetTitlePrefix()
        {
            return "DeleteFilterSetting";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return (NRaas.MasterController.Settings.mFilters.Count > 0);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            List<SimSelection.ICriteria> filters = new List<SimSelection.ICriteria>();

            foreach (SavedFilter filter in NRaas.MasterController.Settings.mFilters)
            {
                filters.Add(new SavedFilter.Item(filter));
            }

            SimSelection.ICriteria selection = new SimSelection.CriteriaSelection(Name, filters).SelectSingle();
            if (selection == null) return OptionResult.Failure;

            Delete(selection.Name);
            return OptionResult.SuccessRetain;
        }
    }
}
