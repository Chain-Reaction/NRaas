using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Options.DoorFiltersGlobal
{
    public class IgnoreAllDoorOptionsFilterOption : FilerListingOption<GameObject>, IDoorGlobalOption
    {
        public override string GetTitlePrefix()
        {
            return "IgnoreAllDoorOptionsFilter";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override Proxy GetList()
        {
            return new ListProxy(GoHere.Settings.mGlobalIgnoreAllDoorOptionsFilterOption);
        }

        protected override void PrivatePerform(IEnumerable<ListedSettingOption<string, GameObject>.Item> results)
        {
            base.PrivatePerform(results);
        }
    }
}
