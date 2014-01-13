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
    public class PushDeathChanceOption : SimIntegerOption, IWriteSimLevelOption, IHouseLevelSimOption
    {
        static bool sInstalled = false;

        public PushDeathChanceOption()
            : base(0)
        { }

        public override string GetTitlePrefix()
        {
            return "PushDeathChance";
        }

        public static bool Installed
        {
            set
            {
                sInstalled = true;
            }
            get
            {
                return sInstalled;
            }
        }

        public override int Value
        {
            get
            {
                if (!sInstalled) return 0;

                return base.Value;
            }
        }

        public override bool ShouldDisplay()
        {
            if (!sInstalled) return false;

            return base.ShouldDisplay();
        }
    }
}

