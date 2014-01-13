using NRaas.CommonSpace.Options;
using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Settings
{
    public class EnableTestingCheats : BooleanOption, Common.IWorldLoadFinished
    {
        public override string GetTitlePrefix()
        {
            return "TestingCheatsEnabled";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new NRaas.OverwatchSpace.Settings.ListingOption(); }
        }

        protected override bool Value
        {
            get
            {
                return NRaas.Overwatch.Settings.mTestingCheatsEnabled;
            }
            set
            {
                NRaas.Overwatch.Settings.mTestingCheatsEnabled = value;

                if (value)
                {
                    Commands.EnableTestingCheats();
                }
                else
                {
                    Commands.DisableTestingCheats();
                }
            }
        }

        public void OnWorldLoadFinished()
        {
            try
            {
                if (NRaas.Overwatch.Settings.mTestingCheatsEnabled)
                {
                    Commands.EnableTestingCheats();
                }
            }
            catch(Exception e)
            {
                Common.Exception("EnableTestingCheats", e);
            }
        }
    }
}
