using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Options.DoorFiltersGlobal
{
    public class IgnoreDoorCostFilterOption : FilerListingOption<GameObject>, IDoorGlobalOption
    {
        public override string GetTitlePrefix()
        {
            return "IgnoreDoorCostFilter";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override Proxy GetList()
        {
            return new ListProxy(GoHere.Settings.mGlobalIgnoreDoorCostFilterOption);
        }

        protected override void PrivatePerform(IEnumerable<ListedSettingOption<string, GameObject>.Item> results)
        {
            base.PrivatePerform(results);
        }
    }
}
