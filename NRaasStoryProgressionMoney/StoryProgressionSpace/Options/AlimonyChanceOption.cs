using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Money;
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
    public class AlimonyChanceOption : SimIntegerOption, IHouseLevelSimOption
    {
        public AlimonyChanceOption()
            : base(50)
        { }

        public override string GetTitlePrefix()
        {
            return "AlimonyChance";
        }

        public override bool ShouldDisplay()
        {
            if (StoryProgression.Main.GetValue<AlimonyScenario.PaymentOption, int>() <= 0) return false;

            return base.ShouldDisplay();
        }
    }
}

