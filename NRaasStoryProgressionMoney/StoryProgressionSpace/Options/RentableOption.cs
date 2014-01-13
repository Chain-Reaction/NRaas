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
    public class RentableOption : SimBooleanOption, IReadLotLevelOption, IWriteLotLevelOption
    {
        public RentableOption()
            : base(true)
        { }

        public override string GetTitlePrefix()
        {
            return "Rentable";
        }

        public override bool Persist()
        {
            CheckRentableScenario.sImmediateUpdate = true;

            return base.Persist();
        }
    }
}

