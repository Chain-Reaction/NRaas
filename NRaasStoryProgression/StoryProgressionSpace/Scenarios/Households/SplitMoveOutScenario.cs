using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Households
{
    public abstract class SplitMoveOutScenario : MoveOutScenario
    {
        SplitFamilyScenario.SplitHome mNewHome = null;

        Household mOldHouse = null;

        public SplitMoveOutScenario(SimDescription sim, SplitFamilyScenario.SplitHome newHome)
            : base(sim, null)
        {
            mNewHome = newHome;
            mOldHouse = sim.Household;
        }
        protected SplitMoveOutScenario(SplitMoveOutScenario scenario)
            : base (scenario)
        {
            mNewHome = scenario.mNewHome;
            mOldHouse = scenario.mOldHouse;
        }

        protected override bool Push()
        {
            if (Sim.Household != mOldHouse)
            {
                mNewHome.House = Sim.Household;
            }

            return base.Push();
        }
    }
}
