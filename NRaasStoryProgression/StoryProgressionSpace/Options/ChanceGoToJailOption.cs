using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Situations;
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
    public class ChanceGoToJailOption : SimIntegerOption, IWriteSimLevelOption, IHouseLevelSimOption
    {
        public ChanceGoToJailOption()
            : base(75)
        { }

        public override string GetTitlePrefix()
        {
            return "GoToJailChance";
        }

        public override int Value
        {
            get
            {
                if (!ShouldDisplay()) return 0;

                return base.Value;
            }
        }

        public override bool ShouldDisplay()
        {
            if (!GoToJailScenario.HasPushInteraction()) return false;

            if ((!GetValue<AllowPushAtDayOption, bool>()) && (!GetValue<AllowPushAtNightOption, bool>()))
            {
                return false;
            }

            return base.ShouldDisplay();
        }
    }
}

