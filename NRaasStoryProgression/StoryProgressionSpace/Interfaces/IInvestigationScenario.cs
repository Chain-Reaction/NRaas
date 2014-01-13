using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Scenarios;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interfaces
{
    public interface IInvestigationScenario
    {
        bool AllowGoToJail
        {
            get;
        }

        string InvestigateStoryName
        {
            get;
        }

        SimDescription Sim
        {
            get;
        }

        SimDescription Target
        {
            get;
        }

        int InvestigateMinimum
        {
            get;
        }

        int InvestigateMaximum
        {
            get;
        }

        bool InstallInvestigation(Scenario.UpdateDelegate func);
    }
}

