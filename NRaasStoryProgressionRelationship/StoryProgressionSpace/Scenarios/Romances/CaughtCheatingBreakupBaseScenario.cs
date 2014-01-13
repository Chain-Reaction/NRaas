using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Romances
{
    public abstract class CaughtCheatingBreakupBaseScenario : BreakupScenario 
    {
        bool mWasMarried = false;

        public CaughtCheatingBreakupBaseScenario(SimDescription sim, bool affair, bool relatedStay)
            : base(sim, sim.Partner, affair, relatedStay)
        { }
        protected CaughtCheatingBreakupBaseScenario(bool affair, bool relatedStay)
            : base(affair, relatedStay)
        { }
        protected CaughtCheatingBreakupBaseScenario(CaughtCheatingBreakupBaseScenario scenario)
            : base (scenario)
        {
            mWasMarried = scenario.mWasMarried;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            mWasMarried = Sim.IsMarried;

            return base.PrivateUpdate(frame);
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (mWasMarried)
            {
                name = "Married" + name;
            }
            else
            {
                name = "Steady" + name;
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }
    }
}
