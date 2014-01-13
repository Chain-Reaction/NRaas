using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Options
{
    public class CelebrityDisgraceOption : SimBooleanOption
    {
        public CelebrityDisgraceOption()
            : base(false)
        { }

        public override string GetTitlePrefix()
        {
            return "CelebrityDisgrace";
        }

        public override bool ShouldDisplay()
        {
            if (!StoryProgression.Main.GetValue<CelebrityDisgraceScenario.Option, bool>()) return false;

            return base.ShouldDisplay();
        }
    }
}

