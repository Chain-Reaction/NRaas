using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
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
    public class AllowCanBePregnantOption : SimBooleanOption, IAdjustForVacationOption
    {
        public AllowCanBePregnantOption()
            : base(true)
        { }

        public override string GetTitlePrefix()
        {
            return "AllowCanBePregnant";
        }

        public bool AdjustForVacationTown()
        {
            if (GameUtils.IsUniversityWorld())
            {
                SetValue(false);
            }
            return true;
        }

        public override bool ShouldDisplay()
        {
            return BaseShouldDisplay();
        }
    }
}

