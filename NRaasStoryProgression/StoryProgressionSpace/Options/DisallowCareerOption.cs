using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
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
    public class DisallowCareerOption : CareerBaseOption, IReadSimLevelOption, IWriteSimLevelOption, IHouseLevelSimOption
    {
        public DisallowCareerOption()
        { }

        public override string GetTitlePrefix()
        {
            return "DisallowByCareer";
        }

        protected override string ValuePrefix
        {
            get { return "Disallowed"; }
        }

        public override bool ShouldDisplay()
        {
            if (!GetValue<AllowCareerProgressionOption, bool>()) return false;

            return base.ShouldDisplay();
        }
    }
}

