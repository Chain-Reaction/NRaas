using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.GoHereSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.GoHereSpace.Options
{
    public class DeleteFilterOption : OperationSettingOption<GameObject>, DoorFiltersGlobal.IDoorGlobalOption, DoorFilters.IDoorOption
    {
        public override string GetTitlePrefix()
        {
            return "DeleteFilter";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            foreach (KeyValuePair<string, bool> pair in FilterHelper.GetFilters())
            {
                if (pair.Key.StartsWith("nraas")) return true;
            }

            return false;
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            FilterHelper.DeleteFilter();

            new Common.AlarmTask(3, TimeUnit.Minutes, GoHere.Settings.ValidateFilters); 

            return OptionResult.SuccessClose;
        }
    }
}