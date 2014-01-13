using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Personalities;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Lots
{
    public abstract class ImmigrantScenario : SimScenario
    {
        protected List<SimDescription> mImmigrants;

        protected ManagerLot.ImmigrationRequirement mRequirement;

        public ImmigrantScenario(List<SimDescription> immigrants, ManagerLot.ImmigrationRequirement requirement)
        { 
            mImmigrants = immigrants;
            mRequirement = requirement;
        }
        protected ImmigrantScenario(SimDescription sim, ManagerLot.ImmigrationRequirement requirement)
            : base (sim)
        {
            mRequirement = requirement;
        }
        protected ImmigrantScenario(ImmigrantScenario scenario)
            : base (scenario)
        {
            mImmigrants = scenario.mImmigrants;
            mRequirement = scenario.mRequirement;
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return mImmigrants;
        }
    }
}
