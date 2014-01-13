using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Sims;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
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
    public class AllowAgingOption : SimBooleanOption
    {
        public AllowAgingOption()
            : base(true)
        { }

        public override string GetTitlePrefix()
        {
            return "AllowAging"; 
        }

        public override bool ShouldDisplay()
        {
            if (!AgingManager.Singleton.Enabled) return false;

            return base.ShouldDisplay();
        }

        protected override bool PrivatePerform()
        {
            if (!base.PrivatePerform()) return false;

            StoryProgression.Main.Scenarios.Post(new UpdateAgingScenario());
            return true;
        }
    }
}

