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

namespace NRaas.TaggerSpace.Options.FilterOptions
{
    public class DeleteFilterOption : OperationSettingOption<GameObject>, IFilterRootOption
    {
        public override string GetTitlePrefix()
        {
            return "DeleteFilter";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            foreach(KeyValuePair<string, bool> pair in FilterHelper.GetFilters())
            {
                if (pair.Key.StartsWith("nraastagger")) return true;
            }

            return false;
        }        

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            FilterHelper.DeleteFilter();

            new Common.AlarmTask(3, TimeUnit.Minutes, Tagger.SetupMapTags); 

            return OptionResult.SuccessClose;
        }
    }
}