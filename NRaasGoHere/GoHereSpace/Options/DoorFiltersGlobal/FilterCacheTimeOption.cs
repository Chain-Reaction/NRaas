using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.GoHereSpace.Options.DoorFiltersGlobal
{
    public class FilterCacheTime : IntegerSettingOption<GameObject>, IDoorGlobalOption
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
                GoHere.Settings.mFilterCacheTime = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "FilterCacheTime";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return GoHere.Settings.Debugging;
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}