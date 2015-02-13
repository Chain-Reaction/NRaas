using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.TaggerSpace.Options.FilterOptions
{
    public class UpdateFilterCacheOption : OperationSettingOption<GameObject>, IFilterRootOption
    {
        public override string GetTitlePrefix()
        {
            return "UpdateFilterCache";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return Tagger.Settings.Debugging;
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {            
            FilterHelper.UpdateFilters();

            Common.Notify(Common.Localize("General:Success"));

            return OptionResult.SuccessClose;
        }
    }
}