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

// legacy, remove once all filterable mods have been updated
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
            return false;
        }

        public OptionResult RunExternal(string mNamespace)
        {
            return new Filters.DeleteFilterSetting().RunExternal(mNamespace);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            return OptionResult.Failure;
        }
    }
}
