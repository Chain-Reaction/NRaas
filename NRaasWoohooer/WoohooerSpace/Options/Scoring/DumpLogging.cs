using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.WoohooerSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.WoohooerSpace.Options.Scoring
{
    public class DumpLogging : OptionItem, IScoringOption
    {
        public override string DisplayValue
        {
            get
            {
                return EAText.GetNumberString(WoohooScoring.Count);
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return NRaas.Woohooer.Settings.Debugging;
        }

        public override string GetTitlePrefix()
        {
            return "DumpLogging";
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            WoohooScoring.Dump(false);
            return OptionResult.SuccessRetain;
        }
    }
}
