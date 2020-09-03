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
    public class CreateFilterOption : OperationSettingOption<GameObject>, IFilterRootOption
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
            FilterHelper.CreateFilter(new List<string>());                       

            return OptionResult.SuccessClose;
        }
    }
}