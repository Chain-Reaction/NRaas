using NRaas.StoryProgressionSpace.Managers;
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
    public abstract class SplitMoveInScenario : MoveInScenario
    {
        SplitFamilyScenario.SplitHome mNewHome = null;

        public SplitMoveInScenario(SimDescription mover, SplitFamilyScenario.SplitHome newHome)
            : base(mover)
        {
            mNewHome = newHome;
            House = mNewHome.House;
        }
        public SplitMoveInScenario(List<SimDescription> movers, SplitFamilyScenario.SplitHome newHome)
            : base(movers)
        {
            mNewHome = newHome;
            House = mNewHome.House;
        }
        protected SplitMoveInScenario(SplitMoveInScenario scenario)
            : base (scenario)
        {
            mNewHome = scenario.mNewHome;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!base.PrivateUpdate(frame)) return false;

            mNewHome.House = House;
            return true;
        }
    }
}
