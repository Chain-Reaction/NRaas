using NRaas.CommonSpace.Helpers;
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
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Settings
{
    public class SaveFilterSetting : FilterSettingOption
    {
        public override string GetTitlePrefix()
        {
            return "SaveFilterSetting";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return false;
        }

        public OptionResult RunExternal(string callingMod, List<string> forbiddenCrit)
        {
            if (Sim.ActiveActor == null) return OptionResult.Failure;
            return new Filters.SaveFilterSetting().RunExternal(callingMod, forbiddenCrit);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            return OptionResult.Failure;
        }
    }
}
