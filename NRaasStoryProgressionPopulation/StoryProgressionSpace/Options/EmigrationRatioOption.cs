using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
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
    public class EmigrationRatioOption : HouseIntegerOption, IReadHouseLevelOption
    {
        public EmigrationRatioOption()
            : base(0)
        { }

        public override string GetTitlePrefix()
        {
            return "EmigrationRatio";
        }

        public override bool ShouldDisplay()
        {
            if (NRaas.StoryProgression.Main.GetValue<EmigrationScenario.Option, int>() <= 0) return false;

            return base.ShouldDisplay();
        }
    }
}

