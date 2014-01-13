using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.ScoringMethods
{
    public class ManagerScoringParameters : DualSimScoringParameters
    {
        StoryProgressionObject mManager;

        public ManagerScoringParameters(StoryProgressionObject manager, SimDescription scoreAgainst, SimDescription other, bool absolute)
            : base(scoreAgainst, other, absolute)
        {
            mManager = manager;
        }

        public StoryProgressionObject Manager
        {
            get { return mManager; }
        }
    }
}
