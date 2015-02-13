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
    public class FilterCacheTime : IntegerSettingOption<GameObject>, IFilterRootOption
    {
        protected override int Value
        {
            get
            {
                return FilterHelper.kFilterCacheTime;
            }
            set
            {
                FilterHelper.kFilterCacheTime = value;
                Tagger.Settings.mFilterCacheTime = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "FilterCacheTime";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return Tagger.Settings.Debugging;
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}