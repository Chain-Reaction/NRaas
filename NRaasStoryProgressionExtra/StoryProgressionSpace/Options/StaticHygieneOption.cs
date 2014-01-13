using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Options
{
    public class StaticHygieneOption : StaticMotiveOption
    {
        public StaticHygieneOption()
        { }

        public override string GetTitlePrefix()
        {
            return "StaticHygiene";
        }

        public override bool ShouldDisplay()
        {
            if (Manager.SimDescription != null)
            {
                if (Manager.SimDescription.IsFrankenstein) return false;
            }

            return base.ShouldDisplay();
        }
    }
}

