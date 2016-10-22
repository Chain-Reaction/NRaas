using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Utilities;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.GoHereSpace.Options
{
    public class CreateFilterOption : OperationSettingOption<GameObject>, DoorFiltersGlobal.IDoorGlobalOption, DoorFilters.IDoorOption
    {
        public override string GetTitlePrefix()
        {
            return "CreateFilter";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            FilterHelper.CreateFilter();

            return OptionResult.SuccessClose;
        }
    }
}