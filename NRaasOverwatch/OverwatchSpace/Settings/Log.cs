using NRaas.CommonSpace.Options;
using NRaas.OverwatchSpace.Actions;
using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Settings
{
    public class Log : BooleanOption, IActionOption
    {
        public override string GetTitlePrefix()
        {
            return "Log";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new NRaas.OverwatchSpace.Settings.ListingOption(); }
        }

        protected override bool Value
        {
            get
            {
                return NRaas.Overwatch.Settings.Logging;
            }
            set
            {
                NRaas.Overwatch.Settings.Logging = value;
            }
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            bool origValue = Common.kDebugging;

            try
            {
                Common.kDebugging = true;

                Common.RecordErrors();
            }
            finally
            {
                Common.kDebugging = origValue;
            }

            return OptionResult.SuccessRetain;
        }
    }
}
