using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.ChemistrySpace.Options.DebuggingLevel
{
    public class OverallDebuggingLevelOption : DebuggingLevelSetting<GameObject>, IPrimaryOption<GameObject>
    {
        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return Chemistry.Settings.Debugging;
        }

        protected override Proxy GetList()
        {
            return new ListProxy(Chemistry.Settings.mOverallDebuggingLevel);
        }

        public override string GetTitlePrefix()
        {
            return "OverallDebuggingLevel";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
