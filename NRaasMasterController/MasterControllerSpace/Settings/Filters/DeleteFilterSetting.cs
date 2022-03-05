using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
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

namespace NRaas.MasterControllerSpace.Settings.Filters
{
    public class DeleteFilterSetting : FilterSettingOption
    {
        string callingMod = string.Empty;

        public override string GetTitlePrefix()
        {
            return "DeleteFilterSetting";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return (NRaas.MasterController.Settings.mFilters.Count > 0);
        }

        public OptionResult RunExternal(string mNamespace)
        {
            callingMod = mNamespace;
            return this.Run(null);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            CommonSelection<SavedFilter.Item>.Results selection = base.RunMultipleFilterSelection(callingMod);
            if (selection == null || selection.Count == 0) return OptionResult.Failure;

            foreach (SavedFilter.Item item in selection)
            {
                Delete(item.Name);
            }

            return OptionResult.SuccessRetain;
        }
    }
}
