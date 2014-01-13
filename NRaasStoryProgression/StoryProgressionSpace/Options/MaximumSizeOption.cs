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
    public class MaximumSizeOption : HouseIntegerOption, IReadHouseLevelOption, IWriteHouseLevelOption, IReadLotLevelOption, IWriteLotLevelOption
    {
        public MaximumSizeOption()
            : base(8)
        { }

        public override string GetTitlePrefix()
        {
            return "MaximumSize";
        }

        protected override bool PrivatePerform()
        {
            if (!base.PrivatePerform()) return false;

            if (Value < 3)
            {
                SetValue (3);
            }

            return true;
        }
    }
}

